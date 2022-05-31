using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(LineRenderer))]

public class SearchKeyWordsScreen : MonoBehaviour
{
    public UrlManager urlManager;

    private List<int> currentScreenFocus;
    private Vector3 averagePoint;
    private string currentS2T="";
    private string oldS2T="";

    public string keywordsCoordinatesFileName = "keywords_coordinates.json";
    private keywordsCoordinates keyCo;
    public DetectedPoints points ;

    public MeshCollider ConeMesh;
    public LineRenderer lineRenderer;

    public PushElipsesPoints pushPoints;


    private void Start()
    {

#if UNITY_EDITOR

        string keywordsCoordinatesPath = Path.Combine(Application.streamingAssetsPath, keywordsCoordinatesFileName);

#elif UNITY_ANDROID

        string keywordsCoordinatesPath =  Path.Combine(Application.persistentDataPath, keywordsCoordinatesFileName);

        //soluation is to manually copy the file to -> adb push <local file> <remote location>

#endif

        if (!File.Exists(keywordsCoordinatesPath))
        {

            Debug.LogError("Could not find " + keywordsCoordinatesPath);

        }
        else
        {

            string jsonString = File.ReadAllText(keywordsCoordinatesPath);
            keyCo = keywordsCoordinates.CreateFromJSON(jsonString);
            Debug.Log("semantic bounding boxes loaded");
        }

        if (lineRenderer != null) points.lineRenderer = lineRenderer;

    }

    private void Update()
    {
        if (currentS2T != oldS2T)
        {
            SearchCoordinates();
            oldS2T = currentS2T;
        }
        points.clearPointNotInCone(ConeMesh);
        points.clearExpiredPoints();
    }

    void OnDrawGizmos()
    {
        //if (Application.isPlaying) points.GizmoElipse();
    }

    public void screenFocus(List<Vector3> points)
    {
        

        averagePoint =  Vector3.zero;

        for (int i = 0; i < points.Count; i++)
        {
            averagePoint += points[i];
        }

        averagePoint /= points.Count;

        currentScreenFocus = new List<int>();

        for (int i = 0; i < urlManager.BoxColliders.Count; i++)
        {
            for (int j = 0; j < points.Count; j++)
            {

                if (urlManager.BoxColliders[i].bounds.Contains(points[j]))
                {
                    currentScreenFocus.Add(i);
                    break;
                }

            }
        }

       

    }

    public void getSpeechToText(string speechToTextOutput)
    {
        oldS2T = currentS2T;
        currentS2T = speechToTextOutput.ToLower();
    }

    private void SearchCoordinates()
    {
        Vector3[] ps = new Vector3[4];
        bool[] psbool = new bool[4];
        Debug.Log(currentScreenFocus);
        for (int j = 0; j < currentScreenFocus.Count; j++)
        {
            int screenNumber = currentScreenFocus[j];
            Page p = keyCo.currentVisualization[screenNumber];
            List<IndexAndLength> indexes = searchKeysInString(p);
            for (int k = 0; k < indexes.Count; k++)
            {
                int index = indexes[k].index;
                if (index >= 0)
                {
                    mapofIndexes map = keyCo.currentVisualization[screenNumber].mapofIndexes[index];

                    map.coordinate = new List<computedcoordinate>();

                    for (int i = 0; i < map.indexes.Length; i++)
                    {
                        computedcoordinate cc = new computedcoordinate();

                        PageElement e = p.getElment(map.keywords[i], map.indexes[i]);

                        BoxCollider c = urlManager.BoxColliders[screenNumber];

                        cc.BoundingBox = convert2DtoLocalToGlobal(e, c);

                        cc.averagePoint = (cc.BoundingBox[0] + cc.BoundingBox[1] + cc.BoundingBox[2] + cc.BoundingBox[3]) / 4;

                        cc.distance = Vector3.Distance(cc.averagePoint, averagePoint);

                        cc.inside = new bool[4];

                        for (int t = 0; t < cc.BoundingBox.Length; t++) {

                            cc.inside[t] = checkIfInside(cc.BoundingBox[t]);

                        }

                        map.coordinate.Add(cc);

                    }

                    computedcoordinate closestcoordinate = map.coordinate.OrderBy(r => r.distance).First();

                    for (int t = 0; t < closestcoordinate.BoundingBox.Length; t++)
                    {
                        Vector3 point = closestcoordinate.BoundingBox[t];
                        if(closestcoordinate.inside[t]) points.AddAPoint(point);
                    }

                    points.Elipse();
                    
                }
            }
        }
    }

