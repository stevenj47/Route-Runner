using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAircraft : MonoBehaviour {
    [SerializeField] GameObject target;

	// Use this for initialization
	void Start () {
        this.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update () {
        this.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
    }
}
