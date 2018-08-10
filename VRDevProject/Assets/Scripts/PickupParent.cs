using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class PickupParent : MonoBehaviour
{

    SteamVR_TrackedObject trackedObj;
    SteamVR_Controller.Device device;

    public Transform sphere;

    //called at start (before Start())
    void Awake ()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per physics step (consistant)
	void FixedUpdate ()
    {
        device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you are Holding 'Touch' on the trigger");
        }
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you have acitvated touch down on the trigger");
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you have acitvated touch up on the trigger");
        }

        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you are Holding 'Press' on the trigger");
        }
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you have acitvated Press down on the trigger");
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("you have acitvated Press up on the trigger");
        }

        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))  //reset sphere position
        {
            Debug.Log("you have acitvated Press up on the Touchpad");
            sphere.transform.position = new Vector3(0,1,0); //reset position
            sphere.GetComponent<Rigidbody>().velocity = Vector3.zero; // Vector3.zero short hand for: new Vector3(0,0,0) // reset veocoity so it stays still
        }
    }

    private void OnTriggerStay(Collider col) //col is collider the the contorler has collided with
    {
        Debug.Log("You have collided with " + col.name + "and activated OnTriggerStay");
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You have collided with " + col.name + "while holding down Touch");
            col.attachedRigidbody.isKinematic = true; //so physics doesn't affect it while holding it
            col.gameObject.transform.SetParent(this.gameObject.transform);
        }
        if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Debug.Log("You have released Touch while colliding with" + col.name);
            col.gameObject.transform.SetParent(null);
            col.attachedRigidbody.isKinematic = false;

            tossObject(col.attachedRigidbody);
        }
    }

    void tossObject(Rigidbody rigidBody)
    {
        Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent; //? is if before ? is true first variable after ? is evaluated and that value is returned and if false second variable is evaluated and returned // basicaly if this is true assign this as origin else assign that as origin // here it is cheaking if this origin exists take it if not take the parent
        if (origin != null)                                                                     //convert contoller local space vectors to unity world space vectors
        {
            rigidBody.velocity = origin.TransformVector(device.velocity);
            rigidBody.angularVelocity = origin.TransformVector(device.angularVelocity);
        }
        else
        {
            rigidBody.velocity = device.velocity;                                               //basic throw the ball
            rigidBody.angularVelocity = device.angularVelocity;
        }
        
    }
}
