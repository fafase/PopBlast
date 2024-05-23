using Cysharp.Threading.Tasks;
using PlasticGui.WebApi.Responses;
using System;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class AppLogin : MonoBehaviour
    {
        [Inject] private IServicesManager m_servicesManager;

        void Start()
        {   
            InitAsync().Forget();
        }

        private void OnAccountSignedIn()
        {
            Debug.Log("Player Account Access token " + PlayerAccountService.Instance.AccessToken);
            AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }

        private async UniTask InitAsync() 
        {           
            try
            {
                await UnityServices.InitializeAsync();
                PlayerAccountService.Instance.SignedIn += OnAccountSignedIn;

                //Shows if a cached session token exist
                Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

                // Shows Current profile
                Debug.Log(AuthenticationService.Instance.Profile);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                AuthenticationService.Instance.SignedIn += OnAuthSignedIn;
                AuthenticationService.Instance.SignedOut += OnSignedOut;
                AuthenticationService.Instance.SignInFailed += OnSignInFailed;

                await m_servicesManager.InitServices();

                Signal.Send(new LoginSignalData(UnityServices.State));
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
            }
        } 
        private void OnAuthSignedIn() 
        {
            //Shows how to get a playerID
            Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");

            //Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            const string successMessage = "Sign in anonymously succeeded!";
            Debug.Log(successMessage);
        }
        private void OnSignedOut() => Debug.Log("Signed Out!");


        private void OnSignInFailed(RequestFailedException exception) 
            => Debug.LogError($"Sign in anonymously failed with error code: {exception.ErrorCode}");

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
}
