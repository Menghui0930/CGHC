using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static Action<int,string> OnLifesChanged;
    public static Action<PlayerMotor> OnDeath;
    public static Action<PlayerMotor> OnRevive;

    [Header("Settings")]
    [SerializeField] private int lifes = 3;

    public int MaxLifes => _maxLifes;

    public int CurrentLifes => _currentLifes;

    private int _maxLifes;
    private int _currentLifes;

    private string status;

    private void Awake()
    {
        _maxLifes = lifes;
    }

    private void Start()
    {
        ResetLife();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            LoseLife();
        }
    }

    public void AddLife()
    {
        _currentLifes += 1;
        if (_currentLifes > _maxLifes)
        {
            _currentLifes = _maxLifes;
        }
        status = "Heal";
        UpdateLifesUI();
    }

    public void LoseLife()
    {
        _currentLifes -= 1;
        if (_currentLifes <= 0)
        {
            _currentLifes = 0;
            OnDeath?.Invoke(gameObject.GetComponent<PlayerMotor>());
        }
        status = "Hurt";
        UpdateLifesUI();
    }

    public void KillPlayer()
    {
        _currentLifes = 0;
        status = "Hurt";
        UpdateLifesUI();
        OnDeath?.Invoke(gameObject.GetComponent<PlayerMotor>());
    }

    public void ResetLife()
    {
        _currentLifes = lifes;
        status = "Heal";
        UpdateLifesUI();
    }

    public void Revive()
    {
        // Camera Follow
        OnRevive?.Invoke(gameObject.GetComponent<PlayerMotor>());
    }

    private void UpdateLifesUI()
    {
        // UIManager
        OnLifesChanged?.Invoke(_currentLifes,status);
    }

    
}


