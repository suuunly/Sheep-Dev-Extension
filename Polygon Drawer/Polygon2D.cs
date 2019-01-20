namespace SDE.Mesh
{
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Polygon2D : MonoBehaviour
    {
        [HideInInspector] public List<Vector3> Points = new List<Vector3>();
        [SerializeField, HideInInspector] private MeshFilter mFilter;
        
        #if UNITY_EDITOR
        [HideInInspector] public Color EDITOR_LineColour = Color.black;
        [HideInInspector] public Color EDITOR_LineColourHover = Color.red;
        [HideInInspector] public int   EDITOR_LineWidth = 4;

        [HideInInspector] public Color EDITOR_PointColour = Color.white;
        [HideInInspector] public Color EDITOR_PointColourHover = Color.red;
        [HideInInspector] public Color EDITOR_PointColourSelect = Color.black;
        [HideInInspector] public float EDITOR_PointRadius = 0.1f;

        [HideInInspector] public bool  EDITOR_UseCollider;
        [HideInInspector] public PolygonCollider2D EDITOR_Collider;
        #endif
        private void Reset()
        {
            mFilter = GetComponent<MeshFilter>();
            mFilter.mesh = new Mesh { name = name + " - [NotGenerated] Polygon"};
        }

        public void ConstructMesh()
        {
            Mesh result;
            if (PolygonGeneration.Create(Points.ToArray(), out result, name + " - Polygon") == EPolygonResult.SUCCESSFUL)
            {
                mFilter.mesh = result;
            }
            
        }
    }
}