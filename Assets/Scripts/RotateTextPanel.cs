using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTextPanel : MonoBehaviour {

    public GameObject Player;
    private float speed = 5.0f;
    // Use this for initialization
    void Start () {
        if (Player == null) throw new System.Exception("Need a Camera");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dir = Player.transform.position - transform.position;
        dir.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(-dir);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed * Time.deltaTime);
    }
}
