using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SP_Player : MonoBehaviour
{
    private bool isMoving;
    private void Update()
    {
        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        isMoving = moveDir != Vector3.zero;
        float moveSpeed = 8f;
        float rotateSpeed = 10;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
