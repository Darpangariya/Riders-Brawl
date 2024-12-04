using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UTC_Easy
using EasyRoads3Dv3;
#endif
#if UNITY_EDITOR
using UnityEditor;

public class PathMenuItem : MonoBehaviour
{
#if UTC_Easy
    [MenuItem("GameObject/Easy Traffic System/Vol2/Intergrations/EasyRoadv3/PathGenerator", false, 10)]
    static void CreatePathGeneratorGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("PathGenerator");
        go.AddComponent<ESPathGenerator>();


        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
#endif
    //
    [MenuItem("GameObject/Easy Traffic System/Vol2/Intergrations/EasyRoadv3/SetEasyIntegration", false, 10)]
    static void EnableIntegration(MenuCommand menuCommand)
    {
        // Create a custom game object
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (!symbols.Contains("UTC_Easy"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols + "UTC_Easy");
        }
        GUI.changed = true;
    }
    //
    [MenuItem("GameObject/Easy Traffic System/Vol2/Intergrations/EasyRoadv3/UnSetEasyIntegration", false, 10)]
    static void DisableIntegration(MenuCommand menuCommand)
    {
        // Create a custom game object
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (symbols.Contains("UTC_Easy"))
        {
            symbols = symbols.Replace("UTC_Easy", "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbols + symbols);
        }
        GUI.changed = true;
    }
}
#endif
[ExecuteInEditMode]
public class ESPathGenerator : MonoBehaviour
{
#if UTC_Easy
    public bool BakeOnStart = true;
    public int LineIteration = 3;
    [HideInInspector] public Vector3[] points;
    [Tooltip("Compulsory")]
    public GameObject vehicle;
    static public ERTrafficDirection trafficDirection;
    [HideInInspector]
    public GameObject PathParent;
    public ESSpawnManager spawnManager;
    // Use this for initialization
    void Start()
    {
        if (Application.isPlaying == false) return;
        if (!BakeOnStart) return;
        if (PathParent == null)
        {
            GameObject path = new GameObject("PathParent");
            PathParent = path;
        }
        if (vehicle == null)
        {
            Debug.LogWarning("EasyRoads3D Warning: A vehicle object needs to be assigned to the Traffic Manager component");
        }
        // Make sure the ERVehicle component is attached to the vehicle object
        if (vehicle.GetComponent<ESLaneAlignment>() == null) vehicle.AddComponent<ESLaneAlignment>();
        // get a reference to the road network
        ERRoadNetwork roadNetwork = new ERRoadNetwork();
        // get all roads in the road network
        // this will be used to instantiate ERVehicle objects on each road
        ERRoad[] roads = roadNetwork.GetRoads();
        int i = 0;
        foreach (ERRoad road in roads)
        {
            i++;
            GameObject go = Instantiate(vehicle);
            go.name = "Vehicle " + i;
            ESLaneAlignment scr = go.GetComponent<ESLaneAlignment>();
            scr.path = PathParent;
            // set the location for this object
            scr.road = road;
            scr.iteration = LineIteration;
            scr.vehicleLocation = VehicleLocation.road;
            scr.laneDirection = ERLaneDirection.Left;
            // create a profile for this object  
            scr.profile = Random.Range(1, 10);
            scr.accelerator = Mathf.Lerp(0.5f, 5f, scr.profile / 10);
            scr.decelerator = Mathf.Lerp(1f, 30f, scr.profile / 10);
            // get lane count
            // set the current lane index and lane data for this object
            int lanes = scr.road.GetLeftLaneCount();
            scr.lane = Random.Range(0, lanes);
            scr.points = road.GetLanePoints(scr.lane, ERLaneDirection.Left);
            points = road.GetLanePoints(scr.lane, ERLaneDirection.Left);
            scr.currentElement = Random.Range(3, scr.points.Length - 3);
            // Set the target speed based on the road type and vehicle profile
            scr.targetSpeed = road.GetSpeedLimit();
            // Set the Minimum speed to Speed Limit - 10% for slowest vehicles to Speed Limit + max 10% for fast vehicles
            // divide by 3.6 for for km/hour
            scr.targetSpeed += Mathf.Lerp(-scr.targetSpeed * 0.1f, scr.targetSpeed * 0.1f, scr.profile / 10) / 3.6f;
            //Debug.Log("Road Speed Limit: " + road.GetSpeedLimit() +" target speed" + scr.targetSpeed);
        }
        /*
        foreach (ERRoad road in roads)
        {
            i++;
            GameObject go = Instantiate(vehicle);
            go.name = "Vehicle " + i + "_2";
            ESLaneAlignment scr = go.GetComponent<ESLaneAlignment>();

            scr.path = PathParent;
            // set the location for this object
            scr.iteration = LineIteration;
            scr.road = road;
            scr.vehicleLocation = VehicleLocation.road;
            scr.laneDirection = ERLaneDirection.Right;

            // create a profile for this object  
            scr.profile = Random.Range(1, 10);
            scr.accelerator = Mathf.Lerp(0.5f, 5f, scr.profile / 10);
            scr.decelerator = Mathf.Lerp(1f, 30f, scr.profile / 10);

            // get lane count


            // set the current lane index and lane data for this object
            int lanes = scr.road.GetLeftLaneCount();
            scr.lane = Random.Range(0, lanes);
            scr.points = road.GetLanePoints(scr.lane, ERLaneDirection.Right);
            scr.currentElement = Random.Range(3, scr.points.Length - 3);
            points = road.GetLanePoints(scr.lane, ERLaneDirection.Right);

            // Set the target speed based on the road type and vehicle profile
            scr.targetSpeed = road.GetSpeedLimit();
            // Set the Minimum speed to Speed Limit - 10% for slowest vehicles to Speed Limit + max 10% for fast vehicles
            // divide by 3.6 for for km/hour
            scr.targetSpeed += Mathf.Lerp(-scr.targetSpeed * 0.1f, scr.targetSpeed * 0.1f, scr.profile / 10) / 3.6f;


            //Debug.Log("Road Speed Limit: " + road.GetSpeedLimit() +" target speed" + scr.targetSpeed);
        }
        */
        // get the traffic direction, Left or Right hand Traffic
        trafficDirection = roadNetwork.GetTrafficDirection();
    }
    void Update()
    {
        //disable noodes here for optize behavooiur
        if (spawnManager == null)
        {
            spawnManager = GameObject.FindObjectOfType<ESSpawnManager>();
        }
        else
        {
            spawnManager.IntegratedEasyRoad3 = true;
        }
    }
#endif
}