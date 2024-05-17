using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Tools
{
    public class MetaController : MonoBehaviour
    {
        [SerializeField] private Button m_settings;
        [SerializeField] private Button m_playBtn;
        [SerializeField] private TextMeshProUGUI m_coins;
        [SerializeField] private TextMeshProUGUI m_lives;

        [Inject] private IPopupManager m_popupManager;
        [Inject] private IPlayerData m_playerData;
        [Inject] private ILevelManager m_levelManager;

        private void Awake()
        {
            int currentLevel = m_playerData.CurrentLevel;
            m_playBtn.GetComponentInChildren<TextMeshProUGUI>().text = currentLevel.ToString();

            m_playBtn.onClick.AddListener(() =>
            {

                if(currentLevel > m_levelManager.Levels.Count) 
                {
                    // This is end of content
                    currentLevel = m_levelManager.Levels.Count - 1;
                }
                PlayPopup popup = (PlayPopup)m_popupManager.Show<PlayPopup>();
                Level level = m_levelManager.Levels[currentLevel-1];
                popup.InitWithLevel(level, () =>  m_popupManager.LoadSceneWithLoadingPopup("GameScene").Forget());
            });

            m_settings.onClick.AddListener(() => OpenSettings());
            m_coins.text = m_playerData.Inventory.Coins.ToString();
            m_lives.text = m_playerData.Inventory.Lives.ToString();

            Signal.Send(new MetaLanding());
        }

        private void OpenSettings()
        {
            m_popupManager.Show<SettingsPopup>();
        }
    }
}
