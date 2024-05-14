using System;
using UnityEngine;

namespace PopBlast.InputSystem
{
    public class InputController : MonoBehaviour
    {

        public event Action<GameObject> RaiseItemTapped;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                if(hit.collider == null)
                {
                    return;
                }
                RaiseItemTapped?.Invoke(hit.collider.gameObject);
            }
        }
    }
}
