using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class LaserFire : MonoBehaviour
{
    [Header("Laser Time Settings")]
    [SerializeField] private float fadeSpeed = 2f;  
    [SerializeField] private float laserDuration = 3f;
    [SerializeField] private float delayTime = 3f;

    [Header("Laser Settings")]
    [SerializeField] private GameObject laserstart;
    [SerializeField] private List<GameObject> laserendTargets; 
    [SerializeField] private bool isAlwaysShow;

    private LineRenderer lineRenderer;
    private Gradient laserGradient;
    private GradientAlphaKey[] alphaKeys;

    private EdgeCollider2D edgeCollider2D;
    private Vector2[] colliderPoints = new Vector2[2];

    private int currentTargetIndex = 0;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
        edgeCollider2D.enabled = false;

        laserGradient = lineRenderer.colorGradient;
        alphaKeys = laserGradient.alphaKeys;

        SetAlpha(0f);

        if (laserendTargets.Count > 0)
        {
            UpdateLaserTarget(); // 初始化激光目标
        }

        if (isAlwaysShow)
        {
            SetAlpha(1f);
            edgeCollider2D.enabled = true;
        }
        else
        {
            StartCoroutine(LaserRoutine());
        }
    }

    private IEnumerator LaserRoutine()
    {
        yield return new WaitForSeconds(delayTime);

        while (true)
        {
            yield return new WaitForSeconds(3f);

            // 显示激光
            yield return StartCoroutine(FadeAlpha(1f, true));

            yield return new WaitForSeconds(laserDuration);

            // 隐藏激光
            yield return StartCoroutine(FadeAlpha(0f, false));

            // 切换到下一个激光目标
            SwitchLaserTarget();
        }
    }

    private IEnumerator FadeAlpha(float targetAlpha, bool open)
    {
        float startAlpha = alphaKeys[0].alpha;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
        edgeCollider2D.enabled = open;
    }

    private void SetAlpha(float alpha)
    {
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = alpha;
        }

        laserGradient.alphaKeys = alphaKeys;
        lineRenderer.colorGradient = laserGradient;
    }

    private void SwitchLaserTarget()
    {
        if (laserendTargets.Count == 0) return;

        // 切换到下一个目标
        currentTargetIndex = (currentTargetIndex + 1) % laserendTargets.Count;

        UpdateLaserTarget();
    }

    private void UpdateLaserTarget()
    {
        if (laserendTargets.Count == 0) return;

        Vector3 start = laserstart.transform.position;
        Vector3 end = laserendTargets[currentTargetIndex].transform.position;

        // 更新 LineRenderer
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // 更新 EdgeCollider2D
        colliderPoints[0] = transform.InverseTransformPoint(start);
        colliderPoints[1] = transform.InverseTransformPoint(end);
        edgeCollider2D.SetPoints(new List<Vector2> { colliderPoints[0], colliderPoints[1] });

        Debug.DrawLine(start, end, Color.green, 1f); // 显示调试射线
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health.Instance.LoseLife();
        }
    }
}
