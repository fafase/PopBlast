using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using Zenject;

public class LevelObjective : ILevelObjective, IInitializable, IDisposable
{
    [Inject] ILevelManager m_levelManager;

    private bool m_isDisposed = false;
    private List<Objective> m_objectives;

    public List<Objective> Objectives => m_objectives;

    public bool IsLevelDone 
    {
        get 
        {
            return m_objectives.FindIndex((obj) => obj.amount > 0) < 0;       
        }
    }

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

    public virtual Objective UpdateObjectives(int itemType, int amount)
    {
        Objective objective = m_objectives.Find((obj) => (int)obj.itemType == itemType);
        if (objective != null) 
        {
            switch (objective.objectiveType) 
            {
                case ObjectiveActionType.Collect:
                    objective.UpdateObjective(amount);
                    break;
                case ObjectiveActionType.Chain:
                    if(amount >= 5) 
                    { 
                        objective.UpdateObjective(1);
                    }
                    break;
            }
        }
        return objective;
    }
}
