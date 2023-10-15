using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
public class Character : MonoBehaviour
{
    Animator _animator;
    public Camera playerCamera = default;
    public GameObject character = default;
    Rigidbody _rb;
    CapsuleCollider _capsuleCollider;
    [SerializeField] float _jumpPower = 100.0f;
    public float _jumpGravityScale = 0.6f;
    float _inputX;
    float _inputZ;
    [SerializeField] float _moveSpeed = 1.0f;
    bool _isGrounded = false;
    public bool _isJumping = false;
    public float _groundDistance = 0.01f;
    public LayerMask _groundMask = ~0;
    /// <summary>走る速度 </summary>
    public float _runSpeed = 3f;
    /// <summary>加速度 </summary>
    public float _acceleration = 10f;
    /// <summary>登れる坂の傾斜の閾値 </summary>
    public float _maxGroundAngle = 45;
    public float turnSmoothing = 0.8f;
    Vector2 movementInput = Vector2.zero;

    Rigidbody _groundRigidbody = null; // 地面のRigidbody
    Vector3 _groundNormal = Vector3.up; //地面の法線
    Vector3 _groundContatPoint = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    //void Update()
    //{

    //    _inputX = Input.GetAxis("Horizontal");
    //    _inputZ = Input.GetAxis("Vertical");
    //}

    void ApplyMotion()
    {
        //右と前のデフォルトを設定
        Vector3 movementRight = Vector3.right;
        Vector3 movementForward = Vector3.forward;

        if(playerCamera != null)
        {
            Vector3 cameraRight = playerCamera.transform.right;
            Vector3 cameraForward = playerCamera.transform.forward;
            //地面に対する方向を計算する
            movementRight = ProjectOnPlane(cameraRight, _groundNormal).normalized;
            movementForward = ProjectOnPlane(cameraForward, _groundNormal).normalized;
        }
        Vector3 movement = movementRight * movementInput.x + movementForward * movementInput.y;
        //プレイヤーの向きを変える
        
        if(character !=  null)
        {
            Vector3 rotateTarget = new Vector3(movement.x, 0, movement.z);
            if (rotateTarget.magnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
                character.transform.rotation = Quaternion.Lerp(lookRotation, character.transform.rotation, turnSmoothing);
            }
        }
        Vector3 velocity = _rb.velocity;
        if(_groundRigidbody != null)
        {
            velocity -= _groundRigidbody.velocity;
        }

        Vector3 groundVelocity = ProjectOnPlane(_rb.velocity, _groundNormal);
        float groundAngle = 90f - Mathf.Asin(_groundNormal.y) * 180f / Mathf.PI;
        bool movingDownhill = movement.y <= 0f;
        if(groundAngle <= _maxGroundAngle || movingDownhill)
        {
            if (groundVelocity.magnitude < _runSpeed)
            {
                _rb.AddForce(movement * _acceleration, ForceMode.Acceleration);
            }
        }
    }

    private void FixedUpdate()
    {
        //_isGrounded = CheckForGround();
        //if (Input.GetButtonDown("Jump"))
        //{
        //    Jump.JumpAction(_rb, _jumpPower, _isGrounded, _isJumping);
        //}
        RaycastHit hitInfo = CheckForGround();
        //衝突していない場合にNullを返す
        _isGrounded = hitInfo.collider != null;

        if (_isGrounded )
        {
            _groundNormal = hitInfo.normal;
            _groundContatPoint = hitInfo.point;
            _groundRigidbody = hitInfo.rigidbody;
        }
        else
        {
            //デフォルトの値にする
            _groundNormal = Vector3.up;
            _groundRigidbody = null;
            _groundContatPoint = transform.TransformPoint(_capsuleCollider.center + Vector3.down * _capsuleCollider.height * 0.5f);
        }
        if (_rb.velocity.y < 0)
        {
            _isJumping = false;
        }

        if(_isJumping)
        {
            _rb.AddForce(Physics.gravity * _rb.mass * (_jumpGravityScale - 1f));
        }
        //Vector3 direction = new Vector3(_inputX, 0, _inputZ).normalized;
        //_rb.AddForce(direction * _moveSpeed);
        if(!_isJumping && _isGrounded)
        {
            ApplyMotion();
        }
    }

    RaycastHit CheckForGround()
    {
        //直線のレイを飛ばす方法
        //Vector3 capsuleBottom = _capsuleCollider.center + Vector3.down * _capsuleCollider.height * 0.5f;
        //Vector3 feetPosition = transform.TransformPoint(capsuleBottom) + Vector3.up * _groundDistance;
        //bool raycastHit = Physics.Raycast(feetPosition, Vector3.down, _groundDistance * 2, _groundMask);

        //スフィア型のレイを飛ばす方法
        float extent = Mathf.Max(0, _capsuleCollider.height * 0.5f - _capsuleCollider.radius);
        //float extent = _capsuleCollider.height * 0.5f - _capsuleCollider.radius;
        Vector3 origin = transform.TransformPoint(_capsuleCollider.center + Vector3.down * extent) + Vector3.up * _groundDistance;

        RaycastHit hitInfo;
        Ray sphereCastRay = new Ray(origin, Vector3.down);
        //bool raycastHit = Physics.SphereCast(sphereCastRay, _capsuleCollider.radius, _groundDistance * 2f, _groundMask);
        Physics.SphereCast(sphereCastRay, _capsuleCollider.radius, out hitInfo, _groundDistance * 2f, _groundMask);
        return hitInfo;
    }

    void Jump(bool state)
    {
        if (state && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isJumping = true;
        }
        if(!state)
        {
            _isJumping = false;
        }
    }
    void OnJump(InputValue inputValue)
    {
        Jump(inputValue.isPressed);
    }

    public void Move(Vector2 input)
    {
        movementInput = input;
    }

    void OnMove(InputValue inputValue)
    {
        Move(inputValue.Get<Vector2>());
    }

    Vector3 ProjectOnPlane(Vector3 vector, Vector3 normal)
    {
        return Vector3.Cross(normal, Vector3.Cross(vector, normal));
    }
}
