using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.RemoteConfig;
using Zenject;

namespace Tools
{
    public class ServicesManager : IServicesManager, IInitializable
    {
        [Inject] private IUserPrefs m_userPrefs;

        public RuntimeConfig AppConfig { get; private set; }
       // public IPlayerData PlayerData {  get; private set; }

       // public void SetPlayerName(string name) => SetPlayerNameAsync(name).Forget();

        private const string PLAYER_DATA = "playerData";
        

        private async UniTask OnUserPrefsUpdate() 
        {
            if (!m_userPrefs.IsDirty) 
            {
                return;
            }
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                { PLAYER_DATA, m_userPrefs.Json }
            });
            m_userPrefs.IsDirty = false;
        }

        //public async UniTask SetPlayerNameAsync(string name)
        //{
        //    PlayerData.DisplayName = name;
        //    await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
        //    {
        //        { PLAYER_DATA, PlayerData }
        //    });
        //}

        public async UniTask InitServices() 
        {
            //PlayerData = await GetPlayerData();
            string json = await GetPlayerData();
            m_userPrefs.SetUserPrefsFromRemote(json);
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            AppConfig = RemoteConfigService.Instance.appConfig;
        }

        private async UniTask<string> GetPlayerData()
        {
            Dictionary<string, Unity.Services.CloudSave.Models.Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PLAYER_DATA });
            if (playerData.Count == 0)
            {
                //PlayerData newData = new PlayerData();
                string json = m_userPrefs.Json;
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    { PLAYER_DATA, json }
                });
                return json;
            }
            else
            {
                if (playerData.TryGetValue(PLAYER_DATA, out var item))
                {
                    return item.Value.GetAsString();
                }
            }
            return null;
        }

        public void Initialize()
        {
            m_userPrefs.OnUpdate += OnUserPrefsUpdate;
        }
    }

    public interface IServicesManager 
    {
        RuntimeConfig AppConfig { get; }
        UniTask InitServices();
        //IPlayerData PlayerData { get; }

        //void SetPlayerName(string name);
    }

    public struct userAttributes
    {
        // Optionally declare variables for any custom user attributes:
        public bool expansionFlag;
    }

    public struct appAttributes
    {
        // Optionally declare variables for any custom app attributes:
        public int level;
        public int score;
        public string appVersion;
    }
}
