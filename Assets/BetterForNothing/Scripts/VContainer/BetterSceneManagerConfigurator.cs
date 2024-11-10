using BetterForNothing.Scripts.Loading;
using BetterForNothing.Scripts.Message;
using BetterForNothing.Scripts.Popup;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace BetterForNothing.Scripts.VContainer
{
    /// <summary>
    ///     Configures core dependencies required for the BetterSceneManager.
    /// </summary>
    public static class BetterSceneManagerConfigurator
    {
        private static IContainerBuilder _builder;
        private static MessagePipeOptions _messagePipeOptions;

        public static void Initialize(IContainerBuilder builder, MessagePipeOptions messagePipeOptions)
        {
            _builder = builder;
            _messagePipeOptions = messagePipeOptions;

            // Register BetterSceneManager as a singleton
            builder.RegisterEntryPoint<BetterSceneManager>().AsSelf();
            builder.RegisterEntryPoint<UIManager>().AsSelf();

            builder.Register<LoadingProgressManager>(Lifetime.Singleton);

            // Register MessagePipe related configurations
            RegisterMessagePipe();
        }

        private static void RegisterMessagePipe()
        {
            // 메시지 브로커 등록
            _builder.RegisterMessageBroker<SceneLoadRequest>(_messagePipeOptions);
            _builder.RegisterMessageBroker<UnifiedLoadProgressMessage>(_messagePipeOptions);
            _builder.RegisterMessageBroker<UnifiedLoadStateMessage>(_messagePipeOptions);
        }
    }
}