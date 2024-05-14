using PopBlast.Items;
using System.Collections.Generic;
using Tools;
using UnityEngine;


[CreateAssetMenu(fileName = "Level Items", menuName = "Game/LevelItems")]
public class LevelItems : ScriptableObject, ILevelItems
{
    [Tooltip("Items prefabs")]
    [SerializeField] private List<GameObject> m_items = null;

    public GameObject GetRandomCoreItems(int itemAmout) => m_items[Random.Range(0, itemAmout)];

    public Sprite GetCoreItem(int item)
    {
        item -= 1;
        return m_items[item].GetComponent<IItem>().GetSprite();
    }
}

public interface ILevelItems 
{
    Sprite GetCoreItem(int itemType);
    GameObject GetRandomCoreItems(int itemAmout);
}
