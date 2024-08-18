using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rotation", menuName = "Animations/Rotation", order = 1)]
public class RotationAnimation : ScriptableObject
{
    public struct InitParams
    {
        public InitParams(float _x, float _y, float _z, float _t)
        {
            x = _x;
            y = _y;
            z = _z;
            t = _t;
        }
        public float x, y, z, t;
    }

    [SerializeField] float[] xRotation, yRotation, zRotation, durations;
    float[] x = new float[2], y = new float[2], z = new float[2], ts = new float[3];
    float maxT = 0, t = 0;
    public float animationRate;
    public void Initialise(InitParams initParams)
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        ts = new float[3];
        for (int i = 0; i < ts.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xRotation[Mathf.Min(x.Length - 1, i)] * initParams.x;
            y[Mathf.Min(y.Length - 1, i)] = yRotation[Mathf.Min(y.Length - 1, i)] * initParams.y;
            z[Mathf.Min(z.Length - 1, i)] = zRotation[Mathf.Min(z.Length - 1, i)] * initParams.z;
            ts[Mathf.Min(ts.Length - 1, i)] = durations[Mathf.Min(ts.Length - 1, i)] * initParams.t;
        }
        maxT = 0;
        foreach (float f in ts)
        {
            maxT = Mathf.Max(maxT, f);
        }
        t = 0;
    }
    public void Initialise()
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        ts = new float[3];
        for (int i = 0; i < ts.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xRotation[Mathf.Min(x.Length - 1, i)];
            y[Mathf.Min(y.Length - 1, i)] = yRotation[Mathf.Min(y.Length - 1, i)];
            z[Mathf.Min(z.Length - 1, i)] = zRotation[Mathf.Min(z.Length - 1, i)];
            ts[Mathf.Min(ts.Length - 1, i)] = durations[Mathf.Min(ts.Length - 1, i)];
        }
        maxT = 0;
        foreach (float f in ts)
        {
            maxT = Mathf.Max(maxT, f);
        }
        t = 0;
    }
    public bool ShouldTerminate()
    {
        bool r = t > maxT;
        if (r)
        {
            t = 0;
        }
        return r;
    }
    public Quaternion ApplyRotation(float deltaTime)
    {
        t += deltaTime;
        float xt = Mathf.Lerp(x[0], x[1], t / ts[0]);
        float yt = Mathf.Lerp(y[0], y[1], t / ts[1]);
        float zt = Mathf.Lerp(z[0], z[1], t / ts[2]);

        Quaternion r = Quaternion.AngleAxis(xt, Vector3.right);
        r *= Quaternion.AngleAxis(yt, Vector3.up);
        r *= Quaternion.AngleAxis(zt, Vector3.forward);
        if (t >= maxT)
        {
            return Quaternion.identity;
        }
        else
        {
            return r;
        }
    }
}
