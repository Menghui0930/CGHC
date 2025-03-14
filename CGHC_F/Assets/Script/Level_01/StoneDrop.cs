using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneDrop : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float minDistanceToPoint = 0.1f;

    [SerializeField] private float rotateSpeed = 90f;

    public List<Vector3> points = new List<Vector3>();

    private bool _playing;
    private bool _moved;
    private int _currentPoint = 0;
    private Vector3 _currentPosition;
    private Vector3 _previousPosition;

    private bool _isWaiting = false;
    private bool isleft = false;
    public bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        _playing = true;
        _previousPosition = transform.position;
        _currentPosition = transform.position;
        if (points.Count > 1)
        {
            transform.position = _currentPosition + points[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            if (!_isWaiting)
            {
                Move();
                RotateObject();

                if (_currentPoint == 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void RotateObject()
    {
        if (isleft)
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
        }
    }

    private void Move()
    {
        if (!_moved)
        {
            transform.position = _currentPosition + points[0];
            _currentPoint++;
            _moved = true;
        }

        transform.position = Vector3.MoveTowards(transform.position, _currentPosition + points[_currentPoint], Time.deltaTime * moveSpeed);

        float distanceToNextPoint = Vector3.Distance(_currentPosition + points[_currentPoint], transform.position);

        if (distanceToNextPoint < minDistanceToPoint)
        {
            _previousPosition = transform.position;
            _currentPoint++;

            if (_currentPoint == points.Count)
            {
                _currentPoint = 0;
            }

            isleft = !isleft;
        }
    }


    private void OnDrawGizmos()
    {
        if (transform.hasChanged && !_playing)
        {
            _currentPosition = transform.position;
        }

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
