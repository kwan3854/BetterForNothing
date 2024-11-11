using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BetterForNothing.Scripts.Loading.handler
{
    public class LoadAdditionalResourcesHandler : ILoadingHandler
    {
        private readonly string _message;
        private readonly LoadingProgressManager _progressManager;

        public LoadAdditionalResourcesHandler(LoadingProgressManager progressManager,
            int order, float weight, string message)
        {
            Debug.Assert(progressManager != null, "progressManager is null.");
            _progressManager = progressManager;
            Order = order;
            Weight = weight;

            _message = message;
        }

        public int Order { get; }
        public float Weight { get; }

        public async UniTask ExecuteAsync(uint index)
        {
            Debug.Log("Loading additional resources...");

            _progressManager.UpdateStepProgress(index, 0.0f);
            _progressManager.UpdateStepMessage(index, _message);

            // 추가적인 리소스 로드 로직
            var elapsedTime = 0.0f;
            while (elapsedTime < 1.0f)
            {
                _progressManager.UpdateStepProgress(index, elapsedTime);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            Debug.Log("Additional resources loaded.");
            _progressManager.UpdateStepProgress(index, 1.0f);
        }
    }
}