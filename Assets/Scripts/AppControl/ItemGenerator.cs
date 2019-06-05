using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PopBlast.Items;

namespace PopBlast.AppControl
{
    public class ItemGenerator : MonoBehaviour
    {
        #region MEMBERS
        [SerializeField] private float waitTimeForCreation = 0.5f;
        [SerializeField] private GameObject[] items = null;

        private Transform[] spawns = null;
        private Item[,] grid;

        private int width, height;

        #endregion

        #region UNITY_LIFECYCLE

        #endregion

        #region PUBLIC_METHODS

        public void Init(int col, int row, Action onCompletion)
        {
            spawns = new Transform[col];
            width = col;
            height = row;

            // generate the spawn points
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
                    Item item = obj.GetComponent<Item>();
                    item.SetGrid(i, j);
                    grid[i, j] = item;
                    obj.SetActive(false);
                }
            }
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    Item left = null, right = null, top = null, bottom = null;
                    if (i > 0)
                    {
                        left = grid[i - 1, j];
                    }
                    if (i < col - 1)
                    {
                        right = grid[i + 1, j];
                    }
                    if (j > 0)
                    {
                        bottom = grid[i, j - 1];
                    }
                    if (j < row - 1)
                    {
                        top = grid[i, j + 1];
                    }
                    grid[i, j].SetNeighbors(left, right, top, bottom);
                }
            }
            StartCoroutine(EnableItemCoroutine(onCompletion));
        }
    
        public void CheckItemNeighbours(GameObject go)
        {
            Item item = go.GetComponent<Item>();
            if(item == null) { return; }
            Item[] results = item.GetSameAdjacentItem();
            if(results.Length == 0) { return; }
            HashSet<Item> toRemove = new HashSet<Item>();
            toRemove.Add(item);
            Queue<Item> queue = new Queue<Item>();
            queue.Enqueue(item);

            while (queue.Count > 0)
            {
                Item current = queue.Dequeue();
                Item[] items = current.GetSameAdjacentItem();
                if (items.Length == 0) { continue; }

                foreach (Item it in items)
                {
                    if (toRemove.Add(it))
                    {
                        queue.Enqueue(it);
                    }
                }
            }
            foreach (Item i in toRemove)
            {
                DestroyImmediate(i.gameObject);
            }
            // Reset position and grid
            for (int i = 0; i < width; i++)
            {
                int empty = 0;
                for (int j = 0; j < height; j++)
                {
                    Item temp = grid[i, j];
                    if (temp == null)
                    {
                        empty++;
                        for (int w = j; w < height; w++)
                        {
                            Item t = grid[i, w];
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

        #endregion

        private IEnumerator EnableItemCoroutine(Action onCompletion)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Item item = grid[j, i];
                    item.gameObject.SetActive(true);
                    item.StartMovement();
                    yield return new WaitForSeconds(waitTimeForCreation);
                }
            }
            onCompletion?.Invoke();
        }
    }
}
