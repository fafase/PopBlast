using System;

namespace Tools 
{
    [Serializable]
    public class PlayerData :  IPlayerData
    {
        public string DisplayName { get; set; } = "";
        public string Id { get; set; } = "";
    }

    public interface IPlayerData 
    {
        string DisplayName { get; set; }
        public string Id { get; set; }
    }
}
