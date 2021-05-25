using UnityEngine;

public class TurnOnDepthBuffer : MonoBehaviour
{
    #region Private Methods

    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    #endregion
}