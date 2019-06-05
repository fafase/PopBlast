using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.Items
{
    public class Item : MonoBehaviour
    {
        #region MEMBERS

        [SerializeField] private ItemType type = ItemType.None;
        [SerializeField] private float seconds = 1f;
        private float col;
        private float row;
        public int Col { get { return (int)col; } }
        public int Row { get { return (int)row; } }

        private Item left, right, top, bottom;

        private IEnumerator moveCoroutine = null;

        #endregion

        #region PUBLIC_METHODS

        public void SetGrid(int newCol, int newRow)
        {
            col = (float)newCol;
            row = (float)newRow;
            if (moveCoroutine != null)
            {
                return;
            }
            moveCoroutine = MoveToPositionCoroutine();
            StartCoroutine(moveCoroutine);
        }

        public void SetNeighbors(Item newleft, Item newRight, Item newTop, Item newBottom)
        {
            left = newleft;
            right = newRight;
            top = newTop;
            bottom = newBottom;
        }
        public void StartMovement()
        {        
            moveCoroutine = MoveToPositionCoroutine();
            StartCoroutine(moveCoroutine);
        }

        public Item[] GetSameAdjacentItem()
        {
            List<Item> list = new List<Item>();
            CheckNeighbor(left, list);
            CheckNeighbor(right, list);
            CheckNeighbor(top, list);
            CheckNeighbor(bottom, list);
            return list.ToArray();
        }

        #endregion

        #region PRIVATE_METHODS

        private void CheckNeighbor(Item item, List<Item> list)
        {
            if (item != null && item.type == type)
            {
                list.Add(item);
            }
        }
        private IEnumerator MoveToPositionCoroutine()
        {
            float timeSinceStarted = 0f;
            float target = row + 0.5f;
            while (Mathf.Approximately(transform.position.y, target) == false)
            {
                timeSinceStarted += Time.deltaTime;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, target, timeSinceStarted / seconds);
                transform.position = pos;
                yield return null;
            }
            moveCoroutine = null;
        }

        #endregion
    }
    public enum ItemType : byte
    {
        None, CakeFull, CakePiece, Candybar, Lollipop, Icecream, Heart
    }
}
