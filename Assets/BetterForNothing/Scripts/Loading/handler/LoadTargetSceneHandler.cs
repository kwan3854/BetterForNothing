using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace BetterForNothing.Scripts.Loading.handler
{
    public class LoadTargetSceneHandler : ILoadingHandler
    {
        private readonly string _message;
        private LoadingProgressManager _progressManager;
        private BetterSceneManager _betterSceneManager;

        public LoadTargetSceneHandler(float weight, string message)
        {
            Debug.Assert(weight > 0, "weight is not positive.");

            Weight = weight;
            _message = message;
        }

        public float Weight { get; }

        public async UniTask ExecuteAsync(uint index)
        {
            var targetSceneName = _betterSceneManager.CurrentSceneLoadRequest.TargetSceneName; // Get target scene name

            _progressManager.UpdateStepProgress(index, 0.0f); // Update progress to 0%
            _progressManager.UpdateStepMessage(index, _message); // Update message

            var targetSceneLoadOp = SceneManager.LoadSceneAsync(targetSceneName);
            if (targetSceneLoadOp != null)
            {
                targetSceneLoadOp.allowSceneActivation = false; // Prevent automatic activation

                while (targetSceneLoadOp.progress < 0.90f)
                {
                    _progressManager.UpdateStepProgress(index, targetSceneLoadOp.progress);
                    await UniTask.Yield();
                }

                targetSceneLoadOp.allowSceneActivation = true; // Allow scene activation
                await targetSceneLoadOp.ToUniTask();
                // Update progress to 100% after loading completes
                _progressManager.UpdateStepProgress(index, 1.0f);
            }
        }

        [Inject]
        public void Inject(LoadingProgressManager progressManager, BetterSceneManager manager)
        {
            // Validate injected dependencies
            Debug.Assert(progressManager != null, "progressManager is null.");
            _progressManager = progressManager;
            _betterSceneManager = manager;
        }
    }
}