using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Tools
{
    public class PlayPopup : Popup
    {
        [SerializeField] private LocalizeStringEvent m_locaString;
        [SerializeField] private Button m_playButton;
        [SerializeField] private GameObject m_objectivePrefab;

        private void Awake()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_playButton.onClick.AddListener(() =>
            {
                Close();
            });
        }

        public void InitWithLevel(Level lvl, ILevelItems lvlItems, Action onPress)
        {
            m_locaString.SetArgument("level", lvl.level.ToString());
            m_playButton.onClick.AddListener(() => onPress?.Invoke());

            Transform parentTr = m_objectivePrefab.transform.parent;
            Popup.SetObjectives(lvl.Objectives, lvlItems, m_objectivePrefab);
        }
    }
}
