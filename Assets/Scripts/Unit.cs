using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

public class Unit : MonoBehaviour {
    public enum Direction { None, Left, Right };
    public enum Group { Player, Enemy };

    public Group group = Group.Enemy;
    public float hp = 2f;
    public float maxHp = 2f;

    //動作狀態標記
    //角色面向
    public Direction faceDirection;
    //走路速度
    public float walkSpeed = 1f;
    //攻擊時間(越短越快)
    public float attackTime = 2f;
    //必備Component們
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    Coroutine _coroutineMove;
    public float damage = 1f;
    Color originColor;
    public Action<Unit> onUnitDead;

    void Awake() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = spriteRenderer.color;
        Debug.Log(originColor.ToString() + "name");
    }
    public void GetHurt(Unit form) {
        hp -= form.damage;
        if (hp <= 0f) {
            hp = 0f;
            Die();
        } else {
            StartCoroutine(HurtAsync());
        }
    }
    IEnumerator HurtAsync() {
        for (int i = 0; i < 5; i++) {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originColor;
            yield return new WaitForSeconds(0.05f);
        }
    }
    void Die() {
        onUnitDead?.Invoke(this);
    }

    //修正面向的函式，這個函式的任務是讓角色持續看著滑鼠位置
    public void SetFace(bool isRight) {
        if (isRight) {
            faceDirection = Direction.Right;
        } else {
            faceDirection = Direction.Left;
        }
        //圖片翻轉
        spriteRenderer.flipX = isRight;
    }
    public void StopMove() {
        if (_coroutineMove != null) {
            StopCoroutine(_coroutineMove);
            _coroutineMove = null;
        }
    }
    public void MoveTo(Vector2 pos, float speedScale = 1f) {
        StopMove();
        _coroutineMove = StartCoroutine(MoveToAsync(pos, speedScale));
    }
    IEnumerator MoveToAsync(Vector2 pos, float speedScale = 1f) {
        while (true) {
            float moveByFrame = walkSpeed * Time.deltaTime * speedScale;
            if (Vector2.Distance((Vector2)transform.position, pos) < moveByFrame) {
                Vector3 targetPos = pos;
                targetPos.z = transform.position.z;
                transform.position = targetPos;
                _coroutineMove = null;
                break;
            } else {
                Vector2 direction = pos - (Vector2)transform.position;
                direction.Normalize();
                transform.position += (Vector3)(direction * moveByFrame);
            }
            yield return null;
        }
    }

    public void MoveToByTime(Vector2 pos, float time) {
        StartCoroutine(MoveToByTimeAsync(pos, time));
    }
    IEnumerator MoveToByTimeAsync(Vector2 pos, float time) {
        //目標
        Vector2 targetPos = pos;
        //起始
        Vector2 startPos = transform.position;
        //如果大於限制距離，強制修正位置
        //移動中的協程，分散每幀運行
        for (float i = 0f; i < time; i += Time.deltaTime) {
            float t = i / time;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        //收尾修正位置
        transform.position = targetPos;
    }

    //攻擊幀
    public virtual void OnAttackShoot() {
        // FXAttack fx = explosionPool.Create(transform.parent);
        // fx.transform.position = transform.position + (Vector3)(_attackDirection * 0.75f);
        // fx.Play();
        // fx.onReturn = (f) => { explosionPool.Return(f); };
    }

}
