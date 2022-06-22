using Unity.XR.PXR;
using UnityEngine;


public class UpdateOverlayTexture : MonoBehaviour
{
    [SerializeField]
    public BrowserView m_browser;

    //[SerializeField]
    //public Camera m_uiCamera;

    //[SerializeField]
    //public MeshRenderer m_hole;

    [SerializeField]
    public PXR_OverLay m_overlay;

    //[SerializeField]
    //public float aspect;

    public void setAspect()
    {
        //m_uiCamera.aspect = (float)m_uiCamera.targetTexture.width / (float)m_uiCamera.targetTexture.height;
        //m_uiCamera.aspect = aspect;
    }


    // Update is called once per frame
    void Update()
    {

        Texture2D tex = new Texture2D(980, 557, TextureFormat.RGBA32, false);
        byte[] res;
        res = m_browser.TakePngScreenShot(980, 557);


        if (res != null && res.Length != 0)
        {
            Debug.Log("res.Length");
            Debug.Log(res.Length);

            tex.LoadRawTextureData(res);
            tex.Apply();

            Debug.Log("res.Length");

            //m_hole.material.mainTexture = tex;
            m_overlay.SetTexture(tex);

        }
    }



}


