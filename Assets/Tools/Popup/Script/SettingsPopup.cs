using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Tools
{
    public class SettingsPopup : Popup, ISettingsPopup
    {
        [SerializeField] private Button m_music;
        [SerializeField] private Button m_sfx;
        [SerializeField] private Button m_credits;
        [SerializeField] private Button m_reset;
        [SerializeField] private LocalizeStringEvent m_customer;
        [SerializeField] private LocalizeStringEvent m_version;

        public override void Init(IPopupManager popupManager) 
        {
            base.Init(popupManager);
            m_customer.SetArgument("id", AuthenticationService.Instance.PlayerId);
            m_version.SetArgument("version", PlayerSettings.bundleVersion);
        }
    }

    public interface ISettingsPopup 
    {
        
    }
}
