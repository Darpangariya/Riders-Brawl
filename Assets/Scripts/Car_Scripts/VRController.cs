using UnityEngine;
using BNG;
using System.Collections;
using Photon.Pun;

public class VRController : MonoBehaviour
{

    public enum TypeOfVehcle
    {
        TwoWheeler,
        FourWheeler,
    };
    [Header("Select type")]
    public TypeOfVehcle typeOfVehcle;
    [SerializeField] Transform playerControl;
    [System.Serializable]
    public class VehcleValue
    {
        public float steerFloat;
        public float accelFloat;
        public bool brakeBool;
        public bool shiftBool;
        public bool wellie;
       
    }

    public VehcleValue vehcleValue;
    public bool twoWheeler = false;
    public bool fourWheeler = false;

    [Header("Bike Controller")]
    public MyBikeControll MyBikecontrols;

    [Header("Car Controller")]
    public VehicleControl VehicleControl;

    public int vehicleGear;
    float reverse;


    private void Start()
    {
        if (BasicNetworkManager.ins.multiplayerPlayer && !BasicNetworkManager.ins.singlePlayer)
        {
            if (!GetComponent<PhotonView>().IsMine)
            {
                //playerControl.gameObject.SetActive(false);
                Destroy(playerControl.gameObject);
                GetComponent<MyBikeControll>().activeControl = false;
            }
        }
    }


    void Update()
    {
        //if (GetComponent<PhotonView>().IsMine)
        //{
        vehcleValue.accelFloat = InputBridge.Instance.RightTrigger;// + -InputBridge.Instance.LeftTrigger;
        vehcleValue.brakeBool = InputBridge.Instance.LeftTrigger > 0? true : false;
        //reverse = InputBridge.Instance.XButton ? -1.0f : 0f;
        //vehcleValue.accelFloat = reverse;
        //if (InputBridge.Instance.RightTrigger == 1)
        //{
        //    vehcleValue.accelFloat = 1.0f;
        //}
        //else if (InputBridge.Instance.XButton)
        //{
        //    vehcleValue.accelFloat = -1.0f;
        //}

        if (InputBridge.Instance.AButton)
        {
            vehcleValue.brakeBool = true;
        }

        else
        {
            vehcleValue.brakeBool = false;
        }
    
        switch (typeOfVehcle)
        {
            case TypeOfVehcle.TwoWheeler:
                BikeCotroll();
                break;
            case TypeOfVehcle.FourWheeler:
                CarControll();
                break;
        }

        //}

        
    }


    void BikeCotroll()
    {
        if (InputBridge.Instance.BButton)
        {
                vehcleValue.wellie = true;
        }
        else
        {
            vehcleValue.wellie = false;
        }
        twoWheeler = true;
        MyBikecontrols.vRControll.steerVR = vehcleValue.steerFloat;
        MyBikecontrols.vRControll.accelVR = vehcleValue.accelFloat;
        MyBikecontrols.vRControll.brakeBool = vehcleValue.brakeBool;
        MyBikecontrols.vRControll.whellie = vehcleValue.wellie;
        fourWheeler = false;
    }

    void CarControll()
    {
        if (InputBridge.Instance.BButton)
        {
            vehcleValue.shiftBool = true;
        }

        else
        {
            vehcleValue.shiftBool = false;
        }

        fourWheeler = true;
        twoWheeler  = false;    
        VehicleControl.steerFloat = vehcleValue.steerFloat;
        VehicleControl.accelFloat = vehcleValue.accelFloat;
        VehicleControl.brakeBool = vehcleValue.brakeBool;
        VehicleControl.shiftBool = vehcleValue.shiftBool;

        vehicleGear = VehicleControl.GetComponent<VehicleControl>().currentGear;
    }

    public void BikeSteeling(float onValueChange)
    {
        vehcleValue.steerFloat = onValueChange;
    }

    public void CarSteering(float onValueChange)
    {
        vehcleValue.steerFloat = -onValueChange;
    }
}
