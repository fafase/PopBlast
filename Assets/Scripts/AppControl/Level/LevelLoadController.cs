using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Tools;
using Zenject;

namespace PopBlast.AppControl.Level
{
    /// <summary>
    /// Front page controller with four buttons to load level with predifined difficulties 
    /// </summary>
    public class LevelLoadController : MonoBehaviour
    {
        #region MEMBERS

        [SerializeField] private Button superEasyBtn = null;
        [SerializeField] private Button easyBtn = null;
        [SerializeField] private Button mediumBtn = null;
        [SerializeField] private Button hardBtn = null;

        [Inject] private IPopupManager m_popupManager;

        #endregion

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            // 4 levels of difficulties
            superEasyBtn.onClick.AddListener(()=> 
            {
                m_popupManager.Show<BasicPopup>();
                //SetLevelSettings(10,10);              
            });
            easyBtn.onClick.AddListener(() => 
            {
                SetLevelSettings(10, 6);
            });
            mediumBtn.onClick.AddListener(() => 
            {
                SetLevelSettings(8, 6);
            });
            hardBtn.onClick.AddListener(() => 
            {
                SetLevelSettings(5, 5);
            });
        }

        #endregion

        #region PRIVATE_METHODS

        // Clamp value [5, 20], stores it in PF, loads next scene
        private void SetLevelSettings(int width, int height)
        {
            width = Mathf.Clamp(width, 5, 20);
            height = Mathf.Clamp(height, 5 , 20);

            PlayerPrefs.SetInt("Width", width);
            PlayerPrefs.SetInt("Height", height);

            SceneManager.LoadScene(1);
        }

        #endregion
    }
}