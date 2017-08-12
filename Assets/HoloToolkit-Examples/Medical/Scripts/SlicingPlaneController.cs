// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Controls slicing planes for use in volumetric rendering
    /// Planes represent maniuplatable viewable regions of the volume
    /// </summary>
    public class SlicingPlaneController : MonoBehaviour, IManipulationHandler
    {
        public float KeyboardMovementSpeed = 1.5f;
        public float GestureMovementSpeed = 1.0f;

        public GameObject PlaneX;
        public GameObject PlaneY;
        public GameObject PlaneZ;

        private bool wasThickSliceEnabled;

        public bool ThickSliceEnabled = true;

        public Material SliceMaterial;
        public Material ThickSliceMaterial;

        public void SetMaterial(Material mat)
        {
            PlaneX.GetComponent<Renderer>().sharedMaterial = mat;
            PlaneY.GetComponent<Renderer>().sharedMaterial = mat;
            PlaneZ.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private void OnEnable()
        {
            //force initial material setting in case user has made strange editor changes
            wasThickSliceEnabled = !ThickSliceEnabled;

            InputManager.Instance.AddGlobalListener(gameObject);
        }

        private void OnDisable()
        {
            InputManager.Instance.RemoveGlobalListener(gameObject);
        }

        public void SetThickSliceEnabled(bool on)
        {
            ThickSliceEnabled = on;
        }

        private void Update()
        {
            HandleDebugKeys();

            Vector4 planeEquation;

            var forward = transform.forward;
            var pos = transform.position;

            planeEquation.x = forward.x;
            planeEquation.y = forward.y;
            planeEquation.z = forward.z;

            //dot product
            planeEquation.w = (planeEquation.x * pos.x +
                               planeEquation.y * pos.y +
                               planeEquation.z * pos.z);

            Shader.SetGlobalVector("CutPlane", planeEquation);

            var cornerPos = transform.localPosition;
            var slabMin = new Vector4(cornerPos.x - 0.5f, cornerPos.y - 0.5f, cornerPos.z - 0.5f, 0.0f);
            var slabMax = new Vector4(cornerPos.x + 0.5f, cornerPos.y + 0.5f, cornerPos.z + 0.5f, 0.0f);

            Shader.SetGlobalVector("SlabMin", slabMin);
            Shader.SetGlobalVector("SlabMax", slabMax);

            if (ThickSliceEnabled != wasThickSliceEnabled)
            {
                SetMaterial(ThickSliceEnabled ? ThickSliceMaterial : SliceMaterial);
                wasThickSliceEnabled = ThickSliceEnabled;
            }

            Shader.SetGlobalMatrix("_SlicingWorldToLocal", transform.worldToLocalMatrix);
        }

        void HandleDebugKeys()
        {
            float movementDelta = KeyboardMovementSpeed * Time.deltaTime;

            var positionDeltaX = transform.localRotation * new Vector3(movementDelta, 0.0f, 0.0f);
            var positionDeltaY = transform.localRotation * new Vector3(0.0f, movementDelta, 0.0f);
            var positionDeltaZ = transform.localRotation * new Vector3(0.0f, 0.0f, movementDelta);

            if (Input.GetKey(KeyCode.L)) { transform.localPosition += positionDeltaX; }
            if (Input.GetKey(KeyCode.J)) { transform.localPosition -= positionDeltaX; }

            if (Input.GetKey(KeyCode.I)) { transform.localPosition += positionDeltaY; }
            if (Input.GetKey(KeyCode.K)) { transform.localPosition -= positionDeltaY; }

            if (Input.GetKey(KeyCode.RightBracket)) { transform.localPosition += positionDeltaZ; }
            if (Input.GetKey(KeyCode.LeftBracket)) { transform.localPosition -= positionDeltaZ; }
        }
        public void OnManipulationStarted(ManipulationEventData eventData)
        {
        }

        public void OnManipulationUpdated(ManipulationEventData eventData)
        {
            float movementDelta = GestureMovementSpeed * Time.deltaTime;

            //TODO: consider tether filter
            transform.localPosition += transform.localRotation * eventData.CumulativeDelta * movementDelta;
        }

        public void OnManipulationCompleted(ManipulationEventData eventData)
        {
        }

        public void OnManipulationCanceled(ManipulationEventData eventData)
        {
        }
    }
}