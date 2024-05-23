using Cysharp.Threading.Tasks;
using TMPro;
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
        [Inject] private IPlayerData m_playerData;
        [Inject] private ILevelManager m_levelManager;
        [Inject] private ILifeManager m_lifeManager;

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
                ((PlayPopup)m_popupManager.Show<PlayPopup>())
                .InitWithLevel(m_levelManager.Levels[currentLevel - 1], OnPressCallback);
            });

            m_settings.onClick.AddListener(() => OpenSettings());

            Signal.Send(new MetaLanding());
        }

        private void OnPressCallback() 
        {
            if (m_lifeManager.HasLife) 
            {
                m_popupManager.LoadSceneWithLoadingPopup("GameScene").Forget();
            }
        }

        private void OpenSettings()
        {
            m_popupManager.Show<SettingsPopup>();
        }
    }
}
