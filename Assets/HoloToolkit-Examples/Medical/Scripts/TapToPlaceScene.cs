// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Lightweight game object placement
    /// </summary>
    public class TapToPlaceScene : MonoBehaviour, IInputClickHandler
    {
        public float DistanceFromHead = 1.0f;

        public bool Placing = true;

        Quaternion initialRotation;

        public void SetPlacing(bool placing)
        {
            Placing = placing;
        }

        private void OnEnable()
        {
            initialRotation = transform.rotation;
        }

        void Update()
        {
            if (Placing)
            {
                var headPosition = Camera.main.transform.position;
                var forward = Camera.main.transform.forward;
                var scenePosition = headPosition + DistanceFromHead * forward;

                var facingRotation = Camera.main.transform.localRotation * initialRotation;
                //only yaw
                facingRotation.x = 0;
                facingRotation.z = 0;

                transform.position = scenePosition;
                transform.rotation = facingRotation;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Placing = false;
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            Placing = false;
        }
    }
}