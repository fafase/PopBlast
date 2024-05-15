using System;
using Zenject;

namespace Tools
{
    [Serializable]
    public class Inventory : IInventory, ICloneable
    {
        private int coins;
        private int lives;

        public int Coins => coins;
        public int Lives => lives;

        public void SetWithConfig(InventoryConfig config) 
        {
            coins = config.coins;
            lives = config.lives;
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
