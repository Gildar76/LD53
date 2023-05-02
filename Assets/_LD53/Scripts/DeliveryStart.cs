using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Threading;

namespace GildarGaming.LD53
{
    public class DeliveryStart
    {
        public GameObject disignatedObjectEnd;
        public GameObject disignatedObjectStart;
        public DeliverableType deliverableType;
        public Building start;
        public Building end;
        public int quantity;
        public float pickupDropOffDelay;
        public float timer;
        public bool started;
        public bool ended;
        public string deliverable;
        public int score;
        public int finalScore = 0;
        public float deliveryTime = 300f;
        public Vector3 startPosition;
        public Vector3 deliveryPosition;
        internal string disgination;
        public float deliveryTimerCOunter = 0;

        public void ResetAll()
        {
            timer = 0;
            started = false;
            ended = false;
            quantity= 0;
            pickupDropOffDelay = 0;
            deliverable = string.Empty;
            score = 0;
            finalScore = 0;
            
            
            
        }

        
    }
}
