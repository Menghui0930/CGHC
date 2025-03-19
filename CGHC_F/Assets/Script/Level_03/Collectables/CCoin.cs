using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCoin : Collectable
{
    [Header("Settings")]
    [SerializeField] private int amountToAdd = 1;
    private bool isCollected = false; 

    protected override void Collect()
    {
        if (isCollected) return;
        isCollected = true;
        Debug.Log("Collect Called!");
        AddCoin();
    }

    // Adds coins to our Global counter
    private void AddCoin()
    {
        CoinManager.Instance.AddCoins(amountToAdd);
        UIManager.Instance.AddMoney(amountToAdd);
    }

}
