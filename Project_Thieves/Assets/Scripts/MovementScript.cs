using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MovementScript : NetworkBehaviour
{
    Rigidbody rb;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    LayerMask jumpMask;

    float yRot;

    private GuardProperties _guardProperties;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer == false)
            return;

        rb = GetComponent<Rigidbody>();
        
        
        foreach (GameObject GO in GameObject.FindGameObjectsWithTag(("Guard")))
        {
            GO.GetComponent<GuardProperties>().GuardObservables.Add(new GuardProperties.GuardThreat(gameObject, 0f));
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLocalPlayer == false)
            return;

        Move();
        Jump();
        Rotate();

    }

    
    
    
    
    
    private void Rotate()
    {
        yRot += -Input.GetAxis("Mouse Y") * Time.deltaTime * 50;
        yRot = Mathf.Clamp(yRot, -180, 180);
        Camera.main.transform.localEulerAngles = new Vector3(yRot, 0, 0);
        transform.Rotate(transform.up, Input.GetAxis("Mouse X"));
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 1.1f, jumpMask);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * 300);
        }
    }

    private void Move()
    {
        Vector3 movementvect = Vector3.zero;
        movementvect += Input.GetAxis("Horizontal") * transform.right * moveSpeed;
        movementvect += Input.GetAxis("Vertical") * transform.forward * moveSpeed;

        rb.velocity = new Vector3(movementvect.x, rb.velocity.y, movementvect.z);
    }
}
