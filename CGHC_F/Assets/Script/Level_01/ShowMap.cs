using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ShowMap : MonoBehaviour
{
    public static ShowMap Instance;

    public List<GameObject> objectsToHide = new List<GameObject>();
    public float fadeDuration = 10.0f;

    private Dictionary<GameObject, Coroutine> activeFades = new Dictionary<GameObject, Coroutine>();


    private void Awake()
    {
        Instance = this;
    }

    public void RevealObjects(bool Hide)
    {
        foreach (GameObject obj in objectsToHide) 
        {
            if (obj != null)
            {
                if (activeFades.ContainsKey(obj))
                {
                    StopCoroutine(activeFades[obj]);
                    activeFades.Remove(obj);
                }

                // 启动新的淡入淡出协程，并记录它
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                Coroutine fadeCoroutine = StartCoroutine(FadeSprite(sr, Hide));
                activeFades[obj] = fadeCoroutine;


            }
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, bool show)
    {
        float startAlpha = sr.color.a;
        float targetAlpha = show ? 1f : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            Color newColor = sr.color;
            newColor.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            sr.color = newColor;
            yield return null;
        }

        // 确保最终 alpha 设为目标值
        Color finalColor = sr.color;
        finalColor.a = targetAlpha;
        sr.color = finalColor;

        // 移除已完成的协程记录
        activeFades.Remove(sr.gameObject);
    }
}
