using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

[CustomEditor(typeof(TrackCreator))]
public class TrackEditor : Editor
{
    TrackCreator creator;
    Track Track
    {
        get
        {
            return creator.track;
        }
    }

    const float segmentSelectDistanceThreshold = 1f; // made bigger cos 3D dist is not fixed by me yet :(
    int selectedSegmentIdx = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Create new"))
        {
            Undo.RecordObject(creator, "Toggle create new");
            creator.CreateTrack();
        }

        bool isClosed = GUILayout.Toggle(Track.IsClosed, "Closed");
        if (isClosed != Track.IsClosed)
        {
            Undo.RecordObject(creator, "Toggle closed");
            Track.IsClosed = isClosed;
        }

        bool autoSetControlPoints = GUILayout.Toggle(Track.AutoSetControlPoints, "Auto set control points");
        if (autoSetControlPoints != Track.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle auto set controls");
            Track.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Input();
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin; // doesn't exactly work in 3D but let's come back to this...

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            if (selectedSegmentIdx != -1)
            {
                Undo.RecordObject(creator, "Split segment");
                Track.SplitSegment(mousePos, selectedSegmentIdx);
            }
            else if (!Track.IsClosed)
            {
                Undo.RecordObject(creator, "Add segment");
                Track.AddSegment(mousePos);
            }
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1) // again need to figure out how to get this to work in 3D...
        {
            float minDistToAnchor = creator.anchorDiameter * 0.5f;
            int closestAnchorIndex = -1;

            for (int i = 0; i < Track.NumPoints; i += 3)
            {
                float dist = Vector3.Distance(mousePos, Track[i]);
                if (dist < minDistToAnchor)
                {
                    minDistToAnchor = dist;
                    closestAnchorIndex = i;
                }
            }

            if (closestAnchorIndex != -1)
            {
                Undo.RecordObject(creator, "Delete segment");
                Track.DeleteSegment(closestAnchorIndex);
            }
        }


        if (guiEvent.type == EventType.MouseMove)
        {
            float minDistToSegment = segmentSelectDistanceThreshold;
            int newSelectedSegmentIdx = -1;

            for (int i = 0; i < Track.NumSegments; i++)
            {
                Vector3[] points = Track.GetPointsInSegment(i);
                float dist = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                if (dist < minDistToSegment)
                {
                    minDistToSegment = dist;
                    newSelectedSegmentIdx = i;
                }
            }

            if (newSelectedSegmentIdx != selectedSegmentIdx)
            {
                selectedSegmentIdx = newSelectedSegmentIdx;
                HandleUtility.Repaint();
            }
        }

    }

    void Draw()
    {

        for (int i = 0; i < Track.NumSegments; i++)
        {
            Vector3[] points = Track.GetPointsInSegment(i);
            if (creator.displayControlPoints)
            {
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
            }
            Color segmentCol = (i == selectedSegmentIdx && Event.current.shift) ? creator.selectedSegmentCol : creator.segmentCol;
            Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentCol, null, 2f);
        }

        
        for (int i = 0; i < Track.NumPoints; i++)
        {
            if (i % 3 == 0 || creator.displayControlPoints)
            {
                Handles.color = (i % 3 == 0) ? creator.anchorCol : creator.controlCol;
                float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiameter;
                Vector3 newPos = Handles.FreeMoveHandle(Track[i], Quaternion.identity, handleSize, Vector3.zero, Handles.CylinderHandleCap);
                if (Track[i] != newPos)
                {
                    Undo.RecordObject(creator, "Move point");
                    Track.MovePoint(i, newPos);
                }
            }

        }
    }

    void OnEnable()
    {
        creator = (TrackCreator)target;
        if (creator.track == null)
        {
            creator.CreateTrack();
        }

    }
}
