﻿using System;
using Gameplay.Managers;
using Gameplay.Player;
using Managers;
using UnityEngine;

namespace Gameplay.Interactable
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private float interactionDistance = 3f;
        [SerializeField] private LayerMask interactableLayer;

        private Camera _camera;
        private PlayerInputHandler _playerInputHandler;

        private void Awake()
        {
            _playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void FixedUpdate()
        {
            // debug the ray 
            Debug.DrawRay(_camera.transform.position, _camera.transform.forward * interactionDistance, Color.red);

            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit,
                    interactionDistance,
                    interactableLayer))
            {
                if (!hit.collider.TryGetComponent(out IInteractable interactable)) return;
                
                InteractionTextManager.Instance.ShowInteractionText(interactable.GetInteractionText());

                if (_playerInputHandler.GetInteractInputDown())
                {
                    interactable.Interact(ActorManager.Instance.Player);
                }
            }
            else
            {
                InteractionTextManager.Instance.HideInteractionText();
            }
        }
    }
}