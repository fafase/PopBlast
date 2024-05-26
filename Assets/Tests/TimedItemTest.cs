using NSubstitute;
using NUnit.Framework;
using System;
using Tools;
using Unity.Services.Core;
using Zenject;

public class TimedItemTest
{
    private ITimedItem m_timedItem;
    private IInitializable m_init;
    private InventoryConfig m_config;
    private IUserPrefs m_userPrefs;
    private TimedItemStorage m_storage;
    private IServicesManager m_servicesManager;
    private string m_storageKey;

   [OneTimeSetUp]
    public void OneTimeSetUp() 
    {
        m_timedItem = new TimedItem<GenericTypeHelper>();
        m_storageKey = m_timedItem.GetType().ToString();
        m_init = m_timedItem as IInitializable;
        m_storage = new TimedItemStorage(0, new DateTime(), default(DateTime));
        m_userPrefs = Substitute.For<IUserPrefs>();
        m_userPrefs.TryGetObject(m_storageKey, out Arg.Any<TimedItemStorage>()).Returns(x => {
            x[1] = m_storage;
            return true;
        });     
        m_servicesManager = Substitute.For<IServicesManager>();
        m_timedItem.SetDependencies(m_userPrefs, m_servicesManager);

        m_config = new InventoryConfig();
        m_config.lifeReload = 10;
        m_config.maxLives = 5;
        GameConfig gameConfig = new GameConfig();
        gameConfig.startInventory = m_config;
        m_servicesManager.GetConfig<GameConfig>().Returns(gameConfig);
        m_init.Initialize();
    }

    [Test]
    public void LifeManagerTestInitializationFullPass()
    {
        m_storage = new TimedItemStorage(5, new DateTime(2020, 1, 1, 15, 15, 15), default(DateTime));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        Assert.AreEqual(m_timedItem.MaxAmount, m_config.maxLives);
        Assert.AreEqual(m_timedItem.RefillTime, m_config.lifeReload);
        Assert.AreEqual(m_timedItem.Amount, m_storage.amount);
    }

    [Test]
    public void LifeManagerTestInitializationRefillAllPass()
    {
        m_storage = new TimedItemStorage(3, DateTime.Now.Subtract(new TimeSpan(0, 5, 0)), default(DateTime));
        m_storage = new TimedItemStorage(5, new DateTime(2020, 1, 1, 15, 15, 15), default(DateTime));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        Assert.AreEqual(m_config.maxLives, m_timedItem.MaxAmount);
        Assert.AreEqual(m_config.lifeReload, m_timedItem.RefillTime);
        Assert.AreEqual(5, m_timedItem.Amount);
    }

    public class TestData
    {
        public int amount;
        public DateTime next;
        public int expectedResult;
    }

    private static TestData[] _testData = new[]
    {
        new TestData() { amount = 0, next = DateTime.Now.Subtract(new TimeSpan(0, 5, 0)), expectedResult = 5 },
        new TestData() { amount = 0, next = DateTime.Now.Add(new TimeSpan(0, 5, 0)), expectedResult = 4 },
        new TestData() { amount = 0, next = DateTime.Now.Add(new TimeSpan(0, 15, 0)), expectedResult = 3 },
        new TestData() { amount = 1, next = DateTime.Now.Add(new TimeSpan(0, 15, 0)), expectedResult = 3 },
        new TestData() { amount = 2, next = DateTime.Now.Add(new TimeSpan(0, 5, 0)), expectedResult = 4 },
        new TestData() { amount = 2, next = DateTime.Now.Add(new TimeSpan(0, 25, 0)), expectedResult = 2 },
    };

    [Test]
    public void LifeManagerTestInitializationNotRefillPass([ValueSource("_testData")] TestData data)
    {
        m_storage = new TimedItemStorage(data.amount, data.next, default(DateTime));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        Assert.AreEqual(m_config.maxLives, m_timedItem.MaxAmount);
        Assert.AreEqual(m_config.lifeReload, m_timedItem.RefillTime);
        Assert.AreEqual(data.expectedResult, m_timedItem.Amount);
    }

    [TestCase(5, 4, true)]
    [TestCase(1, 0, true)]
    [TestCase(0, 0, false)]
    public void LifeManagerTestUseLifePass(int initialAmount, int expectedAmount, bool expectedResult)
    {
        // Set refill time based on missing lives and reducing so that it mocks past action
        int refill = m_config.lifeReload * (m_config.maxLives - initialAmount) - 2;
        DateTime dateTime = (initialAmount == 5) ? DateTime.Now.Subtract(new TimeSpan(0, 20, 0)) : DateTime.Now.AddMinutes(refill);
        m_storage = new TimedItemStorage(initialAmount, dateTime, default(DateTime));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        bool result = m_timedItem.UseItem();

        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(expectedAmount, m_timedItem.Amount);
    }

    [Test]
    public void LifeManagerTestHasUnlimitedStoragePass()
    {
        m_storage = new TimedItemStorage(5, default(DateTime), DateTime.Now.AddMinutes(10));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        Assert.IsTrue(m_timedItem.HasUnlimitedItems);
    }

    [TestCase(10, true)]
    [TestCase(-10, false)]
    public void LifeManagerTestHasUnlimitedAddedPass(int amount, bool expected)
    {
        m_storage = new TimedItemStorage(5, default(DateTime), default(DateTime));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        m_timedItem.AddUnlimited(amount);
        Assert.AreEqual(expected, m_timedItem.HasUnlimitedItems);
    }

    [Test]
    public void LifeManagerTestRefillAlldPass()
    {
        int initialAmount = 1;
        int refill = m_config.lifeReload * (m_config.maxLives - initialAmount) - 2;
        DateTime dateTime = DateTime.Now.AddMinutes(refill);
        m_storage = new TimedItemStorage(initialAmount, dateTime, default(DateTime));
        Signal.Send(new LoginSignalData(ServicesInitializationState.Initialized));

        bool result = m_timedItem.UseItem();
        m_timedItem.RefillAllItems();
        Assert.IsTrue(result);
        Assert.AreEqual(5, m_timedItem.Amount);
    }

    public class GenericTypeHelper : TimedItem<GenericTypeHelper> { }
}
