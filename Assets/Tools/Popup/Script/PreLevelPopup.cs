using UnityEngine;
using UnityEngine.Localization.Components;

namespace Tools
{
    public class PreLevelPopup : Popup
    {
        [SerializeField] private LocalizeStringEvent m_locaString;
        [SerializeField] private GameObject m_objectivePrefab;

        public void InitWithLevel(Level lvl, ILevelItems lvlItems)
        {
            m_locaString.SetArgument("level", lvl.level.ToString());
            Popup.SetObjectives(lvl.Objectives, lvlItems, m_objectivePrefab);
            OnOpen += _ => Close(true);
        }
    }
}
