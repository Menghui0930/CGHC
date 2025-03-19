using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMechanism : MonoBehaviour
{
    [Header("Num1")]
    [SerializeField] private float MechanicNum;
    [SerializeField] public GameObject Mechanic;
    [SerializeField] public GameObject Targetpoint;
    [SerializeField] public float Speed;

    [SerializeField] public GameObject Target;
    [SerializeField] public bool isLaser;

    private bool isPlayerNearby = false;

    private void Start()
    {

    }
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Press K");
            if (isLaser)
            {
                if (Target.GetComponent<LineRenderer>().enabled)
                {
                    Target.GetComponent<LineRenderer>().enabled = false;
                    Target.GetComponent<EdgeCollider2D>().enabled = false;
                }
                else
                {
                    Target.GetComponent<LineRenderer>().enabled = true;
                    Target.GetComponent<EdgeCollider2D>().enabled = true;
                }


            }
            else
                ActivateMechanism(); 
        }

        if (isPlayerNearby && MechanicNum == 3)
        {
            Debug.Log("start Mechanic 03");
            StoneDrop stoneDrop = Mechanic.GetComponent<StoneDrop>();
            stoneDrop.isStart = true;
        }
    }

    private void ActivateMechanism()
    {
        if(MechanicNum == 1)
        {
            Debug.Log("start Mechanic 01");
            StartCoroutine(MoveToTarget());
        }
        if (MechanicNum == 2)
        {
            Debug.Log("start Mechanic 02");
            StartCoroutine(MoveToTarget());
        }
        

    }

    private IEnumerator MoveToTarget()
    {
        Debug.Log("Start Moving Mechanic...");

        while (Vector3.Distance(Mechanic.transform.position, Targetpoint.transform.position) > 0.1f)
        {
            Mechanic.transform.position = Vector3.MoveTowards(Mechanic.transform.position, Targetpoint.transform.position, Time.deltaTime * Speed);
            yield return null; 
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
