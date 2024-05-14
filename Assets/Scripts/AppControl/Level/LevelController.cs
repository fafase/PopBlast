using PopBlast.InputSystem;
using PopBlast.UI;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PopBlast.AppControl
{
    /// <summary>
    /// Class connecting all items in the level
    /// </summary>
    public class LevelController : MonoBehaviour
    {
        [Inject] private ILevelManager m_levelManager;

        private ItemGenerator generator = null;
        private InputController input = null;
        private UIController uiCtrl = null;

        private int m_score = 0;
        private int m_moves = 0;

        #region UNITY_LIFECYCLE

        protected virtual void Awake()
        {
            generator = FindObjectOfType<ItemGenerator>();
            if (generator == null)
            {
                throw new System.Exception("Missing ItemGenerator component");
            }
            input = FindObjectOfType<InputController>();
            if (input == null)
            {
                throw new System.Exception("Missing InputController component");
            }
            uiCtrl = FindObjectOfType<UIController>();
            if (uiCtrl == null)
            {
                throw new System.Exception("Missing UIController component");
            }
            m_moves = m_levelManager.CurrentLevel.moves;
            uiCtrl.SetMoveCount(m_moves);
        }

        protected virtual void Start()
        {
            input.enabled = false;

            Tools.Level currentLevel = m_levelManager.CurrentLevel;
            generator.Init(currentLevel, () =>
            {
                input.enabled = true;
            });
            generator.RaiseEndOfGame += RaiseEndOfGame;
            generator.RaiseItemPop += UpdateScore;

            uiCtrl.RaiseNewGame += UiCtrl_RaiseNewGame;
            uiCtrl.UpdateHiScore(TopScore.GetHiScore().ToString());

            input.RaiseItemTapped += Input_RaiseItemTapped;
        }

        #endregion

        // Event call when new game is required
        // Loads a new scene
        private void UiCtrl_RaiseNewGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Event triggered when Input reports a tap on an item
        // Forward the game object to the generator
        // Set input on/off
        private void Input_RaiseItemTapped(GameObject obj)
        {
            input.enabled = false;
            generator.CheckItemNeighbours(obj, () =>
            {
                input.enabled = true;
            });
            --m_moves;
            uiCtrl.SetMoveCount(m_moves);
            if(m_moves <= 0) 
            {
                RaiseEndOfGame();
            }
        }

        // Event triggered when Generator reports no more possible move
        private void RaiseEndOfGame()
        {
            uiCtrl.SetRestartPanel(true);
        }

        // Update score with new amount
        private void UpdateScore(int amount)
        {
            // If only one item tapped, nothing
            if (amount <= 0) { return; }
            // Based on amount, set feedback to user
            uiCtrl.SetFeedback(amount);
            // Exponential increase based on power of two
            m_score += Mathf.FloorToInt(Mathf.Pow(2, amount));
            // Update the visual of the score
            uiCtrl.UpdateScore(m_score.ToString());
            // Update the hi score if higher
            if (TopScore.SetHiScore(m_score))
            {
                // Update the visual of hiscore if needed
                uiCtrl.UpdateHiScore(TopScore.GetHiScore().ToString());
            }         
        }
    }
}
