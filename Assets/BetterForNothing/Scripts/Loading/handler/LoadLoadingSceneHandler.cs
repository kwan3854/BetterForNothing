using BetterForNothing.Scripts.Popup;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace BetterForNothing.Scripts.Loading.handler
{
    public class LoadLoadingSceneHandler : ILoadingHandler
    {
        private readonly string _message;

        private LoadingProgressManager _progressManager;
        private UIManager _uiManager;

        // Constructor for manual instantiation
        public LoadLoadingSceneHandler(float weight, string message)
        {
            Weight = weight;

            _message = message;
        }

        public float Weight { get; }

        public async UniTask ExecuteAsync(uint index, BetterSceneManager manager)
        {
            var loadingSceneName = manager.CurrentSceneLoadRequest.LoadingSceneName;

            var popup = _uiManager.BuildLoadingPopup();
            await popup.Show();

            await UniTask.WaitForSeconds(0.5f);

            var loadingSceneLoadOp = SceneManager.LoadSceneAsync(loadingSceneName);
            await loadingSceneLoadOp.ToUniTask();

            // _hideLoadingPopupPublisher.Publish(new HideLoadingPopupMessage());
            await popup.Hide();
            _progressManager.UpdateStepProgress(index, 1.0f);
            _progressManager.UpdateStepMessage(index, _message);
        }

        // Method injection for dependencies
        [Inject]
        public void Inject(
            LoadingProgressManager progressManager,
            UIManager uiManager
        )
        {
            Debug.Assert(progressManager != null, "progressManager is null.");
            Debug.Assert(uiManager != null, "uiManager is null.");

            _progressManager = progressManager;
            _uiManager = uiManager;
        }
    }
}