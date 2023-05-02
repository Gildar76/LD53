using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using Unity.VisualScripting;

namespace GildarGaming.LD53
{
    public class UIManager : MonoBehaviour
    {
        PlayerController player;
        public TMP_Text pendingDeliveryText;
        public TMP_Text deliveryText;
        public TMP_Text scoreText;
        public TMP_Text onGoingDeliveryText;
        public DeliveryManager deliveryManager;
        public TMP_Text timeRemaining;
        public TMP_Text distanceText;
        public TMP_Text deliveryFailText;
        public TMP_Text choiceText;
        public TMP_Text deliveryPickedUp;
        public TMP_Text deliveryCompleted;
        public TMP_Text hitpoints;
        public TMP_Text gameOver;
        public void Start()
        {
            player = FindObjectOfType<PlayerController>();
            player.onHitPointChange += OnHitPointChanged;
            player.deliveryAccepted += OnDeliveryAccepted;
            player.onTimeRemaining += OnTimeRemaining;
            deliveryManager.onNewPendingDelivery += OnNewDelivery;
            player.deliveryFailed += OnDeliveryFailed;
            player.onCompleteDelivery += OnDeliveryCOmpleted;
            player.onStartDelivery += OnStartDelivery;
            player.onScoreChange += OnScoreCHange;
            player.OnDeath += OnDeath;
            
        }

        private void OnDeath()
        {
            gameOver.gameObject.SetActive(true);
            pendingDeliveryText.gameObject.SetActive(false);
            choiceText.gameObject.SetActive(false);
            deliveryManager.enabled = false;


        }

        private void OnScoreCHange()
        {
            scoreText.text = player.score.ToString();
        }

        private void OnHitPointChanged(int hp)
        {
            hitpoints.text = hp.ToString();
            if (hp <= 0)
            {
                gameOver.gameObject.SetActive(true);
            }
        }

        private void OnStartDelivery(DeliveryStart obj)
        {
            Debug.Log("Delivery started in ui manager");
            StartCoroutine(ShowTextForSeconds(deliveryPickedUp, 10));
        }

        public IEnumerator ShowTextForSeconds(TMP_Text text, int seconds)
        {
            Debug.Log("Showing started text");
            text.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            text.gameObject.SetActive(false);
            yield return null;
        }

        private void OnDeliveryCOmpleted(DeliveryStart obj)
        {
            StartCoroutine(ShowTextForSeconds(deliveryCompleted, 10));
        }

        private void OnDeliveryFailed(DeliveryStart obj)
        {
            StartCoroutine(ShowDeliveryFailedText());
        }

        private IEnumerator ShowDeliveryFailedText()
        {
            deliveryFailText.text = "You have failed a delivery!!!";
            yield return new WaitForSeconds(10);
            deliveryFailText.text = String.Empty;
        }

        public void LateUpdate()
        {
            distanceText.text = player.distanceToTarget.ToString();    
        }

        private void OnTimeRemaining(int obj)
        {
            timeRemaining.text = obj / 60 + ":" + obj % 60; 

        }

        private void OnDeliveryAccepted(DeliveryStart deli)
        {
            onGoingDeliveryText.text = deli.deliverable + " (" + deli.disgination + ")";
            OnNewDelivery();
            
        }

        private void OnNewDelivery()
        {
            if (deliveryManager.pendingStarts.Count == 0)
            {
                pendingDeliveryText.text = String.Empty;
                choiceText.gameObject.SetActive(false);
                return;
            }
            DeliveryStart newStart = deliveryManager.pendingStarts.Peek();
            if (newStart.deliverableType == DeliverableType.HeavyStuff)
            {
                pendingDeliveryText.text = "Requested delivery: "  + " " + newStart.deliverable + " for a reward of " + newStart.finalScore;
            } else
            {
                pendingDeliveryText.text = "Requested delivery(" + newStart.disgination + "): " + newStart.quantity + " " + newStart.deliverable + " for a reward of " + newStart.finalScore + ".";
            }
            choiceText.gameObject.SetActive(true);

        }
    }
}
