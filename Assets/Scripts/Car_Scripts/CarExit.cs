using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using Photon.Pun;

public class CarExit : MonoBehaviour
{
    [SerializeField] private Transform  playerController,  enterCube, trackingSpace, ExitDestination, xrRig;
    [SerializeField] private HandCollision rightHandModel;
    [SerializeField] private HandCollision leftHandModel;
    Quaternion initialRotation;
    Vector3 initialPosition;
    public VehicleControl vehicle;

    //public XRReff xrReff;

    public float playerHeightExit = -0.025f;
    public bool intheCar = false;

    void Start()
    {
        //if (xrReff.PV.IsMine)
        //{
            initialRotation = trackingSpace.localRotation; // store the initial rotation of the tracking space
            initialPosition = trackingSpace.localPosition; //  store the initial position of the tracking space
        //}
      
    }

    void Update()
    {
        //if (xrReff.PV.IsMine)
        //{
            if (playerController.transform.position.y < -75f && intheCar == true || intheCar == true && InputBridge.Instance.XButtonDown)
            {
                playerController.transform.parent = xrRig.transform;

                playerController.transform.position = ExitDestination.position; //transport player out of the car

                playerController.GetComponent<PlayerGravity>().enabled = true; //enable gravity

                playerController.GetComponent<CharacterController>().enabled = true;

                playerController.GetComponent<BNGPlayerController>().CharacterControllerYOffset = playerHeightExit;

                playerController.GetComponent<PlayerTeleport>().enabled = true;

                playerController.GetComponent<LocomotionManager>().enabled = true;

                playerController.GetComponent<SmoothLocomotion>().enabled = true;

                playerController.GetComponent<PlayerRotation>().enabled = true;

                //reenable hand collision so you can punch things again
                rightHandModel.EnableCollisionOnPoint = true;
                rightHandModel.EnableCollisionOnFist = true;
                leftHandModel.EnableCollisionOnPoint = true;
                leftHandModel.EnableCollisionOnFist = true;

                trackingSpace.localRotation = initialRotation; // reset tracking space rotation
                trackingSpace.localPosition = initialPosition; // reset tracking space position

                vehicle.activeControl = false; // disable car

                enterCube.gameObject.SetActive(true); //enable the enter cube 

                intheCar = false;
            //}
        }
    }
}
