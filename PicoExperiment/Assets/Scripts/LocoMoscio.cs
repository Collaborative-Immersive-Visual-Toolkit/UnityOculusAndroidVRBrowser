using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;

public class LocoMoscio : MonoBehaviour
{
    public float multiply = 1f;

    public Transform head;



    private void Update()
    {
        Vector2 move = ControllerManager.Instance.GetTouchpadAxis();

        if (move.x + move.y != 0f) {

            Debug.Log(move.normalized);

            gameObject.transform.position += new Vector3(head.up.x , 0f, head.up.z) * -move.normalized.y * multiply;
            gameObject.transform.position +=  new Vector3(head.right.x, 0f, head.right.z) * move.normalized.x * multiply;

            
        }
    }

}
