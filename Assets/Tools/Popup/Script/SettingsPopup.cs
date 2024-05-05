using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class SettingsPopup : Popup, ISettingsPopup
    {
        [SerializeField] private Button m_music;
        [SerializeField] private Button m_sfx;
        [SerializeField] private Button m_credits;
        [SerializeField] private Button m_reset;
        [SerializeField] private LocalizedTMP_UGUI m_customer;
        [SerializeField] private LocalizedTMP_UGUI m_version;

        public override void Init(IPopupManager popupManager) 
        {
            base.Init(popupManager);
            string id = AuthenticationService.Instance.PlayerId;
            m_customer.SetArgument("id", id);
            m_version.SetArgument("version", "0.1.0");
        }
    }

    public interface ISettingsPopup 
    {
        
    }
}
