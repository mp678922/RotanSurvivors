using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit {
    public enum AIState { MoveToTarget, Attack, Idle, IdleMove };
    public AIState state;
    public float viewRange = 5f;
    public float attackRange = 5f;

    public float attackCD = 2f;
    float _attackCDCount = 0f;

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Loop());
    }

    IEnumerator Loop() {
        while (true) {
            Turn();
            yield return new WaitForSeconds(1f);
        }
    }

    void Update() {
        _attackCDCount += Time.deltaTime;
    }

    void Turn() {
        Player nearPlayer = FindNearPlayer();
        if (nearPlayer != null) {
            float distance = Vector2.Distance(transform.position, nearPlayer.transform.position);
            if (distance <= attackRange) {
                if (_attackCDCount >= attackCD) {
                    _attackCDCount = 0f;
                    StopMove();
                    Attack(nearPlayer);
                    return;
                }
            }
            animator.Play("WalkFront");
            MoveTo(nearPlayer.transform.position);
        } else {
            //Random.value = 0f~1f;
            if (Random.value < 0.5f) {
                state = AIState.Idle;
                StopMove();
                animator.Play("Idle");
            } else {
                state = AIState.IdleMove;
                float randomX = Random.Range(-3f, 3f);
                float randomY = Random.Range(-3f, 3f);
                MoveTo(transform.position + new Vector3(randomX, randomY, 0f));
                animator.Play("WalkFront");
            }
        }
    }

    Player FindNearPlayer() {
        Player[] players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++) {
            Player player = players[i];
            if (Vector2.Distance(transform.position, player.transform.position) <= viewRange) {
                return player;
            }
        }
        return null;
    }

    Vector2 _attackDirection;
    void Attack(Unit target) {
        StartCoroutine(AttackAsync(target));
    }
    IEnumerator AttackAsync(Unit target) {
        animator.speed = 1f / attackTime;
        //隨機挑選動作
        string attackAnimation = UnityEngine.Random.value < 0.5f ? "Attack1" : "Attack2";
        animator.Play(attackAnimation, 0, 0f);
        //計算攻擊方向
        _attackDirection = (Vector2)(target.transform.position - transform.position).normalized;

        Vector2 targetPos = target.transform.position;
        targetPos += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        Vector2 attackTo = targetPos + (_attackDirection * 2f);
        //移動到該位置(最大距離、保持安全距離、移動總時長)
        MoveToByTime(attackTo, attackTime);
        //等待攻擊動作完成，將狀態標記回待機，這樣Update才可以繼續運作
        yield return new WaitForSeconds(attackTime);
        animator.speed = 1f;
    }
}
