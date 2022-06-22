
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Unity.XR.PXR;


[CustomEditor(typeof(UpdateOverlayTexture))] 
[CanEditMultipleObjects]
public class UpdateOverlayTextureEditor : Editor
{

  
    //public override void OnInspectorGUI()
    //{
    //    // Custom form for Player Preferences
    //    UpdateOverlayTexture Target = (UpdateOverlayTexture)target;

    //    if (GUILayout.Button("Set Aspect"))
    //    {

    //        Target.setAspect();
    //    }

    //    base.DrawDefaultInspector();

    //}
}
#endif
