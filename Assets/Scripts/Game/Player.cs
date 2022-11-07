using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public GameObject armPivot;
    Rigidbody2D rb;
    Combat combat;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        bool swingTool = Input.GetMouseButton(0);
        animator.SetBool("Swinging", swingTool);

        if (Input.GetMouseButtonDown(0))
        {
            armPivot.SetActive(true);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            armPivot.SetActive(false);
        }

        if (swingTool)
        {
            float pickaxeAnimPercentage = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwingWalk")) pickaxeAnimPercentage *= 2;

            armPivot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, (1 - pickaxeAnimPercentage % 1 - 1) * 180));
        }
        ProcessMovement();
    }

    void ProcessMovement()
    {
        float maxSpeed = 8f;
        float desiredSpeed = 0;
        if (Input.GetKey(KeyCode.A))
        {
            desiredSpeed = -maxSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
        }
        if (Input.GetKey(KeyCode.D))
        {
            desiredSpeed = maxSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * 15.0f, ForceMode2D.Impulse);
        }

        float currentSpeed = rb.velocity.x;

        float desiredSpeedDelta = desiredSpeed - currentSpeed;

        float forceRequired = rb.mass * desiredSpeedDelta;

        rb.AddForce(Vector2.right * forceRequired);

        animator.SetBool("Walking", desiredSpeed != 0);
    }
}
