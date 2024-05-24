using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardMatchGame
{
    public class AudioPlayer : MonoBehaviour
    {
        private static AudioPlayer instance;
        [SerializeField]
        private AudioSource audioSourceComponent;
        [SerializeField]
        private AudioClip[] audioClips;
        private float volume = 1f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                audioSourceComponent = GetComponent<AudioSource>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static AudioPlayer Instance
        {
            get
            {
                return instance;
            }
        }

        public void PlayAudioClip(int clipIndex)
        {
            if (audioClips != null && clipIndex >= 0 && clipIndex < audioClips.Length)
            {
                audioSourceComponent.PlayOneShot(audioClips[clipIndex], volume);
            }
            else
            {
                Debug.LogWarning($"AudioPlayer: Invalid audio clip index {clipIndex}");
            }
        }

        public void PlayAudioClip(int clipIndex, float volumeLevel)
        {
            if (audioClips != null && clipIndex >= 0 && clipIndex < audioClips.Length)
            {
                audioSourceComponent.PlayOneShot(audioClips[clipIndex], volumeLevel);
            }
            else
            {
                Debug.LogWarning($"AudioPlayer: Invalid audio clip index {clipIndex}");
            }
        }

        public void SetAudioClips(AudioClip[] newAudioClips)
        {
            audioClips = newAudioClips;
        }

        public void SetVolume(float newVolume)
        {
            volume = newVolume;
        }
    }

}

   
   
