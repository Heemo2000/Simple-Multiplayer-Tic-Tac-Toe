using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SoundManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        private AudioSource audioSource;
        private Coroutine playingCoroutine;
        private bool isMute = false;
        private bool isPaused = false;
        public SoundData Data{get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }
        public bool IsMute { get => isMute; }
        public bool IsPaused { get => isPaused; }

        public void Initialize(SoundData data)
        {
            Data = data;
            audioSource.clip = data.clip;
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;

            audioSource.mute = data.mute;
            audioSource.bypassEffects = data.bypassEffects;
            audioSource.bypassListenerEffects = data.bypassListenerEffects;
            audioSource.bypassReverbZones = data.bypassReverbZones;

            audioSource.priority = data.priority;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.panStereo = data.panStereo;
            audioSource.spatialBlend = data.spatialBlend;
            audioSource.reverbZoneMix = data.reverbZoneMix;
            audioSource.dopplerLevel = data.dopplerLevel;
            audioSource.spread = data.spread;

            audioSource.minDistance = data.minDistance;
            audioSource.maxDistance = data.maxDistance;

            audioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            audioSource.ignoreListenerPause = data.ignoreListenerPause;

            audioSource.rolloffMode = data.rolloffMode;

            isPaused = false;
            isMute = false;
        }

        public void setMuteStatus(bool value)
        {
            isMute = value;
            if(value)
            {
                audioSource.mute = true;
            }
            else
            {
                audioSource.mute = false;
            }
        }

        public void SetPauseStatus(bool value)
        {
            isPaused = value;
            if(value)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
            }
        }

        public void Play()
        {
            if(playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            audioSource.Play();
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(()=> isPaused || audioSource.isPlaying);
            Stop();
        }

        public void Stop()
        {
            if(playingCoroutine != null)
            {
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            isPaused = false;
            isMute = false;
            audioSource.Stop();
            SoundManager.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
        private void Awake() 
        {
            audioSource = GetComponent<AudioSource>();    
        }
    }
}
