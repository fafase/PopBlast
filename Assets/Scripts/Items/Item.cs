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

        private int col;
        private int row;

        private Item left, right, top, bottom;

        #endregion

        #region PUBLIC_METHODS

        public void SetGrid(int newRow, int newCol)
        {
            row = newRow;
            col = newCol;
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
    }
    public enum ItemType : byte
    {
        None, CakeFull, CakePiece, Candybar, Lollipop, Icecream, Heart
    }
}
