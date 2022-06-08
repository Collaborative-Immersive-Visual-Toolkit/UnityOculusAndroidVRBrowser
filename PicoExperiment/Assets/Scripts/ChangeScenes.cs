using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScenes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeScenceToMirror()
    {
        Main.Instance.GoMirror();
    }
    public void ChangeScenceToGame()
    {
        Main.Instance.GoET_lighting();
    }
    public void ChangeScenceToCustom()
    {
        Main.Instance.GoVisualization();
    }
    public void ChangeScenceToCalibration()
    {
        Main.Instance.GoCalibration();
    }
    public void ChangeScenceToCoordinates()
    {
        Main.Instance.GoCoordinates();
    }
}
