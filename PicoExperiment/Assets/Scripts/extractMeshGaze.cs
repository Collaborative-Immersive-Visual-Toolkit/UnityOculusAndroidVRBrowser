using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using System;
using System.Collections.Generic;


public class extractMeshGaze : MonoBehaviour
{
    public LayerMask layerMask;
    public string csv = "C:\\Users\\Riccardo\\OneDrive - Imperial College London\\experiment4\\data\\220_18\\147\\220_18_4_4.csv";
    StreamWriter writer;
    Char[] remove = new Char[] { ' ', '(', ')' };

    void Start() {

        NewData();

        string fileData = System.IO.File.ReadAllText(csv);
        string[] lines  = fileData.Split("\n"[0]);
        string HeadGaze = "";
        string HeadGazeUV = "";

        for (int i=0; i<lines.Length;i++)
        {
            if (i == 0) continue;

            string[] lineData = (lines[i].Trim()).Split(","[0]);

            if (lineData.Length == 61) { 

                Vector3 pos = new Vector3(float.Parse(lineData[4]), float.Parse(lineData[5]), float.Parse(lineData[6]));
                Vector3 forward = new Vector3(float.Parse(lineData[7]), float.Parse(lineData[8]), float.Parse(lineData[9]));
                
                (var point, var pointUV) = CalculateHeadGaze(pos, forward);
                
                Debug.Log(point);
                Debug.Log(pointUV);

                HeadGaze = point == Vector3.zero ? "null,null,null" : point.ToString("F3");
                HeadGazeUV = pointUV == Vector2.zero ? "null,null" : pointUV.ToString("F3");


                lines[i] += "," + HeadGaze.Trim(remove) + "," + HeadGazeUV.Trim(remove);

                writer.WriteLine(lines[i]);
            }
        }

        closeWriter();

    }


    private (Vector3,Vector2) CalculateHeadGaze(Vector3 pos, Vector3 forward)
    {

        Vector3 point;
        Vector2 pointUV;
        RaycastHit hit;

        if (Physics.Raycast(pos, forward, out hit, Mathf.Infinity, layerMask))
        {
            point = hit.point;
            pointUV = hit.textureCoord;
        }
        else
        {
            point = Vector3.zero;
            pointUV = Vector2.zero;
        }

        return (point, pointUV);
    }



 

    public void NewData()
    {

        if (writer != null) closeWriter();

        string path = csv.Replace(".csv","") + "_new_headGaze.csv";

        writer = new StreamWriter(path, true);

        Debug.Log("[headgaze calculator] file saved in " + path);

        string header = "timestamp," +
        "U1PosX, U1PosY, U1PosZ," +
        "U1HeadPosX, U1HeadPosY, U1HeadPosZ," +
        "U1HeadForwardX, U1HeadForwardY, U1HeadForwardZ," +
        "U1HeadUpX, U1HeadUpY, U1HeadUpZ," +
        "U1HeadCone," +
        "U1LeftEyePosX,U1LeftEyePosY,U1LeftEyePosZ," +
        "U1RightEyePosX,U1RightEyePosY,U1RightEyePosZ," +
        "U1LeftEyeVecX,U1LeftEyeVecY,U1LeftEyeVecZ," +
        "U1RightEyeVecX,U1RightEyeVecY,U1RightEyeVecZ," +
        "U1GazeX,U1GazeY,U1GazeZ,U1GazeU,U1GazeV," +
        "U1ControllerRPosX, U1ControllerRPosY, U1ControllerRPosZ," +
        "U1ControllerREAngX, U1ControllerREAngY,  U1ControllerREAngZ," +
        "U1ControllerLPosX, U1ControllerLPosY, U1ControllerLPosZ," +
        "U1ControllerLEAngX, U1ControllerLEAngY,  U1ControllerLEAngZ," +
        "U1PointerLeftX, U1PointerLeftY,  U1PointerLeftZ," +
        "U1PointerRightX, U1PointerRightY,  U1PointerRightZ," +
        "U1PointerLeftU, U1PointerLeftV," +
        "U1PointerRightU, U1PointerRightV," +
        "TranscriptStartTime, Transcript, IsSpeaking, Condition, Visualization, InsightRecording, FoundKey, VisualizationHalf, " +
        "U1HeadGazeX, U1HeadGazeY, U1HeadGazeZ, U1HeadGazeU, U1HeadGazeV";

        writer.WriteLine(header);


    }

    public void closeWriter()
    {


        if (writer != null) writer.Close();

        writer = null;

    }

}
