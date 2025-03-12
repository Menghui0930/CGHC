using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2D : MonoBehaviour
{
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

    // The target reference    
    public PlayerMovement Target { get; set; }

    // Position of the Target  
    public Vector3 TargetPosition { get; set; }

    // Reference of the Target Position known by the Camera    
    public Vector3 CameraTargetPosition { get; set; }

    private float _targetHorizontalSmoothFollow;
    private float _targetVerticalSmoothFollow;

    /*private void Awake()    // REMOVE this because we will center the Camera 
    {					// when the Player is spawned using the CenterOnTarget() method
        CenterOnTarget(playerToFollow);
    }*/


    private void Update()
    {
        MoveCamera();
    }

    // Moves our Camera
    private void MoveCamera()
    {
        // 一个 check 确保玩家有在游戏中
        if (Target == null)
        {
            Debug.Log("no character");
            return;
        }

        // Calculate Position
        //TargetPosition = GetTargetPosition(playerToFollow);
        TargetPosition = GetTargetPosition(Target);
        CameraTargetPosition = new Vector3(TargetPosition.x, TargetPosition.y, 0f);

        // condition ? value_if_true : value_if_false;
        // int x = (5 > 3) ? 10 : 20;
        // 5 > 3 是 true，所以 x = 10

        //所以如果 horizontalFollow 为 True 就采用角色的x轴 为 False 就选摄像机x轴（不动）

        // Follow on selected axis
        float xPos = horizontalFollow ? CameraTargetPosition.x : transform.localPosition.x;
        float yPos = verticalFollow ? CameraTargetPosition.y : transform.localPosition.y;

        // 如果 horizontalFollow 为 true，在相机目标位置上增加偏移
        // Set offset
        CameraTargetPosition += new Vector3(horizontalFollow ? horizontalOffset : 0f, verticalFollow ? verticalOffset : 0f, 0f);

        // 一开始 Camera 就会 Focus去玩家身上了 所以一开始就是玩家的location
        // Camera 与 玩家重叠 因此 CameraTargetPosition与玩家 Location相同
        // horizontalSmoothness = 3，Time.deltaTime = 0.016（假设 60FPS）
        // _targetHorizontalSmoothFollow = Mathf.Lerp(-21.6, -21.6, 3 * 0.016); // t ≈ 0.048
        // 由于 A点 和 B点 同样 不管怎么抽0.048 都是 -21.6 等于没有变化

        // 假如第二次移动 
        // _targetHorizontalSmoothFollow = Mathf.Lerp(-21.6, -20.91, 0.23);
        // _targetHorizontalSmoothFollow = Mathf.Lerp( 当前相机位置, 角色位置, 0.23);
        // 公式： Result=a+(b−a)×t
        // Result=−21.6+(−20.91−(−21.6))×0.23
        // Result=−21.6+(0.69×0.23)
        // Result=−21.6+0.1587
        // Result≈−21.4413

        // 这就是 从 -21.6 到 -20.9 之间抽 0.23 的结果

        // Set smooth value
        _targetHorizontalSmoothFollow = Mathf.Lerp(_targetHorizontalSmoothFollow, CameraTargetPosition.x,
            horizontalSmoothness * Time.deltaTime);

        _targetVerticalSmoothFollow = Mathf.Lerp(_targetVerticalSmoothFollow, CameraTargetPosition.y,
            verticalSmoothness * Time.deltaTime);

        // 拿-21.4413 - 当前相机位置即 -21.6 = 0.2 这就是相机移动的大小方向

        // Get direction towards target pos
        float xDirection = _targetHorizontalSmoothFollow - transform.localPosition.x;
        float yDirection = _targetVerticalSmoothFollow - transform.localPosition.y;
        Vector3 deltaDirection = new Vector3(xDirection, yDirection, 0f);

        // 如果再下一次移动 
        // _targetHorizontalSmoothFollow = Mathf.Lerp( 当前相机移动的新位置, 角色位置, 0.23);
        // 再抽 0.23 再减去当前相机 得到移动大小
        // 等到角色不动 位置就会越来越靠近 当完全一致时 就会停止了

        // New position
        Vector3 newCameraPosition = transform.localPosition + deltaDirection;

        // Apply new position
        transform.localPosition = new Vector3(newCameraPosition.x, newCameraPosition.y, transform.localPosition.z);
    }


    // Returns the position of out target
    private Vector3 GetTargetPosition(PlayerMovement player)
    {
        float xPos = 0f;
        float yPos = 0f;

        xPos += (player.transform.position.x + horizontalOffset) * horizontalInfluence;
        yPos += (player.transform.position.y + verticalOffset) * verticalInfluence;
        // 得到角色的当前位置 
        // x轴 玩家的x轴 + offset目前是0 * 1  是以还是等于玩家的x轴
        // y轴也是一样 

        //储存玩家的位置，xpos，ypos，玩家z轴
        Vector3 positionTarget = new Vector3(xPos, yPos, transform.position.z);
        return positionTarget;
    }

    // Centers our camera in the target position
    private void CenterOnTarget(PlayerMovement player)
    {
        //Vector3 targetPosition = GetTargetPosition(player);
        Target = player;

        Vector3 targetPos = GetTargetPosition(Target);
        _targetHorizontalSmoothFollow = targetPos.x;
        _targetVerticalSmoothFollow = targetPos.y;

        transform.localPosition = targetPos;
    }

    // Reset the target reference
    private void StopFollow(PlayerMovement player)
    {
        Target = null;
    }

    // Gets Target reference and center our camera
    private void StartFollowing(PlayerMovement player)
    {
        Target = player;
        CenterOnTarget(Target);
    }

    private void OnEnable()
    {
        // +=：将 CenterOnTarget 方法添加为事件的监听器

        // 一开始游戏Spawn Player 会传送 这个事件 并执行 CenterOnTarget
        // CenterOnTarget 会定义Camera Target 就是 Player
        LevelManager.OnPlayerSpawn += CenterOnTarget;

    }

    private void OnDisable()
    {
        // -= 也可理解为取消订阅
        LevelManager.OnPlayerSpawn -= CenterOnTarget;

    }
}
