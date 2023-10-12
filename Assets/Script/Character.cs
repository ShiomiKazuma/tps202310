using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
public class Character : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {

        _inputX = Input.GetAxis("Horizontal");
        _inputZ = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        _isGrounded = CheckForGround();
        //if (Input.GetButtonDown("Jump"))
        //{
        //    Jump.JumpAction(_rb, _jumpPower, _isGrounded, _isJumping);
        //}
        if (_rb.velocity.y < 0)
        {
            _isJumping = false;
        }

        if(_isJumping)
        {
            _rb.AddForce(Physics.gravity * _rb.mass * (_jumpGravityScale - 1f));
        }
        Vector3 direction = new Vector3(_inputX, 0, _inputZ).normalized;
        _rb.AddForce(direction * _moveSpeed);
    }

    bool CheckForGround()
    {
        //直線のレイを飛ばす方法
        //Vector3 capsuleBottom = _capsuleCollider.center + Vector3.down * _capsuleCollider.height * 0.5f;
        //Vector3 feetPosition = transform.TransformPoint(capsuleBottom) + Vector3.up * _groundDistance;
        //bool raycastHit = Physics.Raycast(feetPosition, Vector3.down, _groundDistance * 2, _groundMask);

        //スフィア型のレイを飛ばす方法
        //float extent = Mathf.Max(0, _capsuleCollider.height * 0.5f - _capsuleCollider.radius);
        float extent = _capsuleCollider.height * 0.5f - _capsuleCollider.radius;
        Vector3 origin = transform.TransformPoint(_capsuleCollider.center + Vector3.down * extent) + Vector3.up * _groundDistance;
        Ray sphereCastRay = new Ray(origin, Vector3.down);
        bool raycastHit = Physics.SphereCast(sphereCastRay, _capsuleCollider.radius, _groundDistance * 2f, _groundMask);
        return raycastHit;
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
}
