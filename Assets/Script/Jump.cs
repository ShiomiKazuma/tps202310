using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Jump
{
    public static void JumpAction(Rigidbody rb, float jumpPower)
    {
       rb.AddForce(Vector3.up *  jumpPower, ForceMode.Impulse); 
    }
}
