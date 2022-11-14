using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public GameObject armPivot;
    Rigidbody2D rb;
    public Transform feet;
    public SpriteRenderer sr;
    Combat combat;
    int groundLayer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        combat = GetComponent<Combat>();
        groundLayer = LayerMask.GetMask("Terrain");

        combat.OnKnockback += OnKnockback;
        combat.OnDeath += OnDeath;
    }

    // Update is called once per frame
    bool swingTool = false;
    public bool facingRight { get; private set; } = true;

    void OnDeath()
    {
        GameoverScreen.headerText = "Game over";
        UnityEngine.SceneManagement.SceneManager.LoadScene("Gameover");
    }
    void FaceDirection(bool right)
    {
        facingRight = right;
        sr.flipX = !right;
        var armPivot = transform.Find("ArmPivot");
        if (armPivot != null)
        {
            var pos = armPivot.localPosition;
            pos.x = Mathf.Abs(pos.x) * (!right ? 1 : -1);
            armPivot.localPosition = pos;
        }
        var pick = transform.Find("ArmPivot/pickaxe");
        if (pick != null)
        {
            var pos = pick.localPosition;
            pos.x = Mathf.Abs(pos.x) * (right ? 1 : -1);
            pick.localPosition = pos;
        }
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            swingTool = true;
            armPivot.SetActive(true);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            swingTool = false;
            armPivot.SetActive(false);
        }
        animator.SetBool("Swinging", swingTool);

        var animState = animator.GetCurrentAnimatorStateInfo(0);

        if (animState.IsName("Mine") || animState.IsName("SwingWalk"))
        {
            float pickaxeAnimPercentage = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("SwingWalk")) pickaxeAnimPercentage *= 2;

            var multiplier = (facingRight ? (1 - pickaxeAnimPercentage % 1 - 1) : (pickaxeAnimPercentage % 1));
            armPivot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, multiplier * 180));
        }
        ProcessMovement();
    }

    void OnKnockback(bool right)
    {
        var multiplier = right ? 1 : -1;
        var force = new Vector2(4f * multiplier, 3f) * 3;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void ProcessMovement()
    {
        bool canJump = false;
        {
            var collider = sr.GetComponent<BoxCollider2D>();
            Debug.DrawLine(new Vector3(collider.bounds.center.x, collider.bounds.min.y), new Vector3(collider.bounds.center.x, collider.bounds.min.y) + Vector3.up * 0.1f);
            var overlap = Physics2D.OverlapBox(new Vector3(collider.bounds.center.x, collider.bounds.min.y), new Vector2(collider.bounds.extents.x * 1.8f, 0.00001f), 0, groundLayer);
            if (overlap != null)
            {
                canJump = true;
            }
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(Vector2.up * rb.mass * 15.0f, ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            var loadTilemaps = GameObject.Find("Scene Scripts").GetComponent<LoadTilemap>();
            loadTilemaps.maps[TileGroup.ForegroundBasic].SetTile(new Vector3Int(400, 0), loadTilemaps.tiles[2]);
        }

        float currentSpeed = rb.velocity.x;

        float desiredSpeedDelta = Mathf.Round(desiredSpeed - currentSpeed);

        float forceRequired = 10 * rb.mass * (desiredSpeedDelta != 0 ? Mathf.Sign(desiredSpeedDelta) : 0);
        if (desiredSpeed == 0) forceRequired /= 4f;
        if (Mathf.Abs(desiredSpeedDelta) < 1 && desiredSpeed == 0) forceRequired = 0;

        if (forceRequired != 0)
        {
            if (desiredSpeed != 0)
            {
                var desiredDirection = Mathf.Sign(desiredSpeed);
                FaceDirection(desiredDirection > 0);
            }
        }

        rb.AddForce(Vector2.right * forceRequired);

        animator.SetBool("Walking", desiredSpeed != 0);
    }
}
