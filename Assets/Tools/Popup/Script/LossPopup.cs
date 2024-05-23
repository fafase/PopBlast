using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Tools
{
    public class LossPopup : Popup
    {
        [SerializeField] private LocalizeStringEvent m_locaString;
        [SerializeField] private Button m_playButton;

        private void Awake()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_playButton.onClick.AddListener(() => Close());
        }

        public void InitWithLevel(Level lvl, Action onPress, Action onQuit)
        {
            m_locaString.SetArgument("level", lvl.level.ToString());
            m_playButton.onClick.AddListener(() => onPress?.Invoke());
            m_closeBtn.onClick.AddListener(() => onQuit?.Invoke());
        }
    }
}
