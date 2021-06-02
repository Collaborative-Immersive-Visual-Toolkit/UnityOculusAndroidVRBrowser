using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAvatarsManager : MonoBehaviour
{


    public List<GameObject> List;

    public void switchVisibility() {

        
                foreach (GameObject g in List)
                {

                    Transform t = DeepChildSearch(g, "VisualCone");

                    t.GetComponent<RemoteVisualCone>().visible = !t.GetComponent<RemoteVisualCone>().visible;


                }



     }

    public Transform DeepChildSearch(GameObject g, string childName)
    {

        Transform child = null;

        for (int i = 0; i < g.transform.childCount; i++)
        {

            Transform currentchild = g.transform.GetChild(i);

            if (currentchild.gameObject.name == childName)
            {

                return currentchild;
            }
            else
            {

                child = DeepChildSearch(currentchild.gameObject, childName);

                if (child != null) return child;
            }

        }

        return null;
    }


}
