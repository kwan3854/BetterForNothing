using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterForNothing.Scripts.Popup
{
    public class ModalButtonComponent : MonoBehaviour
    {
        [SerializeField] [Required] private TMP_Text buttonText;
        [SerializeField] [Required] private Button button;

        public void SetText(string text)
        {
            buttonText.text = text;
        }

        public void SetOnClick(Action onClick)
        {
            button.onClick.AddListener(() => { onClick(); });
        }

        public Button GetButton()
        {
            return button;
        }
    }
}