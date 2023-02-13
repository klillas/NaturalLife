using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public Material material;
    public float size = 1.0f;
    //public Vector3 maxBoxes = new Vector3(10, 10, 10);
    public int maxBoxes = 10;

    private List<BoxDesign> boxDesigns = new List<BoxDesign>();
    private GameObject combined;

    // Start is called before the first frame update
    void Start()
    {
        var prevBd = new BoxDesign(new Vector3(0, size * 20f, 0), size);
        boxDesigns.Add(prevBd);
        for (int i = 0; i < maxBoxes-1; i++)
        {
            var bd = new BoxDesign(new Vector3(0, size * 20f, 0), size);
            boxDesigns.Add(bd);
            var side = (BoxDesign.Side) UnityEngine.Random.Range(0, 6);
            while (prevBd.ConnectedBoxes.ContainsKey(side))
            {
                side = (BoxDesign.Side)UnityEngine.Random.Range(0, 6);
            }
            prevBd.ConnectBox(bd, side);
            prevBd = bd;
        }

        /*
        boxDesigns.Add(new BoxDesign(new Vector3(0, size * 0.5f, 0), size));
        boxDesigns.Add(new BoxDesign(new Vector3(0, size * 0.5f, 0), size));
        boxDesigns.Add(new BoxDesign(new Vector3(0, size * 0.5f, 0), size));
        boxDesigns.Add(new BoxDesign(new Vector3(0, size * 0.5f, 0), size));
        boxDesigns[0].ConnectBox(boxDesigns[1], BoxDesign.Side.Up);
        boxDesigns[1].ConnectBox(boxDesigns[2], BoxDesign.Side.Up);
        boxDesigns[2].ConnectBox(boxDesigns[3], BoxDesign.Side.Right);
        */
        Combine(boxDesigns);

        foreach (var box in boxDesigns)
        {
            Destroy(box.go);
        }
        boxDesigns.Clear();

        /*
        List<GameObject> gos = new List<GameObject>();
        gos.Add(CreateBox(new Vector3(0, size * 0.5f, 0)));
        gos.Add(CreateBox(new Vector3(0, size + size * 0.5f, 0)));
        Combine(gos);
        */
    }

    // Update is called once per frame
    void Update()
    {
        //CreateBox(new Vector3(Random.Range(-10, 10), Random.Range(2, 20), Random.Range(-10, 10)));
        
    }

    private void OnDrawGizmos()
    {
        /*
        if (combined != null)
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawMesh(combined.GetComponent<MeshFilter>().mesh, transform.position, transform.rotation);
        }
        */
    }

    private void Combine(List<BoxDesign> boxDesigns)
    {
        // Create a new GameObject to hold the combined mesh
        combined = new GameObject("Combined");

        // Get all of the MeshFilters of the objects you want to combine
        var meshFilters = new List<MeshFilter>();
        foreach (var bd in boxDesigns)
        {
            meshFilters.Add(bd.go.GetComponent<MeshFilter>());
        }

        // Combine the meshes into a single mesh
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        for (int i = 0; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        // Add a MeshFilter component to the new GameObject and set the combined mesh
        MeshFilter meshFilter = combined.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;

        // Add a MeshRenderer component to the new GameObject and set the material
        //MeshRenderer meshRenderer = combined.AddComponent<MeshRenderer>();
        //meshRenderer.material = GetComponent<MeshRenderer>().sharedMaterial;

        // Set the position and rotation of the new GameObject to match the original objects
        //combined.transform.position = transform.position;
        combined.transform.position = new Vector3(0f, 0f, 0f);
        //combined.transform.rotation = transform.rotation;

        // Set the parent of the new GameObject to match the original parent, if there was one
        //combined.transform.parent = transform.parent;

        //combined.GetComponent<MeshRenderer>().material = material;

        var meshCollider = combined.AddComponent<MeshCollider>();

        Rigidbody rigidbody = combined.AddComponent<Rigidbody>();
        rigidbody.mass = 1.0f * boxDesigns.Count;

        //var boxCollider = combined.AddComponent<BoxCollider>();
    }

    class BoxDesign
    {
        /// <summary>
        /// Which side a box is connected to
        /// The structure must be like this, numerical logic will find the opposite side by calculating ((side+3)%6)
        /// </summary>
        public enum Side
        {
            Right = 0,
            Up = 1,
            Back = 2,
            Left = 3,
            Down = 4,
            Front = 5
        }

        public Dictionary<Side, BoxDesign> ConnectedBoxes = new Dictionary<Side, BoxDesign>();

        public GameObject go;

        public void ConnectBox(BoxDesign boxDesign, Side side)
        {
            if (ConnectedBoxes.ContainsKey(side))
            {
                if (ConnectedBoxes[side] == boxDesign)
                {
                    return;
                }
                throw new ArgumentException($"{side} is already taken");
            }

            // Move the box to the correct side of this box
            Vector3 direction = Vector3.zero;
            switch (side)
            {
                case Side.Left:
                    direction.x = -1f;
                    break;
                case Side.Right:
                    direction.x = 1f;
                    break;
                case Side.Up:
                    direction.y = 1f;
                    break;
                case Side.Down:
                    direction.y = -1f;
                    break;
                case Side.Back:
                    direction.z = -1f;
                    break;
                case Side.Front:
                    direction.z = 1f;
                    break;
                default:
                    throw new ArgumentException($"{side} is not supported.");
            }
            Vector3 position = direction;
            position.x = go.transform.position.x + (position.x * go.transform.localScale.x);
            position.y = go.transform.position.y + (position.y * go.transform.localScale.y);
            position.z = go.transform.position.z + (position.z * go.transform.localScale.z);
            boxDesign.go.transform.position = position;

            ConnectedBoxes.Add(side, boxDesign);
            boxDesign.ConnectBox(this, (Side)(((int)side + 3) % 6));
        }

        public BoxDesign(Vector3 position, float size)
        {
            Vector3[] vertices = new Vector3[8];
            float halfSize = 0.5f * size;
            vertices[0] = new Vector3(-halfSize, -halfSize, -halfSize);
            vertices[1] = new Vector3(-halfSize, halfSize, -halfSize);
            vertices[2] = new Vector3(halfSize, halfSize, -halfSize);
            vertices[3] = new Vector3(halfSize, -halfSize, -halfSize);
            vertices[4] = new Vector3(-halfSize, -halfSize, halfSize);
            vertices[5] = new Vector3(-halfSize, halfSize, halfSize);
            vertices[6] = new Vector3(halfSize, halfSize, halfSize);
            vertices[7] = new Vector3(halfSize, -halfSize, halfSize);

            Vector2[] uv = new Vector2[8];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);
            uv[4] = new Vector2(0, 0);
            uv[5] = new Vector2(0, 1);
            uv[6] = new Vector2(1, 1);
            uv[7] = new Vector2(1, 0);

            int[] triangles = new int[36];
            // Front
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 3;
            triangles[5] = 0;
            // Back
            triangles[6] = 7;
            triangles[7] = 6;
            triangles[8] = 5;
            triangles[9] = 5;
            triangles[10] = 4;
            triangles[11] = 7;
            // Left
            triangles[12] = 4;
            triangles[13] = 5;
            triangles[14] = 1;
            triangles[15] = 1;
            triangles[16] = 0;
            triangles[17] = 4;
            // Right
            triangles[18] = 3;
            triangles[19] = 2;
            triangles[20] = 6;
            triangles[21] = 6;
            triangles[22] = 7;
            triangles[23] = 3;
            // Top
            triangles[24] = 1;
            triangles[25] = 5;
            triangles[26] = 6;
            triangles[27] = 6;
            triangles[28] = 2;
            triangles[29] = 1;
            // Bottom
            triangles[30] = 4;
            triangles[31] = 0;
            triangles[32] = 3;
            triangles[33] = 3;
            triangles[34] = 7;
            triangles[35] = 4;

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            go = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.position = position;

            go.GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
