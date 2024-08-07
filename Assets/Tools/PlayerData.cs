using System;
using Zenject;

namespace Tools 
{
    [Serializable]
    public class PlayerData : IPlayerData, IInitializable
    {
        [Inject] private IUserPrefs m_userPrefs;
        [Inject] private IInventory m_inventory;
        [Inject] private IServicesManager m_servicesManager;

        public int CurrentLevel { get; private set; }

        private const string CURRENT_LEVEL = "currentLevel";
        private const string INVENTORY = "inventory";
        public IInventory Inventory => m_inventory;
        public IUserPrefs UserPrefs => m_userPrefs;

        public void Initialize()
        {
            Signal.Connect<LoginSignalData>(OnLoginSignal);
            //Signal.Connect<LevelCompleteSignal>(UpdateOnLevelComplete);
        }

        private void OnLoginSignal(LoginSignalData data) 
        {
            if (!m_userPrefs.TryGetInt(CURRENT_LEVEL, out int currentLevel, 1))
            {
                m_userPrefs.SetValue(CURRENT_LEVEL, 1);
            }
            CurrentLevel = currentLevel;
            if (!m_userPrefs.TryGetObject<Inventory>(INVENTORY, out Inventory inventory))
            {
                GameConfig config = m_servicesManager.GetConfig<GameConfig>();
                m_inventory.SetWithConfig(config.startInventory);
                m_userPrefs.SetValue(INVENTORY, m_inventory);
            }
            else
            {
                m_inventory = (Inventory)inventory.Clone();
            }
        }
        public void IncreaseCurrentLevel()
        {
            ++CurrentLevel;
            m_userPrefs.SetValue(CURRENT_LEVEL, CurrentLevel);
        }

        //public void UpdateOnLevelComplete(LevelCompleteSignal data) 
        //{
        //    //GameConfig config = m_servicesManager.GetConfig<GameConfig>();
        //    //int coins = config.levelRewards[data.difficulty];
        //    //m_inventory.AddCoins(coins);
        //}
    }

    public interface IPlayerData
    {
        int CurrentLevel { get; }
        void IncreaseCurrentLevel();
        IInventory Inventory { get; }
        IUserPrefs UserPrefs { get; }
    }
}
