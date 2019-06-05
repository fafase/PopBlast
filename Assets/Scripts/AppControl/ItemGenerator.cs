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
        [SerializeField] private float spawnPositionY = 5f;
        [SerializeField] private float minX = 0f;
        [SerializeField] private float maxX = 0f;
        [SerializeField] private GameObject[] items = null;

        private Transform[] spawns = null;

        #endregion

        #region PUBLIC_METHODS

        public void GenerateItemAtIndex(int row, int col)
        {
            if(col < 0 || col >= spawns.Length) { throw new ArgumentOutOfRangeException("Index is out of range for spawn"); }

            int rand = UnityEngine.Random.Range(0, items.Length);
            Transform spawnTr = spawns[col];
            GameObject obj = Instantiate<GameObject>(items[rand], spawnTr);
            obj.GetComponent<Item>().SetGrid(row, col);
            obj.transform.position = spawnTr.position;
            float scale = (5f / (float)spawns.Length) - 0.05f ;
            obj.transform.localScale = new Vector3(scale, scale, 1f);
        }

        public void Init(int col, int row)
        {
            spawns = new Transform[col];
            float width = Mathf.Abs(minX) + maxX;
            float div = width / col;
            float start = minX + div / 2f;
            for(int i = 0; i < col; i++)
            {
                GameObject obj = new GameObject($"Spawn_{i}");
                spawns[i] = obj.transform;
                obj.transform.position = new Vector3(start + div * i, spawnPositionY, 0f);
                obj.transform.parent = transform;
            }
        }

        #endregion
    }
}
