using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowMap : MonoBehaviour
{
    public List<GameObject> objectsToFade = new List<GameObject>(); // 需要改变透明度的物体
    public float fadeDuration = 0.5f; // 渐变时间
    private Dictionary<GameObject, Coroutine> activeFades = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 玩家进入
        {
            RevealObjects(false); // 显示区域
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 玩家离开
        {
            RevealObjects(true); // 还原透明度
        }
    }

    public void RevealObjects(bool show)
    {
        foreach (GameObject obj in objectsToFade)
        {
            if (obj != null)
            {
                if (activeFades.ContainsKey(obj))
                {
                    StopCoroutine(activeFades[obj]);
                    activeFades.Remove(obj);
                }

                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Coroutine fadeCoroutine = StartCoroutine(FadeSprite(sr, show));
                    activeFades[obj] = fadeCoroutine;
                }
            }
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, bool show)
    {
        float startAlpha = sr.color.a;
        float targetAlpha = show ? 1f : 0.3f; // 变透明（0.3f 可以调整）
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color newColor = sr.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            sr.color = newColor;
            yield return null;
        }

        Color finalColor = sr.color;
        finalColor.a = targetAlpha;
        sr.color = finalColor;

        activeFades.Remove(sr.gameObject);
    }
}
