using System.Collections.Generic;
using BetterForNothing.Scripts.Loading;
using VContainer;

namespace BetterForNothing.Scripts.Message
{
    public class SceneLoadRequest
    {
        public SceneLoadRequest(string targetSceneName,
            string loadingSceneName,
            IObjectResolver containerToResolveHandlers,
            IEnumerable<ILoadingHandler> handlers)
        {
            TargetSceneName = targetSceneName;
            LoadingSceneName = loadingSceneName;
            ContainerToResolveHandlers = containerToResolveHandlers;
            Handlers = handlers;
        }

        public string TargetSceneName { get; }
        public string LoadingSceneName { get; }
        public IObjectResolver ContainerToResolveHandlers { get; }
        public IEnumerable<ILoadingHandler> Handlers { get; }
    }
}