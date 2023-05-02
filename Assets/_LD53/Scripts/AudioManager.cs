using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GildarGaming.LD53
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip engineSound;
        public AudioClip deliveryPickup;
        public AudioClip deliveryComplete;
        public AudioClip deliveryStart;
        public AudioClip collission;
        public AudioClip deliveryFail;
        public AudioClip death;
        public AudioClip deliveryDecline;
        public AudioSource[] audioSources;

        public void Start()
        {
            audioSources= GetComponentsInChildren<AudioSource>();
        }

        public void PlayAudio(AudioClip clip)
        {
            foreach (AudioSource source in audioSources)
            {
                if (!source.isPlaying)
                {
                    
                    source.PlayOneShot(clip);
                    break;
                }
            }
        }

    }
}
