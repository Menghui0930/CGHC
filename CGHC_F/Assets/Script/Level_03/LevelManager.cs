using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static Action<PlayerMotor> OnPlayerSpawn;

    public static LevelManager Instance;

    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;

    public GameObject Player;
    public PlayerMotor _currentPlayer;

    private void Awake()
    {
        Instance = this;
        SpawnPlayer(playerPrefab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RevivePlayer();
        }
    }

    // Spawns our player in the spawnPoint   
    private void SpawnPlayer(GameObject player)
    {
        if (player != null)
        {
            Player = Instantiate(player, levelStartPoint.position, Quaternion.identity);
            _currentPlayer = Player.GetComponent<PlayerMotor>();
            _currentPlayer.GetComponent<Health>().ResetLife();

            // Call Event
            OnPlayerSpawn?.Invoke(_currentPlayer);
        }
    }

    // Revives our player
    private void RevivePlayer()
    {
        if (_currentPlayer != null)
        {
            _currentPlayer.gameObject.SetActive(true);
            _currentPlayer.SpawnPlayer(levelStartPoint);
            _currentPlayer.GetComponent<Health>().ResetLife();
            _currentPlayer.GetComponent<Health>().Revive();
        }
    }

    private void PlayerDeath(PlayerMotor player)
    {
        //_currentPlayer = player;
        _currentPlayer.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Health.OnDeath += PlayerDeath;
    }

    private void OnDisable()
    {
        Health.OnDeath -= PlayerDeath;
    }
}
