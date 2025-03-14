using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 速度
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal"); // 获取水平输入（A/D）
        moveInput.y = Input.GetAxisRaw("Vertical");   // 获取垂直输入（W/S）
        moveInput.Normalize(); // 防止对角线移动比单方向快
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
}
