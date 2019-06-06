using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.Items
{
    /// <summary>
    /// Explosion controller
    /// </summary>
    public class Explosion : MonoBehaviour
    {
        /// <summary>
        /// Remove the explosion object
        /// Attached to the animation event of the explosion
        /// </summary>
        public void RemoveExplosion()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
