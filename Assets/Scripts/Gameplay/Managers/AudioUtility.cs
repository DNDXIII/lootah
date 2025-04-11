using System;
using Shared;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Managers
{
    public static class AudioUtility
    {
        private static AudioManager _audioManager;

        public enum AudioGroups
        {
            EnemyDetection,
            EnemyDeath,
            EnemyAttack,
            EnemySpawn,
            Pickup,
            DamageTick,
            PlayerMovement,
            WeaponShoot
        }

        public static void CreateSfx(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend = 0f,
            float pitch = 1f, float rolloffDistanceMin = 1f, bool randomizePitch = false)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;

            if (randomizePitch)
            {
                pitch = Random.Range(.8f, 1.2f);
            }

            source.pitch = pitch;


            source.Play();

            source.outputAudioMixerGroup = GetAudioGroup(audioGroup);

            TimedSelfDestruct timedSelfDestruct = impactSfxInstance.AddComponent<TimedSelfDestruct>();
            timedSelfDestruct.lifeTime = clip.length;
        }

        private static AudioMixerGroup GetAudioGroup(AudioGroups group)
        {
            if (!_audioManager)
                _audioManager = Object.FindFirstObjectByType<AudioManager>();

            var groups = _audioManager.FindMatchingGroups(group.ToString());

            if (groups.Length > 0)
                return groups[0];

            throw new Exception("Didn't find audio group for " + group);
        }

        public static void SetMasterVolume(float value)
        {
            if (_audioManager == null)
                _audioManager = Object.FindFirstObjectByType<AudioManager>();

            if (value <= 0)
                value = 0.001f;
            float valueInDb = Mathf.Log10(value) * 20;

            _audioManager.SetFloat("MasterVolume", valueInDb);
        }

        public static float GetMasterVolume()
        {
            if (_audioManager == null)
                _audioManager = Object.FindFirstObjectByType<AudioManager>();

            _audioManager.GetFloat("MasterVolume", out var valueInDb);
            return Mathf.Pow(10f, valueInDb / 20.0f);
        }
    }
}