using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Combat combat;

    bool facingRight;

    float nextJump = 0;
    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        combat = GetComponent<Combat>();
        combat.OnDeath += OnDeath;
        combat.OnDamage += OnDamage;
        combat.OnKnockback += (right) =>
        {
            var multiplier = right ? 1 : -1;
            var force = new Vector2(4f * multiplier, 3f) * 3;
            rb.AddForce(force, ForceMode2D.Impulse);
        };
    }

    void OnDeath()
    {
        print("ow oof i'm dead");

        GameoverScreen.headerText = "You win";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameover");
        // Destroy(gameObject);
    }
    void OnDamage(int amount)
    {
        print("ow oof i'm damaged");
    }

    // Update is called once per frame
    void Update()
    {
        var dist = player.position - transform.position;
        if (dist.magnitude > 20) return;

        if (dist.y >= 0.9f && Time.time >= nextJump)
        {
            nextJump = Time.time + 0.8f;
            rb.AddForce(Vector2.up * rb.mass * 12.0f, ForceMode2D.Impulse);
        }

        var wantsDirection = (Mathf.Abs(dist.x) > 0.1) ? Mathf.Sign(dist.x) : 0;

        if (wantsDirection == 1)
        {
            facingRight = true;
        }
        else if (wantsDirection == -1)
        {
            facingRight = false;
        }

        sr.flipX = !facingRight;

        var desiredSpeed = 2.0f * wantsDirection;

        float currentSpeed = rb.velocity.x;

        float desiredSpeedDelta = Mathf.Round(desiredSpeed - currentSpeed);

        float forceRequired = 10 * rb.mass * Mathf.Sign(desiredSpeedDelta);
        if (desiredSpeed == 0) forceRequired /= 4f;
        if (Mathf.Abs(desiredSpeedDelta) < 1 && desiredSpeed == 0) forceRequired = 0;

        rb.AddForce(Vector2.right * forceRequired * Time.deltaTime * 250);
    }

    float nextAttack = 0;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Time.time < nextAttack || !(collision.transform.parent?.CompareTag("Player") ?? false)) return;

        collision.transform.parent.GetComponent<Combat>().DealDamage(10, facingRight);
        nextAttack = Time.time + 1;
    }
}
