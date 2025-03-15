using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private AIState currentState;
    [SerializeField] private AIState remainState;

    // Reference of the Path Follow
    public PathFollow Path { get; set; }

    private void Start()
    {
        Path = GetComponent<PathFollow>();
    }

    private void Update()
    {
        currentState.RunState(this);
    }

    // Update our State to a new one
    public void TransitionToState(AIState newState)
    {
        if (newState != remainState)
        {
            currentState = newState;
        }
    }
}

