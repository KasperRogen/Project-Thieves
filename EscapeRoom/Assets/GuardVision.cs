using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class GuardVision : MonoBehaviour
{

    public static List<GameObject> GuardObservables = new List<GameObject>();

    [Header("Global Vision")]
    [SerializeField] Transform HeadObject;

    [Space(10)]

    [Header("Central Vision")]
    [SerializeField] float CentralVisionDistance;
    [SerializeField] float CentralVisionAngle;

    [Space(10)]

    [Header("Main Vision")]
    [SerializeField] float MainVisionDistance;
    [SerializeField] float MainVisionAngle;

    [Space(10)]

    [Header("Peripheral Vision")]
    [SerializeField] float PeripheralVisionDistance;
    [SerializeField] float PeripheralVisionAngle;
    float CalculatedPeripheralVisionAngle;

    Transform lookTarget;
    Quaternion LastCalcHeadRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CentralVision();
        MainVision();
        PeripheralVision();
    }


    private void LateUpdate()
    {
        if (lookTarget)
        {
            // Store the current head rotation since we will be resetting it
            Quaternion currentLocalRotation = LastCalcHeadRotation;
            // Reset the head rotation so our world to local space transformation will use the head's zero rotation. 
            // Note: Quaternion.Identity is the quaternion equivalent of "zero"
            HeadObject.localRotation = Quaternion.identity;

            Vector3 targetWorldLookDir = (lookTarget.position + Vector3.up * 1.8f) - HeadObject.position;
            Vector3 targetLocalLookDir = HeadObject.InverseTransformDirection(targetWorldLookDir);

            // Apply angle limit
            targetLocalLookDir = Vector3.RotateTowards(
                Vector3.forward,
                targetLocalLookDir,
                Mathf.Deg2Rad * 90, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
                0 // We don't care about the length here, so we leave it at zero
            );

            // Get the local rotation by using LookRotation on a local directional vector
            Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

            // Apply smoothing
            HeadObject.localRotation = Quaternion.Slerp(
                currentLocalRotation,
                targetLocalRotation,
                1 - Mathf.Exp(-2 * Time.deltaTime)
            );

            LastCalcHeadRotation = HeadObject.localRotation;
        } else
        {
            LastCalcHeadRotation = HeadObject.localRotation;
        }
    }


    void CentralVision()
    {
        if(SeesObservable(HeadObject.forward, CentralVisionDistance, CentralVisionAngle))
        {
            Alert();
        }
    }


    void MainVision()
    {
        if(SeesObservable(HeadObject.forward, MainVisionDistance, MainVisionAngle))
        {
            Alert();
        }
    }


    void PeripheralVision()
    {
        GameObject target;
        if(target = SeesObservable(HeadObject.right, PeripheralVisionDistance, PeripheralVisionAngle))
        {
            lookTarget = target.transform;
        }
        else if (target = SeesObservable(-HeadObject.right, PeripheralVisionDistance, PeripheralVisionAngle))
        {
            lookTarget = target.transform;
        }
    }



    GameObject SeesObservable(Vector3 Direction, float distance, float angle)
    {
        if (GuardObservables == null)
            return null;

        foreach (GameObject observable in GuardObservables.Where(o => Vector3.Distance(transform.position, o.transform.position) <= distance))
        {
            Vector3 observableVector = observable.transform.position - HeadObject.transform.position; 

            if(Vector3.Angle(Direction, observableVector) < angle / 2)
            {
                RaycastHit hit;
                Physics.Raycast(HeadObject.transform.position, observableVector, out hit, distance);
                if(hit.transform.gameObject == observable)
                {
                    return hit.transform.gameObject;
                }
            }


        }

        return null;
    }

    private void Alert()
    {
        throw new NotImplementedException();
    }







    private void OnDrawGizmosSelected()
    {
        CalculatedPeripheralVisionAngle = PeripheralVisionAngle * 2 - MainVisionAngle;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Vector3 Startpoint = Quaternion.Euler(0, -CentralVisionAngle / 2, 0) * HeadObject.transform.forward;


        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position + Vector3.up, HeadObject.transform.up, Startpoint, CentralVisionAngle, CentralVisionDistance);

        Handles.color = Color.yellow;
        Startpoint = Quaternion.Euler(0, -MainVisionAngle / 2, 0) * HeadObject.transform.forward;
        Handles.DrawSolidArc(transform.position + Vector3.up, HeadObject.transform.up, Startpoint, MainVisionAngle, MainVisionDistance);

        Handles.color = Color.green;
        Startpoint = Quaternion.Euler(0, -PeripheralVisionAngle / 2, 0) * -HeadObject.transform.right;
        Handles.DrawSolidArc(transform.position + Vector3.up, HeadObject.transform.up, Startpoint, PeripheralVisionAngle, PeripheralVisionDistance);
        Startpoint = Quaternion.Euler(0, -PeripheralVisionAngle / 2, 0) * HeadObject.transform.right;
        Handles.DrawSolidArc(transform.position + Vector3.up, HeadObject.transform.up, Startpoint, PeripheralVisionAngle, PeripheralVisionDistance);




    }




}
