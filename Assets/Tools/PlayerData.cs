using System;
using Zenject;

namespace Tools 
{
    [Serializable]
    public class PlayerData : IPlayerData, IInitializable
    {
        [Inject] private IUserPrefs m_userPrefs;

        private int m_currentLevel;
        public int CurrentLevel => m_currentLevel;

        private const string CURRENT_LEVEL = "currentLevel";

        public void IncreaseCurrentLevel()
        {
            m_currentLevel++;
            m_userPrefs.SetValue(CURRENT_LEVEL, m_currentLevel);
        }

        public void Initialize()
        {
            if (!m_userPrefs.TryGetInt(CURRENT_LEVEL, out m_currentLevel))
            {
                m_userPrefs.SetValue(CURRENT_LEVEL, 1);
            }
        }
    }

    public interface IPlayerData
    {
        int CurrentLevel { get; }
        void IncreaseCurrentLevel();
    }
}
