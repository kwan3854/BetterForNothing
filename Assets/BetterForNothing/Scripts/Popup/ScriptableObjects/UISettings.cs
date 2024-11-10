using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BetterForNothing.Scripts.Popup.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UISettings", menuName = "ScriptableObjects/UISettings", order = 1)]
    public class UISettings : SerializedScriptableObject
    {
        [Title("Loading Popup Prefab")] public GameObject loadingPopupPrefab;

        [Title("Modal Popup Prefab")] public GameObject modalPopupPrefab;

        [Title("Modal Button Prefabs")] [SerializeField]
        private Dictionary<ModalButtonType, ModalButtonComponent> modalButtonPrefabs;


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