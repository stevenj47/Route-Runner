using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Attributes;

public class Momentum : MonoBehaviour {
    public Rigidbody aircraft;
    public float angles;
    public LeapServiceProvider lsp;
    public GameObject OculusEye;
   // public OVRCameraRig camerarig;
    
    
    //private Camera cam;

    private static float momentum;
    private bool turning;

    public bool player_control;


	// Use this for initialization
	void Start () {
        aircraft = GetComponent<Rigidbody>();
        momentum = 0.0f;
        turning = false;
        player_control = false;
        //cam = Camera.main;
    }
	


	// Update is called once per frame
	void Update () {
     
    }


    public void Accelerate_Momentum () {
        if (player_control)
        {
            momentum += 30.0f;
            //accelerate the aircraft
            aircraft.velocity = momentum * aircraft.transform.forward;
            Debug.Log("acceleration to " + momentum);
        }
    }

    public void Decelerate_Momentum () {
        
            momentum = 0.0f;
            aircraft.velocity = momentum * aircraft.transform.forward;
            Debug.Log("deceleration to " + momentum);
        
    }

    public void Turn(Quaternion heading) {

        aircraft.rotation = heading;
    }

    public void StartTurn() {
        Debug.Log("start turn");
        if (player_control)
        {
            turning = true;

            aircraft.transform.rotation = OculusEye.transform.rotation;
            aircraft.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            aircraft.velocity = momentum * aircraft.transform.forward;
        }

    }
    public void EndTurn() {
        Debug.Log("end turn");
        turning = false;
    }


}

