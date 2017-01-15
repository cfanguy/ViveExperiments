using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {
    public GameObject projectile;
    public GameObject paintObj;
    public float projectile_force;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
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

        PickupReleaseObject();
        TriggerAction();
        DrawRay(true);

        /*if (controller.GetPressDown(touchButton))
        {
            DrawRay(true);
        }*/

        if (controller.GetPressUp(touchButton))
        {
            //DrawRay(false);
            Teleport();
        }
    }

    private void PickupReleaseObject()
    {
        if (controller.GetPressDown(gripButton) && pickup != null)
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
                pickup = null;
            }
        }
    }

    private void TriggerAction()
    {
        if (controller.GetPressDown(triggerButton) && pickup.name == "blaster")
        {
            Vector3 newPosition = pickup.transform.position;

            GameObject temp_projectile = Instantiate(projectile, newPosition, pickup.transform.rotation) as GameObject;
            Physics.IgnoreCollision(pickup.GetComponent<Collider>(), temp_projectile.GetComponent<Collider>());

            Rigidbody temp_rigidBody = temp_projectile.AddComponent<Rigidbody>();
            temp_rigidBody.useGravity = false;

            temp_rigidBody.AddForce(pickup.transform.forward * projectile_force);

            Destroy(temp_projectile, 3.0f);
        }

        if (controller.GetPress(triggerButton) && pickup.name == "paint")
        {
            Vector3 newPosition = pickup.transform.position;

            GameObject temp_projectile = Instantiate(paintObj, newPosition, pickup.transform.rotation) as GameObject;
            Physics.IgnoreCollision(pickup.GetComponent<Collider>(), temp_projectile.GetComponent<Collider>());

            Rigidbody temp_rigidBody = temp_projectile.AddComponent<Rigidbody>();
            temp_rigidBody.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "pickup")
        {
            pickup = collider.gameObject;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "pickup")
        {
            pickup = null;
        }
    }

    private void Teleport()
    {
        var wandRef = reference;
        if (wandRef == null)
            return;

        float refY = wandRef.position.y;

        Plane plane = new Plane(Vector3.up, -refY);
        Ray ray = new Ray(this.transform.position, transform.forward);

        bool hasGroundTarget = false;
        float dist = 0f;

        hasGroundTarget = plane.Raycast(ray, out dist);

        if (hasGroundTarget)
        {
            Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
            wandRef.position = ray.origin + ray.direction * dist - new Vector3(wandRef.GetChild(0).localPosition.x, 0f, wandRef.GetChild(0).localPosition.z) - headPosOnGround;
        }
    }

    private void DrawRay(bool toDraw)
    {
        if(toDraw)
        {
            var wandRef = reference;
            if (wandRef == null)
                return;

            float refY = wandRef.position.y;

            Plane plane = new Plane(Vector3.up, -refY);
            Ray ray = new Ray(this.transform.position, transform.forward);

            bool hasGroundTarget = false;
            float dist = 0f;

            hasGroundTarget = plane.Raycast(ray, out dist);

            if (hasGroundTarget)
            {
                Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
                
                Vector3 newPosition = ray.origin + ray.direction * dist - new Vector3(wandRef.GetChild(0).localPosition.x, 0f, wandRef.GetChild(0).localPosition.z) - headPosOnGround;
                DrawLine(ray.origin, newPosition, Color.cyan, 0.01f);
            }
            
        }
    }


    void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(0.005f, 0.005f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
}
