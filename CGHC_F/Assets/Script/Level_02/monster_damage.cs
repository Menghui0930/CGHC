using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock_damage : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Coconut");
            DamagePlayer();

        }
    }
    private void DamagePlayer()
    {
        Health.Instance.LoseLife();

    }
}
