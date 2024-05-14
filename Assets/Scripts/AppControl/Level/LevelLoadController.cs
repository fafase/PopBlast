using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Tools;
using Zenject;

namespace PopBlast.AppControl
{
    /// <summary>
    /// Front page controller with four buttons to load level with predifined difficulties 
    /// </summary>
    public class LevelLoadController : MonoBehaviour
    {


        #region MEMBERS
        [SerializeField] private Button m_settings;
        [SerializeField] private Button m_playBtn;

        [Inject] private IPopupManager m_popupManager;
        [Inject] private IServicesManager m_servicesManager;

        #endregion

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            // 4 levels of difficulties
            m_playBtn.onClick.AddListener(()=> 
            {
                m_popupManager.Show<PlayPopup>();            
            });

            m_settings.onClick.AddListener(() => OpenSettings());
        }

        #endregion

        private void OpenSettings() 
        {
            m_popupManager.Show<SettingsPopup>();
        }
    }
}