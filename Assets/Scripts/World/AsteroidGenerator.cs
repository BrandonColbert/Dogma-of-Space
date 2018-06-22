using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class AsteroidGenerator : MonoBehaviour {
    public int sides = 5;
    public float radius = 1f;

    float[] GenerateAngles() {
        float[] angles = new float[sides];
        
        //float interiorAngleSum = ((float)sides - 2f) * Mathf.PI;
        float equilateralAngle = 2f * Mathf.PI / (float)sides;

        for(int i = 0; i < angles.Length; i++) {
            angles[i] = equilateralAngle;
        }

        for(int i = 0; i < angles.Length; i++) {
            float exchange = MathHelper.Rand(0f, angles[i] - Mathf.Deg2Rad);
            int j = MathHelper.Rand(0, sides - 1);

            if(angles[j] + exchange < 2f * Mathf.PI) {
                angles[i] -= exchange;
                angles[j] += exchange;
            } else {
                float leftover = angles[j] + exchange - 2f * Mathf.PI - Mathf.Deg2Rad;

                angles[i] += -exchange + leftover;
                angles[j] = 2f * Mathf.PI - Mathf.Deg2Rad;
            }
        }
        
        /*
        Debug.Log("======");
        foreach(float f in angles) Debug.Log(f * Mathf.Rad2Deg);
        Debug.Log("Total Angle is: " + (Mathf.Rad2Deg * angles.Aggregate((total, angle) => total + angle)));
        */

        return angles;
    }

    Vector3[] GenerateVertices(float[] angles) {
        Vector3[] vertices = new Vector3[sides];

        float angle = 0f;

        for(int i = 0; i < vertices.Length; i++) {
            angle += angles[i];
            vertices[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        Vector3 center = vertices.Aggregate((total, vertex) => total + vertex) / vertices.Length;

        for(int i = 0; i < vertices.Length; i++) {
            vertices[i] -= center;
        }

        vertices = vertices.OrderBy(v => Mathf.Atan2(-v.y, -v.x)).ToArray();

        return vertices;
    }

    Mesh GenerateMesh(Vector3[] vertices) {
        Mesh mesh = new Mesh();
        
        mesh.vertices = vertices;
        mesh.triangles = new UnityTriangulator(vertices).Triangulate();
        mesh.RecalculateNormals();
        mesh.normals = Enumerable.Repeat(Vector3.back, mesh.normals.Count()).ToArray();
        mesh.RecalculateBounds();

        return mesh;
    }

    public void GenerateAsteroid(int sides, float radius, bool removeAfter = true) {
        this.sides = sides;
        this.radius = radius;
        GenerateAsteroid(removeAfter);
    }

    public void GenerateAsteroid(bool removeAfter = true) {
        if(sides < 3) sides = 3;

        Vector3[] vertices = GenerateVertices(GenerateAngles());
        GetComponent<MeshFilter>().mesh = GenerateMesh(vertices);
        GetComponent<PolygonCollider2D>().points = Array.ConvertAll(vertices, v => (Vector2)v).ToArray();

        BreakableObject bo = GetComponent<BreakableObject>();
        if(bo) {
            float thickness = GetComponent<MeshRenderer>().material.GetFloat("_OutlineThickness") / Mathf.Clamp(Mathf.Sqrt(bo.GetArea() * 0.25f), 1f, 100f);
            GetComponent<MeshRenderer>().material.SetFloat("_OutlineThickness", thickness);
        }

        if(removeAfter) {
            Destroy(this);
        }
    }

    /*
    void Start() {
        GenerateAsteroid();
    }
    */

    /*
    void OnDrawGizmosSelected() {
        if(GetComponent<MeshFilter>().sharedMesh == null) GenerateAsteroid(false);
        //BreakableObject.RenderMesh(GetComponent<MeshFilter>().sharedMesh, transform.position, transform.localScale);
    }
    */
}