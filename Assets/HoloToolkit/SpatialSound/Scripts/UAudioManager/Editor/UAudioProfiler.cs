﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class UAudioProfiler : EditorWindow
    {
        private int currentFrame;
        private List<ProfilerEvent[]> eventTimeline;
        private Vector2 scrollOffset;
        private const int MaxFrames = 300;

        private class ProfilerEvent
        {
            public string EventName = "";
            public string EmitterName = "";
            public string BusName = "";
        }

        [MenuItem("HoloToolkit/UAudioTools/Profiler", false, 200)]
        static void ShowEditor()
        {
            UAudioProfiler profilerWindow = GetWindow<UAudioProfiler>();
            if (profilerWindow.eventTimeline == null)
            {
                profilerWindow.currentFrame = 0;
                profilerWindow.eventTimeline = new List<ProfilerEvent[]>();
            }
            profilerWindow.Show();
        }

        // Only update the currently-playing events 10 times a second - we don't need millisecond-accurate profiling
        private void OnInspectorUpdate()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            ProfilerEvent[] currentEvents = new ProfilerEvent[0];

            if (eventTimeline == null)
            {
                eventTimeline = new List<ProfilerEvent[]>();
            }

            if (UAudioManager.Instance != null && !EditorApplication.isPaused)
            {
                CollectProfilerEvents(currentEvents);
            }

            Repaint();
        }

        // Populate an array of the active events, and add it to the timeline list of all captured audio frames.
        private void CollectProfilerEvents(ProfilerEvent[] currentEvents)
        {
            List<ActiveEvent> activeEvents = UAudioManager.Instance.ProfilerEvents;
            currentEvents = new ProfilerEvent[activeEvents.Count];
            for (int i = 0; i < currentEvents.Length; i++)
            {
                ActiveEvent currentEvent = activeEvents[i];
                ProfilerEvent tempEvent = new ProfilerEvent();
                tempEvent.EventName = currentEvent.audioEvent.Name;
                tempEvent.EmitterName = currentEvent.AudioEmitter.name;

                // The bus might be null, Unity defaults to Editor-hidden master bus.
                if (currentEvent.audioEvent.Bus == null)
                {
                    tempEvent.BusName = "-MasterBus-";
                }
                else
                {
                    tempEvent.BusName = currentEvent.audioEvent.Bus.name;
                }

                currentEvents[i] = tempEvent;
            }
            eventTimeline.Add(currentEvents);

            // Trim the first event if we have exceeded the maximum stored frames.
            if (eventTimeline.Count > MaxFrames)
            {
                eventTimeline.RemoveAt(0);
            }
            currentFrame = eventTimeline.Count - 1;
        }

        // Draw the profiler window.
        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorGUILayoutExtensions.Label("Profiler only active in play mode!");
                return;
            }

            //Fix null reference exception when launching with profiler is open
            if(eventTimeline!=null)
            {
                currentFrame = EditorGUILayout.IntSlider(currentFrame, 0, eventTimeline.Count - 1);
                scrollOffset = EditorGUILayout.BeginScrollView(scrollOffset);

                if (eventTimeline.Count > currentFrame)
                {
                    for (int i = 0; i < eventTimeline[currentFrame].Length; i++)
                    {
                        DrawEventButton(eventTimeline[currentFrame][i], i);
                    }
                }

                EditorGUILayout.EndScrollView();
            }
           
        }

        private void DrawEventButton(ProfilerEvent currentEvent, int id)
        {
            EditorGUILayout.SelectableLabel(currentEvent.EventName + "-->(" + currentEvent.EmitterName + ")-->(" + currentEvent.BusName + ")");
        }
    }
}