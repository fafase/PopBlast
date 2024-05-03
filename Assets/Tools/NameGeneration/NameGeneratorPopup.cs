using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tools
{
    public class NameGeneratorPopup : Popup
    {
        [SerializeField] private TMP_InputField m_input;
        [SerializeField] private TextMeshProUGUI m_name;

        private string m_default;
        INameGenerationInit m_generator;
        public void Init(INameGenerationInit nameGenerationInit)
        {
            m_generator = nameGenerationInit;
            m_default = m_name.text = m_generator.RandomName;
        }

        public void UpdateName(string input)
        {
            m_name.text =  string.IsNullOrEmpty(input) ? m_default : input;
        }

        public string Name => m_name.text;
    }
}
