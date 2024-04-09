using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpAmount = 70;
    private bool isMoving;
    private bool isGrounded;
    private float distToGround;
    private Collider collider;
    private void Start()
    {
        collider = GetComponent<Collider>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    private void Update()
    {
        if (!IsOwner) return;

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

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpAmount, ForceMode.Impulse);

        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down), out hit, distToGround + 0.1f))
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down) * hit.distance, Color.red, 3.0f);
        }
        else
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down) * 1000, Color.blue, 3.0f);
        }
        return Physics.Raycast(transform.position + new Vector3(0, 1f, 0), transform.TransformDirection(Vector3.down), out hit, distToGround + 0.1f);
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
