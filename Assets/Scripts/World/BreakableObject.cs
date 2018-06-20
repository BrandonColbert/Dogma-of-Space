using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
public class BreakableObject : MonoBehaviour {
    ///<summary>Used to check if vectors are equal</summary>
    public class Vector3Comparer : IEqualityComparer<Vector3> {
        public bool Equals(Vector3 v1, Vector3 v2) {
            return v1 == v2;
        }

        public int GetHashCode(Vector3 code) {
            return code.GetHashCode();
        }
    }

    ///<summary>Holds a mesh and where the center of its points are relative to the original mesh</summary>
    public class MeshAt {
        public Mesh mesh;
        public Vector3 center;

        public MeshAt(Mesh mesh, Vector3 center) {
            this.mesh = mesh;
            this.center = center;
        }
    }

    [Tooltip("The minimum area required for a fracture to spawn")]
    public float minFractureArea = 0.1f;

    [Tooltip("Despawn fragments after this amount of time in seconds has elapsed. If set to 0, the fragments won't despawn")]
    public float despawnAfter = 10f;

    [Tooltip("The amount of force this object will break with")]
    public float breakForce = 5f;

    [Tooltip("Position of the line across which the break will initially occur")]
    public Vector3 breakLineStart = Vector3.up, breakLineEnd = Vector3.down;

    [Tooltip("Must go to zero before the object can shatter")]
    public float maxHealth = 100f, health = 0f;

    private Material lastMaterial = null;
    private Color? lastColor = null;

    ///<summary>Uses Gizmos to display what a break will like look</summary>
    public static void RenderMesh(Mesh mesh, Vector3 center, Vector3 scale, Color? triangleColor = null, Color? normalColor = null) {
        if(mesh != null) {
            center = new Vector3(center.x * scale.x, center.y * scale.y, center.z * scale.z);

            Vector3[] vertices = Array.ConvertAll(mesh.vertices, v => new Vector3(v.x * scale.x, v.y * scale.y, v.z * scale.z));
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;

            Gizmos.color = triangleColor == null ? Color.white : triangleColor.Value;
            for(int i = 0; i < triangles.Length; i += 3) {
                Gizmos.DrawLine(center + vertices[triangles[i + 0]], center + vertices[triangles[i + 1]]);
                Gizmos.DrawLine(center + vertices[triangles[i + 1]], center + vertices[triangles[i + 2]]);
                Gizmos.DrawLine(center + vertices[triangles[i + 2]], center + vertices[triangles[i + 0]]);
            }

            Gizmos.color = normalColor == null ? Gizmos.color : normalColor.Value;
            for(int i = 0; i < normals.Length; i++) {
                Gizmos.DrawLine(center + vertices[i], center + vertices[i] + normals[i] * 0.15f);
            }

            Gizmos.color = triangleColor == null ? Color.white : triangleColor.Value;
            foreach(Vector3 vertex in vertices) {
                Gizmos.DrawSphere(center + vertex, 0.025f);
            }

            Gizmos.color = new Color(Gizmos.color.g, Gizmos.color.b, Gizmos.color.r);
            //Gizmos.DrawSphere(center, 0.1f);
        }
    }

    ///<summary>Ensures the break line's point is correctly rotated relative to this object</summary>
    public Vector3 RelativePoint(Vector3 point) {
        return MathHelper.RotateAround(point, Vector3.zero, Quaternion.Inverse(transform.localRotation));
    }

    ///<summary>Gets the area of a mesh by adding the areas of its triangles</summary>
    float GetArea(Mesh mesh) {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        float area = 0f;

        Func<float[], float> triangleArea = delegate(float[] sides) {
            float s = (sides[0] + sides[1] + sides[2]) / 2f;

            return Mathf.Sqrt(s * (s - sides[0]) * (s - sides[1]) *(s - sides[2]));
        };

        for(int i = 0; i < triangles.Length; i += 3) {
            area += triangleArea(new float[]{
                Vector2.Distance(vertices[triangles[i + 0]], vertices[triangles[i + 1]]),
                Vector2.Distance(vertices[triangles[i + 1]], vertices[triangles[i + 2]]),
                Vector2.Distance(vertices[triangles[i + 2]], vertices[triangles[i + 0]])
            });
        }

        //Debug.Log(area + (area < minFractureArea ? " AREA TOO SMALL" : ""));

        return area;
    }

