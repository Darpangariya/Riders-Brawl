using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.InputSystem;
using Unity.VisualScripting;

public class CarAIHandler : MonoBehaviour
{
    public enum AIMode { followPlayer, followWaypoints };
    public enum Vehcle { twoWheeler, fourWheeler };

    [Header("AI Mode")]
    public AIMode aIMode;
    public float maxpeed = 16;
    public bool isAvoidCar = false;
    float avoidMultiplyer = 0;

    [Header("Vechle Mode")]
    public Vehcle vehcle;
    public MyBikeControll vehicleControlBike;
    public VehicleControl vehicleControlCar;


    Vector3 targetPostion;
    public Transform targetTransform = null;
    public float seconds;
    public Vector3 vectorToTarget;
    public WaypointNodes currentWaypoint = null;
    public WaypointNodes[] allWaypoints;
    //public VehicleControl vehicleControl;
    CarLapCounter carLapCounter;


    float currentSteerAmount = 0f; // Initialize this at the class level to keep track of the current steer amount

    [Header("Sensors")]
    public float sensorLength = 5;
    public float frontSensorPos = 0.5f;
    public float sideSensorPos = 0.5f;
    public float frontSensorAngle = 30f;


    [Header("Reset Vehcle Position After Stuck")]
    public Vector3 lastPosition;
    public float stuckTime = 0f;
    public float stuckThreshold = 5f; // Time in seconds before considering it stuck
    public float positionThreshold = 0.2f; // Distance to consider movement (lower = more sensitive)
    public float velocityThreshold = 1f; // Distance to consider movement (lower for more sensitivity)
    public float lastResetTime = -5f; // To track when the last reset happened
    public float resetCooldown = 5f;  // Cooldown period after a reset




    // Start is called before the first frame update
    void Awake()
    {
        //vehicleControl = GetComponent<MyBikeControll>();
        allWaypoints = FindObjectsOfType<WaypointNodes>();
        carLapCounter = GetComponent<CarLapCounter>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        switch (vehcle)
        {
            case Vehcle.twoWheeler:
                SensorBike();
                StartCoroutine(nameof(AIBike));
                //CheckIfStuck();
                break;
            case Vehcle.fourWheeler: 
                SensorCar();
                StartCoroutine(nameof(AICar));
                //CheckIfStuck();
                break;
        }
        if (TimeCountdown.instance.raceStarted && !carLapCounter.isRaceCompleted)
        {
            CheckIfStuck();
        }
        
        
    }

    void SensorCar()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        isAvoidCar = false;
        avoidMultiplyer = 0; // Reset the avoid multiplier every frame

        // Move all sensors in front of the vehicle (in local space)
        sensorStartPos += transform.forward * frontSensorPos;

        

