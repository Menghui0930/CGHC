using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Health")]
    [SerializeField] private Image HealthBar;
    [SerializeField] private Sprite[] HealthImage;
    [SerializeField] private Sprite[] HealthLoseImage;

    [Header("HealthShake")]
    public RectTransform HealthRect;
    public float shakeAmount;
    public float shakeSpeed;

    [Header("Coin")]
    public Image CoinImage;  
    public RectTransform CoinRect; 
    public Text CoinText;           

    private int currentMoney = 0;
    private Coroutine fadeCoroutine;
    private float lastCoinPickupTime;


    [Header("Fade Screen")]
    public Image fadeScreen;
    public float fadeSpeed;
    public bool shouldFadeToBlack, shouldFadeFromBlack;

   

    private void Start()
    {
        CoinImage.color = new Color(1, 1, 1, 0);
        CoinText.color = new Color(1,1,1,0f);
        CoinImage.gameObject.SetActive(false);
        CoinText.gameObject.SetActive(false);
        FadeFromBlack();
    }
    private void Update()
    {
        if (shouldFadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 1f)
            {
                shouldFadeToBlack = false;
            }
        }

        if (shouldFadeFromBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 0f)
            {
                shouldFadeFromBlack = false;
            }
        }

    }

    #region Health
    // Updates the player lifes
    private void OnPlayerLifes(int currentLifes,string status)
    {
        Debug.Log(currentLifes);
        if (status == "Hurt")
        {
            switch (currentLifes)
            {
                case 5:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes - 1]));
                    break;

                case 4:
                    HealthBar.sprite = HealthLoseImage[currentLifes + 1];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes ]));
                    break;

                case 3:
                    HealthBar.sprite = HealthLoseImage[currentLifes + 1];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes ]));
                    break;

                case 2:
                    HealthBar.sprite = HealthLoseImage[currentLifes + 1];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 1:
                    HealthBar.sprite = HealthLoseImage[currentLifes + 1];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 0:
                    HealthBar.sprite = HealthLoseImage[currentLifes + 1];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;
            }
        }
        else if (status == "Heal")
        {
            switch (currentLifes)
            {
                case 5:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 4:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 3:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 2:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 1:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;

                case 0:
                    HealthBar.sprite = HealthLoseImage[currentLifes];
                    StartCoroutine(ShakeBar(HealthImage[currentLifes]));
                    break;
            }

        }

    }
    #endregion

    #region Coin
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        CoinText.text = currentMoney.ToString();

        if (fadeCoroutine != null)
        {
            Debug.Log("Cancel hide");
            StopCoroutine(fadeCoroutine);

        }

        StartCoroutine(ShowAndAnimateUI());
    }

    private IEnumerator ShowAndAnimateUI()
    {
        lastCoinPickupTime = Time.time;
        CoinImage.gameObject.SetActive(true);
        CoinText.gameObject.SetActive(true);

        CoinImage.color = new Color(1, 1, 1, 1);
        CoinText.color = new Color(1, 1, 1, 1);

        Vector3 originalScale = Vector3.one;
        Vector3 bigScale = Vector3.one * 1.2f;

        float duration = 0.3f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            CoinRect.localScale = Vector3.Lerp(originalScale, bigScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < duration)
        {
            CoinRect.localScale = Vector3.Lerp(bigScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        CoinRect.localScale = originalScale;

        yield return new WaitForSeconds(5f);

        fadeCoroutine = StartCoroutine(FadeOutUI());
    }

    private IEnumerator FadeOutUI()
    {

        while (Time.time < lastCoinPickupTime + 5f)
        {
            yield return null;
        }

        float fadeDuration = 1f;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            Color currentColor = CoinImage.color;
            currentColor.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            CoinImage.color = currentColor;
            CoinText.color = currentColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        CoinImage.color = new Color(1, 1, 1, 0);
        CoinText.color = new Color(1, 1, 1, 0);
        CoinImage.gameObject.SetActive(false);
        CoinText.gameObject.SetActive(false);
    }

    private IEnumerator ShakeBar(Sprite HealthBarShake)
    {

        Vector3 originalPosition = HealthRect.localPosition;

        float elapsedTime = 0f;
        float totalShakeDuration = 0.3f;

        while (elapsedTime < totalShakeDuration)
        {
            // Calculate the current vibration offset
            float shakeOffset = Mathf.Sin(elapsedTime * Mathf.PI * shakeSpeed) * shakeAmount;
            HealthRect.localPosition = new Vector3(originalPosition.x, originalPosition.y + shakeOffset, originalPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // After the vibration ends, return to the original position
        HealthRect.localPosition = originalPosition;
        HealthBar.sprite = HealthBarShake;
    }
    #endregion

    public void FadeToBlack()
    {
        shouldFadeToBlack = true;
        shouldFadeFromBlack = false;

    }

    public void FadeFromBlack()
    {
        shouldFadeToBlack = false;
        shouldFadeFromBlack = true;
    }

    private void OnEnable()
    {

        Health.OnLifesChanged += OnPlayerLifes;
    }

    private void OnDisable()
    {
        Health.OnLifesChanged -= OnPlayerLifes;
    }
}