    ///<summary>Gets the area of a mesh by adding the areas of its triangles</summary>
    public float GetArea() {
        return GetArea(GetComponent<MeshFilter>().sharedMesh);
    }

    T[] ConvertToArray<T>(object[] objs) {
        return objs.ToList().ConvertAll(f => (T)f).ToArray();
    }

    ///<summary>Scales the mesh physically and sets the polygon collider's points to that of the mesh</summary>
    public void FormatBreakable() {
        health = maxHealth;

        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
        Vector3 s = transform.localScale;

        for(int i = 0; i < vertices.Count(); i++) {
            Vector3 v = vertices[i];
            vertices[i] = new Vector3(v.x * s.x, v.y * s.y, v.z * s.z);
        }

        transform.localScale = Vector3.one;
        GetComponent<MeshFilter>().mesh.vertices = vertices;
        GetComponent<MeshFilter>().mesh.RecalculateBounds();
        
        GetComponent<PolygonCollider2D>().points = Array.ConvertAll(vertices.OrderBy(v => Mathf.Atan2(-v.y, -v.x)).ToArray(), v => (Vector2)v);
    }

    ///<summary>Gets all the points where the break line will intersect the triangles of the mesh</summary>
    Vector3[] GetBreakPoints(Mesh mesh, Vector2 from, Vector2 to) {
        List<Vector3> breakPoints = new List<Vector3>();
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for(int i = 0; i < triangles.Length; i += 3) {
            Vector2? i1 = MathHelper.VectorIntersection(vertices[triangles[i + 0]], vertices[triangles[i + 1]], from, to),
                     i2 = MathHelper.VectorIntersection(vertices[triangles[i + 1]], vertices[triangles[i + 2]], from, to),
                     i3 = MathHelper.VectorIntersection(vertices[triangles[i + 2]], vertices[triangles[i + 0]], from, to);
            
            if(i1 != null) breakPoints.Add(i1.Value);
            if(i2 != null) breakPoints.Add(i2.Value);
            if(i3 != null) breakPoints.Add(i3.Value);
        }

        //NOTE: Doesn't work with concave geometry (not the rest of this script does anyway)
        if(breakPoints.Count() > 2) {
            Vector3 a = breakPoints[0], b = breakPoints[1];

            foreach(Vector3 c in breakPoints) {
                foreach(Vector3 d in breakPoints) {
                    if(Vector3.Distance(c, d) > Vector3.Distance(a, b)) {
                        a = c;
                        b = d;
                    }
                }
            }

            return new Vector3[] {
                a, b
            };
        }
        
        return breakPoints.ToArray();
    }

    ///<summary>Outputs points to either side of the mesh when broken across the specified line</summary>
    void GetSides(Mesh mesh, Vector2 from, Vector2 to, out Vector3[] weakSide, out Vector3[] strongSide) {
        List<Vector3> weak = new List<Vector3>(), strong = new List<Vector3>();
        Vector3[] vertices = mesh.vertices;
        
        foreach(Vector3 vertex in vertices) {
            float sol = MathHelper.SideOfLine(vertex, from, to);
            if(sol <= 0f) weak.Add(vertex);
            if(sol >= 0f) strong.Add(vertex);
        }

        weakSide = weak.ToArray();
        strongSide = strong.ToArray();
    }

