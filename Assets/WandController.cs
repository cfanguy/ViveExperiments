using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private GameObject pickup;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

	// Use this for initialization
	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if (controller.GetPressDown(triggerButton) && pickup != null)
        {
            if (pickup.transform.parent == null)
            {
                pickup.transform.parent = this.transform;
                pickup.GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                pickup.transform.parent = null;
                pickup.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        pickup = collider.gameObject;
    }

    private void OnTriggerEnd(Collider collider)
    {
        pickup = null;
    }
}
