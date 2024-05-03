using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class PopupManager : MonoBehaviour, IPopupManager
    {
        [SerializeField] private List<Popup> m_popups = new List<Popup>();
        public List<Popup> Popups => m_popups;
        public bool ShouldWaitForCompletion => false;

        private bool m_isInit = false;
        public bool IsInit => m_isInit;
        public RectTransform Container => transform as RectTransform;

        private List<IPopup> m_openPopups = new List<IPopup>();
        public List<IPopup> OpenPopups => m_openPopups;
        public int PopupsCount => m_openPopups.Count;

        private IDictionary<Type, Popup> m_prefabs = new Dictionary<Type, Popup>();

        private void Awake()
        {
            m_prefabs = m_popups.ToDictionary(p => p.GetType(), p => p);

            m_isInit = true;
        }

        public IPopup Show<T>() where T : IPopup 
        {
            if(m_prefabs.TryGetValue(typeof(T), out Popup popup)) 
            {
                Popup instance = Instantiate(popup);
                m_openPopups.Add(instance);
                instance.Init(this);
                return instance;
            }
            Debug.LogError("[PopupManager] Popup was not found in manager list");
            return null;
        }


        public void Close(IPopup popup) 
        {
            m_openPopups.Remove(popup);
        }
    }

    public interface IPopupManager 
    {
        List<Popup> Popups { get; }
        RectTransform Container { get; }

        void Close(IPopup popup);
        IPopup Show<T>() where T : IPopup;
    }
}
