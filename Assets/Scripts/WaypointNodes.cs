using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNodes : MonoBehaviour
{
    [Header("Speed Set once we reach the waypoint")]
    public float maxSpeed = 0;

    [Header("This is the waypoint we are going towards, not yet reached")]
    public float minDistanceToReachWaypoint = 5;

    public WaypointNodes[] nextWayPointNodes;
}
