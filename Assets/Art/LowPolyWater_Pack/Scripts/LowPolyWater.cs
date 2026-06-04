using UnityEngine;

namespace LowPolyWater
{
    public class LowPolyWater : MonoBehaviour
    {
        public float waveHeight = 0.5f;
        public float waveFrequency = 0.5f;
        public float waveLength = 0.75f;

        //Position where the waves originate from
        public Vector3 waveOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);

        MeshFilter meshFilter;
        Mesh mesh;
        Vector3[] vertices;

        // Per-vertex wave phase, precomputed once. The distance term only depends
        // on the (static) vertex XZ position and wave settings, so recomputing it
        // every frame (a sqrt + modulo per vertex) was pure waste on WebGL.
        float[] phases;

        private void Awake()
        {
            //Get the Mesh Filter of the gameobject
            meshFilter = GetComponent<MeshFilter>();
        }

        void Start()
        {
            CreateMeshLowPoly(meshFilter);
            PrecomputePhases();
            // Hint to the engine that this mesh changes often. Only needs to be
            // set once, not every frame.
            mesh.MarkDynamic();
        }

        void PrecomputePhases()
        {
            phases = new float[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];
                v.y = 0.0f;
                float distance = Vector3.Distance(v, waveOriginPosition);
                distance = (distance % waveLength) / waveLength;
                phases[i] = Mathf.PI * 2.0f * distance;
            }
        }

        /// <summary>
        /// Rearranges the mesh vertices to create a 'low poly' effect
        /// </summary>
        /// <param name="mf">Mesh filter of gamobject</param>
        /// <returns></returns>
        MeshFilter CreateMeshLowPoly(MeshFilter mf)
        {
            mesh = mf.sharedMesh;

            //Get the original vertices of the gameobject's mesh
            Vector3[] originalVertices = mesh.vertices;

            //Get the list of triangle indices of the gameobject's mesh
            int[] triangles = mesh.triangles;

            //Create a vector array for new vertices 
            Vector3[] vertices = new Vector3[triangles.Length];
            
            //Assign vertices to create triangles out of the mesh
            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = originalVertices[triangles[i]];
                triangles[i] = i;
            }

            //Update the gameobject's mesh with new vertices
            mesh.vertices = vertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            this.vertices = mesh.vertices;

            return mf;
        }
        
        void Update()
        {
            GenerateWaves();
        }

        /// <summary>
        /// Based on the specified wave height and frequency, generate 
        /// wave motion originating from waveOriginPosition
        /// </summary>
        void GenerateWaves()
        {
            //Time term is the same for every vertex, so compute it once per frame
            float timePhase = Time.time * Mathf.PI * 2.0f * waveFrequency;

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];

                //Oscilate the wave height via sine to create a wave effect
                v.y = waveHeight * Mathf.Sin(timePhase + phases[i]);

                //Update the vertex
                vertices[i] = v;
            }

            //Update the mesh properties. RecalculateNormals is kept so lighting on
            //the waves stays correct; MarkDynamic / meshFilter.mesh reassignment were
            //redundant per-frame work and have been moved to Start / removed.
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }
    }
}