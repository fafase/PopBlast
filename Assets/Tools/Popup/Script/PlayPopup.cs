using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class PlayPopup : Popup
    {
        [SerializeField] private LocalizedTMP_UGUI m_title;
        [SerializeField] private Button m_playButton;

        private void Start()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_playButton.onClick.AddListener(() =>
            {
                Close();
            });
        }

        public void InitWithLevel(Level level) 
        {
            m_title.SetArgument(new LocArgument("level", level.level.ToString()));
        }
    }
}
