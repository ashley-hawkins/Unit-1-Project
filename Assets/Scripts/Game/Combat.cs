using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public int maxHealth = 100;
    public event System.Action OnDeath = delegate { };
    public event System.Action<bool> OnDamage = delegate { };

    int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void DealDamage(int damageAmount, bool fromRight)
    {
        currentHealth -= System.Math.Min(damageAmount, System.Math.Max(currentHealth, 0));
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }
}
