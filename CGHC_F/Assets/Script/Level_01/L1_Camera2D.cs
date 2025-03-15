using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1_Camera2D : MonoBehaviour
{
    public static L1_Camera2D instance;
    [Header("Horizontal")]
    [SerializeField] private bool horizontalFollow = true;
    [SerializeField] private bool verticalFollow = true;

    [Header("Horizontal")]
    [SerializeField][Range(0, 1)] private float horizontalInfluence = 1f;
    [SerializeField] private float horizontalOffset = 0f;
    [SerializeField] private float horizontalSmoothness = 3f;

    [Header("Vertical")]
    [SerializeField][Range(0, 1)] private float verticalInfluence = 1f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float verticalSmoothness = 3f;

    private bool startStory = false;

    // The target reference    
    public TestMovement Target { get; set; }

    // Position of the Target  
    public Vector3 TargetPosition { get; set; }

    // Reference of the Target Position known by the Camera    
    public Vector3 CameraTargetPosition { get; set; }

    private float _targetHorizontalSmoothFollow;
    private float _targetVerticalSmoothFollow;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        MoveCamera();
    }

    // Moves our Camera
    private void MoveCamera()
    {

        if (Target == null)
        {
            Debug.Log("no character");
            return;
        }

  
        TargetPosition = GetTargetPosition(Target);
        CameraTargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, 0f);

        float xPos = horizontalFollow ? CameraTargetPosition.x : transform.localPosition.x;
        float yPos = verticalFollow ? CameraTargetPosition.y : transform.localPosition.y;

        // Set offset
        CameraTargetPosition += new Vector3(horizontalFollow ? horizontalOffset : 0f, verticalFollow ? verticalOffset : 0f, 0f);

        // Set smooth value
        _targetHorizontalSmoothFollow = Mathf.Lerp(_targetHorizontalSmoothFollow, CameraTargetPosition.x,
            horizontalSmoothness * Time.deltaTime);

        _targetVerticalSmoothFollow = Mathf.Lerp(_targetVerticalSmoothFollow, CameraTargetPosition.y,
            verticalSmoothness * Time.deltaTime);

        // Get direction towards target pos
        float xDirection = _targetHorizontalSmoothFollow - transform.localPosition.x;
        float yDirection = _targetVerticalSmoothFollow - transform.localPosition.y;
        Vector3 deltaDirection = new Vector3(xDirection, yDirection, 0f);

        // New position
        Vector3 newCameraPosition = transform.localPosition + deltaDirection;

        // Apply new position
        transform.localPosition = new Vector3(newCameraPosition.x, newCameraPosition.y, transform.localPosition.z);
    }


    // Returns the position of out target
    private Vector3 GetTargetPosition(TestMovement player)
    {
        float xPos = 0f;
        float yPos = 0f;

        xPos += (player.transform.position.x + horizontalOffset) * horizontalInfluence;
        yPos += (player.transform.position.y + verticalOffset) * verticalInfluence;
        Vector3 positionTarget = new Vector3(xPos, yPos, transform.position.z);
        return positionTarget;
    }

    // Centers our camera in the target position
    private void CenterOnTarget(TestMovement player)
    {
        Target = player;

        Vector3 targetPos = GetTargetPosition(Target);
        _targetHorizontalSmoothFollow = targetPos.x;
        _targetVerticalSmoothFollow = targetPos.y;

        transform.localPosition = targetPos;
    }

    // Reset the target reference
    private void StopFollow(TestMovement player)
    {
        Target = null;
    }

    // Gets Target reference and center our camera
    private void StartFollowing(TestMovement player)
    {
        Target = player;
        CenterOnTarget(Target);
    }

    public void UpdateCameraOffset(float newHorizontalOffset, float newVerticalOffset)
    {
        if(startStory)
        {
            return;
        }
        horizontalOffset = newHorizontalOffset;
        verticalOffset = newVerticalOffset;
        Debug.Log("Offset");
    }

    public void StopFollowStory(float newHorizontalOffset, float newVerticalOffset, float newHorizontalSmoothness, float newverticalSmoothness, float CameraTime)
    {
        startStory = true;
        StartCoroutine(StartCameraStory(newHorizontalOffset, newVerticalOffset, newHorizontalSmoothness, newverticalSmoothness,CameraTime));
    }

    public IEnumerator StartCameraStory(float newHorizontalOffset, float newVerticalOffset, float newHorizontalSmoothness, float newverticalSmoothness, float CameraTime)
    {
        float originalHorizontalOffset = horizontalOffset;
        float originalVerticalOffset = verticalOffset;
        horizontalOffset = newHorizontalOffset;
        verticalOffset = newVerticalOffset;
        horizontalSmoothness = newHorizontalSmoothness;
        verticalSmoothness = newverticalSmoothness;

        yield return new WaitForSeconds(CameraTime);
        startStory = false;
        horizontalOffset = originalHorizontalOffset;
        verticalOffset = originalVerticalOffset;

        float duration = 5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; 

            horizontalSmoothness = Mathf.Lerp(newHorizontalSmoothness, 3f, t);
            verticalSmoothness = Mathf.Lerp(newverticalSmoothness, 3f, t);

            yield return null; 
        }

        horizontalSmoothness = 3f;
        verticalSmoothness = 3f;

    }



    private void OnEnable()
    {
        // +=：将 CenterOnTarget 方法添加为事件的监听器

        // 一开始游戏Spawn Player 会传送 这个事件 并执行 CenterOnTarget
        // CenterOnTarget 会定义Camera Target 就是 Player
        L1_LevelManager.OnPlayerSpawn += CenterOnTarget;

    }

    private void OnDisable()
    {
        // -= 也可理解为取消订阅
        L1_LevelManager.OnPlayerSpawn -= CenterOnTarget;

    }
}
