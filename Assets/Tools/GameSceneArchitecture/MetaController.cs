using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Tools
{
    public class MetaController : MonoBehaviour
    {
        [SerializeField] private Button m_settings;
        [SerializeField] private Button m_playBtn;

        [Inject] private IPopupManager m_popupManager;
        [Inject] private IServicesManager m_servicesManager;
        [Inject] private IUserPrefs m_userPrefs;
        [Inject] private ILevelManager m_levelManager;

        private void Awake()
        {
            // 4 levels of difficulties
            m_playBtn.onClick.AddListener(() =>
            {
                PlayPopup popup = (PlayPopup)m_popupManager.Show<PlayPopup>();
                m_userPrefs.TryGetInt("level", out int currentLevel, 1);
                if(currentLevel > m_levelManager.Levels.Count) 
                {
                    // This is end of content
                    currentLevel = m_levelManager.Levels.Count - 1;
                }
                Level level = m_levelManager.Levels[currentLevel];
                popup.InitWithLevel(level);
            });

            m_settings.onClick.AddListener(() => OpenSettings());
        }

        private void OpenSettings()
        {
            m_popupManager.Show<SettingsPopup>();
        }
    }
}
