using UnityEngine;
using System.Collections;

public enum Direction
{
    Right,
    Left
}

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class CharacterController : MonoBehaviour
{
    

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _jumpHeight;

    [SerializeField]
    private float _jumpDuration;

    private float _fallGravity;
    
    private float _jumpSpeed;

    [SerializeField]
    private float _terminalSpeed;

    [SerializeField]
    private float _skinWidth;

    [SerializeField]
    private int _totalRaycastsPerSide = 4;

    public Direction MoveDirection { get; private set; }
    public SpriteRenderer DirectionMonitor;

    // To do jump conditions
    [SerializeField]    
    private bool _isGrounded;
    [SerializeField]
    private bool _wasGrounded;

    [SerializeField]
    private bool _isJumping;

    [SerializeField]
    private Vector2 _velocity;

    [SerializeField]
    private LayerMask _hitMask;

    [SerializeField]
    private TileScanner _sensorLeft;

    [SerializeField]
    private TileScanner _sensorRight;

    public TileScanner ActiveScanner { get; private set; }

    public void Turn()
    {
        MoveDirection = MoveDirection == Direction.Right ? Direction.Left : Direction.Right;
        DirectionMonitor.flipX = MoveDirection != Direction.Right;
        
        _sensorRight.SetEnabled(MoveDirection == Direction.Right);
        _sensorLeft.SetEnabled(MoveDirection == Direction.Left);
        ActiveScanner = MoveDirection == Direction.Right ? _sensorRight : _sensorLeft;
        //transform.localScale = new Vector2(transform.localScale.x*-1, transform.localScale.y);
    }

    public void Jump()
                        {
        if (_isGrounded)
        {
            _velocity.y = _jumpSpeed;
            _isJumping = true;
            _isGrounded = false;
        }
        else
        {
            Debug.Log("Not grounded.");
        }
    }

    void Start()
    {
        ActiveScanner = MoveDirection == Direction.Right ? _sensorRight : _sensorLeft;
        _sensorRight.SetEnabled(MoveDirection == Direction.Right);
        _sensorLeft.SetEnabled(MoveDirection == Direction.Left);
    }

    private bool _on = false;
    public void TurnOn()
    {
        _on = true;
    }

    public void TurnOff()
    {
        _on = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_on)
        {
            _velocity.x = (MoveDirection == Direction.Right ? 1 : -1) * _speed;
        }

        _jumpSpeed = 2*_jumpHeight/_jumpDuration;
        _fallGravity = _jumpSpeed/_jumpDuration;

        if (_isGrounded)
        {
            _velocity.y = 0;
        }
        else
        {
            _velocity.y -= _fallGravity*Time.deltaTime;
        }

        Move(_velocity * Time.deltaTime);
        //_isJumping = false;
    }

    private void Move(Vector2 deltaMovement)
    {
        var skinnedBounds = GetComponent<BoxCollider2D>().bounds;
        skinnedBounds.Expand(-2.0f*_skinWidth);
        var raycastBottomLeft = skinnedBounds.min;
        var raycastTopLeft = new Vector2(skinnedBounds.min.x, skinnedBounds.max.y);
        var raycastBottomRight = new Vector2(skinnedBounds.max.x, skinnedBounds.min.y);

        _wasGrounded = _isGrounded;
        _isGrounded = false;

        #region Horizontal Movement
        if (deltaMovement.x != .0f)
        {
            var rayLen = Mathf.Abs(deltaMovement.x) + _skinWidth;
            var rayDir = deltaMovement.x > 0 ? Vector2.right : Vector2.left;
            var firstRayOrigin = deltaMovement.x > 0 ? (Vector2)raycastBottomRight : (Vector2)raycastBottomLeft;

            var raysVerticalDistance = (raycastTopLeft.y - raycastBottomLeft.y)/(_totalRaycastsPerSide - 1);

            for (int i = 0; i < _totalRaycastsPerSide; i++)
            {
                var rayStart = new Vector2(firstRayOrigin.x, firstRayOrigin.y + i * raysVerticalDistance);

                Debug.DrawRay(rayStart, rayDir * rayLen, Color.white);

                var raycastHit = Physics2D.Raycast(rayStart, rayDir, rayLen, _hitMask);

                if (raycastHit)
                {
                    deltaMovement.x = raycastHit.point.x - rayStart.x;
                    rayLen = Mathf.Abs(deltaMovement.x);
                    deltaMovement.x += deltaMovement.x > 0 ? -_skinWidth : _skinWidth;
                }
            }
        }
        #endregion

        #region Vertical Movement
        //if (deltaMovement.y != .0f)
        {
            var rayLen = Mathf.Abs(deltaMovement.y) + 1.1f*_skinWidth;
            var rayDir = deltaMovement.y > 0 ? Vector2.up : Vector2.down;
            var firstRayOrigin = deltaMovement.y > 0 ? (Vector2)raycastTopLeft : (Vector2)raycastBottomLeft;

            var raysHorizontalDistance = (raycastBottomRight.x - raycastBottomLeft.x + 1.8f * _skinWidth) / (_totalRaycastsPerSide - 1);

            firstRayOrigin.x += deltaMovement.x;
            
            for (int i = 0; i < _totalRaycastsPerSide; i++)
            {
                var rayStart = new Vector2(firstRayOrigin.x + i * raysHorizontalDistance - 0.9f*_skinWidth, firstRayOrigin.y );

                var raycastHit = Physics2D.Raycast(rayStart, rayDir, rayLen, _hitMask);

                if (raycastHit)
                {
                    deltaMovement.y = raycastHit.point.y - rayStart.y;
                    rayLen = Mathf.Abs(deltaMovement.y);
                    deltaMovement.y += deltaMovement.y > 0 ? -_skinWidth : _skinWidth;
                    if (Mathf.Abs(deltaMovement.y) < 0.01)
                    {
                        deltaMovement.y = 0.0f;
                    }
                    if (deltaMovement.y <= 0.02)
                    {
                        _isGrounded = true;
                    }
                }

                Debug.DrawRay(rayStart, rayDir * rayLen, raycastHit ? Color.black : Color.white);
            }
        }
        #endregion

        if (Mathf.Abs(deltaMovement.y) < 0.01)
        {
            deltaMovement.y = 0.0f;
        }
        if (Mathf.Abs(deltaMovement.x) < 0.01)
        {
            deltaMovement.x = 0.0f;
        }
        transform.Translate(deltaMovement, Space.World);
    }
}