    ///<summary>Creates mesh from the points on one side of the original mesh and the points on the line used to break it. Returns null if it can't be made</summary>
    MeshAt CreateFractureMesh(Vector3[] sidePoints, Vector3[] breakPoints) {
        Vector3[] side = sidePoints.Concat(breakPoints).ToArray();

        if(side.Length < 3) return null;

        Vector3 center = side.Aggregate((total, vertex) => total + vertex) / side.Length;

        for(int i = 0; i < side.Length; i++) {
            side[i] -= center;
        }

        side = side.Distinct(new Vector3Comparer()).OrderBy(v => Mathf.Atan2(-v.y, -v.x)).ToArray();

        Mesh mesh = new Mesh();
        mesh.vertices = side;
        mesh.triangles = new UnityTriangulator(side).Triangulate();
        mesh.RecalculateNormals();
        mesh.normals = Enumerable.Repeat(Vector3.back, mesh.normals.Count()).ToArray();
        mesh.RecalculateBounds();

        return new MeshAt(mesh, center);
    }

    ///<summary>
    ///Creates a new fracture from the mesh and it's center based on this object, then returns it.
    ///Returns null if the mesh is null or if the area is too small
    ///Additionally corrects mass relative to area based on the original if provided
    ///</summary>
    GameObject CreateFractureObject(MeshAt meshAt, GameObject original = null) {
        if(meshAt != null) {
            float area = GetArea(meshAt.mesh);
            if(area < minFractureArea) return null;

            GameObject fractureObject = Instantiate(gameObject, transform.position + meshAt.center, original == null ? transform.rotation : Quaternion.identity);

            fractureObject.GetComponent<MeshFilter>().mesh = meshAt.mesh;
            fractureObject.name = gameObject.name;
            fractureObject.GetComponent<BreakableObject>().FormatBreakable();

            if(original != null) {
                float originalMass = original.GetComponent<Rigidbody2D>().mass;
                float originalArea = GetArea(original.GetComponent<MeshFilter>().mesh);

                fractureObject.GetComponent<Rigidbody2D>().mass = area / originalArea * originalMass;
                fractureObject.GetComponent<Rigidbody2D>().velocity = original.GetComponent<Rigidbody2D>().velocity;
                fractureObject.GetComponent<Rigidbody2D>().angularVelocity = original.GetComponent<Rigidbody2D>().angularVelocity;
                fractureObject.GetComponent<BreakableObject>().maxHealth = fractureObject.GetComponent<BreakableObject>().health = area / originalArea * maxHealth;

                fractureObject.transform.parent = original.transform.parent;

                fractureObject.transform.RotateAround(original.transform.localPosition, Vector3.forward, original.transform.eulerAngles.z);
            }

            return fractureObject;
        }

        return null;
    }

    ///<summary>Sends the pieces away from this object's center at the given force, destroying this object afterwards</summary>
    void BlastApart(GameObject[] pieces, float force) {
        Vector3 center = transform.position;

        foreach(GameObject piece in pieces) {
            piece.GetComponent<Rigidbody2D>().AddForce((piece.transform.position - center).normalized * force);
        }

        Destroy(gameObject);
    }

    ///<summary>Breaks the object across a line and flings its pieces away from the center at the given force. Set spawn to false to prevent the pieces from spawning</summary>
    public object[] Break(Mesh mesh, float force, Vector2 from, Vector2 to, bool spawn = true) {
        if(spawn) {
            breakForce = force;
            breakLineStart = from;
            breakLineEnd = to;
        }

        from = RelativePoint(from);
        to = RelativePoint(to);

        Vector3[] breakPoints = GetBreakPoints(mesh, from, to), weakSide, strongSide;
        GetSides(mesh, from, to, out weakSide, out strongSide);

        MeshAt weakFracture = CreateFractureMesh(weakSide, breakPoints);
        MeshAt strongFracture = CreateFractureMesh(strongSide, breakPoints);

        List<object> fractures = new List<object>();

        if(spawn) {
            fractures.Add(CreateFractureObject(weakFracture, gameObject));
            fractures.Add(CreateFractureObject(strongFracture, gameObject));
        } else {
            fractures.Add(weakFracture);
            fractures.Add(strongFracture);
        }

