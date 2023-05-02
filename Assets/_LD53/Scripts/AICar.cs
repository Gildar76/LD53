using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace GildarGaming.LD53
{
    public class AICar : MonoBehaviour
    {
        public NavMeshAgent agent;
        public DeliveryManager deliveryManager;
        public Vector3 waypoint;
        public float timer = 0;
        public float delay = 120;
        public void Start()
        {
            deliveryManager = FindObjectOfType<DeliveryManager>();
            agent = GetComponent<NavMeshAgent>();
            agent.isStopped = false;
            
        }
        
        
        public void Update()
        {
            if (waypoint == Vector3.zero)
            {
                SetWayPoint();
                agent.SetDestination(waypoint);
            }
            if ((transform.position - waypoint).magnitude < 100)
            {
                SetWayPoint();
                agent.SetDestination(waypoint);

            }
        }

        public void SetWayPoint()
        {
            waypoint = deliveryManager.buildings[Random.Range(0, deliveryManager.buildings.Length)].transform.position;
        }
    }
}
