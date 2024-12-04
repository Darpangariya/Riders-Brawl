using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using System;

public class MyBikeControll : MonoBehaviour
{
    public bool activeControl = false;

    //VR Controll

    [System.Serializable]
    public class VRControll
    {
        public float steerVR;
        public float accelVR;
        public bool brakeBool;
        public bool whellie;
        public TMP_Text speedText;
    }
    public VRControll vRControll;

 

    public BikeWheels bikeWheels;

    //For Bike Sounds
    private float Pitch;
    private float PitchDelay;
    public AudioSource IdleEngine, LowEngine, HighEngine;


    [System.Serializable]
    public class BikeWheels
    {
        public ConnectWheel wheels;
        public WheelSetting setting;
    }


    [System.Serializable]
    public class ConnectWheel
    {

        public Transform wheelFront;
        public Transform wheelBack; 

        public Transform AxleFront; 
        public Transform AxleBack; 

    }



    [System.Serializable]
    public class WheelSetting
    {

        public float Radius = 0.3f;
        public float Weight = 1000.0f; 
        public float Distance = 0.2f;

    }

    public BikeSetting bikeSetting;

    [System.Serializable]
    public class BikeSetting
    {


        public bool showNormalGizmos = false;

        public List<Transform> cameraSwitchView;


        public Transform MainBody;
        public Transform bikeSteer;


        public float maxWheelie = 40.0f;
        public float speedWheelie = 30.0f;

        public float slipBrake = 3.0f;


        public float springs = 35000.0f;
        public float dampers = 4000.0f;

        public float bikePower = 120;
        public float shiftPower = 150;
        public float brakePower = 8000;

        public Vector3 shiftCentre = new Vector3(0.0f, -0.6f, 0.0f); 

        public float maxSteerAngle = 30.0f; 
        public float maxTurn = 1.5f;

        public float shiftDownRPM = 1500.0f; 
        public float shiftUpRPM = 4000.0f; 
        public float idleRPM = 700.0f;

        public float stiffness = 1.0f;



        public bool automaticGear = true;

        public float[] gears = { -10f, 9f, 6f, 4.5f, 3f, 2.5f };

        public float LimitBackwardSpeed = 60.0f;
        public float LimitForwardSpeed = 220.0f;


    }

    private Quaternion SteerRotation;

    [HideInInspector]
    public bool grounded = true;

    private float MotorRotation;

    [HideInInspector]
    public bool crash;


    [HideInInspector]
    public float steer = 0; 
    [HideInInspector]
    public bool brake;


    public float slip = 0.0f;


    [HideInInspector]
    public bool Backward = false;

    [HideInInspector]
    public float steer2;

    public float accel = 0.0f; 
    public float Z_Rotation = 5;

    private bool shifmotor;

    [HideInInspector]
    public float curTorque = 100f;

    [HideInInspector]
    public float powerShift = 100;

    [HideInInspector]
    public bool shift;

    private float flipRotate = 0.0f;


    //[HideInInspector]
    public float speed = 0.0f;

    float[] efficiencyTable = { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 1.0f, 1.0f, 0.95f, 0.80f, 0.70f, 0.60f, 0.5f, 0.45f, 0.40f, 0.36f, 0.33f, 0.30f, 0.20f, 0.10f, 0.05f };

    float efficiencyTableStep = 250.0f;


    private float shiftDelay = 0.0f;

    private float shiftTime = 0.0f;


    [HideInInspector]
    public int currentGear = 0;
    [HideInInspector]
    public bool NeutralGear = true;

    [HideInInspector]
    public float motorRPM = 0.0f;



    private float wantedRPM = 0.0f;
    private float w_rotate;

    private Rigidbody myRigidbody;

    private bool shifting;

    private float Wheelie;
    public Quaternion deltaRotation1, deltaRotation2;

    [HideInInspector]
    public float accelFwd = 0.0f;
    [HideInInspector]
    public float accelBack = 0.0f;
    [HideInInspector]
    public float steerAmount = 0.0f;

    private WheelComponent[] wheels;

    private class WheelComponent
    {

        public Transform wheel;
        public Transform axle;
        public WheelCollider collider;
        public Vector3 startPos;
        public float rotation = 0.0f;
        public float maxSteer;
        public bool drive;
        public float pos_y = 0.0f;
    }