        // Front Right Sensor
        Vector3 frontRightSensorPos = sensorStartPos + transform.right * sideSensorPos; // Offset to the right
        if (Physics.Raycast(frontRightSensorPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontRightSensorPos, hit.point, Color.blue); // Front right sensor hit
                isAvoidCar = true;
                avoidMultiplyer -= 1f; // Steer left (stronger)
            }
        }
        // Front right Angle Sensor
        else if (Physics.Raycast(frontRightSensorPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontRightSensorPos, hit.point, Color.cyan); // Front right angled sensor hit
                //isAvoidCar = true;
                //avoidMultiplyer -= 0.5f; // Steer left
            }
        }

        // Front Left Sensor
        Vector3 frontLeftSensorPos = sensorStartPos - transform.right * sideSensorPos; // Offset to the left
        if (Physics.Raycast(frontLeftSensorPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontLeftSensorPos, hit.point, Color.yellow); // Front left sensor hit
                isAvoidCar = true;
                avoidMultiplyer += 1f; // Steer right (stronger)
            }
        }
        // Front left Angle Sensor
        else if (Physics.Raycast(frontLeftSensorPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontLeftSensorPos, hit.point, Color.magenta); // Front left angled sensor hit
                //isAvoidCar = true;
                //avoidMultiplyer += 0.5f; // Steer right
            }
        }

        // Front Center Sensor
        Vector3 frontCenterSensorPos = sensorStartPos; // Middle sensor (no lateral offset)
        if (avoidMultiplyer == 0)
        {
            if (Physics.Raycast(frontCenterSensorPos, transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    Debug.DrawLine(frontCenterSensorPos, hit.point, Color.red); // Front center sensor hit
                    isAvoidCar = true;
                    if (hit.normal.x > 0) 
                    {
                        avoidMultiplyer = -1;
                    }
                    else
                    {
                        avoidMultiplyer = 1;
                    }
                }
            }
        }
    }

    void SensorBike()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position + new Vector3(0, 0.8f, 0);
        isAvoidCar = false;
        avoidMultiplyer = 0; // Reset the avoid multiplier every frame

        sensorStartPos += transform.forward * frontSensorPos;



        // Front Right Sensor
        Vector3 frontRightSensorPos = sensorStartPos + transform.right * sideSensorPos; // Offset to the right
        if (Physics.Raycast(frontRightSensorPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontRightSensorPos, hit.point, Color.blue); // Front right sensor hit
                isAvoidCar = true;
                avoidMultiplyer -= 1f; // Steer left (stronger)
            }
        }
        // Front right Angle Sensor
        else if (Physics.Raycast(frontRightSensorPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontRightSensorPos, hit.point, Color.cyan); // Front right angled sensor hit
                //isAvoidCar = true;
                avoidMultiplyer -= 0.5f; // Steer left
            }
        }

        // Front Left Sensor
        Vector3 frontLeftSensorPos = sensorStartPos - transform.right * sideSensorPos; // Offset to the left
        if (Physics.Raycast(frontLeftSensorPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontLeftSensorPos, hit.point, Color.yellow); // Front left sensor hit
                isAvoidCar = true;
                avoidMultiplyer += 1f; // Steer right (stronger)
            }
        }
        // Front left Angle Sensor
        else if (Physics.Raycast(frontLeftSensorPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(frontLeftSensorPos, hit.point, Color.magenta); // Front left angled sensor hit
                //isAvoidCar = true;
                avoidMultiplyer += 0.5f; // Steer right
            }
        }

        // Front Center Sensor
        Vector3 frontCenterSensorPos = sensorStartPos; // Middle sensor (no lateral offset)
        if (avoidMultiplyer == 0)
        {
            if (Physics.Raycast(frontCenterSensorPos, transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    Debug.DrawLine(frontCenterSensorPos, hit.point, Color.red); // Front center sensor hit
                    isAvoidCar = true;
                    if (hit.normal.x > 0)
                    {
                        avoidMultiplyer = -1;
                    }
                    else
                    {
                        avoidMultiplyer = 1;
                    }
                }
            }
        }
    }


    // https://www.youtube.com/watch?v=vsPzo7IVTHw&list=PLyDa4NP_nvPd1qrfSJB7jLwuF-YQeFr3X&index=1

    IEnumerator AICar()
    {
        if (!carLapCounter.isRaceCompleted)
        {
            yield return new WaitForSeconds(seconds);
            Vector2 inputVector = Vector2.zero;   // Vector y for Accel and vector x for steering
            switch (aIMode)
            {
                case AIMode.followPlayer:
                    FollowPlayer();
                    break;
                case AIMode.followWaypoints:
                    FollowWaypoints();
                    break;
            }
            inputVector.y = ApplyThrottleOrBrake(inputVector.x); // Accelerate
            inputVector.x = TurnTowardTarget(); // Steer
            vehicleControlCar.accelFloat = inputVector.y;
            if (isAvoidCar)
            {
                vehicleControlCar.steerFloat = avoidMultiplyer;
            }
            else if (!isAvoidCar)
            {
                vehicleControlCar.steerFloat = inputVector.x;
            }
        }
        else
            vehicleControlCar.activeControl = false;
    }


    IEnumerator AIBike()
    {
        if (!carLapCounter.isRaceCompleted)
        {
            yield return new WaitForSeconds(seconds);
            Vector2 inputVector = Vector2.zero;   // Vector y for Accel and vector x for steering

            switch (aIMode)
            {
                case AIMode.followPlayer:
                    FollowPlayer();
                    break;
                case AIMode.followWaypoints:
                    FollowWaypoints();
                    break;
            }

            inputVector.y = ApplyThrottleOrBrake(inputVector.x); // Accelerate
            inputVector.x = TurnTowardTarget(); // Steer


            vehicleControlBike.vRControll.accelVR = inputVector.y;
            if (isAvoidCar)
            {
                vehicleControlBike.vRControll.steerVR = avoidMultiplyer;
            }
            else if (!isAvoidCar)
            {
                vehicleControlBike.vRControll.steerVR = inputVector.x;
            }
        }
        else
            vehicleControlBike.activeControl = false;
    }

    void FollowWaypoints()
    {
        if (currentWaypoint == null)
            currentWaypoint = FindClosestWayPoint();
        else if (currentWaypoint != null)
        {
            targetPostion = currentWaypoint.transform.position;

            // Debug line to visualize the path
            Debug.DrawLine(transform.position, targetPostion, Color.green);

            float distanceToWaypoint = (targetPostion - transform.position).magnitude;

            if (distanceToWaypoint <= currentWaypoint.minDistanceToReachWaypoint)
            {
                currentWaypoint = currentWaypoint.nextWayPointNodes[Random.Range(0, currentWaypoint.nextWayPointNodes.Length)];
            }
        }
    }


    WaypointNodes FindClosestWayPoint()
    {
        return allWaypoints
        .OrderBy(t => Vector3.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    void FollowPlayer()
    {
        if (targetTransform == null)
            targetTransform = GameObject.FindGameObjectWithTag("Player").transform;

        else if (targetTransform != null)
            targetPostion = targetTransform.position;
    }

    float TurnTowardTarget()
    {
        vectorToTarget = targetPostion - transform.position;
        Vector3 localTarget = transform.InverseTransformPoint(targetPostion);
        float angleToTarget = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steerAmount = Mathf.Lerp(currentSteerAmount, angleToTarget / 45.0f, Time.deltaTime * 5f);
        steerAmount = Mathf.Clamp(steerAmount, -1.0f, 1.0f);

        currentSteerAmount = steerAmount;
        return steerAmount;
    }



    float ApplyThrottleOrBrake(float inputX)
    {
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > maxpeed)
            return 0.0f;

        return 1.05f - Mathf.Abs(inputX) / 1.0f;
    }


    // In FixedUpdate or AICar/AIBike method:
    void CheckIfStuck()
    {
        // Check if enough time has passed since the last reset to prevent rapid resetting
        if (Time.time - lastResetTime < resetCooldown)
        {
            return; // If still in cooldown, don't do anything
        }

        // Check if the vehicle has moved less than the threshold and is below velocity threshold
        if (Vector3.Distance(transform.position, lastPosition) < positionThreshold &&
            gameObject.GetComponent<Rigidbody>().velocity.magnitude < velocityThreshold)
        {
            stuckTime += Time.deltaTime; // Increment stuck time if the vehicle is stuck
        }
        else
        {
            stuckTime = 0f; // Reset stuck timer if the vehicle is moving
        }

        // If the vehicle has been stuck for longer than the threshold, unstick it
        if (stuckTime >= stuckThreshold)
        {
            UnstickVehicle();
        }

        // Update the last known position for the next check
        lastPosition = transform.position;
    }

    void UnstickVehicle()
    {
        Debug.Log("Vehicle is stuck. Applying corrective action.");

        // Reset vehicle's position to the last known waypoint or nudge forward
        if (currentWaypoint != null)
        {
            transform.position = currentWaypoint.transform.position + Vector3.up * 2; // Move up slightly to avoid ground collision
            transform.rotation = currentWaypoint.transform.rotation; // Align to the waypoint's rotation
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset velocity
        }

        // Record the time of this reset to prevent frequent resets
        lastResetTime = Time.time;
    }

}
