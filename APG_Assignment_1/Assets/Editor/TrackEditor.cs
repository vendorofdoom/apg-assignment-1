using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

[CustomEditor(typeof(TrackCreator))]
public class TrackEditor : Editor
{
    TrackCreator creator;
    Track track;

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
            Undo.RecordObject(creator, "Add segment");
            track.AddSegment(mousePos);
        }
    }

    void Draw()
    {

        for (int i = 0; i < track.NumSegments; i++)
        {
            Vector3[] points = track.GetPointsInSegment(i);
            Handles.color = Color.black;
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2f);
        }

        Handles.color = Color.red;
        for (int i = 0; i < track.NumPoints; i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(track[i], Quaternion.identity, .1f, Vector3.zero, Handles.CylinderHandleCap);
            if (track[i] != newPos)
            {
                Undo.RecordObject(creator, "Move point");
                track.MovePoint(i, newPos);
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
        track = creator.track;

    }
}
