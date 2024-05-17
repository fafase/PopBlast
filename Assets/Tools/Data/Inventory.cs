using System;
using Zenject;

namespace Tools
{
    [Serializable]
    public class Inventory : IInventory, ICloneable
    {
        private int coins;
        private int lives;

        public int lifeReload;
        public int maxLives;

        public int Coins 
        {
            get => coins;
            set => coins = value;
        }
        public int Lives 
        {
            get => lives;
            set => lives = value;
        }

        public void SetWithConfig(InventoryConfig config) 
        {
            coins = config.coins;
            lives = config.lives;
            lifeReload = config.lifeReload;
            maxLives = config.maxLives; 
        }

        public object Clone()
        {
            return new Inventory
            {
                coins = coins,
                lives = lives
            };
        }
    }
    public interface IInventory 
    {
        int Coins { get; }
        int Lives { get; }

        void SetWithConfig(InventoryConfig config);
    }
}
