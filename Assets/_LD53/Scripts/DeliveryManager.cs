using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using System.Linq;
using System;
using TMPro;

namespace GildarGaming.LD53
{
    public enum DeliverableType
    {
        RestaurantFood,Food,HeavyStuff
    }
    public class DeliveryManager : MonoBehaviour
    {
        public GameObject bigMapCanvas;
        public GameObject smallMapCanvas;   
        public GameObject disignationPrefab;
        public event Action onNewPendingDelivery;
        public event Action<DeliveryStart> onPendingStartExpire;
        public Building[] buildings;
        Queue<DeliveryStart> deliveryStartPoints;
        Dictionary<string, int> deliverables;
        Dictionary<DeliverableType, Dictionary<string,int>> deliverableTypes;
        public Queue<DeliveryStart> pendingStarts = new Queue<DeliveryStart>();
        public List<DeliveryStart> allDeliveries = new List<DeliveryStart>();
        public List<DeliveryStart> onGoingDeliveries = new List<DeliveryStart>();
        public float timer = 0;
        public float deliveryDelay = 20f;
        public float pendingTimer = 0;
        float pendingDelay = 40f;
        public PlayerController player;
        Queue<string> disignations= new Queue<string>();
        private void Start()
        {
            player.onStartDelivery += OnStartDelivery;
            player.onCompleteDelivery += OnCOmpleteDelivery;
            player.deliveryAccepted += OnDeliveryAccepted;
            player.deliveryDeclined += OnDeliveryDeclined;
            player.deliveryFailed += OnDeliveryFailed;

            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int i = 0; i < alpha.Length; i++)
            {
                disignations.Enqueue(alpha[i].ToString());
            }
            deliverableTypes = new Dictionary<DeliverableType, Dictionary<string, int>>();
            buildings = UnityEngine.Object.FindObjectsOfType<Building>();
            deliverables = new Dictionary<string, int>();
            deliverables.Add("Pizzas", 5);
            deliverables.Add("pancakes", 5);
            deliverables.Add("plants", 25);
            deliverables.Add("grilled chickens", 5);
            deliverables.Add("loaves of bread", 25);
            deliverables.Add("bowls of garlic soap", 10);
            deliverables.Add("drinks", 25);
            deliverables.Add("flasks of milk", 25);
            deliverables.Add("sacks of pet food", 25);
            

            deliverables.Add("burgers", 5);
            deliverables.Add("Chinese meals", 5);
            deliverables.Add("Sushi", 5);
            deliverables.Add("Salads", 5);
            deliverables.Add("fruits", 5);

            Dictionary<string, int> deliverables2 = new Dictionary<string, int>();
            deliverables2.Add("a table", 200);
            deliverables2.Add("a bed", 500);
            deliverables2.Add("a pano", 10000);
            deliverables2.Add("a set of chairs", 200);
            deliverables2.Add("a computer", 100);
            deliverables2.Add("a live pig...", 200);
            List<string> deliveries1List = Enumerable.ToList(deliverables.Keys);
            List<string> deliveries2List = Enumerable.ToList(deliverables2.Keys);
            deliverableTypes.Add(DeliverableType.HeavyStuff, deliverables2);
            deliverableTypes.Add(DeliverableType.Food, deliverables);
            deliveryStartPoints = new Queue<DeliveryStart>();
            string item = string.Empty;
            for (int i = 0; i < 500; i++)
            {
                DeliveryStart newStart = new DeliveryStart();
                
                if (i > 0 && i % UnityEngine.Random.Range(15,30) == 0)
                {
                    newStart.deliverableType = DeliverableType.HeavyStuff;
                    newStart.quantity= i;
                    newStart.pickupDropOffDelay = 20;
                    item = deliveries2List[UnityEngine.Random.Range(0, deliveries2List.Count)];


                    deliverableTypes[DeliverableType.HeavyStuff].TryGetValue(item, out newStart.score);
                    newStart.deliverable = item;


                } else
                {
                    newStart.deliverableType = DeliverableType.Food;
                    newStart.quantity= UnityEngine.Random.Range(1,20);
                    newStart.pickupDropOffDelay = newStart.quantity * 0.5f;
                    item = deliveries1List[UnityEngine.Random.Range(0, deliveries1List.Count)];

                    deliverableTypes[DeliverableType.Food].TryGetValue(item, out newStart.score);
                    newStart.deliverable = item;
                }
                newStart.disignatedObjectEnd = GameObject.Instantiate(disignationPrefab, newStart.deliveryPosition, Quaternion.identity);
                newStart.disignatedObjectStart = GameObject.Instantiate(disignationPrefab, newStart.startPosition, Quaternion.identity);
                newStart.disignatedObjectStart.SetActive(false);
                newStart.disignatedObjectEnd.SetActive(false);
                deliveryStartPoints.Enqueue(newStart);
                allDeliveries.Add(newStart);
                



            }


        }

