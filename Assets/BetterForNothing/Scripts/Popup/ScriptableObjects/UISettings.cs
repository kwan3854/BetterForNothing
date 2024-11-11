using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BetterForNothing.Scripts.Popup.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "ScriptableObjects/UISettings", order = 1)]
    public class UISettings : SerializedScriptableObject
    {
        [Title("Loading Popup Prefab")] public LoadingPopup loadingPopupPrefab;

        [Title("Modal Popup Prefab")] public ModalPopup modalPopupPrefab;

        [Title("Modal Button Prefabs")] [SerializeField]
        private Dictionary<ModalButtonType, ModalButtonComponent> modalButtonPrefabs;
        
        [Title("Audio Source Prefab")] public GameObject audioSourcePrefab;
        
        [Title("Audio Clips")] public Dictionary<int, AudioClip> globalUISounds;
        
        [Title("DDOL Accessor Prefab")] public GameObject ddolAccessorPrefab;


        public ModalButtonComponent GetModalButtonPrefab(ModalButtonType buttonType)
        {
            if (!modalButtonPrefabs.TryGetValue(buttonType, out var prefab))
            {
                throw new Exception($"Modal button prefab with type {buttonType} not found.");
            }

            return prefab;
        }
    }
}