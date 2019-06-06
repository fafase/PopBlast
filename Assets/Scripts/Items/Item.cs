using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.Items
{
    public class Item : MonoBehaviour , IItem
    {
        #region MEMBERS
        [SerializeField] private GameObject explosion;
        [SerializeField] private ItemType type = ItemType.None;
        [SerializeField] private float seconds = 1f;
        private float col;
        private float row;
        public int Column { get { return (int)col; } }
        public int Row { get { return (int)row; } }

        private IItem left, right, top, bottom;

        private IEnumerator moveCoroutine = null;

        #endregion

        #region PUBLIC_METHODS
        public GameObject GameObject { get { return gameObject; } }

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

        public void SetNeighbors(IItem newleft, IItem newRight, IItem newTop, IItem newBottom)
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

        public IItem[] GetSameAdjacentItems()
        {
            List<IItem> list = new List<IItem>();
            CheckNeighbor(left, list);
            CheckNeighbor(right, list);
            CheckNeighbor(top, list);
            CheckNeighbor(bottom, list);
            return list.ToArray();
        }

        public void DestroyItem()
        {
            ExplosionAnimate();
        }

        #endregion

        #region PRIVATE_METHODS

        private void ExplosionAnimate()
        {
            Instantiate<GameObject>(this.explosion, transform.position, transform.rotation);           
            Destroy(gameObject);
        }

        private void CheckNeighbor(IItem item, List<IItem> list)
        {
            if (item != null && ((Item)item).type == type)
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

    /// <summary>
    /// Item object interface
    /// </summary>
    public interface IItem
    {
        int Row { get; }
        int Column { get; }
        IItem[] GetSameAdjacentItems();
        void StartMovement();
        void SetNeighbors(IItem newleft, IItem newRight, IItem newTop, IItem newBottom);
        void SetGrid(int newCol, int newRow);
        GameObject GameObject { get; }
        void DestroyItem();
    }
}
