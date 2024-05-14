using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

namespace Tools
{
    public class PlayPopup : Popup
    {
        [SerializeField] private LocalizeStringEvent m_locaString;
        [SerializeField] private Button m_playButton;

        private Action m_onPress;
        private void Start()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_playButton.onClick.AddListener(() =>
            {
                m_onPress?.Invoke();
                Close();
            });
        }

        public void InitWithLevel(Level level, Action onPress) 
        {
            m_onPress = onPress;
            m_locaString.SetArgument("level",  level.level.ToString());
        }
    }
}