    private WheelComponent SetWheelComponent(Transform wheel, Transform axle, bool drive, float maxSteer, float pos_y)
    {

        WheelComponent result = new WheelComponent();
        GameObject wheelCol = new GameObject(wheel.name + "WheelCollider");



        wheelCol.transform.parent = transform;
        wheelCol.transform.position = wheel.position;
        wheelCol.transform.eulerAngles = transform.eulerAngles;
        pos_y = wheelCol.transform.localPosition.y;


       
        wheelCol.AddComponent(typeof(WheelCollider));

       
        result.drive = drive;
        result.wheel = wheel;
        result.axle = axle;
        result.collider = wheelCol.GetComponent<WheelCollider>();
        result.pos_y = pos_y;
        result.maxSteer = maxSteer;
        result.startPos = axle.transform.localPosition + new Vector3(0,+ 0.150f,0);

        return result;

    }

    void Awake()
    {

        if (bikeSetting.automaticGear) NeutralGear = false;

        myRigidbody = transform.GetComponent<Rigidbody>();

        SteerRotation = bikeSetting.bikeSteer.localRotation;
        wheels = new WheelComponent[2];

        wheels[0] = SetWheelComponent(bikeWheels.wheels.wheelFront, bikeWheels.wheels.AxleFront, false, bikeSetting.maxSteerAngle, bikeWheels.wheels.AxleFront.localPosition.y - 1);
        wheels[1] = SetWheelComponent(bikeWheels.wheels.wheelBack, bikeWheels.wheels.AxleBack, true, 0, bikeWheels.wheels.AxleBack.localPosition.y);

        wheels[0].collider.transform.localPosition = new Vector3(0, wheels[0].collider.transform.localPosition.y, wheels[0].collider.transform.localPosition.z);
        wheels[1].collider.transform.localPosition = new Vector3(0, wheels[1].collider.transform.localPosition.y, wheels[1].collider.transform.localPosition.z);


        foreach (WheelComponent w in wheels)
        {


            WheelCollider col = w.collider;

            col.suspensionDistance = Mathf.Clamp(bikeWheels.setting.Distance, 0.1f, 0.3f);
            JointSpring js = col.suspensionSpring;

            //js.spring = bikeSetting.springs;
            //js.damper = bikeSetting.dampers;
            js.spring = Mathf.Clamp(bikeSetting.springs, 40000.0f, 60000.0f);
            js.damper = Mathf.Clamp(bikeSetting.dampers, 5000.0f, 8000.0f);
            col.suspensionSpring = js;


            col.radius = bikeWheels.setting.Radius;


            col.mass = bikeWheels.setting.Weight;


            WheelFrictionCurve fc = col.forwardFriction;

            fc.extremumSlip = 0.2f; // Lower value for more grip
            fc.extremumValue = 1.5f;
            fc.asymptoteSlip = 0.4f;
            fc.asymptoteValue = 0.75f;
            fc.stiffness = bikeSetting.stiffness; // Consider increasing stiffness for better grip
            w.collider.forwardFriction = fc;
            w.collider.sidewaysFriction = fc;

        }
    }
    public void ShiftUp()
    {

        float now = Time.timeSinceLevelLoad;

        if (now < shiftDelay) return;

        if (currentGear < bikeSetting.gears.Length - 1)
        {

            if (!bikeSetting.automaticGear)
            {
                if (currentGear == 0)
                {
                    if (NeutralGear) { currentGear++; NeutralGear = false; }
                    else
                    { NeutralGear = true; }
                }
                else
                {
                    currentGear++;
                }
            }
            else
            {
                currentGear++;
            }


               shiftDelay = now + 1.0f;
               shiftTime = 1.0f;
        }
    }

    public void ShiftDown()
    {
           float now = Time.timeSinceLevelLoad;

           if (now < shiftDelay) return;

        if (currentGear > 0 || NeutralGear)
        {

            if (!bikeSetting.automaticGear)
            {

                if (currentGear == 1)
                {
                    if (!NeutralGear) { currentGear--; NeutralGear = true; }
                }
                else if (currentGear == 0) { NeutralGear = false; } else { currentGear--; }
            }
            else
            {
                currentGear--;
            }
                shiftDelay = now + 0.1f;
                shiftTime = 2.0f;
        }
    }

