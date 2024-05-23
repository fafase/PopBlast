using Cysharp.Threading.Tasks;
using PopBlast.InputSystem;
using PopBlast.Items;
using PopBlast.UI;
using Tools;
using UnityEditor;
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
        [Inject] private ILifeManager m_lifeManager;

        private InputController input = null;

        private int m_score = 0;
        private int m_moves = 0;

        private LevelState m_state;

        protected virtual void Awake()
        {
#if DEBUG
            s_instance = this;
#endif
            input = FindObjectOfType<InputController>();
            if (input == null)
            {
                throw new System.Exception("Missing InputController component");
            }
        }

        protected virtual void Start()
        {
            input.enabled = false;

            Level currentLevel = m_levelManager.CurrentLevel;
            generator.Init(currentLevel, () =>
            {
                input.enabled = true;
            });
            generator.RaiseItemPop += UpdateScore;

            m_moves = m_levelManager.CurrentLevel.moves;
            m_coreUI.SetMoveCount(m_moves);
            m_coreUI.SetObjectives(m_levelObjectives.Objectives);
            input.RaiseItemTapped += Input_RaiseItemTapped;
        }

        private void OnApplicationQuit()
        {
            if(m_state != LevelState.Win && m_state != LevelState.Start) 
            {
                m_lifeManager.UseLife();
                m_cloudOperation.FlushOperations(null);
            }
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
            Objective objective = m_levelObjectives.UpdateObjectives(item.ItemType, amount);
            if(objective != null) 
            {
                m_coreUI.UpdateObjectives(item.ItemType, objective.amount);
            }
            m_coreUI.SetMoveCount(m_moves);

            if (m_levelObjectives.IsLevelDone) 
            {
                m_state = LevelState.Win;
                input.RaiseItemTapped -= Input_RaiseItemTapped;
                IPopup popup = m_popupManager.Show<LevelCompletePopup>();
                ((LevelCompletePopup)popup).InitWithLevel(m_playerData.CurrentLevel);
                popup.AddToClose(_ => ResetToMeta());
                Signal.Send(new LevelCompleteSignal(m_levelManager.CurrentLevel.difficulty));
                return;
            }
            else if (m_moves <= 0)
            {
                m_state = LevelState.Loss;
                input.RaiseItemTapped -= Input_RaiseItemTapped;
                m_lifeManager.UseLife();
                IPopup popup = m_popupManager.Show<LossPopup>();
                ((LossPopup)popup).InitWithLevel(m_levelManager.Levels[m_playerData.CurrentLevel - 1], OnLossRestart, OnLossToMeta);
                return;
            }
            m_state = LevelState.Play;
        }

        private void ResetToMeta() 
        {
            m_playerData.IncreaseCurrentLevel();
            m_cloudOperation.FlushOperations(() =>
                SceneManager.LoadSceneAsync(1));
        }

        private void OnLossRestart() 
        {
            m_cloudOperation.FlushOperations(() =>
                 m_popupManager.LoadSceneWithLoadingPopup("GameScene").Forget());
        }

        private void OnLossToMeta() 
        {
            m_cloudOperation.FlushOperations(() =>
                m_popupManager.LoadSceneWithLoadingPopup("Meta").Forget());
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
        }

        public enum LevelState 
        {
            Start, Play, Pause, Win, Loss
        }

#if DEBUG

        private static LevelController s_instance;
#endif

        [MenuItem("Tools/Core/Win Level")]
        public static void WinLevel() { }

        [MenuItem("Tools/Core/Lose Level")]
        public static void LoseLevel() { }

        [MenuItem("Tools/Core/5 moves remaining %m")]
        public static void Set5Moves() 
        {
            s_instance.m_moves = 5;
            s_instance.m_coreUI.SetMoveCount(s_instance.m_moves);
        }
    }
}
