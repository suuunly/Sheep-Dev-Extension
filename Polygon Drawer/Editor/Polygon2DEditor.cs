using System.Collections.Generic;

namespace SDE.Mesh
{
    using UnityEditor;
    using UnityEngine;

    public class SelectionDetails
    {
        public int IndexPoint = -1;
        public bool MouseIsOverPoint;
        public bool IsSelectedPoint;
        public Vector3 StartOfDragPosition;

        public int IndexLine = -1;
        public bool MouseIsOverLine;
    }


    [CustomEditor(typeof(Polygon2D))]
    public class Polygon2DEditor : Editor
    {
        private Polygon2D mTarget;
        private SelectionDetails mSelection;

        private bool mModify;
        private Vector3 mTransformPos;

        private void OnEnable()
        {
            mTarget = target as Polygon2D;
            mSelection = new SelectionDetails();
        }

        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(mTarget);
            base.OnInspectorGUI();

            if (GUILayout.Button((mModify) ? "Stop Modyfing" : "Start Modifying"))
                mModify = !mModify;
            
            mTarget.EDITOR_UseCollider = EditorGUILayout.Toggle("Use Collider:", mTarget.EDITOR_UseCollider);
            if (mTarget.EDITOR_UseCollider)
            {
                if (!mTarget.EDITOR_Collider)
                {
                    mTarget.EDITOR_Collider = mTarget.gameObject.AddComponent<PolygonCollider2D>();
                    ConstructCollider();
                }
            }
            else if(mTarget.EDITOR_Collider) 
                DestroyImmediate(mTarget.EDITOR_Collider);
            
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Line:\t");
            mTarget.EDITOR_LineWidth = EditorGUILayout.IntField(mTarget.EDITOR_LineWidth);
            mTarget.EDITOR_LineColour = EditorGUILayout.ColorField(mTarget.EDITOR_LineColour);
            mTarget.EDITOR_LineColourHover = EditorGUILayout.ColorField(mTarget.EDITOR_LineColourHover);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Point:\t");
            mTarget.EDITOR_PointRadius = EditorGUILayout.FloatField(mTarget.EDITOR_PointRadius);
            mTarget.EDITOR_PointColour = EditorGUILayout.ColorField(mTarget.EDITOR_PointColour);
            mTarget.EDITOR_PointColourHover = EditorGUILayout.ColorField(mTarget.EDITOR_PointColourHover);
            mTarget.EDITOR_PointColourSelect = EditorGUILayout.ColorField(mTarget.EDITOR_PointColourSelect);
            GUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            if (!mModify)
                return;

            Event e = Event.current;
            switch (e.type)
            {
                case EventType.Repaint:
                    DrawPoints();
                    break;

                // make sure that when the object is selected, it doesn't
                // deselect when trying to put down points
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    break;
                default:
                    CalculateInput(e);
                    break;
            }
        }

        private void CalculateInput(Event e)
        {
            // Convert screen space to world space
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            float distanceToPlane = (mTransformPos.z - mouseRay.origin.z) / mouseRay.direction.z;

            Vector3 mousePos = mouseRay.GetPoint(distanceToPlane);

            // check if left mouse click is pressed; if so, add a vertex to the scene
            if (e.button == 0 && e.modifiers == EventModifiers.None)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        OnMouseDownEvent(mousePos);
                        break;
                    case EventType.MouseUp:
                        OnMouseUpEvent(mousePos);
                        break;
                    case EventType.MouseDrag:
                        OnMouseDragEvent(mousePos);
                        break;
                }

