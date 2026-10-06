using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Matrix4
{
    public Vector4 row0;
    public Vector4 row1;
    public Vector4 row2;
    public Vector4 row3;

    public Matrix4(Vector3 R, Vector3 U, Vector3 F, Vector3 P)
    {
        row0 = new Vector4(R.x, U.x, F.x, P.x);
        row1 = new Vector4(R.y, U.y, F.y, P.y);
        row2 = new Vector4(R.z, U.z, F.z, P.z);
        row3 = new Vector4(0, 0, 0, 1);
    }

    public Vector4 Multiply(Vector4 v)
    {
        float x = MFGCore.Dot4(row0, v);
        float y = MFGCore.Dot4(row1, v);
        float z = MFGCore.Dot4(row2, v);
        float w = MFGCore.Dot4(row3, v);

        return new Vector4(x, y, z, w);
    }
}

public static class MFGCore
{
    public static Vector2 AddV2(Vector2 a, Vector2 b)
    {
        Vector2 result = a + b;
        return result;
    }

    public static Vector2 SubtractV2(Vector2 a, Vector2 b)
    {
        Vector2 result = a - b;
        return result;
    }

    public static float MagnitudeV2(Vector2 v)
    {
        float calc = (v.x * v.x) + (v.y * v.y);
        float result = Mathf.Sqrt(calc);
        return result;
    }

    public static float DistanceV2(Vector2 a, Vector2 b)
    {
        float result = MagnitudeV2(SubtractV2(a, b));
        return result;
    }

    public static Vector2 ScaleV2(Vector2 v, float s)
    {
        Vector2 result = new Vector2(v.x * s, v.y * s);
        return result;
    }

    public static Vector2 DivideV2(Vector2 v, float s)
    {
        if (s <= 0.1f && s >= -0.1f)
        {
            Vector2 result = Vector2.zero;
            return result;
        }
        else
        {
            Vector2 result = new Vector2(v.x / s, v.y / s);
            return result;
        }
    }

    public static float DotV2(Vector2 a, Vector2 b)
    {
        float result = (a.x * b.x) + (a.y * b.y);
        return result;
    }

    public static Vector2 NormalizeV2(Vector2 v)
    {
        float len = MagnitudeV2(v);
        if (len <= 0.1f && len >= -0.1f)
        {
            Vector2 result = Vector2.zero;
            return result;
        }
        else
        {
            Vector2 result = DivideV2(v, len);
            return result;
        }
    }

    public static Vector3 AddV3(Vector3 a, Vector3 b)
    {
        Vector3 result = a + b;
        return result;
    }

    public static Vector3 SubtractV3(Vector3 a, Vector3 b)
    {
        Vector3 result = a - b;
        return result;
    }

    public static float MagnitudeV3(Vector3 v)
    {
        float result = (v.x * v.x) + (v.y * v.y) + (v.z * v.z);
        return Mathf.Sqrt(result);
    }

    public static float DistanceV3(Vector3 a, Vector3 b)
    {
        float result = MagnitudeV3(SubtractV3(a, b));
        return result;
    }

    public static Vector3 ScaleV3(Vector3 v, float s)
    {
        Vector3 result = new Vector3(v.x * s, v.y * s, v.z * s);
        return result;
    }

    public static Vector3 DivideV3(Vector3 v, float s)
    {
        if (s <= 0.1f && s >= -0.1f)
        {
            Vector3 result = Vector3.zero;
            return result;
        }
        else
        {
            Vector3 result = new Vector3(v.x / s, v.y / s, v.z / s);
            return result;
        }
    }

    public static Vector3 NormalizeV3(Vector3 v)
    {
        float len = MagnitudeV3(v);
        if (len <= 0.1f && len >= -0.1f)
        {
            Vector3 result = Vector3.zero;
            return result;
        }
        else
        {
            Vector3 result = DivideV3(v, len);
            return result;
        }
    }

