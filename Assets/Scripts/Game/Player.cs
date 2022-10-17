using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    void Start()
    {
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
    }
}