    public bool checkIfInside(Vector3 point)
    {

        Ray rayup = new Ray(point, Vector3.up);
        Ray raydown = new Ray(point, Vector3.down);
        RaycastHit outhitup;
        RaycastHit outhitdown;

        bool hitup = ConeMesh.Raycast(rayup, out outhitup, Mathf.Infinity);
        
        if (hitup)
        {
            bool hitdown = ConeMesh.Raycast(raydown, out outhitdown, Mathf.Infinity);
            if(hitdown) return true;
            else return false;
        }
        else return false;
    }

    public List<IndexAndLength> searchKeysInString(Page p)
    {


        List<string> keys = p.keys;
        string key;
        List<IndexAndLength> indexes = new List<IndexAndLength>();
        
        for (int i = 0; i < keys.Count; i++)
        {
            key = keys[i];

            if (currentS2T.Contains(key))
            {
                Debug.Log(key);
                IndexAndLength newEntry = new IndexAndLength();
                newEntry.index= i;
                newEntry.length = key.Length;
                newEntry.word = key;

                indexes.Add(newEntry);

            }
        }

        indexes = FilteredKeywords(indexes);


        return indexes;

        
    }

    public List<IndexAndLength> FilteredKeywords(List<IndexAndLength> keywords) {

        if (keywords.Count == 0) return keywords;

        List<IndexAndLength> outputkeywords = new List<IndexAndLength>();

        //sort based on keyword length 
        keywords.OrderBy(o => o.length).ToList();

        //the first one is always addedd
        outputkeywords.Add(keywords[0]); 

        //check if shoryter keywords are within larger keywords
        for (int i=1;i< keywords.Count;i++) {

            bool iscontained = false;

            for (int j = 0; j < outputkeywords.Count; j++) {

                if (outputkeywords[j].word.Contains(keywords[i].word))
                {
                    iscontained = true;

                    break;
                }

            }

            if (!iscontained) outputkeywords.Add(keywords[i]);

        }

        return outputkeywords;
    }

    public Vector3[] convert2DtoLocalToGlobal(PageElement e, BoxCollider c) {

        Vector3[] ps = new Vector3[4];
        Vector3 b = c.size;
        Transform t = c.gameObject.transform;


        Vector3 RightTopCorner = new Vector3(e.right * b.x, e.top * b.y * -1, 0f);
        Vector3 LeftTopCorner = new Vector3(e.left * b.x, e.top * b.y*-1, 0f);
        Vector3 RightBottomCorner = new Vector3(e.right * b.x, e.bottom * b.y *-1, 0f);
        Vector3 LeftBottomCorner = new Vector3(e.left * b.x, e.bottom * b.y *-1, 0f);

        //Vector3 RightTopCorner = new Vector3(0f, 0f, 0f);
        //Vector3 LeftTopCorner = new Vector3(0f, 0f, 0f);
        //Vector3 RightBottomCorner = new Vector3(0f, 0f, 0f);
        //Vector3 LeftBottomCorner = new Vector3(0f, 0f, 0f);

        ps[0] = t.TransformPoint(RightTopCorner);
        ps[1] = t.TransformPoint(LeftTopCorner);
        ps[2] = t.TransformPoint(RightBottomCorner);
        ps[3] = t.TransformPoint(LeftBottomCorner);

        return ps;

    }

    public void changeVis(visualization v) {

        keyCo.setVisualization(v);

    }
}

public class IndexAndLength
{
    public int index;
    public int length;
    public string word;

}

[Serializable]
public class keywordsCoordinates
{

    public Page BoxAndWhiskers;
    public Page BoxAndWhiskers_Gender;
    public Page BoxAndWhiskers_Third;

