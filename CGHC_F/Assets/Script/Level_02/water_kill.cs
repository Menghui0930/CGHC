using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Coconut");
            PlayerDie();

        }
    }
    private void PlayerDie()
    {
        Health.Instance.KillPlayer();

    }
}
