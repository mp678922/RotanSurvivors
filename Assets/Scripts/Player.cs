using System;
using System.Collections;
using UnityEngine;
public class Player : Unit {

    public enum State { Idle, Walk, Attack, Hurt };
    public State state;
    //攻擊限制距離
    public float attackRange = 1.2f;
    //攻擊維持安全距離
    public float attackKeepRange = 0.75f;
    public Attack attackOrigin;
    Pool<Attack> attackPool;
    //攻擊方向
    Vector2 _attackDirection;

    void Start() {
        attackPool = new Pool<Attack>(attackOrigin);
    }
    void OnEnable() {
        state = State.Idle;
    }

    void Update() {

        //如果在播放攻擊動作的時候，不繼續後續程式碼
        if (state == State.Attack) { return; }

        //設定面向:讓角色持續看著滑鼠
        SetFace();

        //這是移動向量，當沒有輸入移動指令時會維持在Vector3.zero的狀態。
        Vector3 directionForce = Vector3.zero;
        //這是移動方向，判斷動畫往前走/往後走用的變數，當角色左右移動時會被輸入移動方向。
        Direction moveDirection = Direction.None;
        //這是角色動畫播放速度，默認為1，當移動速度高時，移動速度會讓他變很快。
        float animatorSpeed = 1f;

        //這系列是位移時的輸入，修改directionForce讓角色移動。
        //輸入左右(A、D)時，會改變moveDirection，作為判斷前進/後退動畫的依據。
        if (Input.GetKey(KeyCode.W)) {
            directionForce += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S)) {
            directionForce += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A)) {
            directionForce += Vector3.left;
            moveDirection = Direction.Left;
        }
        if (Input.GetKey(KeyCode.D)) {
            directionForce += Vector3.right;
            moveDirection = Direction.Right;
        }

        //這邊是動畫控制
        if (directionForce == Vector3.zero) {
            //當沒有輸入移動指令時，維持在閒置姿勢
            animator.Play("Idle");
            //狀態標記閒置
            state = State.Idle;
        } else {
            //當輸入移動指令時的任務
            //移動速度會蓋變動畫播放器的播放速度，越高越快
            animatorSpeed = walkSpeed / 2f;
            //角色面向等於移動方向時，播放前進動作，反之後退
            if (moveDirection == faceDirection) {
                animator.Play("WalkFront");
            } else {
                animator.Play("WalkBack");
            }
            //狀態標記移動中
            state = State.Walk;
        }

        //正規化移動向量，維持移動速度穩定
        directionForce.Normalize();
        //把物體目前的座標拿出來
        Vector3 pos = transform.position;
        //用direction資訊改變座標
        pos = pos + (directionForce * walkSpeed * Time.deltaTime);
        //把改變後的座標，放回物體上
        transform.position = pos;
        //把動畫播放速度填回
        animator.speed = animatorSpeed;

        //當按下滑鼠左鍵的時候執行攻擊
        if (Input.GetMouseButton(0)) {
            Attack();
        }
    }
    void Attack() {
        StartCoroutine(AttackAsync());
    }
    IEnumerator AttackAsync() {
        state = State.Attack;
        animator.speed = 1f / attackTime;
        //隨機挑選動作
        string attackAnimation = UnityEngine.Random.value < 0.5f ? "Attack1" : "Attack2";
        animator.Play(attackAnimation, 0, 0f);
        //計算攻擊方向
        _attackDirection = (MouseWorldPoint() - (Vector2)transform.position).normalized;
        //移動到該位置(最大距離、保持安全距離、移動總時長)
        MoveToMouse(attackRange, attackKeepRange, attackTime * 0.5f);
        //等待攻擊動作完成，將狀態標記回待機，這樣Update才可以繼續運作
        yield return new WaitForSeconds(attackTime);
        state = State.Idle;
    }

    public void SetFace() {
        //滑鼠位置
        Vector2 mousePos = Input.mousePosition;
        //角色位置，並把世界座標轉換成螢幕座標，讓他跟滑鼠可以比較。
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        //當滑鼠在角色右邊時，標記lookRight為真，以此讓程式識別角色面向。
        bool lookRight = mousePos.x - playerPos.x > 0f;
        if (lookRight) {
            faceDirection = Direction.Right;
        } else {
            faceDirection = Direction.Left;
        }
        //圖片翻轉
        spriteRenderer.flipX = lookRight;
    }

    //移動到滑鼠位置(最大距離、保持安全距離、移動總時長)
    void MoveToMouse(float maxRange, float keepRange, float time) {
        StartCoroutine(MoveToMouseAsync(maxRange, keepRange, time));
    }
    IEnumerator MoveToMouseAsync(float maxRange, float keepRange, float time) {
        //目標
        Vector2 targetPos = MouseWorldPoint() - (_attackDirection * keepRange);
        //起始
        Vector2 startPos = transform.position;
        //起始與目標之距離
        float distance = Vector2.Distance(targetPos, startPos);
        //如果大於限制距離，強制修正位置
        if (distance > maxRange) {
            targetPos = startPos + (_attackDirection * maxRange);
        }
        //移動中的協程，分散每幀運行
        for (float i = 0f; i < time; i += Time.deltaTime) {
            float t = i / time;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        //收尾修正位置
        transform.position = targetPos;
    }
    //讓滑鼠左標轉為世界座標的工具函式
    Vector2 MouseWorldPoint() {
        Vector2 camera = Camera.main.transform.position;
        Vector2 mouse = Input.mousePosition;
        mouse.x -= Screen.width / 2f;
        mouse.y -= Screen.height / 2f;
        mouse /= 100f;
        return camera + mouse;
    }

    public override void OnAttackShoot() {
        Attack attack = attackPool.Create();
        attack.GetComponent<FXAttack>()?.Play();
        attack.DoAttack(this);
    }

}
