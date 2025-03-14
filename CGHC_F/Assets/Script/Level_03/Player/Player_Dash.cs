using System.Collections;
using UnityEngine;

public class PlayerDash : PlayerStates
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private LayerMask groundLayer;  // 设置墙体的 Layer

    private bool _isDashing = false;
    private bool _canDash = true;

    private int _dashAnimatorParameter = Animator.StringToHash("Dash");
    private int _idleAnimatorParameter = Animator.StringToHash("Idle");
    private int _runAnimatorParameter = Animator.StringToHash("Run");

    protected override void InitState()
    {
        base.InitState();
        _canDash = true;
    }

    public override void ExecuteState()
    {
        if (_isDashing) return;

        _animator.SetBool(_dashAnimatorParameter, false);
    }

    protected override void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash 开始");
        _isDashing = true;
        _canDash = false;
        _playerController.Conditions.IsDashing = true;

        _animator.SetBool(_dashAnimatorParameter, true);
        _animator.SetBool(_idleAnimatorParameter, false);
        _animator.SetBool(_runAnimatorParameter, false);

        float direction = _playerController.FacingRight ? 1f : -1f;
        if (direction == 0) direction = 1f;

        Vector2 startPosition = _playerController.transform.position;
        float adjustedDistance = GetDashDistance(startPosition, direction, dashDistance);

        float dashSpeed = adjustedDistance / dashDuration;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            _playerController.transform.position += new Vector3(dashSpeed * direction * Time.deltaTime, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        _playerController.Conditions.IsDashing = false;
        _animator.SetBool(_dashAnimatorParameter, false);

        Debug.Log("Dash 结束");

        _isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    // **检测前方是否有墙，并调整 Dash 距离**
    private float GetDashDistance(Vector2 startPosition, float direction, float maxDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPosition, Vector2.right * direction, maxDistance, groundLayer);
        Debug.DrawRay(startPosition, Vector2.right * direction * maxDistance, Color.red, 0.2f);

        if (hit.collider != null)
        {
            Debug.Log("检测到障碍物：" + hit.collider.gameObject.name);
            return hit.distance - 0.1f;  // 预留一点距离，避免贴紧墙壁
        }
        return maxDistance;
    }

    public override void SetAnimation()
    {
        _animator.SetBool(_dashAnimatorParameter, _isDashing);
    }
}