        private void OnDeliveryFailed(DeliveryStart obj)
        {
            
        }

        private void OnDeliveryAccepted(DeliveryStart obj)
        {
            onNewPendingDelivery?.Invoke();
        }

        private void OnDeliveryDeclined(DeliveryStart obj)
        {
            if (pendingStarts.Count == 0) return;
            pendingStarts.Dequeue();
            onNewPendingDelivery?.Invoke();
            obj.disignatedObjectEnd.SetActive(false);
            obj.disignatedObjectStart.SetActive(false);
        }

        private void OnCOmpleteDelivery(DeliveryStart obj)
        {
            deliveryStartPoints.Enqueue(obj);
            Debug.Log("Delivery completed");
            obj.started = false;
            obj.disignatedObjectEnd.SetActive(false);
        }

        private void OnStartDelivery(DeliveryStart delivery)
        {
            delivery.started= true;
            delivery.disignatedObjectStart.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (bigMapCanvas.activeInHierarchy)
                {
                    foreach (var item in allDeliveries)
                    {
                        item.disignatedObjectStart.GetComponentInChildren<TMP_Text>().fontSize = 12;
                        item.disignatedObjectEnd.GetComponentInChildren<TMP_Text>().fontSize = 12;
                    }
                    bigMapCanvas.SetActive(false);
                    smallMapCanvas.SetActive(true);

                }
                else
                {

                    foreach (var item in allDeliveries)
                    {
                        item.disignatedObjectStart.GetComponentInChildren<TMP_Text>().fontSize = 36;
                        item.disignatedObjectEnd.GetComponentInChildren<TMP_Text>().fontSize = 36;
                    }
                    bigMapCanvas.SetActive(true);
                    smallMapCanvas.SetActive(false);


                }
            }
            if (pendingStarts.Count > 0)
            {
                pendingTimer += Time.deltaTime;
                if (pendingTimer > pendingDelay)
                {
                    pendingTimer = 0;
                    DeliveryStart oldStart =  pendingStarts.Dequeue();
                    if (oldStart != null)
                    {
                        disignations.Enqueue(oldStart.disgination);
                        oldStart.disignatedObjectEnd.SetActive(false);
                        oldStart.disignatedObjectStart.SetActive(false);
                        deliveryStartPoints.Enqueue(oldStart);

                    }

                }
            } else { pendingTimer = 0; }
            
            timer += Time.deltaTime;
            if (timer >= deliveryDelay)
            {
                timer = 0;
                DeliveryStart newStart = deliveryStartPoints.Dequeue();
                Building a = buildings[UnityEngine.Random.Range(0, buildings.Length - 1)];
                Building b = buildings[UnityEngine.Random.Range(0, buildings.Length - 1)];
                newStart.startPosition = a.transform.position;
                newStart.deliveryPosition = b.transform.position;
                newStart.finalScore = newStart.score * (int)Vector3.Distance(newStart.startPosition, b.transform.position) / 50;
                if (newStart.finalScore < newStart.score) newStart.finalScore = newStart.score;

                pendingStarts.Enqueue(newStart);
                newStart.disgination = disignations.Dequeue();
                newStart.disignatedObjectEnd.transform.position = new Vector3(newStart.deliveryPosition.x, 100, newStart.deliveryPosition.z);
                newStart.disignatedObjectEnd.SetActive(true);
                newStart.disignatedObjectEnd.GetComponentInChildren<TMP_Text>().text = newStart.disgination.ToString() + "d";
                newStart.disignatedObjectStart.transform.position = new Vector3(newStart.startPosition.x, 100, newStart.startPosition.z);
                newStart.disignatedObjectStart.SetActive(true);
                newStart.disignatedObjectStart.GetComponentInChildren<TMP_Text>().text = newStart.disgination.ToString();

                onNewPendingDelivery?.Invoke();


            }

        }
    }
}
