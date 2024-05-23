using System;
using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace PopBlast.UI
{
    /// <summary>
    /// Controls the view of the level
    /// </summary>
    public class UIController : MonoBehaviour, ICoreUI
    {
        [Inject] private ILevelItems m_levelItems;

        [SerializeField] private GameObject m_objectivePrefab;
        [SerializeField] private Transform m_objectiveContainer;

        [SerializeField] private TextMeshProUGUI m_moveTxt;
        [SerializeField] private Button quitBtn = null;
        [SerializeField] private TextMeshProUGUI feedbackTxt = null;

        [Header("Feedback")]
        [Space()]
        [SerializeField] private float feedbackTimer = 1f;
        [Tooltip("For each key value, provide an amount and a message. Amount is how many item destroyed in one tap")]
        [SerializeField] private FeedbackKeyValue[] keyValues;


        protected virtual void Awake()
        {
            quitBtn.onClick.AddListener(()=> 
            {
                SceneManager.LoadScene(0);
            });
            SetFeedbackPanelOff();
        }


        /// <summary>
        /// Set feedback panel based on item destroyed amount
        /// </summary>
        /// <param name="amount"></param>
        public void SetFeedback(int amount)
        {
            // if amount is lower than first item, nothing happens
            if (amount < keyValues[0].amount)
            {
                return;
            }
            string message = null;
            // if amount is greater than last item, use last item
            if (amount > keyValues[keyValues.Length - 1].amount)
            {
                message = keyValues[keyValues.Length - 1].message;
            }
            else
            {
                // Find index and use message
                int index = Array.FindIndex(keyValues, (o) => { return o.amount == amount; });
                message = message = keyValues[index].message;
            }

            feedbackTxt.transform.parent.gameObject.SetActive(true);
            feedbackTxt.text = message;
            Invoke("SetFeedbackPanelOff", feedbackTimer);
        }

        private void SetFeedbackPanelOff()
        {
            feedbackTxt.transform.parent.gameObject.SetActive(false);
        }
        public void SetMoveCount(int moves) => m_moveTxt.text = moves.ToString();

        private List<UIObjective> m_objectives;
        public void SetObjectives(List<Objective> objectives)
        {
            m_objectives = new List<UIObjective>();
            foreach (Objective objective in objectives) 
            {
                GameObject obj = Instantiate(m_objectivePrefab);
                obj.transform.SetParent(m_objectiveContainer.transform, false);
                obj.SetActive(true);
                Sprite sprite = m_levelItems.GetCoreItem((int)objective.itemType);
                Image img = obj.GetComponent<Image>();
                img.sprite = sprite;
                TextMeshProUGUI txt = obj.GetComponentInChildren<TextMeshProUGUI>();
                txt.text = objective.amount.ToString();
                UIObjective uiObj = new UIObjective(img, txt, objective.itemType);
                m_objectives.Add(uiObj);  
            }
        }

        public void UpdateObjectives(int type, int amount)
        {
            ObjectiveItemType oit = (ObjectiveItemType)type;
            UIObjective o = m_objectives.Find((obj) => obj.type == oit);
            if (o != null)
            {
                o.txtComp.text = amount.ToString();
            }
        }

        class UIObjective
        {
            public Image image;
            public TextMeshProUGUI txtComp;
            public ObjectiveItemType type;
            public UIObjective(Image img, TextMeshProUGUI txt, ObjectiveItemType t) 
            {
                image = img;
                txtComp = txt;
                type = t;
            }
        }

        [Serializable]
        public struct FeedbackKeyValue
        {
            /// <summary>
            /// Amount of item to be destroyed in single tap
            /// </summary>
            public int amount;
            /// <summary>
            /// Feedback message
            /// </summary>
            public string message;
        }
    }

    public interface ICoreUI
    {
        void SetFeedback(int amount);
        void SetMoveCount(int m_moves);
        void SetObjectives(List<Objective> objectives);
        void UpdateObjectives(int itemType, int amount);
    }
}
