using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PopBlast.UI
{
    /// <summary>
    /// Controls the view of the level
    /// </summary>
    public class UIController : MonoBehaviour
    {   
        [SerializeField] private TextMeshProUGUI m_moveTxt;
        [SerializeField] private Button restartBtn = null;
        [SerializeField] private Button quitBtn = null;
        [SerializeField] private Text scoreTxt = null;
        [SerializeField] private Text hiScoreTxt = null;
        [SerializeField] private Text feedbackTxt = null;

        [Header("Feedback")]
        [Space()]
        [SerializeField] private float feedbackTimer = 1f;
        [Tooltip("For each key value, provide an amount and a message. Amount is how many item destroyed in one tap")]
        [SerializeField] private FeedbackKeyValue[] keyValues;

        /// <summary>
        /// Event triggered when a new game is started
        /// </summary>
        public event Action RaiseNewGame;

        protected virtual void Awake()
        {
            restartBtn.onClick.AddListener(() =>
            {
                RaiseNewGame?.Invoke();
            });
            quitBtn.onClick.AddListener(()=> 
            {
                SceneManager.LoadScene(0);
            });
            SetRestartPanel(false);
            UpdateScore(0.ToString());
            SetFeedbackPanelOff();
        }

        /// <summary>
        /// Set the restart panel active
        /// </summary>
        /// <param name="active"></param>
        public void SetRestartPanel(bool active)
        {
            restartBtn.gameObject.SetActive(active);
        }

        /// <summary>
        /// Update the user score
        /// </summary>
        /// <param name="score">Value to be displayed</param>
        public void UpdateScore(string score)
        {
            if (string.IsNullOrEmpty(score))
            {
                return;
            }
            scoreTxt.text = $"Pts : {score}";
        }

        /// <summary>
        /// Update hi score with new score
        /// </summary>
        /// <param name="score"></param>
        public void UpdateHiScore(string score)
        {
            if (string.IsNullOrEmpty(score))
            {
                return;
            }
            hiScoreTxt.text = $"1st : {score}";
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


        #region DATA_TYPES
        
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
        #endregion
    }
}