    void Update()
    {

        if (activeControl)
        {

            if (!bikeSetting.automaticGear)
            {
                if (Input.GetKeyDown("page up"))
                {
                    ShiftUp();


                }
                if (Input.GetKeyDown("page down"))
                {
                    ShiftDown();

                }
            }

        }
        float adjustedSteerAngle = bikeSetting.maxSteerAngle * Mathf.Clamp(1.0f - (speed / bikeSetting.LimitForwardSpeed), 0.2f, 1.0f);
        //steer2 = Mathf.LerpAngle(steer2, steer * -adjustedSteerAngle, Time.deltaTime * 20.0f);

        steer2 = Mathf.LerpAngle(steer2, steer * -bikeSetting.maxSteerAngle, Time.deltaTime * 50.0f);

        MotorRotation = Mathf.LerpAngle(MotorRotation, steer2 * bikeSetting.maxTurn * (Mathf.Clamp(speed / Z_Rotation, 0.0f, 1.0f)), Time.deltaTime * 10.0f);

        if (bikeSetting.bikeSteer)
            bikeSetting.bikeSteer.localRotation = SteerRotation * Quaternion.Euler(0, wheels[0].collider.steerAngle, 0); // this is 90 degrees around y axis

        if (!crash)
        {
            flipRotate = (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270) ? 180.0f : 0.0f;

            Wheelie = Mathf.Clamp(Wheelie, 0, bikeSetting.maxWheelie);


            if (shifting)
            {
                Wheelie += bikeSetting.speedWheelie * Time.deltaTime / (speed / 50);
            }
            else
            {
                Wheelie = Mathf.MoveTowards(Wheelie, 0, (bikeSetting.speedWheelie * 2) * Time.deltaTime * 1.3f);
            }

            deltaRotation1 = Quaternion.Euler(-Wheelie, 0, flipRotate - transform.localEulerAngles.z + (MotorRotation));
            deltaRotation2 = Quaternion.Euler(0, 0, flipRotate - transform.localEulerAngles.z);

            if (speed >= 0)
            {
                myRigidbody.MoveRotation(myRigidbody.rotation * deltaRotation2);
                bikeSetting.MainBody.localRotation = deltaRotation1;

            }
            else
            {
                bikeSetting.MainBody.localRotation = Quaternion.identity;
                Wheelie = 0;
            }
        }
    }

