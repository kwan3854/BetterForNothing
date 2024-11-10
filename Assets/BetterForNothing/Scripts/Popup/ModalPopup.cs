using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace BetterForNothing.Scripts.Popup
{
    public class ModalPopup : MonoBehaviour, IPopup
    {
        private const float AnimationDuration = 0.3f;
        [SerializeField] [Required] private GameObject popupPanel;
        [SerializeField] [Required] private Image backgroundImage;
        [SerializeField] [Required] private TMP_Text messageText;
        [SerializeField] [Required] private HorizontalLayoutGroup buttonGroup;
        private Color _defaultBackgroundImageColor;
        private UIManager _uiManager;

        private void Awake()
        {
            _defaultBackgroundImageColor = backgroundImage.color;
        }

        public async UniTask Show(bool shouldBlockInput = true)
        {
            DontDestroyOnLoad(gameObject);

            if (shouldBlockInput)
            {
                backgroundImage.gameObject.SetActive(true);
                var startColor = _defaultBackgroundImageColor;
                startColor.a = 0;
                backgroundImage.color = startColor;
                backgroundImage
                    .DOFade(_defaultBackgroundImageColor.a, AnimationDuration)
                    .SetEase(Ease.OutExpo)
                    .WithCancellation(this.GetCancellationTokenOnDestroy())
                    .Forget();
                backgroundImage.raycastTarget = true;
            }
            else
            {
                backgroundImage.gameObject.SetActive(false);
                backgroundImage.raycastTarget = false;
            }

            // Scale animation and fade in
            popupPanel.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            await UniTask.WhenAll(
                popupPanel.transform
                    .DOScale(Vector3.one, AnimationDuration)
                    .SetEase(Ease.OutExpo)
                    .WithCancellation(this.GetCancellationTokenOnDestroy()),
                popupPanel.GetComponent<CanvasGroup>().DOFade(1, AnimationDuration)
                    .SetEase(Ease.OutExpo)
                    .WithCancellation(this.GetCancellationTokenOnDestroy())
            );
        }

        public async UniTask Hide()
        {
            await UniTask.WhenAll(
                popupPanel.transform
                    .DOScale(new Vector3(1.5f, 1.5f, 1.5f), AnimationDuration)
                    .SetEase(Ease.OutExpo)
                    .WithCancellation(this.GetCancellationTokenOnDestroy()),
                popupPanel.GetComponent<CanvasGroup>().DOFade(0, AnimationDuration)
                    .SetEase(Ease.OutExpo)
                    .WithCancellation(this.GetCancellationTokenOnDestroy())
            );

            Destroy(gameObject);
        }

        [Inject]
        public void Inject(
            UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        public void SetMessage(string message)
        {
            messageText.text = message;
        }

        public void SetButtons(IEnumerable<ModalButton> buttons)
        {
            Debug.Assert(buttonGroup != null, "buttonGroup is null.");
            Debug.Assert(_uiManager != null, "_uiManager is null.");

            if (buttons == null) return;

            // Clear existing buttons
            foreach (Transform child in buttonGroup.transform) Destroy(child.gameObject);

            foreach (var button in buttons)
            {
                var modalButton = _uiManager.BuildModalButton(button.ButtonType, button.Text, button.OnClick);

                // Add button to button group
                modalButton.transform.SetParent(buttonGroup.transform);

                // Set scale to 1
                modalButton.transform.localScale = Vector3.one;
            }
        }

        public void SetWidthHeight(float width, float height)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public List<Button> GetButtons()
        {
            return
                (from Transform child
                        in buttonGroup.transform
                    select child.GetComponent<ModalButtonComponent>()
                        .GetButton())
                .ToList();
        }
    }
}