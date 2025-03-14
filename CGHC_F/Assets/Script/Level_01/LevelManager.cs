using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action<PlayerMovement> OnPlayerSpawn;

    public static LevelManager Instance;
    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;

    public GameObject characterInstance;
    public Vector3 startCameraPos;

    private PlayerMovement _currentPlayer;

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
            _currentPlayer = characterInstance.GetComponent<PlayerMovement>();
            startCameraPos = characterInstance.transform.position;

        }
    }
}
