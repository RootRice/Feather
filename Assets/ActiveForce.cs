using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActiveForce", menuName = "Forces/ActiveForce", order = 1)]
public class ActiveForce : ScriptableObject
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

    [SerializeField] float[] xForce, yForce, zForce, durations;
    float[] x = new float[2], y = new float[2], z = new float[2], ts = new float[3];
    float t = 0;
    float maxT = 0;

    [SerializeField] ForceConstraint.OngoingTag[] ongoingEffects;
    [SerializeField] ForceConstraint.InitialTag[] initialEffects;

    public ActiveForce(float[] _x, float[] _y, float[] _z, float[] _ts)
    {
        x = _x;
        y = _y;
        z = _z;
        ts = _ts;
        foreach (float f in ts)
        {
            maxT = Mathf.Max(maxT, f);
        }
    }
    public void Initialise()
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        ts = new float[3];
        for (int i = 0; i < ts.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xForce[Mathf.Min(x.Length - 1, i)];
            y[Mathf.Min(y.Length - 1, i)] = yForce[Mathf.Min(y.Length - 1, i)];
            z[Mathf.Min(z.Length - 1, i)] = zForce[Mathf.Min(z.Length - 1, i)];
            ts[Mathf.Min(ts.Length - 1, i)] = durations[Mathf.Min(ts.Length - 1, i)];
        }
        maxT = 0;
        foreach (float f in ts)
        {
            maxT = Mathf.Max(maxT, f);
        }
        t = 0;
    }
    public void Initialise(float _x, float _y, float _z, float _t)
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        ts = new float[3];
        for(int i = 0; i < ts.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xForce[Mathf.Min(x.Length - 1, i)] * _x;
            y[Mathf.Min(y.Length - 1, i)] = yForce[Mathf.Min(y.Length - 1, i)] * _y;
            z[Mathf.Min(z.Length - 1, i)] = zForce[Mathf.Min(z.Length - 1, i)] * _z;
            ts[Mathf.Min(ts.Length - 1, i)] = durations[Mathf.Min(ts.Length - 1, i)] * _t;
        }
        maxT = 0;
        foreach (float f in ts)
        {
            maxT = Mathf.Max(maxT, f);
        }
        t = 0;
    }
    public void Initialise(InitParams initParams)
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        ts = new float[3];
        for (int i = 0; i < ts.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xForce[Mathf.Min(x.Length - 1, i)] * initParams.x;
            y[Mathf.Min(y.Length - 1, i)] = yForce[Mathf.Min(y.Length - 1, i)] * initParams.y;
            z[Mathf.Min(z.Length - 1, i)] = zForce[Mathf.Min(z.Length - 1, i)] * initParams.z;
            ts[Mathf.Min(ts.Length - 1, i)] = durations[Mathf.Min(ts.Length - 1, i)] * initParams.t;
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

    public Vector3 ApplyForce(float deltaTime)
    {
        t += deltaTime;
        float xt = Mathf.Lerp(x[0], x[1], t / ts[0]);
        float yt = Mathf.Lerp(y[0], y[1], t / ts[1]);
        float zt = Mathf.Lerp(z[0], z[1], t / ts[2]);

        Vector3 r = new Vector3(xt, yt, zt) * deltaTime;
        if (t >= maxT)
        {
            return Vector3.zero;
        }
        else
        {
            return r;
        }
    }

    public ForceConstraint.OngoingTag[] CheckOngoingConstraints()
    {
        return ongoingEffects;
    }

    public ForceConstraint.InitialTag[] CheckInitialConstraints()
    {
        return initialEffects;
    }
}
