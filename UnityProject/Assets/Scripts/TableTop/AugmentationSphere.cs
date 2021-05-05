using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TableTop { 
    public class AugmentationSphere : MonoBehaviour
    {
        private GameObject Sphere;

        private Renderer r;

        private Material m;
        void Start()
        {
            CreateSphere();
            HideSphere();
        }

        private void CreateSphere()
        {

            Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            Destroy(Sphere.GetComponent<Collider>());


            //scale
            Vector3 scale = Sphere.transform.localScale;
            scale.Set(0.02f, 0.02f, 0.02f);
            Sphere.transform.localScale = scale;

            //material
            r = Sphere.GetComponent<Renderer>();
            m = Resources.Load("Materials/agumentation", typeof(Material)) as Material;
            r.material = m;


        }
        
        public void UpdateSphereLocation(Vector3 newPosition)
        {

            if (Sphere == null) CreateSphere();

            if (Sphere.activeSelf == false) ShowSphere();

            Sphere.transform.position = newPosition;

        }
        
        public void HideSphere()
        {

            if (Sphere != null) Sphere.SetActive(false);

        }

        private void ShowSphere()
        {

            if (Sphere != null) Sphere.SetActive(true);

        }

        public Vector3? Position() {

            if (Sphere != null & Sphere.activeSelf ) return Sphere.transform.position;

            return null;

        }
        public void ResetMaterialColor()
        {
            if (r.material == m) return;
            r.material = m;

        }

        public void ChanegMaterialColor(Color c)
        {

            r.material.color = c;

        }

        public void OnDisable()
        {
            if (Sphere != null) Destroy(Sphere);
        }

    }

}

