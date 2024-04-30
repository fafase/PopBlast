using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class Loading : MonoBehaviour
{
    public void OnInitStart()
    {
        Debug.Log("Start init");
    }

    public void OnInitComplete(List<InitializationResult> results)
    {
        Debug.Log("End init");
        SceneManager.LoadSceneAsync("Meta");
    }
}
