using System;
using Zenject;

namespace Tools 
{
    [Serializable]
    public class PlayerData : IPlayerData, IInitializable
    {
        [Inject] private IUserPrefs m_userPrefs;

        //private int m_currentLevel;
        public int CurrentLevel { get; private set; }

        private const string CURRENT_LEVEL = "currentLevel";

        public void IncreaseCurrentLevel()
        {
            ++CurrentLevel;
            m_userPrefs.SetValue(CURRENT_LEVEL, CurrentLevel);
        }

        public void Initialize()
        {
            if(!m_userPrefs.TryGetInt(CURRENT_LEVEL, out int currentLevel, 1)) 
            {
                m_userPrefs.SetValue(CURRENT_LEVEL, 1);
            }
            CurrentLevel = currentLevel;
        }
    }

    public interface IPlayerData
    {
        int CurrentLevel { get; }
        void IncreaseCurrentLevel();
    }
}
