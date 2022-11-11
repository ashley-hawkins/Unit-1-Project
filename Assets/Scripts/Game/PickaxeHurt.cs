using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeHurt : MonoBehaviour
{
    // Start is called before the first frame update
    public Player player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision);
        Combat combat = collision.GetComponent<Combat>();
        if (combat == null || combat.CompareTag("Player")) return;
        print("oh");
        combat.DealDamage(10, player.facingRight);
    }
}
