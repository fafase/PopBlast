using System;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class TimedItem<T> : ITimedItem, IInitializable, IDisposable, ITickable where T : TimedItem<T>
    {
        [Inject] protected IServicesManager m_servicesManager;
        [Inject] protected IUserPrefs m_userPrefs;

        protected string m_storageKey;
        protected string StorageKey
        {
            get
            {
                if (string.IsNullOrEmpty(m_storageKey))
                {
                    m_storageKey = GetType().ToString();
                }
                return m_storageKey;
            }
        }
        protected int m_maxAmount;
        protected int m_refillTime;

        protected int m_amount;
        protected DateTime m_nextLife;
        protected DateTime m_unlimited;
        public int MaxAmount => m_maxAmount;
        public int RefillTime => m_refillTime;
        public int Amount => m_amount;
        public DateTime NextLife => m_nextLife;
        protected bool m_disposed;
        public event Action OnValueChange;

#if DEBUG
        protected static T s_instance;
#endif

        public virtual void Initialize()
        {
            Signal.Connect<LoginSignalData>(OnLoginSignal);
#if DEBUG
            s_instance = this as T;
#endif
        }

        private void OnLoginSignal(LoginSignalData data)
        {
            InventoryConfig config = m_servicesManager.GetConfig<GameConfig>().startInventory;
            m_maxAmount = config.maxLives;
            m_refillTime = config.lifeReload;
            if (m_userPrefs.TryGetObject<TimedItemStorage>(StorageKey, out TimedItemStorage storage))
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
        public void Tick()
        {
            if (!HasAllItems && m_nextLife < DateTime.Now)
            {
                AddItem(1)
; m_nextLife = DateTime.Now.AddMinutes(m_refillTime);
                OnValueChange?.Invoke();
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
            Signal.Disconnect<LoginSignalData>(OnLoginSignal);
        }

        public bool UseItem()
        {
            if (m_amount == 0) { return false; }
            --m_amount;
            if (DateTime.Compare(m_nextLife, DateTime.Now) < 0)
            {
                SetNextItemTimer();
            }
            SaveData();
            return true;
        }


        private void SetNextItemTimer() => m_nextLife = DateTime.Now.AddMinutes(m_refillTime);
        public void AddItem(int amount, bool allowOver = false)
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
            if (minutes < 0)
            {
                return;
            }
            if (DateTime.Compare(m_unlimited, DateTime.Now) < 0)
            {
                m_unlimited = DateTime.Now;
            }
            m_unlimited = m_unlimited.AddMinutes(minutes);
            SaveData();
        }

        public void RefillAllItems()
        {
            m_amount = m_maxAmount;
            SaveData();
        }

        public bool HasUnlimitedItems => DateTime.Compare(m_unlimited, DateTime.Now) > 0;

        public bool HasItem => m_amount > 0;
        public bool HasAllItems => m_amount == m_maxAmount;
        public void SaveData()
        {
            DateTime nextFull = DateTime.Now;
            if (m_amount < m_maxAmount)
            {
                TimeSpan ts = m_nextLife.Subtract(DateTime.Now);
                DateTime dt = DateTime.Now.Add(ts);
                int lifeDiff = m_maxAmount - m_amount - 1;
                nextFull = dt.AddMinutes(lifeDiff * m_refillTime);
            }
            m_userPrefs.SetValue(StorageKey, new TimedItemStorage(m_amount, nextFull, m_unlimited));
            OnValueChange?.Invoke();
        }


#if UNITY_INCLUDE_TESTS
        public void SetDependencies(IUserPrefs up, IServicesManager sm)
        { m_userPrefs = up; m_servicesManager = sm; }

#endif
    }

    [Serializable]
    public class TimedItemStorage
    {
        public int amount;
        public DateTime nextFull;
        public DateTime unlimited;
        public TimedItemStorage(int amount, DateTime nextFull, DateTime unlimited)
        {
            this.amount = amount;
            this.nextFull = nextFull;
            this.unlimited = unlimited;
        }
    }
    public interface ITimedItem
    {
        int Amount { get; }
        bool UseItem();
        event Action OnValueChange;
        void AddItem(int amount, bool allowOver = false);
        void AddUnlimited(int minutes);
        void RefillAllItems();
        bool HasUnlimitedItems { get; }
        bool HasItem { get; }
        bool HasAllItems { get; }
        void SaveData();
        int MaxAmount { get; }
        int RefillTime { get; }
        DateTime NextLife { get; }
#if UNITY_INCLUDE_TESTS
        void SetDependencies(IUserPrefs up, IServicesManager sm);
#endif
    }
}
