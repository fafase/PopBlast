using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.Items
{
    public class Explosion : MonoBehaviour
    {
        public void RemoveExplosion()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
