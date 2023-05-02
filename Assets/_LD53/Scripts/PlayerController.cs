using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace GildarGaming.LD53
{
    public class PlayerController : MonoBehaviour
    {
        public LayerMask aiLayerMask;
        public LayerMask roadLayer;
        bool grounded;
        public int hitPoints = 100;
        public AudioManager audioManager;
        public int score;
        public event Action OnDeath;
        public event Action onScoreChange;
       public Queue<DeliveryStart> queuedUpDeliveries = new Queue<DeliveryStart>();     
        public event Action<DeliveryStart> onStartDelivery;
        public event Action<DeliveryStart> onCompleteDelivery;
        public event Action<DeliveryStart> deliveryAccepted;
        public event Action<DeliveryStart> deliveryFailed;
        public event Action<DeliveryStart> deliveryDeclined;
        public event Action<int> onTimeRemaining;
        int failCount = 0;
        public Camera minimapCam;
        public float distanceToTarget = 0;
        DeliveryStart currentDelivery;
        DeliveryManager deliveryManager;
        public AudioSource engineSound;
        public AudioSource collissionSound;
        Rigidbody rb;
        public event Action<int> onHitPointChange;
        private float forceMultiplyer = 25f;
        Vector2 inputAxis;
        public void Start()
        {
            deliveryManager = FindObjectOfType<DeliveryManager>();
            rb = GetComponent<Rigidbody>();
        }

        private void LateUpdate()
        {
            minimapCam.transform.position = new Vector3(transform.position.x, 300f,transform.position.z);
        }




        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
            if (hitPoints < 0)
            {
                audioManager.PlayAudio(audioManager.death);
                this.enabled = false;
                OnDeath?.Invoke();
                

            }
            inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            engineSound.pitch = 1 + inputAxis.y * 2;
            if (Input.GetKeyDown(KeyCode.Space) && deliveryManager.pendingStarts.Count > 0)  
            { 
                if (currentDelivery== null)
                {
                    currentDelivery = deliveryManager.pendingStarts.Dequeue();
                    deliveryAccepted?.Invoke(currentDelivery);
                    currentDelivery.deliveryTimerCOunter = 180f;
                } else
                {
                    queuedUpDeliveries.Enqueue(deliveryManager.pendingStarts.Dequeue());
                    deliveryAccepted?.Invoke(currentDelivery);

                }
                audioManager.PlayAudio(audioManager.deliveryPickup);

            } else if (currentDelivery != null)
            {

                int oldCOunter = (int)currentDelivery.deliveryTimerCOunter;
                currentDelivery.deliveryTimerCOunter -= Time.deltaTime;
                int newCOunter = (int)currentDelivery.deliveryTimerCOunter;
                if (newCOunter == 0)
                {
                    failCount++;
                    if (failCount >= 3)
                    {
                        hitPoints = 0;
                    }
                    audioManager.PlayAudio(audioManager.deliveryFail);
                    deliveryFailed?.Invoke(currentDelivery);
                    currentDelivery = null;
                    if (queuedUpDeliveries.Count > 0)
                    {
                        currentDelivery = queuedUpDeliveries.Dequeue();
                        deliveryAccepted?.Invoke(currentDelivery);
                        currentDelivery.deliveryTimerCOunter = 180f;
                    }

                }
                if (oldCOunter != newCOunter)
                {
                    onTimeRemaining?.Invoke(newCOunter);
                }

                if (currentDelivery.started == false)
                {
                    distanceToTarget = Vector3.Distance(transform.position, currentDelivery.startPosition);
                } else if (currentDelivery.started == true)
                {
                    distanceToTarget = Vector3.Distance(transform.position, currentDelivery.deliveryPosition);
                    Debug.Log(currentDelivery.startPosition);
                    Debug.Log(currentDelivery.deliveryPosition);
                }
                
                if (distanceToTarget < 20 && currentDelivery.started == false)
                {
                    Debug.Log("Delivery started in player");
                    onStartDelivery?.Invoke(currentDelivery);
                    //deliveryManager.pendingStarts.Dequeue();
                    currentDelivery.started = true;
                    audioManager.PlayAudio(audioManager.deliveryStart);
                    distanceToTarget = Vector3.Distance(transform.position, currentDelivery.deliveryPosition);
                }
                else if (distanceToTarget < 20 && currentDelivery.started == true)
                {
                    score += currentDelivery.finalScore;
                    onScoreChange?.Invoke();
                    onCompleteDelivery?.Invoke(currentDelivery);
                    audioManager.PlayAudio(audioManager.deliveryComplete);
                    currentDelivery.started = false;
                    currentDelivery = null;
                    if (queuedUpDeliveries.Count > 0)
                    {
                        currentDelivery = queuedUpDeliveries.Dequeue();
                        deliveryAccepted?.Invoke(currentDelivery);
                        currentDelivery.deliveryTimerCOunter = 180f;
                    }
                        
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (deliveryManager.pendingStarts.Count > 0) 
                {
                    deliveryDeclined?.Invoke(deliveryManager.pendingStarts.Peek());
                    audioManager.PlayAudio(audioManager.deliveryFail);
                }
                
            }
            
        }
        public void FixedUpdate()
        {
            rb.angularVelocity = new Vector3(0, inputAxis.x * 3, 0);
            //Debug.Log("torque" + rb.angularVelocity);
            if (inputAxis.magnitude > 0 && rb.velocity.magnitude < 10)
            {
                if (inputAxis.y< 0)
                {
                    rb.AddRelativeForce(new Vector3(0, 0, inputAxis.y * forceMultiplyer / 10f));
                } else
                {
                    rb.AddRelativeForce(new Vector3(0, 0, inputAxis.y * forceMultiplyer));
                }
                grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 50, transform.position.z), Vector3.down, 100f, roadLayer);
                if (grounded)
                {
                    rb.drag = 0.1f;
                } else
                {
                    Debug.Log("Not grounded");
                    rb.drag = 8;
                    
                }
                //if (rb.GetAccumulatedTorque().magnitude < 20f)
                //{
                        

                //}
                
                
            }
            
        }
        public void OnCollisionEnter(Collision collision)
        {
            if (!collissionSound.isPlaying && collision.gameObject.GetComponent<Building>() != null)
            {
                hitPoints -= 1;

                onHitPointChange(hitPoints);
                collissionSound.Play();
            }
            if (collision.gameObject.layer == aiLayerMask)
            {
                rb.velocity = Vector3.zero; ;
                hitPoints -= 1;

                onHitPointChange(hitPoints);
                collissionSound.Play();

            }
        }

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entered trigger" + other.gameObject.name);
            Debug.Log(other.gameObject.layer);
            Debug.Log(aiLayerMask);
            if (other.gameObject.layer == aiLayerMask)
            {
                Debug.Log("Entered trigger on AI " + other.gameObject.name);
                rb.velocity= Vector3.zero; 
                hitPoints -= 1;

                onHitPointChange(hitPoints);
                collissionSound.Play();

            }
        }
    }
}
