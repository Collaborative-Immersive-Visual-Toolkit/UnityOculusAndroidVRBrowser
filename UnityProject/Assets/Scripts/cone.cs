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

    private List<Vector3> OldPositions;
    public bool visible = true;

    // Start is called before the first frame update
    void Start()
    {
        //Load text from a JSON file (Assets/Resources/Text/jsonFile01.json)
        //var jsonTextFile = Resources.Load<TextAsset>("vectors_cone_20_77");

        c = ConeVectors.CreateFromJSON(jsonTextFile);

        c.init(head,lr);
    }

    // Update is called once per frame
    // Update is called once per frame
    void FixedUpdate()
    {

        c.ComputeRaycast();
        if (visible) c.updateLineRender(lr);

        if (OldPositions != c.positions)
        {
            var data = c.SerializeData();
            RaiseVisualConeChangeEvent(data);
        }
        OldPositions = c.positions;

    }


    public void SwitchVis()
    {
        visible = !visible;

        if (!visible)
        {

            c.clearLineRender(lr);
        }
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

    public List<Vector3> positions;

    public float distances;

    public Color c;

    // Bit shift the index of the layer (8) to get a bit mask
    //public int layerMask = 1 << 9;
    private static int octagon;
    private static int inverseOctagon;
    private int layerMask;

    public void init(Transform h, LineRenderer lr)
    {

        head = h;

        directions = new Vector3[vectorsList.Length];

        hits = new RaycastHit[vectorsList.Length];

        c = lr.materials[0].color;



        octagon = 1 << LayerMask.NameToLayer("octagon");
        inverseOctagon = 1 << LayerMask.NameToLayer("inverseOctagon");
        layerMask = octagon | inverseOctagon;
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

        ComputeWorldToLocal();

        int i = vectorsList.Length;
        hits = new RaycastHit[i];
        positions = new List<Vector3>();
        distances = 0;

        while (i > 0)
        {
            i--;        
            if (Physics.Raycast(head.position, directions[i], out hits[i], 6f, layerMask))
            {
                if (hits[i].collider.gameObject.name == "inverse") {

                    if (hits[i].point.y > 1) {
                        positions.Add(new Vector3(hits[i].point.x,1.959f, hits[i].point.z));
                    }
                    else {
                        positions.Add(new Vector3(hits[i].point.x,0.625f, hits[i].point.z));
                    }
                }
                else
                {
                    positions.Add(hits[i].point);
                }              
                distances+=hits[i].distance;
            }

        }

    }

    public void updateLineRender(LineRenderer lr) {

        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ToArray());
        c.a = 1f - ((distances / positions.Count) / 6.5f);
        lr.materials[0].color = c;
        
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

        //object[] data = new object[] { positions.ToArray(), c.a, PhotonNetwork.NickName };

        object[] data = new object[] { positions.ToArray(), c.a, PhotonNetwork.NickName };

        return data;
    }

    public void clearLineRender(LineRenderer lr)
    {
       
            lr.positionCount = 0;
        

    }

}

