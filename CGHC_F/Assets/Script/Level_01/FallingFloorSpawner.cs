using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingFloorSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject FloorPrefab; 
    [SerializeField] private Transform startPoint; 
    [SerializeField] private Transform endPoint; 
    [SerializeField] private float fallSpeed = 2f; 
    [SerializeField] private float spawnInterval = 3f; 

    private void Start()
    {
        StartCoroutine(SpawnPlanks());
    }

    private IEnumerator SpawnPlanks()
    {
        while (true)
        {
            GameObject floor = Instantiate(FloorPrefab, startPoint.position, Quaternion.identity);
            StartCoroutine(MovePlank(floor));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator MovePlank(GameObject floor)
    {
        while (floor != null && floor.transform.position.y > endPoint.position.y)
        {
            floor.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        if (floor != null)
        {
            Destroy(floor);
        }
    }
}
