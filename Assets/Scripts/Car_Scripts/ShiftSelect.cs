using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftSelect : MonoBehaviour
{
    public GameObject stickShifter;
    public VehicleControl VehicleControl; //edit
    public int currentGear;


    [ContextMenu("Manual")]
    public void Standard()
    {
        stickShifter.SetActive(true);
        VehicleControl.carSetting.automaticGear = false;
    }


    [ContextMenu("Automatic")]
    public void Automatic()
    {
        currentGear = VehicleControl.currentGear;

        stickShifter.SetActive(false);
        if (currentGear < 1)
        {
            VehicleControl.ShiftUp();
        }
        VehicleControl.carSetting.automaticGear = true;
    }

    public void SwitchToFWD()
    {
        VehicleControl.carWheels.wheels.frontWheelDrive = true;
        VehicleControl.carWheels.wheels.backWheelDrive = false;
    }

    public void SwitchToRWD()
    {
        VehicleControl.carWheels.wheels.frontWheelDrive = false;
        VehicleControl.carWheels.wheels.backWheelDrive = true;
    }

    public void SwitchTo4WD()
    {
        VehicleControl.carWheels.wheels.frontWheelDrive = true;
        VehicleControl.carWheels.wheels.backWheelDrive = true;
    }

    public void CanvasClose()
    {
        this.gameObject.SetActive(false);
    }

    public void CanvasOpen()
    {
        this.gameObject.SetActive(true);
    }

}
