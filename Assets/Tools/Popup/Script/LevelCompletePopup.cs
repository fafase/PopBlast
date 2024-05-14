using UnityEngine;
using UnityEngine.Localization.Components;

namespace Tools
{
    public class LevelCompletePopup : Popup 
    {
        [SerializeField] private LocalizeStringEvent m_locaString;
        public void ConfirmLevelComplete() 
        {
            Close();
        }

        public void InitWithLevel(int level)
        {
            m_locaString.SetArgument("level", level.ToString());
        }
    }
}
