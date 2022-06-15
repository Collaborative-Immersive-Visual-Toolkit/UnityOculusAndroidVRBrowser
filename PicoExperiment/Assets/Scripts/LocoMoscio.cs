using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;

public class LocoMoscio : MonoBehaviour
{
    public float multiply = 1f;

    private void Update()
    {
        Vector2 move = ControllerManager.Instance.GetTouchpadAxis();

        if (move.x + move.y != 0f) {

            gameObject.transform.position += new Vector3(move.x* multiply, 0f, move.y* multiply);
        }
    }

}
