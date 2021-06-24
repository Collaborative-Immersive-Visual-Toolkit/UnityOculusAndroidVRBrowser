using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputs : MonoBehaviour
{

        [SerializeField]
        private Transform _controller = null;
        public Transform Controller
        {
            get 
            {
               _controller = DeepChildSearch(gameObject, "hand_right");
                return _controller; 
            }
                 
        }


        [SerializeField]
        private Transform _localHead;
        public Transform LocalHead
        {
            get
            {
                _localHead = DeepChildSearch(gameObject, "head_JNT");
                return _localHead;
            }

        }


        [SerializeField]
        private Transform _pointer;
        public Transform Pointer
        {
            get
            {
                _pointer = DeepChildSearch(gameObject, "Pointer");
                return _pointer;
            }

        }



    public Transform DeepChildSearch(GameObject g, string childName) {

        Transform child = null;

        for (int i = 0; i< g.transform.childCount; i++) {

            Transform currentchild = g.transform.GetChild(i);

            if (currentchild.gameObject.name == childName)
            {

                return currentchild;
            }
            else {

                child = DeepChildSearch(currentchild.gameObject, childName);

                if (child != null) return child;
            }

        }

        return null;
    }
}
