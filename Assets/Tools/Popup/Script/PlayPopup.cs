using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Zenject;

namespace Tools
{
    public class PlayPopup : Popup
    {
        [SerializeField] private LocalizeStringEvent m_locaString;
        [SerializeField] private Button m_playButton;
        [SerializeField] private GameObject m_objectivePrefab;

        private void Awake()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_playButton.onClick.AddListener(() =>
            {
                Close();
            });
        }

        public void InitWithLevel(Level lvl, ILevelItems lvlItems, Action onPress)
        {
            m_locaString.SetArgument("level", lvl.level.ToString());
            m_playButton.onClick.AddListener(() => onPress?.Invoke());

            Transform parentTr = m_objectivePrefab.transform.parent;
            SetObjectives(lvl.Objectives, lvlItems);
        }

        public void SetObjectives(List<Objective> objectives, ILevelItems lvlItems)
        {
            Transform parentTr = m_objectivePrefab.transform.parent;
            foreach (Objective objective in objectives)
            {
                GameObject obj = Instantiate(m_objectivePrefab);
                obj.transform.SetParent(parentTr, false);
                obj.SetActive(true);
                Sprite sprite = lvlItems.GetCoreItem((int)objective.itemType);
                Image img = obj.GetComponent<Image>();
                img.sprite = sprite;
                TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
                txt.text = objective.amount.ToString();
            }
        }
    }
}
