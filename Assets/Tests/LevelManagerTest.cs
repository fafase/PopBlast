using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using NUnit.Framework;
using Tools;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

public class LevelManagerTest
{
    private ILevelManager m_levelManager;

    [OneTimeSetUp]
    public void OneTimeSetUp() 
    {
        m_levelManager = new LevelManager();
        string target = $"{Application.persistentDataPath}/Levels/";
        if (Directory.Exists(target))
        {
            Directory.Delete(target, true);
        }
    }


    [UnityTest]
    public IEnumerator LevelManagerLoadLocalBundlePass()
    {
        bool isRunning = true;
        m_levelManager.OnInitDone += () => isRunning = false;

        IInitializable init = (IInitializable)m_levelManager;
        init.Initialize();
        while (isRunning) 
        {
            yield return null;
        }
        Assert.AreEqual(10, m_levelManager.Levels.Count);
        Assert.AreEqual(1, m_levelManager.Levels[0].level);

    }
}
