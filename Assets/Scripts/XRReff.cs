using BNG;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRReff : MonoBehaviour
{
    public static XRReff xrReff;
    public HandCollision rightHandGroveModel, leftHandGroveModel;
    public Grabber rightGrabber, leftGrabber;
    public Transform trackingSpace, xrRig;
    public GameObject playerController;
    public PhotonView PV;


    private void Start()
    {
        xrReff = this;
    }

}
