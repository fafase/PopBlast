using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public class LocalizationTest
{
    private ILocalization m_localization;

    [OneTimeSetUp]
    public void OneTimeSetUp() 
    {
        m_localization = ScriptableObject.CreateInstance<Localization>();
        TextAsset en = EditorUtility.LoadTextAsset("English");
        TextAsset fr = EditorUtility.LoadTextAsset("French");

        m_localization.LocalizationTextAsset = new List<TextAsset>() { en, fr};
        m_localization.DefaultTextAsset = en;
        m_localization.IsInit = true;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() 
    {
        PlayerPrefs.SetString("PPLocale", "English");
    }

    [Test]
    public void LocalizationTestMissingDefaultPass()
    {
        ILocalization localization = ScriptableObject.CreateInstance<Localization>();
        Assert.IsNotNull(localization);
        Exception e = Assert.Throws<Exception>(localization.SetDefault);
        Assert.That(e.Message, Is.EqualTo("[Localization] Missing default localization"));
    }
    
    [Test]
    public void LocalizationTestMissingKeyPass()
    {
        string missing = m_localization.GetLocalization("missingKey");   
        string defaultValue = m_localization.GetLocalization("missingKey", "Default");

        Assert.IsNull(missing);
        Assert.AreEqual("Default", defaultValue);
    }

    [Test]
    public void LocalizationTestGetLocalizationPass() 
    {
        m_localization.SetDefault();
        string name = m_localization.GetLocalization("appName");
        string version = m_localization.GetLocalization("version");
        string description = m_localization.GetLocalization("EVENTS/description");
        string settings = m_localization.GetLocalization("GENERIC/settings");

        Assert.AreEqual("GameName", name);
        Assert.AreEqual("1.0.0", version);
        Assert.AreEqual("Here is the description of the event", description);
        Assert.AreEqual("Settings", settings);
    }


    [Test]
    public void LocalizationTestRemoteLocalizationPass() 
    {
        List<TextAsset> assets = m_localization.LocalizationTextAsset;
        JObject json = new JObject();
        json["version"] = "1.0.1"; json["locale"] = "English"; json["appName"] = "GameName";
        json["GENERIC"] = new JObject 
        {   { "settings",  "Settings" }
        };
        json["EVENTS"] = new JObject
        {   { "eventName",  "Name for event" },
            { "description", "This event" },
            { "score", "Score" }
        };

        m_localization.SetLocalizationFromRemote(new List<string>() { json.ToString() });
        m_localization.SetWithLocale("English");

        string name = m_localization.GetLocalization("appName");
        string version = m_localization.GetLocalization("version");
        string description = m_localization.GetLocalization("EVENTS/description");
        string score = m_localization.GetLocalization("EVENTS/score");
        string loading = m_localization.GetLocalization("GENERIC/loading");
        
        Assert.AreEqual("GameName", name);
        Assert.AreEqual("1.0.1", version);
        Assert.AreEqual("This event", description);
        Assert.AreEqual("Score", score);
        Assert.IsNull(loading);
    }

    [Test]
    public void LocalizationTestSetDefaultPass()
    {
        m_localization.SetDefault();
        string locale = m_localization.Locale;
        Assert.AreEqual("English", locale);
    }

    [Test]
    public void LocalizationTestSwapLanguagePass() 
    {
        m_localization.SetWithLocale("French");
        string locale = m_localization.Locale;
        string loading = m_localization.GetLocalization("GENERIC/loading");
          
        Assert.AreEqual("French", locale);
        Assert.AreEqual("Chargement", loading);
    }

    [Test]
    public void LocalizationTestFormatInputPass()
    {
        m_localization.SetWithLocale("English");

        List<LocArgument> locFormats = new List<LocArgument>()
        {
            new LocArgument("playerScore", 1555.ToString()),
            new LocArgument("name", "Jeff")
        };
        string resultScore = m_localization.GetLocalization("EVENTS/score", locFormats);

        locFormats = new List<LocArgument>()
        {
            new LocArgument("test", "This is a test")
        };
        string resultTest = m_localization.GetLocalization("EVENTS/otherFormat", locFormats);

        locFormats = new List<LocArgument>()
        {
            new LocArgument("test", "")
        };
        string resultMissing = m_localization.GetLocalization("EVENTS/otherFormat", locFormats);

        Assert.AreEqual("Score 1555, well done Jeff!", resultScore);
        Assert.AreEqual("This is a test here", resultTest);
        Assert.AreEqual("{ } here", resultMissing);
    }
    [Test]
    public void LocalizationTestSwapLanguageResultPass()
    {
        m_localization.SetDefault();

        bool resultEn = m_localization.SetWithLocale("English");
        bool resultSu = m_localization.SetWithLocale("Sudanese");

        Assert.IsTrue(resultEn);
        Assert.IsFalse(resultSu);
        Assert.AreEqual("English", m_localization.Locale);
    }
}
