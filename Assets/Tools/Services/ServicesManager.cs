using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;

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
            RetrieveCachedInfo();
            string json = await GetPlayerData();
            m_userPrefs.SetUserPrefsFromRemote(json);
            await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
            AppConfig = RemoteConfigService.Instance.appConfig;
        }

        private void RetrieveCachedInfo() 
        {
            if (!PlayerPrefs.HasKey(PLAYER_DATA)) 
            {
                
            }
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
    }

    public interface IServicesManager 
    {
        RuntimeConfig AppConfig { get; }
        UniTask InitServices();
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
