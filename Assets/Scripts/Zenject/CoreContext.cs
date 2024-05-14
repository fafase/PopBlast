using PopBlast.AppControl;
using PopBlast.UI;
using UnityEngine;
using Zenject;

public class CoreContext : MonoInstaller
{
    [SerializeField] private LevelItems m_levelItems;
    [SerializeField] private UIController m_uiCtrl;
    [SerializeField] private ItemGenerator m_itemGenerator;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<LevelItems>().FromNewScriptableObject(m_levelItems).AsSingle().NonLazy();
        Container.BindInterfacesTo<LevelObjective>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<UIController>().FromInstance(m_uiCtrl);
        Container.BindInterfacesTo<ItemGenerator>().FromInstance(m_itemGenerator);
    }
}