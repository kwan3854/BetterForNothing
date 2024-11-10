using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace BetterForNothing.Scripts.Loading.handler
{
    public class FakeLoadingHandler : ILoadingHandler
    {
        private readonly float _duration;
        private readonly string _message;
        private LoadingProgressManager _progressManager;

        public FakeLoadingHandler(float weight, float duration, string message)
        {
            Debug.Assert(duration > 0, "duration is not positive.");

            Weight = weight;
            _duration = duration;
            _message = message;
        }

        public float Weight { get; }

        public async UniTask ExecuteAsync(uint index, BetterSceneManager manager)
        {
            // Smoothing out the loading process every frame
            var elapsedTime = 0.0f;

            _progressManager.UpdateStepProgress(index, 0.0f);
            _progressManager.UpdateStepMessage(index, _message);

            while (elapsedTime < _duration)
            {
                _progressManager.UpdateStepProgress(index, elapsedTime / _duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            _progressManager.UpdateStepProgress(index, 1.0f);
        }

        [Inject]
        public void Inject(LoadingProgressManager progressManager)
        {
            Debug.Assert(progressManager != null, "progressManager is null.");
            _progressManager = progressManager;
        }
    }
}