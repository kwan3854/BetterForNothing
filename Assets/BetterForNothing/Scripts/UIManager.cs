using System;
using System.Collections.Generic;
using BetterForNothing.Scripts.Popup;
using BetterForNothing.Scripts.Popup.ScriptableObjects;
using BetterForNothing.Scripts.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace BetterForNothing.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UIManager : IInitializable
    {
        public AudioSource GlobalAudioSource => _globalAudioSource;
        
        private readonly IObjectResolver _container;
        private AudioSource _globalAudioSource;
        
        private readonly Dictionary<int, AudioClip> _globalUISounds = new();

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
            
            PreloadDDOLAccessor();
            PreloadGlobalAudioSource();
            PreloadGlobalAudioClips();
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
        
        public void PlaySoundOnGlobal(int index)
        {
            Debug.Assert(_globalAudioSource != null, "Global audio source not found.");
            Debug.Assert(_globalUISounds.ContainsKey(index), $"Audio clip with index {index} not found.");
            
            _globalAudioSource.PlayOneShot(_globalUISounds[index]);
        }
        
        public AudioClip GetUISoundByIndex(int index)
        {
            Debug.Assert(_globalUISounds.ContainsKey(index), $"Audio clip with index {index} not found.");
            
            return _globalUISounds[index];
        }

        public void ClearAllLoadingPopups()
        {
            var ddolObjects = DontDestroyOnLoadAccessor.Instance.GetAllRootsOfDontDestroyOnLoad();
            
            // Find all loading popups even in children
            foreach (var ddolObject in ddolObjects)
            {
                var loadingPopups = ddolObject.GetComponentsInChildren<LoadingPopup>();
                foreach (var loadingPopup in loadingPopups)
                {
                    loadingPopup.Hide().Forget();
                }
            }
        }
        
        public void ClearAllModalPopups()
        {
            var ddolObjects = DontDestroyOnLoadAccessor.Instance.GetAllRootsOfDontDestroyOnLoad();
            
            // Find all modal popups even in children
            foreach (var ddolObject in ddolObjects)
            {
                var modalPopups = ddolObject.GetComponentsInChildren<ModalPopup>();
                foreach (var modalPopup in modalPopups)
                {
                    modalPopup.Hide().Forget();
                }
            }
        }
        
        private void PreloadGlobalAudioSource()
        {
            // Preload global UI AudioSource
            var audioSource = _container.Instantiate(UISettings.audioSourcePrefab);
            // Set global audio source to not destroy on load
            Object.DontDestroyOnLoad(audioSource);
            
            _globalAudioSource = audioSource.GetComponent<AudioSource>();
        }
        
        private void PreloadDDOLAccessor()
        {
            // Preload DontDestroyOnLoadAccessor
            var ddolAccessor = _container.Instantiate(UISettings.ddolAccessorPrefab);
            
            // Set DontDestroyOnLoadAccessor to not destroy on load
            Object.DontDestroyOnLoad(ddolAccessor);
        }

        private void PreloadGlobalAudioClips()
        {
            foreach (var audioClip in UISettings.globalUISounds)
            {
                _globalUISounds.Add(audioClip.Key, audioClip.Value);
                Debug.Log($"Audio clip with index {audioClip.Key} loaded.");
            }
        }
    }
}