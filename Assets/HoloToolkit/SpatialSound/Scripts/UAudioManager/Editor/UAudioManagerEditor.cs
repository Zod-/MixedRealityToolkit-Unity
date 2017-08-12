﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.Unity
{
    [CustomEditor(typeof(UAudioManager))]
    public class UAudioManagerEditor : UAudioManagerBaseEditor<AudioEvent>
    {
        private void OnEnable()
        {
            myTarget = (UAudioManager)target;
            SetUpEditor();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalEventInstanceLimit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalInstanceBehavior"));
            DrawInspectorGUI(false);
        }
    }
}