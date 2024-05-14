using PopBlast.AppControl;
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
        private Transform m_target;

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
        public void SetGrid(int newCol, int newRow, IItemGenerator item)
        {
            col = (float)newCol;
            row = (float)newRow;
            m_target = item.Target;
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
        public IItem [] GetSameTypeNeighbours()
        {
            List<IItem> list = new List<IItem>();
            // Check all four neighbours if same type
            // Stores in list if same
            CheckNeighbor(left, list);
            CheckNeighbor(right, list);
            CheckNeighbor(top, list);
            CheckNeighbor(bottom, list);
            // return list as array
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

        private void CheckNeighbor(IItem neighbours, List<IItem> list)
        {
            // if the neighbours is not null (current item is on edge) and same type as current item
            // add to list
            if (neighbours != null && ((Item)neighbours).type == type)
            {
                list.Add(neighbours);
            }
        }

        // Interpolation movement based on time
        private IEnumerator MoveToPositionCoroutine(Action onCompletion)
        {
            float target = m_target.position.y  + row + 0.5f;
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, target, timer / movementSeconds);
                transform.position = pos;
                yield return null;
            }
            Vector3 p = transform.position;
            transform.position = new Vector3(p.x, target, 0f);
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
        void SetGrid(int newCol, int newRow, IItemGenerator item);
        GameObject GameObject { get; }
        void DestroyItem();
    }
}
