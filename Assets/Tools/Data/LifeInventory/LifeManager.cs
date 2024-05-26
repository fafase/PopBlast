using System;
using UnityEditor;

namespace Tools
{
    public class LifeManager : TimedItem<LifeManager>, ILifeManager
    {
        public bool UseLife() => UseItem();

        public void AddLife(int amount, bool allowOver = false) => AddItem(amount, allowOver);

        public void RefillAllLives() => RefillAllItems();

        public bool HasUnlimitedLives => HasUnlimitedItems;
        public bool HasLife => HasItem;
        public bool HasAllLives => HasAllItems;

        public void RegisterOnLifeChange(Action listener) => OnValueChange += listener;
        public void UnregisterOnLifeChange(Action listener) => OnValueChange -= listener;

        [MenuItem("Tools/Life/Add 1 Life")]
        public static void Add1LifeMenu() => s_instance.AddLife(1);
        [MenuItem("Tools/Life/Refill lives")]
        public static void RefillLifeMenu() => s_instance.AddLife(5);
        
        [MenuItem("Tools/Life/Remove 1 Life")]
        public static void Remove1LifeMenu() => s_instance.UseLife();
    }

    public interface ILifeManager
    {
        int Amount { get; }
        bool UseLife();
        void RegisterOnLifeChange(Action listener);
        void UnregisterOnLifeChange(Action listener);
        void AddLife(int amount, bool allowOver = false);
        void AddUnlimited(int minutes);
        void RefillAllLives();
        bool HasUnlimitedLives { get; }
        bool HasLife { get; }
        bool HasAllLives { get; }
        void SaveData();
        int MaxAmount { get; }
        int RefillTime { get; }
        DateTime NextLife {  get; }
#if UNITY_INCLUDE_TESTS
        void SetDependencies(IUserPrefs up, IServicesManager sm);
#endif
    }
}
