using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class CoinManager : ICoinManager, IInitializable, IDisposable
    {
        [Inject] private IServicesManager m_servicesManager;
        [Inject] private IUserPrefs m_userPrefs;

        private int m_coins;
        public int Coins => m_coins;

        private const string COIN_STORAGE = "coinStorage";

#if DEBUG
        private static CoinManager s_instance;
#endif
        public void Initialize()
        {
            Signal.Connect<LoginSignalData>(OnLoginSignal);
            Signal.Connect<LevelCompleteSignal>(OnLevelComplete);
#if DEBUG
            s_instance = this;
#endif
        }

        private void OnLevelComplete(LevelCompleteSignal data)
        {
            int lvlDiff = data.difficulty;
            int coinReward = m_servicesManager.GetConfig<GameConfig>().levelReward[lvlDiff];
            AddCoins(coinReward);
        }

        private void OnLoginSignal(LoginSignalData data)
        {          
            if (!m_userPrefs.TryGetInt(COIN_STORAGE, out m_coins))
            {
                InventoryConfig config = m_servicesManager.GetConfig<GameConfig>().startInventory;
                m_userPrefs.SetValue(COIN_STORAGE, config.coins);
            }
        }
        public void Dispose()
        {
          
        }

        public void AddCoins(int amount) 
        {
            m_coins += amount;
            m_userPrefs.SetValue(COIN_STORAGE, m_coins);
        }
    }
    public interface ICoinManager 
    {
        void AddCoins(int amount);
        int Coins { get; }
    }
}
