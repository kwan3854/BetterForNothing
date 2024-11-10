using System;

namespace BetterForNothing.Scripts.Popup
{
    public struct ModalButton
    {
        public readonly ModalButtonType ButtonType;
        public readonly string Text;
        public readonly Action OnClick;

        public ModalButton(ModalButtonType buttonType, string text, Action onClick)
        {
            ButtonType = buttonType;
            Text = text;
            OnClick = onClick;
        }
    }
}