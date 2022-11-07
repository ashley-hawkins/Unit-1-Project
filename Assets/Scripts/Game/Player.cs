using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    Rigidbody2D rb;
    Combat combat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        bool swinging = animator.GetBool("Swinging");
        bool walking = animator.GetBool("Walking");

        if (Input.GetKey(KeyCode.S))
        {
            if (!swinging)
                animator.SetBool("Swinging", true);
        }
        else if (swinging)
        {
            animator.SetBool("Swinging", false);
        }

        if (Input.GetKey(KeyCode.W))
        {
            if (!walking)
                animator.SetBool("Walking", true);
        }
        else if (walking)
        {
            animator.SetBool("Walking", false);
        }

        ProcessMovement();
    }

    void ProcessMovement()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector2.left * 1.5f);
        }
        if (Input.GetKey(KeyCode.S))
        {
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector2.right * 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * 5.0f, ForceMode2D.Impulse);
        }
    }
}
