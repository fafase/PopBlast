using PopBlast.InputSystem;
using PopBlast.Items;
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
        [Inject] private ILevelObjective m_levelObjectives;
        [Inject] private IItemGenerator generator = null;
        [Inject] private ICoreUI m_coreUI;
        [Inject] private IPopupManager m_popupManager;
        [Inject] private IPlayerData m_playerData;
        [Inject] private ICloudOperation m_cloudOperation;

        private InputController input = null;

        private int m_score = 0;
        private int m_moves = 0;

        #region UNITY_LIFECYCLE

        protected virtual void Awake()
        {
            input = FindObjectOfType<InputController>();
            if (input == null)
            {
                throw new System.Exception("Missing InputController component");
            }
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

            m_coreUI.RaiseNewGame += UiCtrl_RaiseNewGame;
            m_coreUI.UpdateHiScore(TopScore.GetHiScore().ToString());
            m_moves = m_levelManager.CurrentLevel.moves;
            m_coreUI.SetMoveCount(m_moves);
            m_coreUI.SetObjectives(m_levelObjectives.Objectives);
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
            int amount = generator.CheckItemNeighbours(obj, () =>
            {
                input.enabled = true;
            });
            --m_moves;
            IItem item = obj.GetComponent<IItem>();
            Objective objective = m_levelObjectives.UpdateObjectives((int)item.ItemType, amount);
            if(objective != null) 
            {
                m_coreUI.UpdateObjectives((int)item.ItemType, objective.amount);
            }
            m_coreUI.SetMoveCount(m_moves);

            if (m_moves <= 0) 
            {
                input.RaiseItemTapped -= Input_RaiseItemTapped;
                RaiseEndOfGame(false);           
            }
            if (m_levelObjectives.IsLevelDone) 
            {
                input.RaiseItemTapped -= Input_RaiseItemTapped;
                RaiseEndOfGame(true);
            }
        }

        // Event triggered when Generator reports no more possible move
        private void RaiseEndOfGame(bool win)
        {
            if (win) 
            {
                IPopup popup = m_popupManager.Show<LevelCompletePopup>();
                ((LevelCompletePopup)popup).InitWithLevel(m_playerData.CurrentLevel);
                popup.AddToClose(_ => ResetToMeta());
            }
            else
            {
                m_coreUI.SetRestartPanel(true);
            }
        }
        private void ResetToMeta() 
        {
            m_playerData.IncreaseCurrentLevel();
            m_cloudOperation.FlushOperations(dict => 
            SceneManager.LoadSceneAsync(1));

        }

        // Update score with new amount
        private void UpdateScore(int amount)
        {
            // If only one item tapped, nothing
            if (amount <= 0) { return; }
            // Based on amount, set feedback to user
            m_coreUI.SetFeedback(amount);
            // Exponential increase based on power of two
            m_score += Mathf.FloorToInt(Mathf.Pow(2, amount));
            // Update the visual of the score
            m_coreUI.UpdateScore(m_score.ToString());
            // Update the hi score if higher
            if (TopScore.SetHiScore(m_score))
            {
                // Update the visual of hiscore if needed
                m_coreUI.UpdateHiScore(TopScore.GetHiScore().ToString());
            }         
        }
    }
}
