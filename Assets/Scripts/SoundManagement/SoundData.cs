using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.SoundManagement
{
    [CreateAssetMenu(fileName = "Sound Data", menuName = "Sound/Sound Data")]
    [System.Serializable]
    public class SoundData : ScriptableObject
    {
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;

        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;

        public int priority = 128;

        [Range(0.0f, 1.0f)]
        public float volume = 1.0f;

        [Range(-3.0f, 3.0f)]
        public float pitch = 1.0f;
        public float panStereo = 0.0f;
        public float spatialBlend = 0.0f;
        public float reverbZoneMix = 1.0f;
        public float dopplerLevel = 1.0f;
        public float spread = 0.0f;

        public float minDistance = 1.0f;
        public float maxDistance = 500.0f;

        public bool ignoreListenerVolume = false;
        public bool ignoreListenerPause = false;

        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
    }
}
