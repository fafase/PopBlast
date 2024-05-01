using Codice.Client.BaseCommands.WkStatus.Printers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public abstract class Popup : MonoBehaviour, IPopup
    {
        [SerializeField] private Button m_closeBtn;

        private IPopupManager m_popupManager;
        public void Init(IPopupManager popupManager)
        {
            m_popupManager = popupManager;
            transform.SetParent(m_popupManager.Container, false);
            m_closeBtn.onClick.AddListener(Close);
        }

        public void Close() 
        {
            m_popupManager.Close(this);
            m_closeBtn.onClick.RemoveAllListeners();
            Destroy(gameObject);
        }
    }

    public interface IPopup 
    {
        void Close();
        void Init(IPopupManager popupManager);
    }
}
