using System.Linq;
using Shared;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer[] audioMixers;

        public AudioMixerGroup[] FindMatchingGroups(string subPath)
        {
            return audioMixers.Select(t => t.FindMatchingGroups(subPath))
                .FirstOrDefault(results => results != null && results.Length != 0);
        }

        public void SetFloat(string floatName, float value)
        {
            foreach (var t in audioMixers)
            {
                if (t != null)
                {
                    t.SetFloat(floatName, value);
                }
            }
        }

        public void GetFloat(string floatName, out float value)
        {
            value = 0f;
            foreach (var t in audioMixers)
            {
                if (t != null)
                {
                    t.GetFloat(floatName, out value);
                    break;
                }
            }
        }
    }
}