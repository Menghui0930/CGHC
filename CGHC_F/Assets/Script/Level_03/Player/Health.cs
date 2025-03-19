using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static Health Instance;
    public static Action<int,string> OnLifesChanged;
    public static Action<PlayerMotor> OnDeath;
    public static Action<PlayerMotor> OnRevive;

    [Header("Settings")]
    [SerializeField] private int lifes = 3;
    [SerializeField] private float invincibilityDuration = 1.0f; // 无敌时间
    [SerializeField] private float blinkInterval = 0.2f; // 闪烁间隔

    public int MaxLifes => _maxLifes;

    public int CurrentLifes => _currentLifes;

    private int _maxLifes;
    private int _currentLifes;

    private string status;
    private bool invincible = false; // 是否处于无敌状态
    private float invincibilityTimer = 0f;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        Instance = this;
        _maxLifes = lifes;
        _spriteRenderer = GetComponent<SpriteRenderer>();
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

        // 处理无敌时间倒计时
        if (invincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                invincible = false;
                StopCoroutine("BlinkEffect");
                _spriteRenderer.color = new Color(1, 1, 1, 1); // 还原颜色
            }
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

        if (invincible) return; // 如果无敌，跳过扣血

        _currentLifes -= 1;
        if (_currentLifes <= 0)
        {
            _currentLifes = 0;
            OnDeath?.Invoke(gameObject.GetComponent<PlayerMotor>());
        }
        status = "Hurt";
        UpdateLifesUI();

        // 进入无敌状态
        invincible = true;
        invincibilityTimer = invincibilityDuration;
        StartCoroutine(Invincibility()); // 触发无敌时间
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

    private IEnumerator Invincibility()
    {
        invincible = true;
        StartCoroutine(BlinkEffect()); // 开始闪烁效果

        yield return new WaitForSeconds(invincibilityDuration); // 等待无敌时间结束
        invincible = false;

        // 确保恢复正常透明度
        _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator BlinkEffect()
    {
        while (invincible)
        {
            for (float t = 0; t < blinkInterval; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(1f, 0.3f, t / blinkInterval);
                _spriteRenderer.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            for (float t = 0; t < blinkInterval; t += Time.deltaTime)
            {
                float alpha = Mathf.Lerp(0.3f, 1f, t / blinkInterval);
                _spriteRenderer.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
        }

        _spriteRenderer.color = new Color(1, 1, 1, 1); // 结束后恢复正常
    }

}


