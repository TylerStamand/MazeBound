using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pcAnim : MonoBehaviour {
    private Rigidbody2D rb;

    public Animator anim;
    public float moveSpeed;

    public float x, y;
    private bool isWalking;

    private Vector3 moveDir;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

    }
    private void Update() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        if (x != 0 || y != 0) {
            anim.SetFloat("x", x);
            anim.SetFloat("y", y);
            if (!isWalking) {
                isWalking = true;
                anim.SetBool("isMoving", isWalking);

            }
        } else {
            if (isWalking) {
                isWalking = false;
                anim.SetBool("isMoving", isWalking);
            }
        }

        moveDir = new Vector3(x, y).normalized;
    }



}

// https://www.youtube.com/watch?v=EwWK5hHLFq0 
// code credits 