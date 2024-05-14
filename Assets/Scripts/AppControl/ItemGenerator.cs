using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PopBlast.Items;
using Tools;
using Zenject;

namespace PopBlast.AppControl
{
    /// <summary>
    /// Component controlling the grid of items
    /// Main business logic of the system
    /// </summary>
    public class ItemGenerator : MonoBehaviour, IItemGenerator
    {
        [Inject] private ILevelItems m_items;
        [SerializeField] private Transform m_top;
        [SerializeField] private Transform m_bottom;

        [Tooltip("Waiting time for row creation of item")]
        [SerializeField] private float waitTimeForCreation = 0.5f;

        private Transform[] spawns = null;
        private IItem[,] grid;
        private Level m_level;
        private int width, height;

        /// <summary>
        /// Event triggered when no more moves are possible
        /// </summary>
        public event Action<bool> RaiseEndOfGame;

        /// <summary>
        /// Event raised when pop happens, parameter indicates how many items at once
        /// </summary>
        public event Action<int> RaiseItemPop;

        public Transform Top => m_top;
        public Transform Target => m_bottom;

        #region PUBLIC_METHODS

        /// <summary>
        /// Initializes the ItemGenerator with column (width) and row (height)
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="onCompletion"></param>
        public void Init(Level level, Action onCompletion)
        {
            m_level = level;
            int col = level.Column;
            int row = level.Row;
            spawns = new Transform[col];
            width = col;
            height = row;

            float offsetX = (col / 2f);
            // Generate the spawn points
            for (int i = 0; i < col; i++)
            {
                GameObject obj = new GameObject($"Spawn_{i}");
                spawns[i] = obj.transform;
                obj.transform.position = new Vector3(i + 0.5f - offsetX, m_top.position.y, 0f);
                obj.transform.parent = transform;
            }

            // Generate all items
            grid = new Item[col, row];
            for (int i = 0; i < col; i++)
            {
                Transform spawnTr = spawns[i];
                for (int j = 0; j < row; j++)
                {
                    GameObject obj = Instantiate<GameObject>(m_items.GetRandomCoreItems(m_level.items), spawnTr);
                    obj.transform.position = spawnTr.position;
                    IItem item = obj.GetComponent<IItem>();
                    item.SetGrid(i, j, this);
                    item.GameObject.SetActive(false);
                    grid[i, j] = item;
                }
            }
            SetNeighboursGrid();
            StartCoroutine(EnableItemCoroutine(onCompletion));
        }
        
        /// <summary>
        /// Checks the neighbours of the given game object 
        /// Process a tree search to find all consecutive similar neighbours
        /// Clears them all from screen and loads new ones
        /// </summary>
        /// <param name="go"></param>
        /// <param name="onCompletion"></param>
        public int CheckItemNeighbours(GameObject go, Action onCompletion)
        {
            Item item = go.GetComponent<Item>();
            if(item == null)
            {
                onCompletion?.Invoke();
                return 0;
            }
            IItem[] results = item.GetSameTypeNeighbours();
            if(results.Length == 0)
            {
                onCompletion?.Invoke();
                return 0;
            }
            int amount = ProcessItemToRemove(item);
            CheckForEmptySpaces();
            StartCoroutine(CreateNewItemsCoroutine(amount, ()=> 
            {
                SetNeighboursGrid();
                if (CheckForRemainingMovement() == false)
                {
                    RaiseEndOfGame?.Invoke(false);
                }
                onCompletion?.Invoke();
            }));
            return amount;
        }

        #endregion

