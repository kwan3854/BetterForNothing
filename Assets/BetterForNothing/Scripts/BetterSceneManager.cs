using BetterForNothing.Scripts.Loading;
using BetterForNothing.Scripts.Message;
using BetterForNothing.Scripts.Popup;
using Common.Enum;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace BetterForNothing.Scripts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BetterSceneManager : IStartable
    {
        private readonly IObjectResolver _container;
        private readonly UIManager _uiManager;

        public BetterSceneManager(
            ISubscriber<SceneLoadRequest> loadSceneSubscriber,
            UIManager uiManager,
            IObjectResolver container)
        {
            var disposableBagBuilder = DisposableBag.CreateBuilder();
            loadSceneSubscriber
                .Subscribe(message => LoadSceneWithPipeline(message).Forget())
                .AddTo(disposableBagBuilder);
            // Never need to be disposed
            // It is a singleton class
            disposableBagBuilder.Build();
            _container = container;
            _uiManager = uiManager;
        }

        public SceneLoadRequest CurrentSceneLoadRequest { get; private set; }

        public void Start()
        {
            // For NonLazy Instantiation.
        }

        public async UniTask LoadSceneAdditive(SceneName sceneName, bool shouldShowLoadingPopup = true)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);

            // Show loading popup
            LoadingPopup loadingPopup = null;
            if (shouldShowLoadingPopup)
            {
                loadingPopup = _uiManager.BuildLoadingPopup();
                await loadingPopup.Show();
            }

            await asyncOperation.ToUniTask();

            var scene = SceneManager.GetSceneByName(sceneName.ToString());

            // Handle Duplicated Camera
            if (scene.IsValid())
            {
                var rootGameObjects = scene.GetRootGameObjects();
                foreach (var rootGameObject in rootGameObjects)
                    if (rootGameObject.CompareTag("MainCamera"))
                        rootGameObject.SetActive(false);
            }

            // Handle Duplicated event system
            var eventSystem = Object.FindObjectsOfType<EventSystem>();
            if (eventSystem.Length > 1)
                for (var i = 1; i < eventSystem.Length; i++)
                    Object.Destroy(eventSystem[i].gameObject);

            // Hide loading popup
            if (loadingPopup != null) await loadingPopup.Hide();

            // activate scene
            SceneManager.SetActiveScene(scene);
        }

        public async UniTask UnloadScene(SceneName sceneName, bool shouldShowLoadingPopup = true)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneName.ToString());

            // Show loading popup
            LoadingPopup loadingPopup = null;
            if (shouldShowLoadingPopup)
            {
                loadingPopup = _uiManager.BuildLoadingPopup();
                await loadingPopup.Show();
            }

            await asyncOperation.ToUniTask();

            // Hide loading popup
            if (loadingPopup != null) await loadingPopup.Hide();
        }

        private async UniTask LoadSceneWithPipeline(SceneLoadRequest sceneLoadRequest)
        {
            var container = sceneLoadRequest.ContainerToResolveHandlers;
            // Inject handlers
            foreach (var handler in sceneLoadRequest.Handlers) container.Inject(handler);

            var pipeline = new SceneLoadPipeline(sceneLoadRequest.Handlers);
            _container.Inject(pipeline);
            CurrentSceneLoadRequest = sceneLoadRequest;
            await pipeline.ExecutePipeline(this);
        }
    }
}