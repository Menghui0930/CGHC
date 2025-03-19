using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject CheckPointText;
    [SerializeField] private float fadeDuration = 0.5f; // 淡入淡出时间
    [SerializeField] private float displayTime = 2f; // 显示时间
    [SerializeField] private float moveSpeed = 20f; // 文字上移速度

    private TextMeshPro textMesh;
    private RectTransform textRect;

    private void Start()
    {
        textMesh = CheckPointText.GetComponent<TextMeshPro>();
        textRect = CheckPointText.GetComponent<RectTransform>();
        textMesh.color = new Color(1,1,1,0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("CheckPoint");
            LevelManager.Instance.SetSpawnpoint(transform.position);
            StartCoroutine(ShowCheckpointText());
        }
    }

    private IEnumerator ShowCheckpointText()
    {
        yield return StartCoroutine(FadeText(0, 1, fadeDuration)); // 淡入
        yield return new WaitForSeconds(displayTime); // 停留
        yield return StartCoroutine(FadeText(1, 0, fadeDuration)); // 淡出
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {

        float elapsed = 0f;
        Vector3 startPos = textRect.position; // 记录初始位置

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            textMesh.alpha = alpha;

            textRect.position = startPos + new Vector3(0, elapsed * moveSpeed);
            yield return null;
        }

        textMesh.alpha = endAlpha; // 确保最终值正确
    }
}
