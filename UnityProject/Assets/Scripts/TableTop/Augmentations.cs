using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TableTop {

    public enum AUGMENTOPTIONS
    {
        sphere = 0,
        circle = 1,
        lightProjector=2
    }

    public enum AUGMENTINPUT
    {
        head = 0,
        mouse = 1,
        controller = 2,
        headTilt =3
    }

    public class Augmentations : MonoBehaviour
    {

        [SerializeField]
        public AUGMENTINPUT optionInput = AUGMENTINPUT.mouse;

        [SerializeField]
        public bool sphereAugmentation = true;

        [SerializeField]
        public bool coneAugmentation = true;

        [SerializeField]
        public bool circleAugmentation = true;

        [SerializeField]
        public bool AugmentationAlwaysVisible = false;

        public RayOnMap RayOnMap
        {
            get { return _rayOnMap; }
            set { _rayOnMap = value; }
        }

        
        public Vector3? PointOnMap;

        public Vector3? RemotePointOnMap;

        //private

        [SerializeField]
        private RayOnMap _rayOnMap;


        void Update()
        {


            switch (optionInput)
            {

                case AUGMENTINPUT.mouse:

                    PointOnMap = _rayOnMap.MouseRay();

                    break;


                case AUGMENTINPUT.head:

                    PointOnMap = _rayOnMap.HeadRay();

                    RemotePointOnMap = _rayOnMap.RemoteHeadRay();

                    break;

                case AUGMENTINPUT.controller:

                    PointOnMap = _rayOnMap.ControllerRay();

                    RemotePointOnMap = _rayOnMap.RemoteControllerRay();

                    break;

              

            }

            if (sphereAugmentation)
            {
               

                switch (optionInput)
                {


                    case AUGMENTINPUT.head:

                        SphereAugmentation(Sphere, PointOnMap, inputsManager.Instance.LocalHead);

                        SphereAugmentation(RemoteSphere, RemotePointOnMap, inputsManager.Instance.RemoteHead);

                        break;

                   

                    case AUGMENTINPUT.controller:

                        SphereAugmentation(Sphere, PointOnMap, inputsManager.Instance.Controller);

                        SphereAugmentation(RemoteSphere, RemotePointOnMap, inputsManager.Instance.RemoteController);

                        break;


                }
            }



        }


        //Agumentation Sphere
        [SerializeField]
        public AugmentationSphere Sphere;
        [SerializeField]
        public AugmentationSphere RemoteSphere;

        private void SphereAugmentation(AugmentationSphere asphere, Vector3? pointOnMap, Transform pointerTransform)
        {

            if (asphere == null)
            {

                CreateAugmentationSpheres();

                return;

            }
            else if (pointOnMap == null && AugmentationAlwaysVisible == false)
            {

                asphere.HideSphere();

                return;

            }
            else if (pointOnMap == null && AugmentationAlwaysVisible == true && pointerTransform != null)
            {

                Vector3 newpos = pointerTransform.forward.normalized * 2 + pointerTransform.position;

                asphere.UpdateSphereLocation(newpos);

                asphere.ChanegMaterialColor(Color.black);

                return;

            }
            else if (pointOnMap == null && AugmentationAlwaysVisible == true && pointerTransform == null)
            {

                asphere.HideSphere();

                return;

            }
            else
            {

                Vector3 pointOnMapsafe = (Vector3)pointOnMap;

                asphere.UpdateSphereLocation(pointOnMapsafe);

                asphere.ResetMaterialColor();

                return;

            }

        }

        private void CreateAugmentationSpheres()
        {

            Sphere = gameObject.AddComponent<AugmentationSphere>();
            RemoteSphere = gameObject.AddComponent<AugmentationSphere>();

        }

    }
}

