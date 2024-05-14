using UnityEngine;
using Zenject;

public class CoreContext : MonoInstaller
{
    [SerializeField] private LevelItems m_levelItems;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<LevelItems>().FromNewScriptableObject(m_levelItems).AsSingle().NonLazy();
        Container.BindInterfacesTo<LevelObjective>().FromNew().AsSingle().NonLazy();
    }
}