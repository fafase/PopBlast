using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level Items", menuName = "Game/LevelItems")]
public class LevelItems : ScriptableObject, ILevelItems
{
    [Tooltip("Items prefabs")]
    [SerializeField] private List<GameObject> m_items = null;

    public GameObject GetRandomCoreItems(int itemAmout) => m_items[Random.Range(0, itemAmout)];
}

public interface ILevelItems 
{
    GameObject GetRandomCoreItems(int itemAmout);
}
