using System;
using System.Collections.Generic;

namespace Tools
{
    [Serializable]
    public abstract class Config 
    {
       
    }
    [Serializable]
    public class GameConfig : Config
    {
        public const string CONFIG = "gameConfig";
        public string version;
        public int endContent;
        public InventoryConfig startInventory;
    }

    [Serializable]
    public class InventoryConfig
    {
        public int coins;
        public int lives;
        public int lifeReload;
        public int maxLives;
    }

    [Serializable]
    public class NameConfig : Config
    {
        public const string CONFIG = "nameConfig";
        public string title { get; set; }
        public string language { get; set; }
        public List<string> playerNames { get; set; }
        public List<string> forbidden { get; set; }

        public string GetRandomName()
        {
            int index = UnityEngine.Random.Range(0, playerNames.Count);
            return playerNames[index];
        }
    }
}
