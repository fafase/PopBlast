using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Level Objectives", menuName = "Game/Level Objectives")]
public class LevelObjective : ScriptableObject, ILevelObjective, IInitializable, IDisposable
{
    [Inject] ILevelManager m_levelManager;

    private bool m_isDisposed = false;
    private List<Objective> m_objectives;

    public virtual void Initialize()
    {
        m_objectives = new List<Objective>();
        Level lvl = m_levelManager.CurrentLevel;
        foreach(Objective objective in lvl.Objectives)
        {
            m_objectives.Add((Objective)objective.Clone());
        }
    }
    public virtual void Dispose()
    {
        if (m_isDisposed) { return; }
        m_isDisposed = true;
    }

    public virtual void UpdateObjectives(ItemType itemType, int amount) 
    {
        Objective objective = m_objectives.Where((objective) => objective.itemType == itemType).First();
        if(objective == null) 
        {
            return;
        }
        bool objectiveDone = objective.UpdateObjective(amount);
    }
}
