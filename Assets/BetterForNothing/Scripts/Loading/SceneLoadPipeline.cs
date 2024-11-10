using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;

namespace BetterForNothing.Scripts.Loading
{
    public class SceneLoadPipeline
    {
        private readonly List<ILoadingHandler> _handlers;

        public SceneLoadPipeline(IEnumerable<ILoadingHandler> handlers)
        {
            _handlers = new List<ILoadingHandler>(handlers);
        }

        [Inject]
        public void Inject(LoadingProgressManager progressManager)
        {
            var stepsWeight = new float[_handlers.Count];
            for (var i = 0; i < _handlers.Count; i++) stepsWeight[i] = _handlers[i].Weight;
            progressManager.Configure(stepsWeight);
        }

        public async UniTask ExecutePipeline(BetterSceneManager manager)
        {
            uint index = 0;
            foreach (var handler in _handlers)
            {
                await handler.ExecuteAsync(index, manager);
                index++;

                // dispose handler
                if (handler is IDisposable disposable) disposable.Dispose();
            }
        }
    }
}