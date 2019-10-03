
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine.AI;
using Panda;
using UnityEngine.Serialization;

public class GuardVision : MonoBehaviour
{


    [Header("Global Vision")] [SerializeField]
    Transform HeadObject;
    [SerializeField] private float MaxHeadRotation = 90;
    [SerializeField]
    public List<Vision> visions = new List<Vision>();

    
    
    
    
    
    
    
    Quaternion LastCalcHeadRotation;

    

    private GuardProperties _guardProperties;


    
    [Serializable]
    public class Vision
    {
        public string Name;
        public Transform StartPoint;
        public float Distance;
        public float Angle;
        public float DetectionRate;
        public Color DEBUG_COLOR;
        public Vector3 Direction;

    }

    private NavMeshAgent agent;
    
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _guardProperties = GetComponent<GuardProperties>();
    }
    



//    private void LateUpdate()
//    {
//        if (lookTarget)
//        {
//            // Store the current head rotation since we will be resetting it
//            Quaternion currentLocalRotation = LastCalcHeadRotation;
//            // Reset the head rotation so our world to local space transformation will use the head's zero rotation. 
//            // Note: Quaternion.Identity is the quaternion equivalent of "zero"
//            HeadObject.localRotation = Quaternion.identity;
//
//            Vector3 targetWorldLookDir = (lookTarget.position + Vector3.up * 1.8f) - HeadObject.position;
//            Vector3 targetLocalLookDir = HeadObject.InverseTransformDirection(targetWorldLookDir);
//
//            // Apply angle limit
//            targetLocalLookDir = Vector3.RotateTowards(
//                Vector3.forward,
//                targetLocalLookDir,
//                Mathf.Deg2Rad * MaxHeadRotation, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
//                0 // We don't care about the length here, so we leave it at zero
//            );
//
//            // Get the local rotation by using LookRotation on a local directional vector
//            Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
//
//            // Apply smoothing
//            HeadObject.localRotation = Quaternion.Slerp(
//                currentLocalRotation,
//                targetLocalRotation,
//                1 - Mathf.Exp(-2 * Time.deltaTime)
//            );
//
//            LastCalcHeadRotation = HeadObject.localRotation;
//        } else
//        {
//            LastCalcHeadRotation = HeadObject.localRotation;
//        }
//    }









    [Task]
    void CheckVision()
    {
        bool anySeen = false;
        
        foreach (Vision vision in visions)
        {
            foreach (GameObject observedGameObject in SeesObservable(vision))
            {
                GuardProperties.GuardThreat target = _guardProperties.GuardObservables.Find(GO => GO.GameObject == observedGameObject && 
                                                                  GO.detectionLevel < _guardProperties.MaxDetectionLevel);
                if (target != null)
                {
                    target.detectionLevel += vision.DetectionRate * Time.fixedDeltaTime;
                    
                    anySeen = true;
                }
            }
        }


        if (anySeen == false)
        {
            _guardProperties.GuardObservables.Where(GO => GO.detectionLevel > 0).ToList()
                .ForEach(GO => GO.detectionLevel -= _guardProperties.DetectionFalloffRate * Time.fixedDeltaTime);
        }
        Task.current.Succeed();
    }


    List<GameObject> SeesObservable(Vision vision)
    {
        List<GameObject> seenObservables = new List<GameObject>();
        if (_guardProperties.GuardObservables == null)
            return null;

        foreach (GuardProperties.GuardThreat observable in _guardProperties.GuardObservables.Where(o =>
            Vector3.Distance(transform.position, o.GameObject.transform.position) <= vision.Distance))
        {
            Vector3 observableVector = observable.GameObject.transform.position - HeadObject.transform.position;

            if (Vector3.Angle(vision.StartPoint.TransformDirection(vision.Direction), observableVector) < vision.Angle / 2)
            {
                Physics.Raycast(HeadObject.transform.position, observableVector, out var hit, vision.Distance);
                if (hit.transform.gameObject == observable.GameObject)
                {
                    seenObservables.Add(hit.transform.gameObject);
                }
            }
        }

        return seenObservables;
    }

    [Task]
    public void ChasePlayer()
    {
        _guardProperties.currentState = GuardProperties.GuardStates.CHASING;
        agent.speed = 8;
        agent.destination = _guardProperties.GuardObservables.OrderByDescending(GO => GO.detectionLevel)
            .First().GameObject.transform.position;
        Task.current.Succeed();
    }


#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        
        foreach (Vision vision in visions)
        {
            Vector3 Startpoint = Quaternion.Euler(0, -vision.Angle / 2, 
                0) * vision.StartPoint.TransformDirection(vision.Direction);
            Handles.color = vision.DEBUG_COLOR;
            Handles.DrawSolidArc(transform.position + Vector3.up, 
                transform.up, 
                Startpoint, vision.Angle,
                vision.Distance);
    
        }
        

        
        
        
        
    }


#endif
}