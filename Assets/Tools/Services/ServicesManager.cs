using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;

namespace Tools
{
    public class ServicesManager : MonoBehaviour, IServicesManager
    {
        public RuntimeConfig AppConfig { get; private set; }
        public IPlayerData PlayerData {  get; private set; }

        public void SetPlayerName(string name) => SetPlayerNameAsync(name).Forget();

        private const string PLAYER_DATA = "playerData";
        public async UniTask SetPlayerNameAsync(string name)
        {
            PlayerData.DisplayName = name;
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                { PLAYER_DATA, PlayerData }
            });
        }

        void Awake() 
        {
            Signal.Connect<LoginSignalData>( data =>
            {
                AppConfig = data.AppConfig;
                PlayerData = data.PlayerData;
            });
        }
    }

    public interface IServicesManager 
    {
        RuntimeConfig AppConfig { get; }
        IPlayerData PlayerData { get; }

        void SetPlayerName(string name);
    }
}
