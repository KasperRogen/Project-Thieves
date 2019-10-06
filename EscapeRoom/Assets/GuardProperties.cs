using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Panda;

public class GuardProperties : NetworkBehaviour
{
    
    
    [Header("Detection Properties")] 
    public float DetectionFalloffRate;
    public float DetectionThreshold;
    public float MaxDetectionLevel;
    public GuardStates currentState;
    
    
    
    
    
    [SerializeField]
    public List<GuardThreat> GuardObservables = new List<GuardThreat>();
    
    [Serializable]
    public class GuardThreat
    {
        public GuardThreat(GameObject gameObject, float detectionLevel)
        {
            GameObject = gameObject;
            this.detectionLevel = detectionLevel;
        }

        public GameObject GameObject;
        public float detectionLevel;
    }

    
    public enum GuardStates
    {
        IDLING,
        INSPECTING,
        CHASING,
        SHOOTING,
        DEAD
    }
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    
    
    
    
    
    
    [Task]
    void HasDetectedTarget()
    {
        Task.current.Complete(GuardObservables.Any(GO => GO.detectionLevel >= DetectionThreshold));
    }
    
    
}
