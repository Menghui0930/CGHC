using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarMove : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float waitTime = 3f; 
    [SerializeField] private float delayTime = 3f; 

    public List<Vector3> points = new List<Vector3>();

    private int _currentIndex = 0;
    private Vector3 _currentPosition;

    private void Start()
    {
        if (points.Count < 1)
        {
            enabled = false;
            return;
        }
        _currentPosition = transform.position;
        transform.position = _currentPosition + points[0]; 
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(delayTime);

        while (true)
        {
            int nextIndex = (_currentIndex + 1) % points.Count; 

            if (_currentIndex == 0) 
            {
                yield return new WaitForSeconds(waitTime);
            }

            while (Vector3.Distance(transform.position, _currentPosition + points[nextIndex]) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _currentPosition + points[nextIndex], moveSpeed * Time.deltaTime);
                yield return null;
            }

            _currentIndex = nextIndex; 
        }
    }

    private void OnDrawGizmos()
    {
        

        if (points != null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (i < points.Count)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(_currentPosition + points[i], 0.4f);

                    Gizmos.color = Color.black;
                    if (i < points.Count - 1)
                    {
                        Gizmos.DrawLine(_currentPosition + points[i], _currentPosition + points[i + 1]);
                    }

                    if (i == points.Count - 1)
                    {
                        Gizmos.DrawLine(_currentPosition + points[i], _currentPosition + points[0]);
                    }
                }
            }
        }
    }
}
