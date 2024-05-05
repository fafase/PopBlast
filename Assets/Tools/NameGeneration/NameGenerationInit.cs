using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class NameGenerationInit : MonoBehaviour, IInit, INameGenerationInit
    {
        [Inject] private IServicesManager m_servicesManager;
        [Inject] private IPopupManager m_popupManager;
        public bool IsInit { get; private set; }
        private NameConfig m_config;

        public async UniTask<InitializationResult> InitAsync()
        {
            if (m_servicesManager == null || m_servicesManager.AppConfig == null)
            {
                return new InitializationResult(false, GetType().ToString());
            }
            IPlayerData playerData = m_servicesManager.PlayerData;
            if (playerData == null)
            {
                Debug.Log("[NameGenerationInit] No player data found");
                return new InitializationResult(false, GetType().ToString());
            }
            if (!string.IsNullOrEmpty(playerData.DisplayName))
            {
                return new InitializationResult(true, GetType().ToString());
            }
            string json = m_servicesManager.AppConfig.GetJson("nameGenerator");
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log("[NameGenerationInit] No name generator config found");
                return new InitializationResult(false, GetType().ToString());
            }
            m_config = JsonConvert.DeserializeObject<NameConfig>(json);
            NameGeneratorPopup popup = m_popupManager.Show<NameGeneratorPopup>() as NameGeneratorPopup;
            popup.Init(this);
            popup.AddToClose(_ => 
            {
                string name = popup.Name;
                if (!string.IsNullOrEmpty(name)) 
                {
                    m_servicesManager.SetPlayerName(name);
                }
            });
            while (popup.IsOpen) 
            {
                await UniTask.Yield();
            }
            IsInit = true;
            return new InitializationResult(true, GetType().ToString());
        }

        public string RandomName=> m_config.GetRandomName();

        [Serializable]
        public class NameConfig
        {
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
    public interface INameGenerationInit
    {
       // void SetName(string name);
        string RandomName { get; }
    }
}