using System;
using System.Collections;
using UnityEngine;
public class Controller_0616 : MonoBehaviour {
    public enum Direction { Left, Right };
    public Direction faceDirection;
    public float speed = 1f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float attackTime = 0.33f;

    void Start() {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        SetFace();
        Vector3 directionForce = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            directionForce += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S)) {
            directionForce += Vector3.down;
        }
        if (Input.GetKey(KeyCode.A)) {
            directionForce += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            directionForce += Vector3.right;
        }

        directionForce.Normalize();

        if (directionForce == Vector3.zero) {
            animator.Play("Idle");
        } else {
            animator.Play("Walk");
        }

        //把物體目前的座標拿出來
        Vector3 pos = transform.position;
        //用direction資訊改變座標
        pos = pos + (directionForce * speed * Time.deltaTime);
        //把改變後的座標，放回物體上
        transform.position = pos;
    }

    void SetFace() {
        Vector2 mousePos = Input.mousePosition;
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        bool lookRight = mousePos.x >= playerPos.x;
        if (lookRight) {
            faceDirection = Direction.Right;
        } else {
            faceDirection = Direction.Left;
        }
        spriteRenderer.flipX = lookRight;
    }


}
