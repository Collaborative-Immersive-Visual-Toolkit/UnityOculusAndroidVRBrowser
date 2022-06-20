using Unity.XR.PXR;
using UnityEngine;


public class UpdateOverlayTexture : MonoBehaviour
{
    [SerializeField]
    public Camera m_uiCamera;

    [SerializeField]
    public MeshRenderer m_hole;

    [SerializeField]
    public PXR_OverLay m_overlay;

    [SerializeField]
    public float aspect;

    public void setAspect()
    {
        m_uiCamera.aspect = (float)m_uiCamera.targetTexture.width / (float)m_uiCamera.targetTexture.height;
        //m_uiCamera.aspect = aspect;
    }


    // Update is called once per frame
    void Update()
    {

        m_hole.material.mainTexture = m_uiCamera.targetTexture;
        m_overlay.SetTexture(m_uiCamera.targetTexture);
    }



}


