using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerShooting : MonoBehaviour
{
    private Stopwatch _timeSinceLastShooting = new Stopwatch();

    [SerializeField]
    float shootingCooldown = 2000f;

    [SerializeField] private ParticleSystem effect;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _timeSinceLastShooting.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _timeSinceLastShooting.ElapsedMilliseconds > shootingCooldown)
        {
            _timeSinceLastShooting.Restart();
            Instantiate(effect.gameObject, 
                transform.GetChild(0).position + transform.GetChild(0).forward * 0.3f, 
                transform.GetChild(0).transform.rotation);
            RaycastHit hit;
            Camera cam = transform.GetChild(0).GetComponent<Camera>();
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 20))
            {
                Debug.Log(hit.transform.root.name);
                if (hit.transform.root.CompareTag("Guard"))
                {
                    hit.transform.root.GetComponent<GuardVision>().Die();
                }
            }
        }
    }
}
