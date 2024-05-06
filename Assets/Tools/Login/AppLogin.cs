using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
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
                await m_servicesManager.InitServices();

                Signal.Send(new LoginSignalData(UnityServices.State));
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
            }
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
}
