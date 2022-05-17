using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstantForce", menuName = "Forces/ConstantForce", order = 1)]
public class ConstForce : ScriptableObject, Constraints
{

    [SerializeField] float[] xForce, yForce, zForce, durations;
    float[] x = new float[2], y = new float[2], z = new float[2];
    float t;

    [SerializeField] Constraints.OngoingTag[] ongoingEffects;
    [SerializeField] Constraints.InitialTag[] initialEffects;

    public ConstForce(float[] _x, float[] _y, float[] _z, float[] _ts)
    {
        x = _x;
        y = _y;
        z = _z;
    }
    public void Initialise()
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        for (int i = 0; i < x.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xForce[Mathf.Min(x.Length - 1, i)];
            y[Mathf.Min(y.Length - 1, i)] = yForce[Mathf.Min(y.Length - 1, i)];
            z[Mathf.Min(z.Length - 1, i)] = zForce[Mathf.Min(z.Length - 1, i)];
        }
        t = 0;
    }
    public void Initialise(float _x, float _y, float _z, float _t)
    {
        x = new float[2];
        y = new float[2];
        z = new float[2];
        for (int i = 0; i < x.Length; i++)
        {
            x[Mathf.Min(x.Length - 1, i)] = xForce[Mathf.Min(x.Length - 1, i)] * _x;
            y[Mathf.Min(y.Length - 1, i)] = yForce[Mathf.Min(y.Length - 1, i)] * _y;
            z[Mathf.Min(z.Length - 1, i)] = zForce[Mathf.Min(z.Length - 1, i)] * _z;
        }
        t = 0;
    }

    public Vector3 ApplyForce(float deltaTime, bool reset)
    {
        if(reset)
        {
            t = 0;
            return Vector3.zero;
        }

        t += deltaTime;
        float xt = Mathf.Lerp(x[0], x[1], Mathf.Clamp01(t / durations[0]));
        float yt = Mathf.Lerp(y[0], y[1], Mathf.Clamp01(t / durations[1]));
        float zt = Mathf.Lerp(z[0], z[1], Mathf.Clamp01(t / durations[2]));

        Vector3 r = new Vector3(xt, yt, zt) * deltaTime;
        return r;
    }

    public Constraints.InitialTag[] CheckInitialConstraints()
    {
        return initialEffects;
    }

    public Constraints.OngoingTag[] CheckOngoingConstraints()
    {
        return ongoingEffects;
    }
}
