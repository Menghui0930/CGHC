using System.Collections;
using UnityEngine;

public class PlayerAttack : PlayerStates
{
    [Header("Settings")]
    [SerializeField] private float attackDuration = 0.5f; // 攻击动画时间
    private int _attackAnimatorParameter = Animator.StringToHash("Attack");
    private int _idleAnimatorParameter = Animator.StringToHash("Idle");

    private bool _isAttacking = false;

    protected override void InitState()
    {
        base.InitState();
        _isAttacking = false;
    }

    public override void ExecuteState()
    {
        if (_isAttacking)
        {
            _animator.SetBool(_attackAnimatorParameter, true);
            _animator.SetBool(_idleAnimatorParameter, false); // 彻底禁用 Idle
        }
    }

    protected override void GetInput()
    {
        if (Input.GetMouseButtonDown(0) && !_isAttacking)  // ✅ 改成鼠标左键
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        _isAttacking = true;

        _animator.SetBool(_attackAnimatorParameter, true);
        _animator.SetBool(_idleAnimatorParameter, false); // 彻底禁用 Idle

        yield return new WaitForSeconds(attackDuration);

        _animator.SetBool(_attackAnimatorParameter, false);
        _animator.SetBool(_idleAnimatorParameter, true);
        _isAttacking = false;
    }

    private IEnumerator AttackCoroutine()
    {
        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animator.SetBool("Attack", false);
    }

    public override void SetAnimation()
    {
        _animator.SetBool(_attackAnimatorParameter, _isAttacking);
    }
}
