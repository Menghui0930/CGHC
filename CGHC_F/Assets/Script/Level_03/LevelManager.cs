using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static Action<PlayerMotor> OnPlayerSpawn,LevelChange;

    public static LevelManager Instance;

    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float waitToRespawn;
    [SerializeField] private float moveSpeed = 3f; 
    private Vector3 SpawnPoint;

    public GameObject Player;
    public PlayerMotor _currentPlayer;

    private void Awake()
    {
        Instance = this;
        SpawnPoint = levelStartPoint.transform.position;
        SpawnPlayer(playerPrefab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RevivePlayer();
        }
    }
    // Revives our player
    private void RevivePlayer()
    {
        if (_currentPlayer != null)
        {
            _currentPlayer.gameObject.SetActive(true);
            _currentPlayer.SpawnPlayer(SpawnPoint);
            _currentPlayer.GetComponent<Health>().ResetLife();
            _currentPlayer.GetComponent<Health>().Revive();
        }
    }



    // Spawns our player in the spawnPoint   
    private void SpawnPlayer(GameObject player)
    {
        if (player != null)
        {
            Player = Instantiate(player, SpawnPoint, Quaternion.identity);
            _currentPlayer = Player.GetComponent<PlayerMotor>();
            _currentPlayer.GetComponent<Health>().ResetLife();

            // Call Event
            OnPlayerSpawn?.Invoke(_currentPlayer);
        }
    }

    public void SetSpawnpoint(Vector3 spawnPoint)
    {
        Debug.Log("Change SpawnPoint");
        SpawnPoint = spawnPoint;
        Debug.Log(SpawnPoint);
    }

    private void PlayerDeath(PlayerMotor player)
    {
        //_currentPlayer = player;
        _currentPlayer.gameObject.SetActive(false);
        StartCoroutine(RespawnCo());
    }

    private IEnumerator RespawnCo()
    {
        //stopcharacter
        yield return new WaitForSeconds(waitToRespawn - (1 / UIManager.Instance.fadeSpeed));
        UIManager.Instance.FadeToBlack();
        yield return new WaitForSeconds((1 / UIManager.Instance.fadeSpeed) + .2f);
        UIManager.Instance.FadeFromBlack();
        //character start
        RevivePlayer();
    }

    public void EndLevel(string levelToload)
    {
        Debug.Log("ExitLevel");
        StartCoroutine(EndLevelCo(levelToload));
        StartCoroutine(MovingFace());
    }

    private IEnumerator EndLevelCo(string LevelToload)
    {
        // Playerstop
        LevelChange?.Invoke(_currentPlayer);
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.FadeToBlack();
        yield return new WaitForSeconds((1f / UIManager.Instance.fadeSpeed) + .25f);
        SceneManager.LoadScene(LevelToload);
    }

    private IEnumerator MovingFace()
    {
        float moveTime = 3f;  
        float elapsedTime = 0f;

        PlayerController Player_controller = Player.GetComponent<PlayerController>();
        Animator animator = Player.GetComponent<Animator>();
        
        float moveDirection = Player_controller.FacingRight ? 1f : -1f;

        while (elapsedTime < moveTime)
        {
            animator.SetBool("Run", true);
            elapsedTime += Time.deltaTime;
            Player.transform.position += new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0, 0);
            yield return null;
        }
    }

    private void OnEnable()
    {
        Health.OnDeath += PlayerDeath;
        LevelExit.LevelLoad += EndLevel;
    }

    private void OnDisable()
    {
        Health.OnDeath -= PlayerDeath;
        LevelExit.LevelLoad -= EndLevel;
    }
}
