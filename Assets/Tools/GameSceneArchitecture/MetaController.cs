using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;

namespace Tools
{
    public class MetaController : MonoBehaviour
    {
        [SerializeField] private Button m_settings;
        [SerializeField] private Button m_playBtn;

        [Inject] private IPopupManager m_popupManager;
        [Inject] private IServicesManager m_servicesManager;
        [Inject] private IPlayerData m_playerData;
        [Inject] private ILevelManager m_levelManager;

        private void Awake()
        {
            int currentLevel = m_playerData.CurrentLevel;
            m_playBtn.GetComponentInChildren<TextMeshProUGUI>().text = currentLevel.ToString();

            m_playBtn.onClick.AddListener(() =>
            {
                PlayPopup popup = (PlayPopup)m_popupManager.Show<PlayPopup>();
                if(currentLevel > m_levelManager.Levels.Count) 
                {
                    // This is end of content
                    currentLevel = m_levelManager.Levels.Count - 1;
                }
                Level level = m_levelManager.Levels[currentLevel-1];
                popup.InitWithLevel(level);
            });

            m_settings.onClick.AddListener(() => OpenSettings());

            Signal.Send(new MetaLanding());
        }

        private void OpenSettings()
        {
            m_popupManager.Show<SettingsPopup>();
        }
    }
}
