using TMPro;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class CoinUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_coinHud;

        [Inject] private ICoinManager m_coinManager;

        private void Start()
        {           
            m_coinHud.text = m_coinManager.Coins.ToString();
        }
    }
}
