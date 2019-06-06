using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PopBlast.Items;

namespace PopBlast.AppControl
{
    /// <summary>
    /// Component controlling the grid of items
    /// </summary>
    public class ItemGenerator : MonoBehaviour
    {
        #region MEMBERS

        [Tooltip("Waiting time for row creation of item")]
        [SerializeField] private float waitTimeForCreation = 0.5f;
        [Tooltip("Items prefabs")]
        [SerializeField] private GameObject[] items = null;

        private Transform[] spawns = null;
        private IItem[,] grid;

        private int width, height;

        public event Action RaiseEndOfGame;

        /// <summary>
        /// Event raised when pop happens, parameter indicates how many items at once
        /// </summary>
        public event Action<int> RaiseItemPop;

        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Initializes the ItemGenerator with column (width) and row (height)
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="onCompletion"></param>
        public void Init(int col, int row, Action onCompletion)
        {
            spawns = new Transform[col];
            width = col;
            height = row;

            // Generate the spawn points
            for (int i = 0; i < col; i++)
            {
                GameObject obj = new GameObject($"Spawn_{i}");
                spawns[i] = obj.transform;
                float posY = (float)row + 1f;
                obj.transform.position = new Vector3(i + 0.5f, posY, 0f);
                obj.transform.parent = transform;
            }

            // Generate all items
            grid = new Item[col, row];
            for (int i = 0; i < col; i++)
            {
                Transform spawnTr = spawns[i];
                for (int j = 0; j < row; j++)
                {
                    int rand = UnityEngine.Random.Range(0, items.Length);
                    GameObject obj = Instantiate<GameObject>(items[rand], spawnTr);
                    obj.transform.position = spawnTr.position;
                    IItem item = obj.GetComponent<IItem>();
                    item.SetGrid(i, j);
                    item.GameObject.SetActive(false);
                    grid[i, j] = item;
                }
            }
            SetGrid();
            StartCoroutine(EnableItemCoroutine(onCompletion));
        }
        
        /// <summary>
        /// Checks the neighbours of the given game object 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="onCompletion"></param>
        public void CheckItemNeighbours(GameObject go, Action onCompletion)
        {
            Item item = go.GetComponent<Item>();
            if(item == null)
            {
                onCompletion?.Invoke();
                return;
            }
            IItem[] results = item.GetSameAdjacentItems();
            if(results.Length == 0)
            {
                onCompletion?.Invoke();
                return;
            }
            ProcessItemToRemove(item);
            CheckForEmptySpaces();
            StartCoroutine(CreateNewItem(()=> 
            {
                SetGrid();
                if (CheckForRemainingMovement() == false)
                {
                    RaiseEndOfGame?.Invoke();
                }
                onCompletion?.Invoke();
            }));
        }

        #endregion

        private void ProcessItemToRemove(Item item)
        {
            HashSet<IItem> toRemove = new HashSet<IItem>();
            toRemove.Add(item);
            Queue<IItem> queue = new Queue<IItem>();
            queue.Enqueue(item);

            while (queue.Count > 0)
            {
                IItem current = queue.Dequeue();
                IItem[] items = current.GetSameAdjacentItems();
                if (items.Length == 0) { continue; }

                foreach (IItem it in items)
                {
                    if (toRemove.Add(it))
                    {
                        queue.Enqueue(it);
                    }
                }
            }
            RaiseItemPop?.Invoke(toRemove.Count);
            foreach (IItem i in toRemove)
            {
                grid[i.Column, i.Row] = null;
                i.DestroyItem();
            }
        }

        private void CheckForEmptySpaces()
        {
            for (int i = 0; i < width; i++)
            {
                int empty = 0;
                for (int j = 0; j < height; j++)
                {
                    IItem temp = grid[i, j];
                    if (temp == null)
                    {
                        empty++;
                        for (int w = j; w < height; w++)
                        {
                            IItem t = grid[i, w];
                            if (t != null)
                            {
                                t.SetGrid(i, j);
                                grid[i, j] = t;
                                grid[i, w] = null;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool CheckForRemainingMovement()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    IItem item = grid[i, j];
                    if (item.GetSameAdjacentItems().Length > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerator CreateNewItem(Action onCompletion)
        {
            for (int i = 0; i < height; i++)
            {
                for(int j = 0; j < width; j++)
                {
                    if (grid[j, i] == null)
                    {
                        Transform spawnTr = spawns[j];
                        int rand = UnityEngine.Random.Range(0, items.Length);
                        GameObject obj = Instantiate<GameObject>(items[rand], spawnTr);
                        obj.transform.position = spawnTr.position;
                        IItem item = obj.GetComponent<IItem>();
                        item.SetGrid(j, i);
                        grid[j, i] = item;
                        item.StartMovement();
                    }
                }
                yield return new WaitForSeconds(waitTimeForCreation);
            }
            onCompletion?.Invoke();
        }

        private IEnumerator EnableItemCoroutine(Action onCompletion)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    IItem item = grid[j, i];
                    item.GameObject.SetActive(true);
                    item.StartMovement();
                    yield return new WaitForSeconds(waitTimeForCreation);
                }
            }
            onCompletion?.Invoke();
        }

        private void SetGrid()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    IItem left = null, right = null, top = null, bottom = null;
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
}
