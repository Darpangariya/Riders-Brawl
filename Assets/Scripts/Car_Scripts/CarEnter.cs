using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using Photon.Pun;

public class CarEnter : MonoBehaviour
{

    [SerializeField] private GameObject playerController, enterCube;
    [SerializeField] private Transform carDestination, vehicle;
    [SerializeField] private HandCollision rightHandModel;
    [SerializeField] private HandCollision leftHandModel;
    [SerializeField] VRController vrController;
    Quaternion seatRotation; // Rotation of the player once in the car
    Vector3 seatPosition;  // Position of player in the car
    public float carPlayerHeight = -0.35f;




    public void EnterCar()
    {
        seatRotation = carDestination.rotation;
        seatPosition = carDestination.position;

        playerController.transform.rotation = seatRotation; // Set position of the player
        playerController.transform.position = seatPosition; // Set Rotation of the player

        playerController.GetComponent<BNGPlayerController>().CharacterControllerYOffset = carPlayerHeight; // Set height of player once in the car

        playerController.GetComponent<CharacterController>().enabled = false; //  disable character contoller

        playerController.GetComponent<PlayerTeleport>().enabled = false; //  disable player teleport

        playerController.GetComponent<LocomotionManager>().enabled = false; //  disable player locomotion manager

        playerController.GetComponent<SmoothLocomotion>().enabled = false; //  disable player smooth locomotion

        playerController.GetComponent<PlayerGravity>().enabled = false; // disable player gravity

        playerController.GetComponent<PlayerRotation>().enabled = false; //  disable player rotation

        //disable hand collision or suffer beating the car around while you are in it
        rightHandModel.EnableCollisionOnPoint = false;
        rightHandModel.EnableCollisionOnFist = false;
        leftHandModel.EnableCollisionOnPoint = false;
        leftHandModel.EnableCollisionOnFist = false;

            playerController.transform.parent = vehicle.transform; //set player contoller parent from XRrig to the vehicle/


        enterCube.SetActive(false); //  disable the enter cube

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
                EnterCar();
        }
    }
}
