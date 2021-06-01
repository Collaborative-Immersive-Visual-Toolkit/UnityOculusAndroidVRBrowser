using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;

public class cone : MonoBehaviourPun
{
    public Transform head;

    public TextAsset jsonTextFile;

    private ConeVectors c;

    public LineRenderer lr;

    // Start is called before the first frame update
    void Start()
    {
        //Load text from a JSON file (Assets/Resources/Text/jsonFile01.json)
        //var jsonTextFile = Resources.Load<TextAsset>("vectors_cone_20_77");

        c = ConeVectors.CreateFromJSON(jsonTextFile);

        c.init(head);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        c.ComputeRaycast();
        c.updateLineRender(lr);
        RaiseVisualConeChangeEvent(c.SerializeData());
        //c.RaycastForward();


    }


    public void RaiseVisualConeChangeEvent(object[] data)
    {

        PhotonNetwork.RaiseEvent(MasterManager.GameSettings.VisualConeChange, data, Photon.Realtime.RaiseEventOptions.Default, SendOptions.SendReliable);

    }

}

[System.Serializable]
public class ConeVectors
{
    public Vector3[] vectorsList;

    public Transform head;

    public Vector3[] directions;

    public RaycastHit[] hits;

    public RaycastHit currenthit;

    private List<Vector3> positions;


    // Bit shift the index of the layer (8) to get a bit mask
    public int layerMask = 1 << 9;

    public void init(Transform h)
    {

        head = h;

        directions = new Vector3[vectorsList.Length];

        hits = new RaycastHit[vectorsList.Length];
    }

    public static ConeVectors CreateFromJSON(TextAsset jsonString)
    {
        return JsonUtility.FromJson<ConeVectors>(jsonString.ToString());
    }

    public void ComputeWorldToLocal()
    {

        for (int i = 0; i < vectorsList.Length; i++)
        {

            directions[i] = head.localToWorldMatrix * vectorsList[i];

        }

    }

    public void ComputeRaycast()
    {
        positions = new List<Vector3>();

        ComputeWorldToLocal();

        int i = vectorsList.Length;
        hits = new RaycastHit[i];

        while (i > 0)
        {

            i--;
            //(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask);
            if (Physics.Raycast(head.position, directions[i], out hits[i], 6f, layerMask))
            {
                //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //sphere.transform.position = hits[i].point;
                //sphere.transform.localScale = new Vector3(.1f, .1f, .1f);
                //currenthit = hits[i];

                positions.Add(hits[i].point);
            }

        }

    }

    public void updateLineRender(LineRenderer lr) {

        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());
    }

    public void RaycastForward()
    {     
        int i = vectorsList.Length;
        RaycastHit hit = new RaycastHit();
   
        if (Physics.Raycast(head.position, head.forward, out hit,6f, layerMask))
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = hit.point;
            sphere.transform.localScale = new Vector3(.1f,.1f,.1f) ;
            currenthit = hit;
        }     

    }

    public object[] SerializeData() {

        object[] data = new object[] { positions, PhotonNetwork.NickName };

        return data;
    }

 
}

