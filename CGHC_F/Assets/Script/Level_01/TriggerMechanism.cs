using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMechanism : MonoBehaviour
{
    [SerializeField] private float MechanicNum;
    [SerializeField] public GameObject Mechanic;
    [SerializeField] public GameObject Targetpoint;
    [SerializeField] public float Speed;
    
    private bool isPlayerNearby = false;
    

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Press K");
            ActivateMechanism(); 
        }
    }

    private void ActivateMechanism()
    {
        if(MechanicNum == 1)
        {
            Debug.Log("start Mechanic");
            StartCoroutine(MoveToTarget());
        }

    }

    private IEnumerator MoveToTarget()
    {
        Debug.Log("Start Moving Mechanic...");

        while (Vector3.Distance(Mechanic.transform.position, Targetpoint.transform.position) > 0.1f)
        {
            Mechanic.transform.position = Vector3.MoveTowards(Mechanic.transform.position, Targetpoint.transform.position, Time.deltaTime * Speed);
            yield return null; // 等待下一帧
        }

        Debug.Log("Mechanic Reached Target!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            isPlayerNearby = true;
            Debug.Log("Player In the zone");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Player Out of zone");
        }
    }
}
