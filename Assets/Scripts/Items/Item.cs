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

        private Item left, right, top, bottom;

        private IEnumerator moveCoroutine = null;

        #endregion

        #region PUBLIC_METHODS

        public void SetGrid(int newRow, int newCol)
        {
            row = (float)newRow;
            col = (float)newCol;
            if(moveCoroutine != null)
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

        public override bool Equals(object other)
        {
            if(other == null) { return false; }
            Item item = other as Item;
            if(item == null) { return false; }
            return item == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Item a, Item b)
        {
            return a.type == b.type;
        }

        public static bool operator !=(Item a, Item b)
        {
            return a.type != b.type;
        }

        #endregion

        #region PRIVATE_METHODS

        private IEnumerator MoveToPositionCoroutine()
        {
            float timeSinceStarted = 0f;
           
            while (Mathf.Approximately(transform.position.y, row) == false)
            {
                timeSinceStarted += Time.deltaTime;
                Vector3 pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, row, timeSinceStarted / seconds);
                transform.position = pos;
                yield return null;
            }
        }

        #endregion
    }
    public enum ItemType : byte
    {
        None, CakeFull, CakePiece, Candybar, Lollipop, Icecream, Heart
    }
}
