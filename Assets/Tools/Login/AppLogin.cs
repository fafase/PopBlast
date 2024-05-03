using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Tools
{
    public class AppLogin : MonoBehaviour
    {
        async void Start()
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
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Signal.Send(new LoginSignalData(UnityServices.State));
                //UpdateUI();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
                //m_ExceptionText.text = $"{ex.GetType().Name}: {ex.Message}";
            }

            //Signal.Send(new LoginSignalData(UnityServices.State));
        }
    }

    public class LoginSignalData :SignalData
    {
        public ServicesInitializationState State { get; }
        public LoginSignalData(ServicesInitializationState state)
        {
            State = state;
        }
    }
}
