using System.Collections;
using UnityEngine;

public class PlayerDash : PlayerStates
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 3f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

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

        // 只有不在 Dash 状态，才会允许 Idle/Run 动画
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

        // 确保 Idle 和 Run 动画不播放
        _animator.SetBool(_dashAnimatorParameter, true);
        _animator.SetBool(_idleAnimatorParameter, false);
        _animator.SetBool(_runAnimatorParameter, false);

        float dashSpeed = dashDistance / dashDuration;
        float direction = _playerController.FacingRight ? 1f : -1f;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            _playerController.SetHorizontalForce(dashSpeed * direction);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _playerController.SetHorizontalForce(0);
        yield return new WaitForSeconds(0.1f);

        _playerController.Conditions.IsDashing = false;
        _animator.SetBool(_dashAnimatorParameter, false);

        Debug.Log("Dash 结束");

        _isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }

    public override void SetAnimation()
    {
        _animator.SetBool(_dashAnimatorParameter, _isDashing);
    }
}
