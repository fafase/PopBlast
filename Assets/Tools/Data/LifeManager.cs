using System;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class LifeManager : IInitializable, ILifeManager, IDisposable
    {
        [Inject] private IServicesManager m_servicesManager;
        [Inject] private IUserPrefs m_userPrefs;
        
        private const string LIFE_STORAGE = "lifeStorage";
        private int m_maxAmount;
        private int m_refillTime;

        private int m_amount;
        private DateTime m_nextLife;
        private DateTime m_unlimited;
        public int MaxAmount => m_maxAmount;
        public int RefillTime => m_refillTime;  
        public int Amount => m_amount;
        public DateTime NextLife => m_nextLife;
        private bool m_disposed;

        public void Initialize()
        {
            InventoryConfig config = m_servicesManager.GetConfig<GameConfig>().startInventory;
            m_maxAmount = config.maxLives;
            m_refillTime = config.lifeReload;
            if (m_userPrefs.TryGetObject<LifeStorage>(LIFE_STORAGE, out LifeStorage storage))
            {
                m_unlimited = storage.unlimited;
                int result = DateTime.Compare(storage.nextFull, DateTime.Now);
                if (result < 0)
                {
                    // Full time for life
                    m_amount = m_maxAmount;
                }
                else
                {
                    // Timer is not over
                    TimeSpan ts = storage.nextFull - DateTime.Now;
                    int timeRemaining = (int)ts.TotalSeconds;
                    int timeForLife = (m_refillTime * 60);

                    int remainingTimeForTimer = timeRemaining % timeForLife;
                    int removeLife = (timeRemaining / timeForLife);
                    m_amount = m_maxAmount - removeLife - 1;
                    m_nextLife = DateTime.Now.AddSeconds(remainingTimeForTimer);
                }
            }
        }

        public void Dispose()
        {
            if (m_disposed) 
            {
                return;
            }
            m_disposed = true;
            SaveData(); 
        }

        public bool UseLife()
        {
            if(m_amount == 0) { return false; }
            --m_amount;
            SaveData();
            return true;
        }

        public void AddLife(int amount, bool allowOver = false)
        { 
            m_amount += amount;
            if (allowOver) 
            {
                return;
            }
            m_amount = Mathf.Clamp(m_amount, 0, m_maxAmount);
            SaveData();
        }

        public void AddUnlimited(int minutes)
        {
            if(minutes < 0) 
            {
                return;
            }
            if(DateTime.Compare(m_unlimited, DateTime.Now) < 0) 
            {
                m_unlimited = DateTime.Now;
            }
            m_unlimited = m_unlimited.AddMinutes(minutes);  
            SaveData();
        }

        public void RefillAllLives() => m_amount = m_maxAmount;

        public bool HasUnlimitedLives => DateTime.Compare(m_unlimited, DateTime.Now) > 0;

        public bool HasLife => false;
       
        public void SaveData()
        {
            DateTime nextFull = DateTime.Now;
            if (m_amount < m_maxAmount)
            {
                TimeSpan ts = m_nextLife.Subtract(DateTime.Now);
                DateTime dt = DateTime.Now.Add(ts);
                int lifeDiff = m_maxAmount - m_amount - 1;
                dt = dt.AddMinutes(lifeDiff * m_refillTime);
            }
            m_userPrefs.SetValue(LIFE_STORAGE, new LifeStorage(m_amount, nextFull, m_unlimited));
        }

#if UNITY_INCLUDE_TESTS
        public void SetDependencies(IUserPrefs up, IServicesManager sm) 
        { m_userPrefs = up; m_servicesManager = sm; }
#endif
    }
    [Serializable]
    public class LifeStorage
    {
        public int amount;
        public DateTime nextFull;
        public DateTime unlimited;
        public LifeStorage(int amount, DateTime nextFull, DateTime unlimited)
        {
            this.amount = amount;
            this.nextFull = nextFull;
            this.unlimited = unlimited;
        }
    }
    public interface ILifeManager
    {
        int Amount { get; }
        bool UseLife();
        void AddLife(int amount, bool allowOver = false);
        void AddUnlimited(int minutes);
        void RefillAllLives();
        bool HasUnlimitedLives { get; }
        bool HasLife { get; }
        void SaveData();
        int MaxAmount { get; }
        int RefillTime { get; }
#if UNITY_INCLUDE_TESTS
        void SetDependencies(IUserPrefs up, IServicesManager sm);
#endif
    }
}
