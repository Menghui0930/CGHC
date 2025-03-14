using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1_LevelManager : MonoBehaviour
{
    public static Action<TestMovement> OnPlayerSpawn;

    public static L1_LevelManager Instance;
    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;

    public GameObject characterInstance;
    public Vector3 startCameraPos;

    private TestMovement _currentPlayer;

    private void Awake()
    {
        Instance = this;
        SpawnPlayer(playerPrefab);
    }
    // Start is called before the first frame update
    void Start()
    {
        OnPlayerSpawn?.Invoke(_currentPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawns our player in the spawnPoint   
    private void SpawnPlayer(GameObject player)
    {
        if (player != null)
        {
            characterInstance = Instantiate(player, levelStartPoint.position, Quaternion.identity);
            _currentPlayer = characterInstance.GetComponent<TestMovement>();
            startCameraPos = characterInstance.transform.position;

        }
    }
}