                if (!mSelection.IsSelectedPoint)
                    UpdateMouseOverDetails(mousePos);
            }
        }

        private void OnMouseDragEvent(Vector3 mousePos)
        {
            if (mSelection.IsSelectedPoint)
            {
                mTarget.Points[mSelection.IndexPoint] = mousePos;
                RepaintPoints();
                ConstructMesh();
            }
        }

        private void ConstructMesh()
        {
            mTarget.ConstructMesh();
            ConstructCollider();
        }

        private void ConstructCollider()
        {
            if (mTarget.EDITOR_UseCollider)
                mTarget.EDITOR_Collider.points = mTarget.Points.CastToArray(original => (Vector2)original);
        }

        private void OnMouseUpEvent(Vector3 mousePos)
        {
            if (mSelection.IsSelectedPoint)
            {
                List<Vector3> points = mTarget.Points;
                int pointIndex = mSelection.IndexPoint;

                // In order for undo record to work, the position has to first be set to its start position,
                // then updated to the new position
                points[pointIndex] = mSelection.StartOfDragPosition;
                Undo.RecordObject(mTarget, "Move Point");
                points[pointIndex] = mousePos;

                mSelection.IsSelectedPoint = false;
                mSelection.IndexPoint = -1;

                // repaint the final movement
                RepaintPoints();
            }
        }
        
        private void OnMouseDownEvent(Vector3 mousePos)
        {
            if (!mSelection.MouseIsOverPoint)
            {
                // if it is over a line, then get the index of the next point connected
                // (allowing for inserting it between the last and next point) : otherwise, just add it to the end
                mSelection.IndexPoint = mSelection.MouseIsOverLine ? mSelection.IndexLine + 1 : mTarget.Points.Count;

                // Allow for undo/redo points
                Undo.RecordObject(mTarget, "Add Point");
                mTarget.Points.Insert(mSelection.IndexPoint, mousePos);
                ConstructMesh();
            }

            mSelection.IsSelectedPoint = true;
            mSelection.StartOfDragPosition = mousePos;

            // force repaint everytime a point is added
            RepaintPoints();
        }

        private void UpdateMouseOverDetails(Vector3 mousePos)
        {
            int mouseOverPointIndex = -1;
            List<Vector3> points = mTarget.Points;
            int count = points.Count;

            /* Loop through the points and see if the mouse pos is within the radius of any of the points.
             if not, then it returns -1 */
            mouseOverPointIndex = points.FindIndex((point) => Vector3.Distance(mousePos, point) < mTarget.EDITOR_PointRadius);

            // if it is different, then store it, and check if the mouse is over a point or not
            if (mouseOverPointIndex != mSelection.IndexPoint)
            {
                mSelection.IndexPoint = mouseOverPointIndex;
                mSelection.MouseIsOverPoint = mouseOverPointIndex != -1;

                RepaintPoints();
            }

            // if it is, then it cannot be over a line
            if (mSelection.MouseIsOverPoint)
            {
                mSelection.MouseIsOverLine = false;
                mSelection.IndexLine = -1;
            }
            else
            {
                int mouseOverLineIndex = -1;
                float closestLineDst = mTarget.EDITOR_PointRadius;
                for (int i = 0; i < count; i++)
                {
                    Vector3 nextPointInShape = points[(i + 1) % count];
                    float dstFromMouseToLine =
                        HandleUtility.DistancePointToLineSegment(mousePos, points[i], nextPointInShape);
                    if (dstFromMouseToLine < closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;
                    }
                }

                if (mSelection.IndexLine != mouseOverLineIndex)
                {
                    mSelection.IndexLine = mouseOverLineIndex;
                    mSelection.MouseIsOverLine = mouseOverLineIndex != -1;

                    RepaintPoints();
                }
            }
        }

        private static void RepaintPoints()
        {
            HandleUtility.Repaint();
        }

        private void DrawPoints()
        {
            List<Vector3> points = mTarget.Points;
            int count = points.Count;
            for (int i = 0; i < count; i++)
            {
                // make sure that it does not go out of bounds
                Vector3 nextPoint = points[(i + 1) % count];
                Vector3 thisPoint = points[i];

                if (i == mSelection.IndexLine)
                {
                    Handles.color = mTarget.EDITOR_LineColourHover;
                    Handles.DrawLine(thisPoint, nextPoint);
                }
                else
                {
                    Handles.color = mTarget.EDITOR_LineColour;
                    Handles.DrawDottedLine(thisPoint, nextPoint, mTarget.EDITOR_LineWidth);
                }

                Handles.color = (i == mSelection.IndexPoint)
                    ? (mSelection.IsSelectedPoint) ? mTarget.EDITOR_PointColourSelect : mTarget.EDITOR_PointColourHover
                    : mTarget.EDITOR_PointColour;


                Handles.DrawSolidDisc(thisPoint, mTarget.transform.forward, mTarget.EDITOR_PointRadius);
            }
        }
    }
}