    public Page BoxAndWhiskers2;
    public Page BoxAndWhiskers2_Gender;
    public Page BoxAndWhiskers2_Third;

    public Page Histograms;
    public Page Histograms_Gender;
    public Page Histograms_Third;

    public Page Oscar;
    public Page Oscar_Gender;
    public Page Oscar_Third;

    public Page Scatterplot1;
    public Page Scatterplot1_Gender;
    public Page Scatterplot1_Third;

    public Page Scatterplot2;
    public Page Scatterplot2_Gender;
    public Page Scatterplot2_Third;

    public Page StackBarChart;
    public Page StackBarChart_Gender;
    public Page StackBarChart_Third;

    public Page Instructions;
    public Page Instructions_Gender;
    public Page Instructions_Third;

    public visualization VisualizationNumber;

    public Dictionary<int, Page> visualization1 = new Dictionary<int, Page>();
    public Dictionary<int, Page> visualization2 = new Dictionary<int, Page>();
    public Dictionary<int, Page> visualization3 = new Dictionary<int, Page>();
    public Dictionary<int, Page> currentVisualization = new Dictionary<int, Page>();

    public static keywordsCoordinates CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<keywordsCoordinates>(jsonString);

    }

    public void setVisualization(visualization v)
    {

        visualization VisualizationNumber = v;

        visualization1 = new Dictionary<int, Page>
        {
            { 0, BoxAndWhiskers2 }, 
            { 1, Histograms },
            { 2, BoxAndWhiskers },
            { 3, StackBarChart },
            { 4, Oscar },
            { 5, Scatterplot1 },
            { 6, Scatterplot2 },
            { 7, Instructions },

        };

        visualization2 = new Dictionary<int, Page>
        {
            { 0, BoxAndWhiskers2_Gender },
            { 1, Histograms_Gender },
            { 2, BoxAndWhiskers_Gender },
            { 3, StackBarChart_Gender },
            { 4, Oscar_Gender },
            { 5, Scatterplot1_Gender },
            { 6, Scatterplot2_Gender },
            { 7, Instructions_Gender },
        };

        visualization3 = new Dictionary<int, Page>
        {
            { 0, BoxAndWhiskers2_Third },
            { 1, Histograms_Third },
            { 2, BoxAndWhiskers_Third },
            { 3, StackBarChart_Third },
            { 4, Oscar_Third },
            { 5, Scatterplot1_Third },
            { 6, Scatterplot2_Third },
            { 7, Instructions_Third },


        };

        if (VisualizationNumber == visualization.visualization1) currentVisualization = visualization1;
        else if (VisualizationNumber == visualization.visualization2) currentVisualization = visualization2;
        else if (VisualizationNumber == visualization.visualization3) currentVisualization = visualization3;
    }

   
}

[Serializable]
public enum visualization
    {

        visualization1 = 0,
        visualization2 = 1,
        visualization3 = 2,

    }

[Serializable]
public class Page
{
    public PageElement[] graphs;
    public List<string> keys = new List<string>();
    public PageElement[] scatter;
    public PageElement[] xaxeslabels;
    public PageElement[] xticks;
    public PageElement[] yaxeslabels;
    public PageElement[] yticks;
    public mapofIndexes[] mapofIndexes;

    public PageElement? getElment(string key, int index)
    {

        switch (key)
        {

            case "xaxeslabels": return xaxeslabels[index];
            case "xticks": return xticks[index];
            case "yaxeslabels": return yaxeslabels[index];
            case "yticks": return yticks[index];
            case "scatterpoints": return scatter[index];
        }

        return null;
    }

}

[Serializable]
public class mapofIndexes
{
    public int[] indexes;
    public string[] keywords;
    public List<computedcoordinate> coordinate;

}

[serializable]
public class computedcoordinate {

    public float distance;
    public Vector3 averagePoint;
    public Vector3[] BoundingBox;
    public bool[] inside;

}

[Serializable]
public class PageElement
{
    public float bottom;
    public float left;
    public string name;
    public float right;
    public float top;
}

[Serializable]
public class RectCorners
{
    public float bottom;
    public float left;
    public float right;
    public float top;
}

