using Tools;
using UnityEngine;
using Zenject;

public class ZenjectProjectContext : MonoInstaller
{
    [SerializeField] private GameObject m_popupManager;
    [SerializeField] private Localization m_localization;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<CloudOperation>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PopupManager>().FromComponentInNewPrefab(m_popupManager).AsSingle().NonLazy();
        Container.BindInterfacesTo<UserPrefs>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<ServicesManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<Localization>().FromNewScriptableObject(m_localization).AsSingle().NonLazy();
        Container.BindInterfacesTo<LevelManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerData>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<Inventory>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<LifeManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<CoinManager>().FromNew().AsSingle().NonLazy();
    }
}