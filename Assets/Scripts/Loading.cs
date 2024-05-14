using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Loading : MonoBehaviour
{
    [Inject] IPopupManager m_popupManager;
    public void OnInitStart()
    {
        Debug.Log("Start init");
    }

    public void OnInitComplete(List<InitializationResult> results)
    {
        Debug.Log("End init");
        m_popupManager.LoadSceneWithLoadingPopup("Meta").Forget();
    }
}
