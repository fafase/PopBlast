using PopBlast.InputSystem;
using PopBlast.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PopBlast.AppControl.Level
{
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

        private void Awake()
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
        // Start is called before the first frame update
        void Start()
        {
            input.enabled = false;
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

        private void UiCtrl_RaiseNewGame()
        {
            TopScore.SetHiScore(score);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Input_RaiseItemTapped(GameObject obj)
        {
            input.enabled = false;
            generator.CheckItemNeighbours(obj, () =>
            {
                input.enabled = true;
            });
        }

        private void Generator_RaiseEndOfGame()
        {
            uiCtrl.SetRestartPanel(true);
        }

        private void UpdateScore(int amount)
        {
            if (amount <= 0) { return; }
            uiCtrl.SetFeedback(amount);
            score += Mathf.FloorToInt(Mathf.Pow(2, amount));
            uiCtrl.UpdateScore(score.ToString());
            if (TopScore.SetHiScore(score))
            {
                uiCtrl.UpdateHiScore(TopScore.GetHiScore().ToString());
            }         
        }

        #endregion
    }
}
