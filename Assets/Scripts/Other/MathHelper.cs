using System;
using System.Security.Cryptography;
using UnityEngine;

public class MathHelper {
    ///<summary>Returns 0 for on the line, &gt; 0 for weak side, and &lt; 0 for strong side</summary>
    public static float SideOfLine(Vector2 start, Vector2 end, Vector2 point) {
        return (end.x - start.x) * (point.y - start.y) - (end.y - start.y) * (point.x - start.x);
    }

    ///<summary>Returns a vector where the first line (vector 1 and 2) intersect with the second line (vector 3 and 4) or null if there is no intersection</summary>
    public static Vector2? VectorIntersection(Vector2 l1s, Vector2 l1e, Vector2 l2s, Vector2 l2e) {
        float x1 = l1s.x;
        float y1 = l1s.y;

        float x2 = l1e.x;
        float y2 = l1e.y;

        float x3 = l2s.x;
        float y3 = l2s.y;

        float x4 = l2e.x;
        float y4 = l2e.y;

        if((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4) == 0) {
            return null;
        }

        float a = (y2 - y1) / (x2 - x1);
        float c = a * -x1 + y1;

        float b = (y4 - y3) / (x4 - x3);
        float d = b * -x3 + y3;

        if(a == b) {
            return null;
        }

        if(x1 == x2) {
            c = 0f;
        } else if(x3 == x4) {
            d = 0f;
        }

        Vector2 v = new Vector2((d - c) / (a - b), a * (d - c) / (a - b) + c);

        if(x1 == x2) {
            v.x = x1;
            v.y = b * v.x + d;
        } else if(x3 == x4) {
            v.x = x3;
            v.y = a * v.x + c;
        }

        if(y1 == y2) {
            v.y = y1;
        } else if(y3 == y4) {
            v.y = y3;
        }

        float dot1 = Vector2.Dot(l1e - l1s, v - l1s);
        //float dot2 = Vector2.Dot(l2e - l2s, v - l2s);

        //Debug.Log("Line1) Slope: " + a + ", Y-Intercept: " + c);
        //Debug.Log("Line2) Slope: " + b + ", Y-Intercept: " + d);
        //Debug.Log(v);

        if(dot1 <= 0f || dot1 >= Mathf.Pow(Vector2.Distance(l1e, l1s), 2f)/* || dot2 <= 0f || dot2 >= Mathf.Pow(Vector2.Distance(l2e, l2s), 2f)*/) {
            return null;
        }

        return v;
    }

    private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

    ///<summary>Random from min and max</summary>
    public static float Rand(float min, float max) {
        byte[] data = new byte[4];
        rng.GetBytes(data);
        float next = Mathf.Abs((float)BitConverter.ToInt32(data, 0) / (float)int.MaxValue);

        return min + next * (max - min);
    }

    ///<summary>Random from min and max</summary>
    public static int Rand(int min, int max) {
        return (int)Rand((float)min, (float)max);
    }

    ///<summary>Rotates the point around the pivot.false Taken from here: http://answers.unity.com/comments/1434008/view.html</summary>
    public static Vector3 RotateAround(Vector3 point, Vector3 pivot, Quaternion rotation) {
         return rotation * (point - pivot) + pivot;
     }

    public static Vector2 ConstrainedVectorChange(Vector2 value, Vector2 change, Vector2 target) {
        return new Vector2(ConstrainedChange(value.x, change.x, target.x), ConstrainedChange(value.y, change.y, target.y));
    }

    public static float ConstrainedChange(float value, float change, float target) {
        float n = value + change, r = value;
        
        if(value < target) {
            if(n > target) {
                r = target;
            } else if(n > value) {
                r = n;
            }
        } else if(value > target) {
            if(n < target) {
                r = target;
            } else if(n < value) {
                r = n;
            }
        }

        return r;
    }
}