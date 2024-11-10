using System;
using BetterForNothing.Scripts.Message;
using MessagePipe;
using UnityEngine;

namespace BetterForNothing.Scripts.Loading
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LoadingProgressManager
    {
        private readonly IPublisher<UnifiedLoadProgressMessage> _progressPublisher;
        private readonly IPublisher<UnifiedLoadStateMessage> _statePublisher;
        private float[] _stepsProgress;
        private float[] _stepsWeight;

        private float _totalProgress;

        public LoadingProgressManager(
            IPublisher<UnifiedLoadProgressMessage> progressPublisher,
            IPublisher<UnifiedLoadStateMessage> statePublisher)
        {
            Debug.Assert(progressPublisher != null, "progressPublisher is null.");
            _progressPublisher = progressPublisher;
            _statePublisher = statePublisher;
        }

        public void Configure(float[] stepsWeight)
        {
            _stepsWeight = stepsWeight;
            _stepsProgress = new float[stepsWeight.Length];
            _totalProgress = 0;
        }

        public void UpdateStepProgress(uint stepIndex, float progress)
        {
            try
            {
                _stepsProgress[stepIndex] = progress * _stepsWeight[stepIndex];
                CalculateTotalProgress();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error occurred while updating step progress: {e}");
                throw;
            }
        }

        public void UpdateStepMessage(uint stepIndex, string message)
        {
            var progressMessage = new UnifiedLoadStateMessage(message);
            _statePublisher.Publish(progressMessage);
        }

        private void CalculateTotalProgress()
        {
            _totalProgress = 0;
            foreach (var stepProgress in _stepsProgress) _totalProgress += stepProgress;

            var progressMessage = new UnifiedLoadProgressMessage(_totalProgress);

            _progressPublisher.Publish(progressMessage);
        }
    }
}