    void FixedUpdate()
    {

        speed = myRigidbody.velocity.magnitude * 2.7f;


        if (crash)
        {
            myRigidbody.constraints = RigidbodyConstraints.None;
            myRigidbody.centerOfMass = Vector3.zero;
        }
        else
        {
            if (brake || accel <= 0.0f)
            {
                // Move the center of mass slightly forward and downward during braking
                myRigidbody.centerOfMass = bikeSetting.shiftCentre + new Vector3(0, -0.1f, -0.2f);
            }
            //myRigidbody.constraints = RigidbodyConstraints.FreezeRotationZ; // Ensure only Z-axis rotation is constrained if needed
            else
            {
                myRigidbody.centerOfMass = bikeSetting.shiftCentre + new Vector3(0, 0.1f, 0.2f);
            }
        }

        if (activeControl)
        {

            accel = 0.0f;
            shift = false;
            brake = false;

            if (!crash)
            {

                steer = Mathf.MoveTowards(steer, vRControll.steerVR, 0.1f); //steerVR  Input.GetAxis("Horizontal")
                //accel = Input.GetAxis("Vertical"); //Input.GetAxis("Vertical"); //accelVR;
                accel = vRControll.accelVR;//Input.GetKey(KeyCode.W) ? 1.0f : 0.0f; //Input.GetAxis("Vertical"); //accelVR;
                brake = vRControll.brakeBool;//Input.GetKey(KeyCode.S); //vRControll.brakeBool; //Input.GetButton("Jump");
                shift = vRControll.whellie;  //Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
            }
            else
            {
                steer = 0;
            }
        }
        else
        {
            accel = 0.0f;
            steer = 0.0f;
            shift = false;
            brake = false;
        }


        if (bikeSetting.automaticGear && (currentGear == 1) && (accel < 0.0f))
        {
            if (speed < 1.0f)
                ShiftDown();

        }
        else if (bikeSetting.automaticGear && (currentGear == 0) && (accel > 0.0f))
        {
            if (speed < 5.0f)
                ShiftUp(); 

        }
        else if (bikeSetting.automaticGear && (motorRPM > bikeSetting.shiftUpRPM) && (accel > 0.0f) && speed > 10.0f && !brake)
        {
            ShiftUp(); 

        }
        else if (bikeSetting.automaticGear && (motorRPM < bikeSetting.shiftDownRPM) && (currentGear > 1))
        {
            ShiftDown(); 
        }

        if (speed < 1.0f) Backward = true;
        

        if (currentGear == 0 && Backward == true)
        {
            if (speed < bikeSetting.gears[0] * -10)
                accel = -accel; 
        }
        else
        {
            Backward = false;
        }

        wantedRPM = (5500.0f * accel) * 0.1f + wantedRPM * 0.9f;

        float rpm = 0.0f;
        int motorizedWheels = 0;
        bool floorContact = false;
        int currentWheel = 0;

        foreach (WheelComponent w in wheels)
        {
            WheelHit hit;
            WheelCollider col = w.collider;

            if (w.drive)
            {
                if (!NeutralGear && brake && currentGear < 2)
                {
                    rpm += accel * bikeSetting.idleRPM;
                    
                    if (rpm > 1)
                    {
                        bikeSetting.shiftCentre.z = Mathf.PingPong(Time.time * (accel * 10), 0.5f) - 0.25f;
                    }
                    else
                    {
                        bikeSetting.shiftCentre.z = 0.0f;
                    }
                    
                }
                else
                {
                    if (!NeutralGear)
                    {
                        rpm += col.rpm;
                    }
                    else
                    {
                        rpm += ((bikeSetting.idleRPM * 2.0f) * accel);
                    }
                }

                motorizedWheels++;
            }


            if (crash)
            {
                w.collider.enabled = false;
                w.wheel.GetComponent<Collider>().enabled= true;
            }
            else
            {
                w.collider.enabled = true;
                w.wheel.GetComponent<Collider>().enabled = false;
            }




            if (brake || accel < 0.0f)
            {
                if ((brake && w == wheels[1]))// ||(accel < 0.0f))
                {

                    if (brake)// && (accel > 0.0f))
                    {
                        slip = Mathf.Lerp(slip, bikeSetting.slipBrake, accel * 0.01f);
                    }
                    else if (speed > 1.0f)
                    {
                        slip = Mathf.Lerp(slip, 1.0f, 0.002f);
                    }
                    else
                    {
                        slip = Mathf.Lerp(slip, 1.0f, 0.02f);
                    }


                    wantedRPM = 0.0f;
                    col.brakeTorque = bikeSetting.brakePower;
                    w.rotation = w_rotate;

                }
            }
            else
            {

                col.brakeTorque = accel == 0 ? col.brakeTorque = 3000 : col.brakeTorque = 0;

                slip = Mathf.Lerp(slip, 1.0f, 0.02f);

                w_rotate = w.rotation;

            }

            WheelFrictionCurve fc = col.forwardFriction;


            if (w == wheels[1])
            {
                fc.stiffness = bikeSetting.stiffness / slip;
                col.forwardFriction = fc;
                fc = col.sidewaysFriction;
                fc.stiffness = bikeSetting.stiffness / slip;
                col.sidewaysFriction = fc; 
            }

            if (shift && (currentGear > 1 && speed > 50.0f) && shifmotor)
            {
                shifting = true;
                if (powerShift == 0) { shifmotor = false; }

                powerShift = Mathf.MoveTowards(powerShift, 0.0f, Time.deltaTime * 10.0f);

                curTorque = powerShift > 0 ? bikeSetting.shiftPower : bikeSetting.bikePower;
            }
            else
            {
                shifting = false;

                if (powerShift > 20)
                {
                    shifmotor = true;
                }

                powerShift = Mathf.MoveTowards(powerShift, 100.0f, Time.deltaTime * 5.0f);
                curTorque = Mathf.Lerp(curTorque, bikeSetting.bikePower * accel, Time.deltaTime * 5.0f);
            }


            w.rotation = Mathf.Repeat(w.rotation + Time.deltaTime * col.rpm * 360.0f / 60.0f, 360.0f);
            w.wheel.localRotation = Quaternion.Euler(w.rotation,0.0f, 0.0f);

            Vector3 lp = w.axle.localPosition;


            if (col.GetGroundHit(out hit) && (w == wheels[1] || (w == wheels[0] && Wheelie == 0)))
            {

                lp.y -= Vector3.Dot(w.wheel.position - hit.point, transform.TransformDirection(0, 1, 0)) - (col.radius);
                lp.y = Mathf.Clamp(lp.y, w.startPos.y - bikeWheels.setting.Distance, w.startPos.y + bikeWheels.setting.Distance);


                floorContact = floorContact || (w.drive);


                if (!crash)
                {

                    myRigidbody.angularDrag = 10.0f;
                }
                else
                {
                    myRigidbody.angularDrag = 0.0f;

                }

                grounded = true;

                if (w.collider.GetComponent<WheelSkidmarks>())
                w.collider.GetComponent<WheelSkidmarks>().enabled = true;

            }
            else
            {
                grounded = false;

                if (w.collider.GetComponent<WheelSkidmarks>())
                w.collider.GetComponent<WheelSkidmarks>().enabled = false;

                lp.y = w.startPos.y - bikeWheels.setting.Distance;

                if (!wheels[0].collider.isGrounded && !wheels[1].collider.isGrounded)
                {

                    myRigidbody.centerOfMass = new Vector3(0, 0.2f, 0);
                    myRigidbody.angularDrag = 1.0f;

                    myRigidbody.AddForce(0, -10000, 0);
                }

            }


            currentWheel++;
            w.axle.localPosition = lp;

        }

        if (motorizedWheels > 1)
        {
            rpm = rpm / motorizedWheels;
        }


        motorRPM = 0.95f * motorRPM + 0.05f * Mathf.Abs(rpm * bikeSetting.gears[currentGear]);
        if (motorRPM > 5500.0f) motorRPM = 5200.0f;


        int index = (int)(motorRPM / efficiencyTableStep);
        if (index >= efficiencyTable.Length) index = efficiencyTable.Length - 1;
        if (index < 0) index = 0;

        float newTorque = curTorque * bikeSetting.gears[currentGear] * efficiencyTable[index];

        foreach (WheelComponent w in wheels)
        {
            WheelCollider col = w.collider;

            if (w.drive)
            {

                if (Mathf.Abs(col.rpm) > Mathf.Abs(wantedRPM))
                {

                    col.motorTorque = 0;
                }
                else
                {
                    
                    float curTorqueCol = col.motorTorque;

                    if (!brake && accel != 0 && NeutralGear == false)
                    {
                        if ((speed < bikeSetting.LimitForwardSpeed && currentGear > 0) ||
                            (speed < bikeSetting.LimitBackwardSpeed && currentGear == 0))
                        {

                            col.motorTorque = curTorqueCol * 0.9f + newTorque * 1.0f;
                        }
                        else
                        {
                            col.motorTorque = 0;
                            col.brakeTorque = 2000;
                        }


                    }
                    else
                    {
                        col.motorTorque = 0;

                    }
                }

            }

            float SteerAngle = Mathf.Clamp((speed) / bikeSetting.maxSteerAngle, 1.0f, bikeSetting.maxSteerAngle);
            col.steerAngle = steer * (w.maxSteer / SteerAngle);

        }

        Pitch = Mathf.Clamp(1.2f + ((motorRPM - bikeSetting.idleRPM) / (bikeSetting.shiftUpRPM - bikeSetting.idleRPM)), 1.0f, 10.0f);

        shiftTime = Mathf.MoveTowards(shiftTime, 0.0f, 0.1f);

        if (Pitch == 1)
        {
            IdleEngine.volume = Mathf.Lerp(IdleEngine.volume, 1.0f, 0.1f);
            LowEngine.volume = Mathf.Lerp(LowEngine.volume, 0.5f, 0.1f);
            HighEngine.volume = Mathf.Lerp(HighEngine.volume, 0.0f, 0.1f);

        }
        else
        {

            IdleEngine.volume = Mathf.Lerp(IdleEngine.volume, 1.8f - Pitch, 0.1f);


            if ((Pitch > PitchDelay || accel > 0) && shiftTime == 0.0f)
            {
                LowEngine.volume = Mathf.Lerp(LowEngine.volume, 0.0f, 0.2f);
                HighEngine.volume = Mathf.Lerp(HighEngine.volume, 1.0f, 0.1f);
            }
            else
            {
                LowEngine.volume = Mathf.Lerp(LowEngine.volume, 0.5f, 0.1f);
                HighEngine.volume = Mathf.Lerp(HighEngine.volume, 0.0f, 0.2f);
            }




            HighEngine.pitch = Pitch;
            LowEngine.pitch = Pitch;

            PitchDelay = Pitch;
        }
        //vRControll.speedText.text = speed.ToString() + "KM/h: ";
        vRControll.speedText.text = String.Format("{0:0}", speed) + "KM/h";



    }
    void OnDrawGizmos()
    {

        if (!bikeSetting.showNormalGizmos || Application.isPlaying) return;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        Gizmos.matrix = rotationMatrix;
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(Vector3.up/1.6f, new Vector3(0.5f, 1.0f, 2.5f));
        Gizmos.DrawSphere(bikeSetting.shiftCentre, 0.2f);

    }
}