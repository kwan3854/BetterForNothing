using Cysharp.Threading.Tasks;

namespace BetterForNothing.Scripts.Loading
{
    public interface ILoadingHandler
    {
        float Weight { get; }
        UniTask ExecuteAsync(uint index, BetterSceneManager manager);
    }
}