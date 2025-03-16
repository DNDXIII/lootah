using System;
using Gameplay.Player;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ExperienceBarManager : MonoBehaviour
    {
        private Slider _experienceBar;
        private PlayerLeveling _playerLeveling;

        private void Awake()
        {
            _experienceBar = GetComponent<Slider>();
        }

        private void Start()
        {
            _playerLeveling = ActorManager.Instance.Player.gameObject.GetComponent<PlayerLeveling>();
            if (_playerLeveling == null)
                throw new NullReferenceException("PlayerLeveling component not found on Player GameObject");
        }

        private void Update()
        {
            _experienceBar.value = _playerLeveling.CurrentExperienceRatio;
        }
    }
}