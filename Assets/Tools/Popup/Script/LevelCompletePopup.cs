using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class LevelCompletePopup : Popup 
    {
        [SerializeField] private LocalizedTMP_UGUI m_title;
        public void ConfirmLevelComplete() 
        {
            Close();
        }

        public void InitWithLevel(int level)
        {
            m_title.SetArgument(new LocArgument("level", level.ToString()));
        }
    }
}
