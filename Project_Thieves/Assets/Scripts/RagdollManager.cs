using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DisableRagdollParts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableRagdoll()
    {
        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
        Rigidbody[] rigidbodies = this.gameObject.GetComponentsInChildren<Rigidbody>();
        Vector3 movementVector = GetComponent<NavMeshAgent>().velocity;

        
        
        foreach (Collider collider in colliders)
        {
            collider.isTrigger = false;
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = movementVector;
        }
    }

    


    void DisableRagdollParts()
    {
//        Collider[] colliders = this.gameObject.GetComponentsInChildren<Collider>();
//
//
//        foreach (Collider collider in colliders)
//        {
//            collider.isTrigger = true;
//        }
    }


}
