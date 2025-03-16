using Shared;
using UnityEngine;
using UnityEngine.Audio;

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
            PlayerMovement
        }

        public static void CreateSfx(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend,
            float pitch = 1f, float rolloffDistanceMin = 1f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
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

            Debug.LogWarning("Didn't find audio group for " + group);
            return null;
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