        fractures.RemoveAll(obj => obj == null);

        if(spawn) {
            BlastApart(fractures.ConvertAll(f => (GameObject)f).ToArray(), force);
        }

        return fractures.ToArray();
    }

    ///<summary>Breaks the object across a line and flings its pieces away from the center at the given force. Set spawn to false to prevent the pieces from spawning</summary>
    public object[] Break(float force, Vector2 from, Vector2 to, bool spawn = true) {
        return Break(GetComponent<MeshFilter>().sharedMesh, force, from, to, spawn);
    }

    ///<summary>Breaks the object across the currently given line and force</summary>
    public object[] Break() {
        health = 0f;
        return Break(breakForce, breakLineStart, breakLineEnd);
    }

    public void Shatter(Vector2 from, Vector2 to, float force, float damage = -1f, int fractureModifier = -1, bool spawn = true) {
        if(spawn) {
            breakForce = force;
            breakLineStart = from;
            breakLineEnd = to;
        }

        health -= damage < 0 ? force : damage;
        if(health > 0f) return;

        Func<Vector2, float, Vector3> RandomVectorOffset = delegate(Vector2 original, float range) {
            return new Vector3(original.x + UnityEngine.Random.Range(-range, range), original.y + UnityEngine.Random.Range(-range, range));
        };

        Func<float, Vector3> RandomCirclePoint = delegate(float radius) {
            float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        };

        Func<MeshAt, int, int, List<MeshAt>> reshatter = null;
        reshatter = delegate(MeshAt meshAt, int maxSegments, int segment) {
            List<MeshAt> fragments = new List<MeshAt>();

            //Vector3 layer = transform.position + meshAt.center;

            if(maxSegments > 0) {
                float scale = (float)segment / (float)maxSegments * Vector2.Distance(from, to) / 2f;
                Vector3 start = segment == 0 ? (Vector3)from : RandomCirclePoint(scale);
                Vector3 end = segment == 0 ? RandomVectorOffset(to, Mathf.Sqrt(Vector2.Distance(from, to))) : RandomCirclePoint(scale);

                MeshAt[] pieces = ConvertToArray<MeshAt>(Break(meshAt.mesh, 0f, start, end, false)).ToList().ConvertAll(p => new MeshAt(p.mesh, meshAt.center + p.center)).ToArray();

                if(segment < maxSegments && pieces.Count() > 0) {
                    int randomizeWhich = UnityEngine.Random.Range(0, 201);

                    //Debug.Log("During segment " + segment + ", got " + randomizeWhich);

                    if(randomizeWhich > 50 || randomizeWhich < 25) {
                        fragments.AddRange(reshatter(pieces[0], maxSegments, segment + 1));
                    } else {
                        fragments.Add(pieces[0]);
                    }
                    
                    if(pieces.Count() > 1) {
                        if(randomizeWhich > 50 || (25 <= randomizeWhich && randomizeWhich < 50)) {
                            fragments.AddRange(reshatter(pieces[1], maxSegments, segment + 1));
                        } else {
                            fragments.Add(pieces[1]);
                        }
                    }
                } else {
                    fragments.Add(meshAt);
                }
            } else {
                fragments.Add(meshAt);
            }

            return fragments;
        };

        if(spawn) {
            if(lastMaterial != null) {
                GetComponent<Renderer>().material = lastMaterial;
            }

            if(lastColor != null) {
                GetComponent<Renderer>().material.color = lastColor.Value;
            }

            int fractureAmount = fractureModifier < 0 ? (int)((GetArea(GetComponent<MeshFilter>().mesh) + 3f * Mathf.Log(force + 1)) * 0.5f) : fractureModifier;

            //Debug.Log("Fracturing: " + fractureAmount);

            List<MeshAt> partMeshAts = reshatter(new MeshAt(GetComponent<MeshFilter>().sharedMesh, Vector3.zero), fractureAmount, 0);

            List<GameObject> partObjects = partMeshAts.ConvertAll(ma => CreateFractureObject(ma, gameObject));
            partObjects.RemoveAll(obj => obj == null);

            BlastApart(partObjects.ToArray(), force);

            foreach(GameObject piece in partObjects) {
                piece.GetComponent<BreakableObject>().Vanish();
            }
        }
    }

