using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float fallMultiplier = 1.5f;

    [Header("Collisions")]
    [SerializeField] private LayerMask collideWith;
    [SerializeField] private LayerMask slopeLayerMask; // 斜坡层
    [SerializeField] private int verticalRayAmount = 4;
    [SerializeField] private int horizontalRayAmount = 4;

    #region Properties
    public bool FacingRight { get; set; }
    public float Gravity => gravity;
    public Vector2 Force => _force;
    public PlayerConditions Conditions => _conditions;
    public float Friction { get; set; }
    #endregion

    #region Internal
    private BoxCollider2D _boxCollider2D;
    private PlayerConditions _conditions;

    private Vector2 _boundsTopLeft;
    private Vector2 _boundsTopRight;
    private Vector2 _boundsBottomLeft;
    private Vector2 _boundsBottomRight;

    private float _boundsWidth;
    private float _boundsHeight;

    private float _currentGravity;
    private Vector2 _force;
    private Vector2 _movePosition;
    private float _skin = 0.25f; 

    private float _internalFaceDirection = 1f;
    private float _faceDirection;
    private float _wallFallMultiplier;
    #endregion

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _conditions = new PlayerConditions();
        _conditions.Reset();
    }

    private void Update()
    {
        ApplyGravity();
        StartMovement();

        SetRayOrigins();
        GetFaceDirection();
        RotateModel();

        CollisionBelow(); 
        if (FacingRight)
        {
            HorizontalCollision(1);
        }
        else
        {
            HorizontalCollision(-1);
        }
        CollisionAbove();

        transform.Translate(_movePosition, Space.Self);

        SetRayOrigins();
        CalculateMovement();
    }

    #region Collision

    private void CollisionBelow()
    {
        Friction = 0f;

        if (_movePosition.y < -0.0001f)
        {
            _conditions.IsFalling = true;
        }
        else
        {
            _conditions.IsFalling = false;
        }

        if (!_conditions.IsFalling)
        {
            _conditions.IsCollidingBelow = false;
            return;
        }

        float rayLenght = _boundsHeight / 2f + _skin;
        if (_movePosition.y < 0)
        {
            rayLenght += Mathf.Abs(_movePosition.y);
        }

        Vector2 leftOrigin = (_boundsBottomLeft + _boundsTopLeft) / 2f;
        Vector2 rightOrigin = (_boundsBottomRight + _boundsTopRight) / 2f;
        leftOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);
        rightOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);

        for (int i = 0; i < verticalRayAmount; i++)
        {
            LayerMask groundMask = collideWith | slopeLayerMask;
            Vector2 rayOrigin = Vector2.Lerp(leftOrigin, rightOrigin, (float)i / (float)(verticalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, rayLenght, groundMask);
            Debug.DrawRay(rayOrigin, -transform.up * rayLenght, Color.green);

            if (hit)
            {
                // 计算斜坡角度
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle > 0f && slopeAngle <= 45f) // 只处理小于45°的斜坡
                {
                    _movePosition.y = -hit.distance + _boundsHeight / 2f + _skin;
                    _movePosition.x += hit.normal.x * Mathf.Abs(_movePosition.y); // 适应斜坡角度
                }
                else
                {
                    _movePosition.y = -hit.distance + _boundsHeight / 2f + _skin;
                }

                _conditions.IsCollidingBelow = true;
                _conditions.IsFalling = false;

                if (Mathf.Abs(_movePosition.y) < 0.01f)
                {
                    _movePosition.y = 0f;
                }
                break; // 确保只取第一个有效碰撞点
            }
        }
    }

    private void HorizontalCollision(int direction)
    {
        Vector2 rayHorizontalBottom = (_boundsBottomLeft + _boundsBottomRight) / 2f;
        Vector2 rayHorizontalTop = (_boundsTopLeft + _boundsTopRight) / 2f;
        rayHorizontalBottom += (Vector2)transform.up * _skin;
        rayHorizontalTop -= (Vector2)transform.up * _skin;

        float rayLenght = Mathf.Abs(_force.x * Time.deltaTime) + _boundsWidth / 2f + _skin * 2f;

        for (int i = 0; i < horizontalRayAmount; i++)
        {
            // 跳过最底部的射线
            if (i == 0)
            {
                continue;
            }

            Vector2 rayOrigin = Vector2.Lerp(rayHorizontalBottom, rayHorizontalTop, (float)i / (horizontalRayAmount - 1));
            //RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction * transform.right, rayLenght, collideWith);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction * transform.right, rayLenght, collideWith & ~slopeLayerMask);
            Debug.DrawRay(rayOrigin, transform.right * rayLenght * direction, Color.cyan);

            if (hit)
            {
                Vector2 hitNormal = hit.normal;

                    if (direction >= 0)
                    {
                        _movePosition.x = hit.distance - _boundsWidth / 2f - _skin * 2f;
                        _conditions.IsCollidingRight = true;
                    }
                    else
                    {
                        _movePosition.x = -hit.distance + _boundsWidth / 2f + _skin * 2f;
                        _conditions.IsCollidingLeft = true;
                    }
                    _force.x = 0f;
                
            }
        }
    }

    private void CollisionAbove()
    {
        if (_movePosition.y < 0)
        {
            return;
        }

        float rayLenght = _movePosition.y + _boundsHeight / 2f;
        Vector2 rayTopLeft = (_boundsBottomLeft + _boundsTopLeft) / 2f;
        Vector2 rayTopRight = (_boundsBottomRight + _boundsTopRight) / 2f;
        rayTopLeft += (Vector2)transform.right * _movePosition.x;
        rayTopRight += (Vector2)transform.right * _movePosition.x;

        for (int i = 0; i < verticalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(rayTopLeft, rayTopRight, (float)i / (float)(verticalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, transform.up, rayLenght, collideWith);
            Debug.DrawRay(rayOrigin, transform.up * rayLenght, Color.red);

            if (hit)
            {
                _movePosition.y = hit.distance - _boundsHeight / 2f;
                _conditions.IsCollidingAbove = true;
            }
            else
            {
                _conditions.IsCollidingAbove = false;
            }
        }
    }

    #endregion


    #region Movement
    private void CalculateMovement()
    {
        if (Time.deltaTime > 0)
        {
            _force = _movePosition / Time.deltaTime;
        }
    }

    private void StartMovement()
    {
        _movePosition = _force * Time.deltaTime;
        _conditions.Reset();
    }

    public void SetHorizontalForce(float xForce) => _force.x = xForce;
    public void SetVerticalForce(float yForce) => _force.y = yForce;
    public void AddHorizontalMovement(float xForce) => _force.x += xForce;

    private void ApplyGravity()
    {
        _currentGravity = gravity;
        if (_force.y < 0)
        {
            _currentGravity *= fallMultiplier;
        }
        _force.y += _currentGravity * Time.deltaTime;

        if (_wallFallMultiplier != 0)
        {
            _force.y *= _wallFallMultiplier;
        }
    }

    public void SetWallClingMultiplier(float fallM) => _wallFallMultiplier = fallM;
    #endregion

    #region Direction
    private void GetFaceDirection()
    {
        _faceDirection = _internalFaceDirection;
        FacingRight = _faceDirection == 1;

        if (_force.x > 0.0001f)
        {
            _faceDirection = 1f;
            FacingRight = true;
        }
        else if (_force.x < -0.0001f)
        {
            _faceDirection = -1f;
            FacingRight = false;
        }

        _internalFaceDirection = _faceDirection;
    }

    private void RotateModel()
    {
        transform.localScale = FacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
    }
    #endregion

    #region Ray Origins
    private void SetRayOrigins()
    {
        Bounds playerBounds = _boxCollider2D.bounds;
        _boundsBottomLeft = new Vector2(playerBounds.min.x, playerBounds.min.y);
        _boundsBottomRight = new Vector2(playerBounds.max.x, playerBounds.min.y);
        _boundsTopLeft = new Vector2(playerBounds.min.x, playerBounds.max.y);
        _boundsTopRight = new Vector2(playerBounds.max.x, playerBounds.max.y);

        _boundsHeight = Vector2.Distance(_boundsBottomLeft, _boundsTopLeft);
        _boundsWidth = Vector2.Distance(_boundsBottomLeft, _boundsBottomRight);
    }
    #endregion
}