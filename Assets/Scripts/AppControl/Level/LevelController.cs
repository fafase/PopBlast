using PopBlast.InputSystem;
using PopBlast.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopBlast.AppControl.Level
{
    /// <summary>
    /// Class connecting all items in the level
    /// </summary>
    public class LevelController : MonoBehaviour
    {
        #region MEMBERS

        [Header("Create level values")]
        [Range(5, 20)]
        [SerializeField] private int row = 0;
        [Range(5, 20)]
        [SerializeField] private int col = 0;

        private ItemGenerator generator = null;
        private InputController input = null;
        private UIController uiCtrl = null;

        private int score = 0;

        #endregion

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
            if (input == null)
            {
                throw new System.Exception("Missing UIController component");
            }
        }

        protected virtual void Start()
        {
            input.enabled = false;

            CheckForSettings();

            FindObjectOfType<GridGenerator>().Init(col, row);
            generator.Init(col, row, () =>
            {
                input.enabled = true;
            });
            generator.RaiseEndOfGame += Generator_RaiseEndOfGame;
            generator.RaiseItemPop += UpdateScore;

            uiCtrl.RaiseNewGame += UiCtrl_RaiseNewGame;
            uiCtrl.UpdateHiScore(TopScore.GetHiScore().ToString());

            input.RaiseItemTapped += Input_RaiseItemTapped;
        }

        #endregion

        #region PRIVATE_METHODS

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
        }

        // Event triggered when Generator reports no more possible move
        private void Generator_RaiseEndOfGame()
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
            score += Mathf.FloorToInt(Mathf.Pow(2, amount));
            // Update the visual of the score
            uiCtrl.UpdateScore(score.ToString());
            // Update the hi score if higher
            if (TopScore.SetHiScore(score))
            {
                // Update the visual of hiscore if needed
                uiCtrl.UpdateHiScore(TopScore.GetHiScore().ToString());
            }         
        }

        private void CheckForSettings()
        {
            // If came from start level, values should exist
            col = PlayerPrefs.GetInt("Width", col);
            row = PlayerPrefs.GetInt("Height", row);
            // Delete values so editor can start from Game scene
            PlayerPrefs.DeleteKey("Width");
            PlayerPrefs.DeleteKey("Height");
        }

        #endregion
    }
}
