using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public Material material;
    public float size = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CreateBox(new Vector3(Random.Range(-10, 10), Random.Range(2, 20), Random.Range(-10, 10)));
    }

    private void CreateBox(Vector3 position)
    {
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(-size, -size, -size);
        vertices[1] = new Vector3(-size, size, -size);
        vertices[2] = new Vector3(size, size, -size);
        vertices[3] = new Vector3(size, -size, -size);
        vertices[4] = new Vector3(-size, -size, size);
        vertices[5] = new Vector3(-size, size, size);
        vertices[6] = new Vector3(size, size, size);
        vertices[7] = new Vector3(size, -size, size);

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

        GameObject go = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.position = position;

        go.GetComponent<MeshFilter>().mesh = mesh;
        go.GetComponent<MeshRenderer>().material = material;

        Rigidbody rigidbody = go.AddComponent<Rigidbody>();
        rigidbody.mass = 1.0f;

        var boxCollider = go.AddComponent<BoxCollider>();
    }
}
