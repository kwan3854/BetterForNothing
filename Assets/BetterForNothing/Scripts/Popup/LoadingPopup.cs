using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace BetterForNothing.Scripts.Popup
{
    public class LoadingPopup : MonoBehaviour, IPopup
    {
        private const float AnimationDuration = 0.3f;
        [SerializeField] [Required] private GameObject popupPanel;
        [SerializeField] [Required] private Image backgroundImage;

        private Color _defaultBackgroundImageColor;

        private void Awake()
        {
            _defaultBackgroundImageColor = backgroundImage.color;
        }

        public async UniTask Show(bool shouldBlockInput = true)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(true);

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

            // popup animation with dotween (scale and fade in)
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
            // popup animation with dotween (scale and fade out)
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
    }
}