        private int ProcessItemToRemove(Item item)
        {
            // Using hashset to avoid duplicate
            HashSet<IItem> toRemove = new HashSet<IItem>();
            // Add first item, the one that got tapped on
            toRemove.Add(item);
            Queue<IItem> queue = new Queue<IItem>();
            // Enqueue item for tree search
            queue.Enqueue(item);

            while (queue.Count > 0)
            {
                // Get oldest item
                IItem current = queue.Dequeue();
                // Get all neighbours with same type
                // Continue if none
                IItem[] items = current.GetSameTypeNeighbours();
                if (items.Length == 0) { continue; }

                // For each of the neighbours, add to the queue
                // Using hashset, if a neighbour is already in the list, it is not added to the queue
                // It was either already processed or already added to the list
                foreach (IItem it in items)
                {
                    if (toRemove.Add(it))
                    {
                        queue.Enqueue(it);
                    }
                }
            }
            // Forward how many items were found
            // Used for score and feedback
            RaiseItemPop?.Invoke(toRemove.Count);
            int amount = toRemove.Count;
            // Destroy item and set grid position to null
            foreach (IItem i in toRemove)
            {
                grid[i.Column, i.Row] = null;
                i.DestroyItem();
            }
            return amount;
        }

        // Check for empty space in a column
        //Iterate from bottom to top
        // When a free space is found, it searches for next item in the column and assign its new grid position
        private void CheckForEmptySpaces()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    IItem temp = grid[i, j];

                    // if empty spot
                    if (temp == null)
                    { 
                        // iterate above for item
                        for (int w = j; w < height; w++)
                        {
                            IItem t = grid[i, w];
                            // if item is found
                            if (t != null)
                            {
                                // Assign item with empty spot location
                                t.SetGrid(i, j, this);
                                // Update grid
                                grid[i, j] = t;
                                // Set old item grid spot to null
                                grid[i, w] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }

        //Iterate the whole grid and search for an item with same neighbours indicating a possible move
        private bool CheckForRemainingMovement()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    IItem item = grid[i, j];
                    // If the item has a neighbour with same type, there is a possible move
                    if (item.GetSameTypeNeighbours().Length > 0)
                    {
                        return true;
                    }
                }
            }
            // No possible move left
            return false;
        }
        
        // Create new item and assign their grid position
        // Coroutine to allow items not to stack on top of each other
        private IEnumerator CreateNewItemsCoroutine(int amount, Action onCompletion)
        {
            for (int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    if (grid[j, i] == null)
                    {
                        // Get the spawn point
                        Transform spawnTr = spawns[j];
                        // Create new item 
                        GameObject obj = Instantiate<GameObject>(m_items.GetRandomCoreItems(m_level.items), spawnTr);
                        // Set position at spawn
                        obj.transform.position = spawnTr.position;
                        // Populate the IItem values
                        IItem item = obj.GetComponent<IItem>();
                        item.SetGrid(j, i, this);
                       
                        // Sart the movement with callback
                        item.StartMovement(()=> 
                        {
                            // Decrease amount and check if all are done
                            if (--amount == 0)
                            {
                                onCompletion?.Invoke();
                            }
                        });
                        // Set the grid with new IItem
                        grid[j, i] = item;
                    }
                }
                yield return new WaitForSeconds(waitTimeForCreation);
            }
           // onCompletion?.Invoke();
        }

        private IEnumerator EnableItemCoroutine(Action onCompletion)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    IItem item = grid[j, i];
                    item.GameObject.SetActive(true);
                    item.StartMovement(null);
                    yield return new WaitForSeconds(waitTimeForCreation);
                }
            }
            onCompletion?.Invoke();
        }

        // Set all four neighbours for the items of the grid
        private void SetNeighboursGrid()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    IItem left = null, right = null, top = null, bottom = null;
                    // for each item, checking if it is not on the edge to avoid out of bounds exception
                    if (i > 0)
                    {
                        left = grid[i - 1, j];
                    }
                    if (i < width - 1)
                    {
                        right = grid[i + 1, j];
                    }
                    if (j > 0)
                    {
                        bottom = grid[i, j - 1];
                    }
                    if (j < height - 1)
                    {
                        top = grid[i, j + 1];
                    }
                    grid[i, j].SetNeighbors(left, right, top, bottom);
                }
            }
        }
    }

    public interface IItemGenerator 
    {
        Transform Target { get; }
    }
}
