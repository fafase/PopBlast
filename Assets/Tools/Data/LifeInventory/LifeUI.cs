using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class LifeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_livesAmount;
        [SerializeField] private TextMeshProUGUI m_lifeHud;

        [Inject] private ILifeManager m_lifeManager;

        private void Start()
        {
            SetUI();
            m_lifeManager.RegisterOnLifeChange(SetUI);
        }
        private void Update()
        {
            UpdateTimer();
        }
        private void SetUI()
        {
            if (m_lifeManager.HasUnlimitedLives) 
            {
                // Show timer

                // Show unlimited heart
                return;
            }
            if (m_lifeManager.HasAllLives) 
            {
                // Show "FULL" text
                m_lifeHud.text = "Full";
            }
            else 
            {
                m_lifeHud.text = (m_lifeManager.NextLife - DateTime.Now).ToString(@"hh\:mm\:ss");
            }
            m_livesAmount.text = m_lifeManager.Amount.ToString();
        }

        private void UpdateTimer() 
        {
            if (m_lifeManager.HasAllLives) 
            {
                return;
            }
            // TODO add timer logic to discard 0 values
            var diff = m_lifeManager.NextLife - DateTime.Now;
            m_lifeHud.text = diff.ToString(@"hh\:mm\:ss");
        }
    }
}
