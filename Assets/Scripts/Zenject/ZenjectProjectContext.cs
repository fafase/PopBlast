using Tools;
using UnityEngine;
using Zenject;

public class ZenjectProjectContext : MonoInstaller
{
    [SerializeField] private GameObject m_popupManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<CloudOperation>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PopupManager>().FromComponentInNewPrefab(m_popupManager).AsSingle().NonLazy();
        Container.BindInterfacesTo<UserPrefs>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<ServicesManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<LevelManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PlayerData>().FromNew().AsSingle().NonLazy();

    }
}