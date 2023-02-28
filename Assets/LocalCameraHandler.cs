using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class LocalCameraHandler : MonoBehaviour
{

    public Transform cameraAnchorPoint;

    [SerializeField] Camera localCamera;

    Vector2 viewInput;

    public Rigidbody rb;

    //public ControllerPrototype myCar;

    //Vector3 ShipVel;
    public float faceDir;

    float cameraRotationX = 0;
    float cameraRotationY = 0;

    float rotSpeed = 10;

    private void Awake()
    {
        localCamera = GetComponent<Camera>();
    }
    // Start is called before the first frame update


    void Start()
    {

        //myCar = GameObject.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Avatar");
        foreach (GameObject player in players)
        {
            ControllerPrototype cpt = player.GetComponent<ControllerPrototype>();
            if (cpt.HasStateAuthority)
            {
                this.cameraAnchorPoint = player.transform;
                break;
            }
        }

        if (localCamera.enabled)
        {
            //localCamera.transform.parent = null;
        }




    }

    // Update is called once per frame
    void LateUpdate()
    {

        //ShipVel = rb.velocity;

        //myCar = GameObject.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            ControllerPrototype cpt = player.GetComponent<ControllerPrototype>();
            if (cpt.HasStateAuthority)
            {
                //this.cameraAnchorPoint = player.transform;
                this.transform.parent = player.transform.GetChild(0).transform;
                break;
            }
        }

        if (localCamera.enabled)
        {
            //localCamera.transform.parent = null;
        }

        transform.localPosition = new Vector3(0, 3, -4);
        transform.localRotation = Quaternion.Euler(20, 0, 0);

        /*
        if (cameraAnchorPoint == null)
            return;

        if (!localCamera.enabled)
            return;
        */

        //localCamera.transform.position = cameraAnchorPoint.position;

        //localCamera.transform.rotation = Quaternion.Slerp(localCamera.transform.rotation, cameraAnchorPoint.rotation, 0.05f);



    }

}
