using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.RemoteConfig;

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Tools
{
    public class AppLogin : MonoBehaviour
    {
        private const string PlayerData = "playerData";
        void Start()
        {
            InitAsync().Forget();
        }

        private async UniTask InitAsync() 
        {
            //UnityServices.Initialize() will initialize all services that are subscribed to Core
            await UnityServices.InitializeAsync();

            Debug.Log($"Unity services initialization: {UnityServices.State}");

            //Shows if a cached session token exist
            Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

            // Shows Current profile
            Debug.Log(AuthenticationService.Instance.Profile);

            AuthenticationService.Instance.SignedIn += () =>
            {
                //Shows how to get a playerID
                Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");

                //Shows how to get an access token
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

                const string successMessage = "Sign in anonymously succeeded!";
                Debug.Log(successMessage);
            };

            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("Signed Out!");
            };
            //You can listen to events to display custom messages
            AuthenticationService.Instance.SignInFailed += errorResponse =>
            {
                Debug.LogError($"Sign in anonymously failed with error code: {errorResponse.ErrorCode}");
            };
            RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                // TODO => move to service manager
                IPlayerData playerData = await GetPlayerData();
 
                await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());

                Signal.Send(new LoginSignalData(UnityServices.State, RemoteConfigService.Instance.appConfig, playerData));
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
            }
        } 


        private async UniTask<IPlayerData> GetPlayerData() 
        {
            Dictionary<string, Unity.Services.CloudSave.Models.Item> playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { PlayerData });
            if(playerData.Count == 0) 
            {
                PlayerData newData = new PlayerData();
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> 
                {
                    { PlayerData, newData }
                });
                return newData;
            }
            else 
            {
                if (playerData.TryGetValue(PlayerData, out var item))
                {
                    return item.Value.GetAs<PlayerData>();
                }
            }

            return null;
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
        private void ApplyRemoteConfig(ConfigResponse response)
        {
            switch (response.requestOrigin)
            {
                case ConfigOrigin.Default:
                    Debug.Log("No settings loaded this session and no local cache file exists; using default values.");
                    break;
                case ConfigOrigin.Cached:
                    Debug.Log("No settings loaded this session; using cached values from a previous session.");
                    break;
                case ConfigOrigin.Remote:
                    Debug.Log("New settings loaded this session; update values accordingly.");
                    break;
            }
        }
    }

    public class LoginSignalData :SignalData
    {
        public ServicesInitializationState State { get; }
        public RuntimeConfig AppConfig { get; }

        public IPlayerData PlayerData { get; }

        public LoginSignalData(ServicesInitializationState state, RuntimeConfig appConfig, IPlayerData playerData)
        {
            State = state;
            AppConfig = appConfig;
            PlayerData = playerData;    
        }
    }
}
