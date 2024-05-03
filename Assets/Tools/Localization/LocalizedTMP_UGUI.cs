using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tools
{
    [AddComponentMenu("UI/Localized Text UI")]
    public class LocalizedTMP_UGUI : TextMeshProUGUI
    {
        [SerializeField] private Localization m_localizer;
        [SerializeField] private string m_localizationKey;
        [SerializeField] private List<LocArgument> m_arguments;

        public IReadOnlyList<LocArgument> Arguments => m_arguments;
        public string LocalizationKey => m_localizationKey;

        protected override void Awake()
        {
            base.Awake();
            text = m_localizer.GetLocalization(m_localizationKey, m_arguments);
        }

        public void SetLocalizedText()
        {
            SetArguments(m_arguments);
        }

        public void SetArgument(string key, string value)
        {
            SetArgument(new LocArgument(key, value));
        }
        public void SetArgument(LocArgument arg)
        {
            SetArguments(new List<LocArgument>() { arg });
        }

        public void SetArguments(List<LocArgument> arg) 
        {
            string t = m_localizer.GetLocalization(m_localizationKey, arg);
            text = t;
        }
    }
}
