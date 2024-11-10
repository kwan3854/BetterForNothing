using Cysharp.Threading.Tasks;

namespace BetterForNothing.Scripts.Popup
{
    public interface IPopup
    {
        UniTask Show(bool shouldBlockInput = true);
        UniTask Hide();
    }
}