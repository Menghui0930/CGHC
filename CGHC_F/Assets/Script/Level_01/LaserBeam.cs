using System.Collections;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public Transform firePoint; 
    public float maxDistance = 10f; 
    public LayerMask obstacleLayer; 

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider; 
    private Vector2[] colliderPoints = new Vector2[2]; 

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        edgeCollider.isTrigger = true; 
    }

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        if (!lineRenderer || !firePoint) return;

        Vector3 direction = firePoint.up;

        // 发射 Raycast
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, maxDistance, obstacleLayer);

        Vector3 endPoint = firePoint.position + direction * maxDistance; 
        if (hit.collider != null)
        {
            endPoint = hit.point; 
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);


        Vector2 localStart = transform.InverseTransformPoint(firePoint.position);
        Vector2 localEnd = transform.InverseTransformPoint(endPoint);
        edgeCollider.SetPoints(new System.Collections.Generic.List<Vector2> { localStart, localEnd });



        Debug.DrawLine(firePoint.position, endPoint, Color.red, 0.1f);
    }

 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health.Instance.LoseLife();
        }
    }


    void OnDrawGizmos()
    {
        if (firePoint == null) return;

        Vector3 direction = firePoint.up;
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, direction, maxDistance, obstacleLayer);

        Vector3 endPoint = firePoint.position + direction * maxDistance;
        if (hit.collider != null)
        {
            endPoint = hit.point;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(firePoint.position, endPoint);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(endPoint, 0.1f);
    }

}