    public static float Dot(Vector3 a, Vector3 b)
    {
        float result = (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
        return result;
    }

    public static float DegreesToRadians(float degrees)
    {
        float result = degrees * (Mathf.PI / 180);
        return result;
    }

    public static float RadiansToDegrees(float radians)
    {
        float result = radians * (180 / Mathf.PI);
        return result;
    }

    public static float AngleFromVector2(Vector2 v)
    {
        float result = Mathf.Atan2(v.y, v.x);
        return result;
    }

    public static Vector2 Vector2FromAngle(float radians)
    {
        Vector2 result = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        return result;
    }

    public static Vector3 ForwardFromYawPitch(float yawRadians, float pitchRadians)
    {
        float x = Mathf.Sin(yawRadians) * Mathf.Cos(pitchRadians);
        float y = Mathf.Sin(pitchRadians);
        float z = Mathf.Cos(yawRadians) * Mathf.Cos(pitchRadians);
        Vector3 result = new Vector3(x, y, z);
        return result;
    }

    public static Vector3 CrossProduct(Vector3 a, Vector3 b)
    {
        float x = (a.y * b.z) - (a.z * b.y);
        float y = (a.z * b.x) - (a.x * b.z);
        float z = (a.x * b.y) - (a.y * b.x);
        Vector3 result = new Vector3(x, y, z);
        return result;
    }
    public static Vector3 DirectionFromBasis(Vector3 localDir, Vector3 R, Vector3 U, Vector3 F)
    {
        Vector3 newR = ScaleV3(R, localDir.x);
        Vector3 newU = ScaleV3(U, localDir.y);
        Vector3 newF = ScaleV3(F, localDir.z);
        Vector3 result = AddV3(AddV3(newR, newU), newF);
        return result;
    }

    public static Vector3 LocalPointToWorldPoint(Vector3 P, Vector3 localPoint, Vector3 R, Vector3 U, Vector3 F)
    {
        Vector3 newR = ScaleV3(R, localPoint.x);
        Vector3 newU = ScaleV3(U, localPoint.y);
        Vector3 newF = ScaleV3(F, localPoint.z);
        Vector3 ruf = AddV3(AddV3(newR, newU), newF);
        Vector3 result = AddV3(P, ruf);
        return result;
    }

    public static Vector3 LerpV3(Vector3 A, Vector3 B, float time)
    {
        Vector3 result = MFGCore.ScaleV3(MFGCore.AddV3(A, MFGCore.SubtractV3(B, A)), time);
        return result;
    }

    public static float Dot4(Vector4 a, Vector4 b)
    {
        float result = a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        return result;
    }

    public static void BuildBasisFromForward(Vector3 forward, out Vector3 R, out Vector3 U, out Vector3 F)
    {
        F = NormalizeV3(forward);
        U = new Vector3(0, 1, 0);
        R = NormalizeV3(CrossProduct(U, F));
        U = CrossProduct(F, R);
    }

    public static Vector3 TransformPoint(Matrix4 M, Vector3 p)
    {
        Vector4 v = new Vector4(p.x, p.y, p.z, 1f);
        Vector4 outV = M.Multiply(v);
        return new Vector3(outV.x, outV.y, outV.z);
    }

    public static Vector3 RotateAroundAxis(Vector3 v, Vector3 axis, float angleRad)
    {
        axis = NormalizeV3(axis);
        float sinAngle = Mathf.Sin(angleRad);
        float cosAngle = Mathf.Cos(angleRad);
        Vector3 result = v * cosAngle + axis * Dot(v, axis) * (1 - cosAngle) + CrossProduct(axis, v) * sinAngle;
        return result;
    }

    public static Vector3 Reflect(Vector3 velocity, Vector3 normal)
    {
        normal = NormalizeV3(normal);
        float dot = Dot(velocity, normal);
        Vector3 correction = normal * (2 * dot);
        return velocity - correction;
    }

    public static Vector3 ApplyRestitution(Vector3 reflectedVelocity, float restitution)
    {
        restitution = Clamp(restitution, 0, 1.2f);
        Vector3 result = reflectedVelocity * restitution;
        return result;
    }

    public static float Clamp(float x, float min, float max)
    {
        if (x <= min)
        {
            return min;
        }
        else if (x >= max)
        {
            return max;
        }
        else
        {
            return x;
        }
    }
}

public class Quat
{
    public float w;
    public float x;
    public float y;
    public float z;

    public Quat(float w, float x, float y, float z)
    {
        this.w = w;
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Quat(Vector3 v)
    {
        w = 0;
        x = v.x; y = v.y; z = v.z;
    }

    public Quat(Vector3 axis, float angleRad)
    {
        axis = MFGCore.NormalizeV3(axis);
        float halfAngle = angleRad * 0.5f;
        w = Mathf.Cos(halfAngle);
        x = axis.x * Mathf.Sin(halfAngle);
        y = axis.y * Mathf.Sin(halfAngle);
        z = axis.z * Mathf.Sin(halfAngle);
    }

    public static Quat operator *(Quat a, Quat b)
    {
        float w = a.w * b.w - (a.x * b.x + a.y * b.y + a.z * b.z);
        float x = a.w * b.x + b.w * a.x + (a.y * b.z - a.z * b.y);
        float y = a.w * b.y + b.w * a.y + (a.z * b.x - a.x * b.z);
        float z = a.w * b.z + b.w * a.z + (a.z * b.y - a.x * b.x);
        return new Quat(w, x, y, z);
    }

    public Quat Inverse()
    {
        Quat inverse = new Quat(w, -x, -y, -z);
        return inverse;
    }

    public Vector3 RotateVector(Vector3 v)
    {
        Quat p = new Quat(0, v.x, v.y, v.z);
        Quat pInv = this * p * this.Inverse();
        return new Vector3(pInv.x, pInv.y, pInv.z);
    }

    public Quaternion ToUnityQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}
