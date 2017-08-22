using UnityEngine;
using System.Collections;

public class Navigation : MonoBehaviour 
{
    //the viewer's game object
    public GameObject view_object = null;

    //speed at which we move between waypoints
    public float speed = 0.05f;


    void Start () 
	{
        //first, if the view object is null, use the camera object
        if (view_object == null)
        {
            throw new System.Exception("Please attach camera parent object");
        }
    }

	void Update () {	}


	//moves the player to the current waypoint - if the player is within .05 it snaps them directly on it
	public void MoveTo(GameObject waypoint)
	{
        //view_object.transform.position = waypoint.transform.position;
        iTween.MoveTo(view_object, iTween.Hash("position", waypoint.transform.position, "easeType", "easeInOutExpo"));

    }
}
