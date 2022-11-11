using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    Rigidbody2D rb;
    Combat combat;

    float nextJump = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        combat = GetComponent<Combat>();
        combat.OnDeath += OnDeath;
        combat.OnDamage += OnDamage;
    }

    void OnDeath()
    {
        print("ow oof i'm dead");
    }
    void OnDamage(bool right)
    {
        print("ow oof i'm damaged");
    }

    // Update is called once per frame
    void Update()
    {
        var dist = player.position - transform.position;
        if (dist.magnitude > 20) return;

        if (dist.y >= 1 && Time.time >= nextJump)
        {
            nextJump = Time.time + 0.8f;
            rb.AddForce(Vector2.up * rb.mass * 12.0f, ForceMode2D.Impulse);
        }

        var wantsDirection = (Mathf.Abs(dist.x) > 0.1) ? Mathf.Sign(dist.x) : 0;

        var desiredSpeed = 2.0f * wantsDirection;

        float currentSpeed = rb.velocity.x;

        float desiredSpeedDelta = Mathf.Round(desiredSpeed - currentSpeed);

        float forceRequired = 10 * rb.mass * Mathf.Sign(desiredSpeedDelta);
        if (desiredSpeed == 0) forceRequired /= 4f;
        if (Mathf.Abs(desiredSpeedDelta) < 1 && desiredSpeed == 0) forceRequired = 0;

        rb.AddForce(Vector2.right * forceRequired);
    }
}
