using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.Items
{
    /// <summary>
    /// Base class for the Item object
    /// Implements IItem to minimize the autocompletion list
    /// </summary>
    public class Item : MonoBehaviour , IItem
    {
        #region MEMBERS

        [Tooltip("Explosion prefab for this item")]
        [SerializeField] private GameObject explosion;
        [Tooltip("Type of the item")]
        [SerializeField] private ItemType type = ItemType.None;
        [Tooltip("Period for item movement")]
        [SerializeField] private float movementSeconds = 1f;

        private float col;
        private float row;

        /// <summary>
        /// Get the current column
        /// </summary>
        public int Column { get { return (int)col; } }
        /// <summary>
        /// Get the current row
        /// </summary>
        public int Row { get { return (int)row; } }

        private IItem left, right, top, bottom;

        #endregion

        #region PUBLIC_METHODS

        /// <summary>
        /// Get the game object for this IItem
        /// </summary>
        public GameObject GameObject
        {
            get { return gameObject; }
        }

        /// <summary>
        /// Set the item with a new grid value
        /// </summary>
        /// <param name="newCol"></param>
        /// <param name="newRow"></param>
        public void SetGrid(int newCol, int newRow)
        {
            col = (float)newCol;
            row = (float)newRow;
            StartCoroutine(MoveToPositionCoroutine(null));
        }

        /// <summary>
        /// Set the item with new neighbours
        /// </summary>
        /// <param name="newleft"></param>
        /// <param name="newRight"></param>
        /// <param name="newTop"></param>
        /// <param name="newBottom"></param>
        public void SetNeighbors(IItem newleft, IItem newRight, IItem newTop, IItem newBottom)
        {
            left = newleft;
            right = newRight;
            top = newTop;
            bottom = newBottom;
        }

        /// <summary>
        /// Start the movement for this item
        /// </summary>
        public void StartMovement(Action onCompletion)
        {        
            StartCoroutine(MoveToPositionCoroutine(onCompletion));
        }

        /// <summary>
        /// Get a collection of neighbours with same type
        /// </summary>
        /// <returns></returns>
        public IItem[] GetSameTypeNeighbours()
        {
            List<IItem> list = new List<IItem>();
            CheckNeighbor(left, list);
            CheckNeighbor(right, list);
            CheckNeighbor(top, list);
            CheckNeighbor(bottom, list);
            return list.ToArray();
        }

        /// <summary>
        /// Destroy the item with explosion effect
        /// </summary>
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

        private IEnumerator MoveToPositionCoroutine(Action onCompletion)
        {
            float timeSinceStarted = 0f;
            float target = row + 0.5f;
            while (Mathf.Approximately(transform.position.y, target) == false)
            {
                timeSinceStarted += Time.deltaTime;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, target, timeSinceStarted / movementSeconds);
                transform.position = pos;
                yield return null;
            }
            onCompletion?.Invoke();
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
        IItem[] GetSameTypeNeighbours();
        void StartMovement(Action onCompletion);
        void SetNeighbors(IItem newleft, IItem newRight, IItem newTop, IItem newBottom);
        void SetGrid(int newCol, int newRow);
        GameObject GameObject { get; }
        void DestroyItem();
    }
}
