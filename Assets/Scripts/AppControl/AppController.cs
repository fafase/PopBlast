using PopBlast.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.AppControl
{
    public class AppController : MonoBehaviour
    {
        #region MEMBERS
        [Header("Create level values")]
        [Range(5, 20)]
        [SerializeField] private int row = 0;
        [Range(5, 20)]
        [SerializeField] private int col = 0;


        private ItemGenerator generator = null;
        private InputController input = null;

        #endregion

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            generator = FindObjectOfType<ItemGenerator>();
            if(generator == null)
            {
                throw new System.Exception("Missing ItemGenerator component");
            }
            input = FindObjectOfType<InputController>();
            if (input == null)
            {
                throw new System.Exception("Missing InputController component");
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            input.enabled = false;
            FindObjectOfType<GridGenerator>().Init(col,row);
            generator.Init(col, row, ()=> 
            {
                input.enabled = true;
            });
            input.RaiseItemTapped += Input_RaiseItemTapped;
        }

        #endregion

        #region PRIVATE_METHODS

        private void Input_RaiseItemTapped(GameObject obj)
        {
            generator.CheckItemNeighbours(obj);
        }

        #endregion
    }
}
