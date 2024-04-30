using Codice.Client.BaseCommands.WkStatus.Printers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Tools
{
    public abstract class Popup : MonoBehaviour, IPopup
    {
        private IPopupManager m_popupManager;
        public void Init(IPopupManager popupManager)
        {
            m_popupManager = popupManager;
            transform.SetParent(m_popupManager.Container, false);
        }
        public void Close() 
        {
            m_popupManager.Close(this);
            Destroy(gameObject);
        }
    }

    public interface IPopup 
    {
        void Close();
        void Init(IPopupManager popupManager);
    }
}
