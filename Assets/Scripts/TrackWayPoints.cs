using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackWayPoints : MonoBehaviour
{

    public Color lineColor;
    [Range(0f, 1)] public float sphereRadius;
    public List<Transform> nodes = new List<Transform>();


    public void Update()
    {
        Transform[] path = GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();
        for (int i = 1; i < path.Length; i++)
        {
            nodes.Add(path[i]);
            //Debug.LogError("Hello");
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentWayPoint = nodes[i].position;
            Vector3 previousWayPoint = Vector3.zero;

            if (i != 0) previousWayPoint = nodes[i - 1].position;
            else if (i == 0) previousWayPoint = nodes[nodes.Count - 1].position;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = lineColor;

        //path = GetComponentsInChildren<Transform>();

        //nodes = new List<Transform>();
        //for (int i = 1; i < path.Length; i++)
        //{
        //    nodes.Add(path[i]);
        //    Debug.LogError("Hello");
        //}

        //for (int i = 0; i < nodes.Count; i++)
        //{
        //    Vector3 currentWayPoint = nodes[i].position;
        //    Vector3 previousWayPoint = Vector3.zero;

        //    if( i  != 0 ) previousWayPoint = nodes[i - 1].position;
        //    else if( i == 0 ) previousWayPoint = nodes[nodes.Count - 1].position;

            //Gizmos.DrawLine( previousWayPoint, currentWayPoint );
            //Gizmos.DrawSphere(currentWayPoint, sphereRadius);
        }
    } 

