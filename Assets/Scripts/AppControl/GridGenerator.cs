using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.AppControl
{
    /// <summary>
    /// Generate the grid based on width and height
    /// </summary>
    public class GridGenerator : MonoBehaviour
    {
        #region MEMBERS
        [Tooltip("Background image")]
        [SerializeField] private Transform background = null;

        #endregion

        #region PUBLIC_METHODS

        public void Init(int width, int height)
        {
            SetCamera(width, height);
            SetBackground(width, height);
        }

        #endregion

        #region PRIVATE_METHODS

        // Set the camera position and size based on width and height
        private void SetCamera(float width, float height)
        {
            // Set new position based on width and height
            Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
            float margin = 1f;
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float gridRatio = width / height;
            float size = (gridRatio >= screenRatio) ? width : height / 2f + margin;
            Camera.main.orthographicSize = size;
        }

        private void SetBackground( float width, float height)
        {
            background.position = new Vector3(width / 2f, height / 2f, 0f);
            background.localScale = new Vector3(width, height, 1f);
        }

        #endregion
    }
}
