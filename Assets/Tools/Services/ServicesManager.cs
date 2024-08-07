using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;
using static Tools.NameGenerationInit;

namespace Tools
{
    public class ServicesManager : IServicesManager, IInitializable
    {
        [Inject] private IUserPrefs m_userPrefs;

        public RuntimeConfig AppConfig { get; private set; }

        private const string PLAYER_DATA = "playerData";

        public void Initialize()
        {

        }

        public async UniTask InitServices() 
        {
            //UnityServices.Initialize() will initialize all services that are subscribed to Core
            await UnityServices.InitializeAsync();
            Debug.Log($"Unity services initialization: {UnityServices.State}");
            await SetUserPrefsWithRemote();
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            AppConfig = RemoteConfigService.Instance.appConfig;
            Signal.Connect<FlushOperation>(_ => SetUserPrefsWithRemote().Forget());
        }

        public async UniTask SetUserPrefsWithRemote()
        {
            string json = await GetPlayerData();
            m_userPrefs.MergeContentFromRemote(json);
        }

        private async UniTask<string> GetPlayerData()
        {
            Dictionary<string, Unity.Services.CloudSave.Models.Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PLAYER_DATA });
            if (playerData.Count == 0)
            {
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

        public T GetConfig<T>(string key) 
        {
            if(AppConfig == null) 
            {
                throw new System.Exception("[ServicesManager] AppConfig is not ready");
            }
            string json = AppConfig.GetJson(key);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"[ServicesManager] Could not find config {key}");
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
        public T GetConfig<T>() where T: Config
        {
            if (AppConfig == null)
            {
                throw new System.Exception("[ServicesManager] AppConfig is not ready");
            }
            var type = typeof(T);
            string key = type.GetField("CONFIG")?.GetValue(null)?.ToString();
            if (string.IsNullOrEmpty(key)) 
            {
                throw new System.Exception("[ServicesManager] Could not retrieve key");
            }
            string json = AppConfig.GetJson(key);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"[ServicesManager] Could not find config {key}");
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public interface IServicesManager 
    {
        RuntimeConfig AppConfig { get; }
        UniTask InitServices();
        T GetConfig<T>(string key);
        T GetConfig<T>() where T : Config;
        UniTask SetUserPrefsWithRemote();
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
