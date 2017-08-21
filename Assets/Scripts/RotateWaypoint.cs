using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWaypoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        gameObject.transform.Rotate(0, 90*Time.fixedDeltaTime, 0);
    }
}
