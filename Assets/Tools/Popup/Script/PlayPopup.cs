using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tools
{
    public class PlayPopup : Popup
    {
        [SerializeField] private LocalizedTMP_UGUI m_title;
        [SerializeField] private Button m_playButton;

        private void Start()
        {
            m_title.SetArgument(new LocArgument("level", CurrentLevel.ToString()));
            m_playButton.onClick.RemoveAllListeners();
            m_playButton.onClick.AddListener(() =>
            {
                Close();
                SetLevelSettings(5, 5);
            });
        }

        private int CurrentLevel => 1;

        private void SetLevelSettings(int width, int height)
        {
            width = Mathf.Clamp(width, 5, 20);
            height = Mathf.Clamp(height, 5, 20);

            PlayerPrefs.SetInt("Width", width);
            PlayerPrefs.SetInt("Height", height);

            SceneManager.LoadScene(2);
        }
    }
}
