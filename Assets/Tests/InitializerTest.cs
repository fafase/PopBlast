using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using NUnit.Framework;
using Tools;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;

public class InitializerTest
{
    private IInitializer m_initializer;
    [OneTimeSetUp]
    public void OneTimeSetUp() 
    {
        m_initializer = Substitute.For<IInitializer>();
        m_initializer.InitObjects.Returns(new List<UnityEngine.Object>()
        {
            new GameObject("Go").AddComponent<TestClassA>(),
            new GameObject("Go").AddComponent<TestClassB>()
        });
    }
    [SetUp]
    public void SetUp() 
    {
        m_initializer.IsInit = false;
    }
    [UnityTest]
    public IEnumerator InitializerTestSimplePasses()
    {        
        UniTask<List<InitializationResult>> asyncOperation = new InitializerProcess().Init(m_initializer);
        
        while (!asyncOperation.Status.IsCompleted())
        {
            yield return null;
        }
        List<InitializationResult> results = asyncOperation.GetAwaiter().GetResult();

        Assert.IsNotNull(results.Find((r) => r.Message.Equals("TestClassA")));
        Assert.IsNotNull(results.Find((r) => r.Message.Equals("TestClassB")));
    }

    [UnityTest]
    public IEnumerator InitializerTestMultiplePasses()
    {
        var asyncOperation = new InitializerProcess().Init(m_initializer);
        while (!asyncOperation.Status.IsCompleted())
        {
            yield return null;
        }
        List<InitializationResult> results = asyncOperation.GetAwaiter().GetResult();
        Assert.AreEqual(2, results.Count);
    }

    [UnityTest]
    public IEnumerator InitializerTestMultipleFailPasses()
    {
        UniTask<List<InitializationResult>> asyncOperation = new InitializerProcess().Init(m_initializer);

        while (!asyncOperation.Status.IsCompleted())
        {
            yield return null;
        }
        List<InitializationResult> results = asyncOperation.GetAwaiter().GetResult();
        InitializationResult r = results.Find((r) => !r.Success);
        Assert.IsNotNull(r);
        Assert.AreEqual("TestClassB", r.Message);
    }

    [UnityTest]
    public IEnumerator InitializerTestMultipleNoWaitPasses()
    {
        InitializerProcess process = new InitializerProcess();
        UniTask<List<InitializationResult>> asyncOperation = process.Init(m_initializer);

        while (!asyncOperation.Status.IsCompleted())
        {
            process.Cancel();
            yield return null;
        }
        // This is not working !!?? Wont pass...
        LogAssert.Expect(LogType.Exception, "[Error] [Initilization] Process was cancelled");
        List<InitializationResult> results = asyncOperation.GetAwaiter().GetResult();
        Assert.IsNull(results);
    }


    public class TestClassA : MonoBehaviour, IInit
    {
        public bool IsInit { get; private set; }

        public async UniTask<InitializationResult> InitAsync()
        {
            await Task.Delay(500);
            return  new InitializationResult(true, "TestClassA");
        }
    }
    public class TestClassB : MonoBehaviour, IInit
    {
        public bool IsInit { get; private set; }

        public async UniTask<InitializationResult> InitAsync()
        {
            await Task.Delay(500);
            return new InitializationResult(false, "TestClassB");
        }
    }
}
