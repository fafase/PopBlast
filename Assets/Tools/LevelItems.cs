using Codice.Client.BaseCommands.Merge.Xml;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    [CreateAssetMenu(fileName = "Level Items", menuName = "Game/LevelItems")]
    public class LevelItems : ScriptableObject, ILevelItems
    {
        [Tooltip("Items prefabs")]
        [SerializeField] private List<GameObject> m_items = null;

        public GameObject GetRandomCoreItems(int itemAmout) => m_items[UnityEngine.Random.Range(0, itemAmout)];

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

    /// <summary>
    /// Item object interface
    /// </summary>
    public interface IItem
    {
        int Row { get; }
        int Column { get; }
        IItem[] GetSameTypeNeighbours();
        void StartMovement(Action onCompletion);
        void SetNeighbors(IItem newleft, IItem newRight, IItem newTop, IItem newBottom);
        void SetGrid(int newCol, int newRow, IItemGenerator item);
        GameObject GameObject { get; }
        void DestroyItem();
        Sprite GetSprite();

        int ItemType { get; }
    }

    public interface IItemGenerator
    {
        Transform Target { get; }
        event Action<int> RaiseItemPop;

        int CheckItemNeighbours(GameObject obj, Action value);
        void Init(Level currentLevel, Action value);
    }
}
