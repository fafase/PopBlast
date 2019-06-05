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
        [SerializeField] private float waitTimeForCreation = 0.5f;

        private ItemGenerator generator = null;

        #endregion

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            generator = FindObjectOfType<ItemGenerator>();
            if(generator == null)
            {
                throw new System.Exception("Missing ItemGenerator component");
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            FindObjectOfType<GridGenerator>().Init(col,row);
            generator.Init(col, row);
            StartCoroutine(CreateItemsCoroutine());
        }

        #endregion

        #region PRIVATE_METHODS

        private IEnumerator CreateItemsCoroutine ()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    generator.GenerateItemAtIndex(i, j);
                    yield return new WaitForSeconds(waitTimeForCreation);
                } 
            }
        }

        #endregion
    }
}