    public void Shatter() {
        health = 0f;
        Shatter(breakLineStart, breakLineEnd, breakForce);
    }

    void Vanish() {
        if(despawnAfter > 0f) {
            StartCoroutine(VanishOverTime());
        }
    }

    IEnumerator VanishOverTime() {
        Material material = GetComponent<Renderer>().material;
        lastMaterial = material;
        material.SetFloat("_Mode", 2);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        GetComponent<Renderer>().material = material;

        Color c = material.color;
        lastColor = c;
        float time = despawnAfter / 2f;
        float interval = 0.1f;
        float end = 5.7f;

        yield return new WaitForSeconds(time);

        for(float i = 0; i < end; i += interval) {
            GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, (Mathf.Sin(Mathf.Pow(i + 1.25f, 2f)) + 1f) /2f);

            yield return new WaitForSeconds(time / end * interval);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position + breakLineStart, transform.position + breakLineEnd);
        
        /*
        Func<Vector2, float, Vector3> RandomVectorOffset = delegate(Vector2 original, float range) {
            return new Vector3(original.x + UnityEngine.Random.Range(-range, range), original.y + UnityEngine.Random.Range(-range, range));
        };

        Func<float, Vector3> RandomCirclePoint = delegate(float radius) {
            float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            return new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        };

        Action<MeshAt, Color, int, int> reshatter = null;
        reshatter = delegate(MeshAt meshAt, Color color, int maxSegments, int segment) {
            Vector3 layer = transform.position + meshAt.center + transform.forward * (-0.5f * (float)(1 + segment));
            RenderMesh(meshAt.mesh, layer, transform.localScale, color);

            if(maxSegments > 0) {
                float scale = (float)segment / (float)maxSegments * Vector3.Distance(breakLineStart, breakLineEnd) / 2f;
                Vector3 start = segment == 0 ? breakLineStart : RandomCirclePoint(scale);
                Vector3 end = segment == 0 ? RandomVectorOffset(breakLineEnd, 0.25f) : RandomCirclePoint(scale);

                Gizmos.color = Color.black;
                Gizmos.DrawLine(layer + meshAt.center + start, layer + meshAt.center + end);

                MeshAt[] pieces = ConvertToArray<MeshAt>(Break(meshAt.mesh, 0f, start, end, false)).ToList().ConvertAll(p => new MeshAt(p.mesh, meshAt.center + p.center)).ToArray();

                if(segment < maxSegments) {
                    if(pieces.Count() > 0) {
                        reshatter(pieces[0], Color.red, maxSegments, segment + 1);
                    }

                    if(pieces.Count() > 1) {
                        reshatter(pieces[1], Color.blue, maxSegments, segment + 1);
                    }
                }
            }
        };

        reshatter(new MeshAt(GetComponent<MeshFilter>().sharedMesh, Vector3.zero), Color.white, 1, 0);
        */
        /*
        RenderMesh(GetComponent<MeshFilter>().sharedMesh, transform.position + transform.forward * -0.25f, transform.localScale, Color.white);

        MeshAt[] fractures = ConvertToArray<MeshAt>(Break(0f, breakLineStart, breakLineEnd,  false));

        if(fractures.Count() > 0) {
            RenderMesh(fractures[0].mesh, transform.position + fractures[0].center + transform.forward * -0.5f, transform.localScale, Color.red);
        }

        if(fractures.Count() > 1) {
            RenderMesh(fractures[1].mesh, transform.position + fractures[1].center + transform.forward * -0.75f, transform.localScale, Color.blue);
        }
        */
    }

    void Start() {
        //FormatBreakable();
    }
}