using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopBlast.InputSystem
{
    public class InputController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
                if(hit.collider == null)
                {
                    return;
                }
                Debug.Log(hit.collider.gameObject.name);

            }
        }
    }
}
