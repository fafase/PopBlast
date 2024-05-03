using Tools;
using UnityEngine;
using Zenject;

public class ZenjectProjectContext : MonoInstaller
{
    [SerializeField] private GameObject m_popupManager;
    [SerializeField] private ServicesManager m_servicesManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PopupManager>().FromComponentInNewPrefab(m_popupManager).AsSingle().NonLazy();
        Container.BindInterfacesTo<ServicesManager>().FromComponentInNewPrefab(m_servicesManager).AsSingle().NonLazy();
    }
}