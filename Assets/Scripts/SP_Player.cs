using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SP_Player : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpAmount = 10;
    private bool isMoving;
    private bool isGrounded;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);
        }
        if (Physics.Raycast(transform.position, Vector3.down, 0.4f))
        {
            Debug.Log("a");
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
