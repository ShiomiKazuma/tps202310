using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
public class Character : MonoBehaviour
{
    public Camera playerCamera = default;
    public GameObject character = default;
    Rigidbody _rb;
    CapsuleCollider _capsuleCollider;
    [SerializeField] float _jumpPower = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump.JumpAction(_rb, _jumpPower);
        }
    }
}
