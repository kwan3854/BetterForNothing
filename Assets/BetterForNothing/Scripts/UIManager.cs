using System;
using System.Collections.Generic;
using BetterForNothing.Scripts.Popup;
using BetterForNothing.Scripts.Popup.ScriptableObjects;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BetterForNothing.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIManager : IInitializable
    {
        private readonly IObjectResolver _container;


        public UIManager(IObjectResolver container)
        {
            _container = container;
        }

        private UISettings UISettings { get; set; }

        public void Initialize()
        {
            // Load UI Settings ScriptableObject
            UISettings = Resources.Load<UISettings>("UISettings");

            if (UISettings == null) Debug.LogError("UISettings not found in Resources folder.");
        }

        public LoadingPopup BuildLoadingPopup()
        {
            var loadingPopup = _container.Instantiate(UISettings.loadingPopupPrefab).GetComponent<LoadingPopup>();
            return loadingPopup;
        }

        public ModalPopup BuildModalPopup(string message, IEnumerable<ModalButton> buttons)
        {
            var modalPopup = _container.Instantiate(UISettings.modalPopupPrefab).GetComponent<ModalPopup>();
            modalPopup.SetMessage(message);
            modalPopup.SetButtons(buttons);

            // Add close event to every button
            foreach (var button in modalPopup.GetButtons())
                button.onClick.AddListener(() => modalPopup.Hide().Forget());

            return modalPopup;
        }

        public ModalButtonComponent BuildModalButton(ModalButtonType buttonType, string text, Action onClick)
        {
            var prefab = UISettings.GetModalButtonPrefab(buttonType);
            var instance = _container.Instantiate(prefab);
            instance.SetText(text);
            instance.SetOnClick(onClick);
            return instance;
        }
    }
}