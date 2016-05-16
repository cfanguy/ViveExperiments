using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private GameObject pickup;

    private SteamVR_Controller.Device controller
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }
    private Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    private SteamVR_TrackedObject trackedObj;

	// Use this for initialization
	private void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        
    }

    // Update is called once per frame
    private void Update () {
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

        if(controller.GetPressDown(touchButton))
        {
            Debug.Log("Pressed Down");
            Teleport();
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

    private void Teleport()
    {
        var t = reference;
        if (t == null)
            return;

        float refY = t.position.y;

        Plane plane = new Plane(Vector3.up, -refY);
        Ray ray = new Ray(this.transform.position, transform.forward);

        bool hasGroundTarget = false;
        float dist = 0f;

        hasGroundTarget = plane.Raycast(ray, out dist);

        if (hasGroundTarget)
        {
            Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
            t.position = ray.origin + ray.direction * dist - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
        }
    }
}
