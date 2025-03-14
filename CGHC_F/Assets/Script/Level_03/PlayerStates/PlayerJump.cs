using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerStates
{
    [Header("Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float wallJumpForceX = 5f; // 增加墙跳的水平推力
    [SerializeField] private float wallJumpForceY = 6f; // 增加墙跳的垂直推力

    private int _jumpAnimatorParameter = Animator.StringToHash("Jump");
    private int _fallAnimatorParameter = Animator.StringToHash("Fall");
    private int _walkclingAnimatorParameter = Animator.StringToHash("WalkCling");

    public int JumpsLeft { get; set; }

    protected override void InitState()
    {
        base.InitState();
        JumpsLeft = maxJumps;
    }

    public override void ExecuteState()
    {
        if (_playerController.Conditions.IsCollidingBelow && _playerController.Force.y == 0f)
        {
            JumpsLeft = maxJumps;
            _playerController.Conditions.IsJumping = false;
            _playerController.Conditions.IsFalling = false; // 确保落地后停止 Fall
        }
        else if (_playerController.Force.y < 0 &&
                 !_playerController.Conditions.IsCollidingBelow &&
                 !_playerController.Conditions.IsWallClinging)  // 🔥 确保不在爬墙时触发 Fall
        {
            _playerController.Conditions.IsJumping = false;
            _playerController.Conditions.IsFalling = true;
        }

        // **检查是否在爬墙，并设置 WalkCling 动画**
        _animator.SetBool(_walkclingAnimatorParameter, _playerController.Conditions.IsWallClinging);
    }


    protected override void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_playerController.Conditions.IsWallClinging)
            {
                WallJump();
            }
            else
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        if (!CanJump())
        {
            return;
        }

        if (JumpsLeft == 0)
        {
            return;
        }

        JumpsLeft -= 1;

        float jumpForce = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(_playerController.Gravity));
        _playerController.SetVerticalForce(jumpForce);
        _playerController.Conditions.IsJumping = true;
        _playerController.Conditions.IsFalling = false; // 跳跃时停止 Falling
    }

    private void WallJump()
    {
        float jumpDirection = _playerController.Conditions.IsCollidingLeft ? 1f : -1f; // 确定跳跃方向

        // 设置墙跳的推力
        _playerController.SetHorizontalForce(jumpDirection * wallJumpForceX);
        _playerController.SetVerticalForce(wallJumpForceY);

        // 退出墙爬状态
        _playerController.Conditions.IsWallClinging = false;
        _playerController.SetWallClingMultiplier(0f);

        // 设置跳跃状态
        _playerController.Conditions.IsJumping = true;
        _playerController.Conditions.IsFalling = false; // 墙跳时不进入 Falling 状态
    }

    private bool CanJump()
    {
        if (_playerController.Conditions.IsWallClinging)
        {
            return true; // 在墙上时允许跳跃
        }

        return _playerController.Conditions.IsCollidingBelow || JumpsLeft > 0;
    }

    public override void SetAnimation()
    {
        // **爬墙动画**
        _animator.SetBool(_walkclingAnimatorParameter, _playerController.Conditions.IsWallClinging);

        // **跳跃动画**
        _animator.SetBool(_jumpAnimatorParameter, _playerController.Conditions.IsJumping);

        // **下落动画 (🔥 只有在不爬墙时才触发)**
        _animator.SetBool(_fallAnimatorParameter, _playerController.Conditions.IsFalling
                                                  && !_playerController.Conditions.IsWallClinging);
    }

}
