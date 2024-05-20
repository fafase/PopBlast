using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Tools;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using Zenject;
using System;
public class CloudOperationTest
{
    private IUserPrefs m_userPrefs;
    private ICloudOperation m_cloudOperation;

    //private string m_testKey = "CO_Test_Key";
    private List<Operation> m_originalOperations;

    [OneTimeSetUp]
    public void OneTimeSetUp() 
    {
        m_userPrefs = Substitute.For<IUserPrefs>();
        m_cloudOperation = new CloudOperation(m_userPrefs);
        ((IInitializable)m_cloudOperation).Initialize();
        m_originalOperations = m_cloudOperation.Operations;

    }
    [OneTimeTearDown]
    public void OneTimeTearDown() 
    {
        m_cloudOperation.Operations.Clear();
        m_cloudOperation.Operations.AddRange(m_originalOperations);
        ((IDisposable)m_cloudOperation).Dispose();
    }


    [UnityTest]
    public IEnumerator CloudOperationTestDisposePasses()
    {

        yield return null;
    }

    [Test]
    public void CloudOperationAddOperationPasses() { }

    [UnityTest]
    public IEnumerator CloudOperationTestSendOperationsPasses()
    {

        yield return null;
    }
}
