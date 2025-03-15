using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    public static Action<string> LevelLoad;
    public string LevelToLoad;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            LevelLoad?.Invoke(LevelToLoad);
        }
    }
}
