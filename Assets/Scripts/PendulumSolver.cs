using System;
using System.Collections.Generic;
using UnityEngine;

public class PendulumSolver : MonoBehaviour
{
    [Tooltip("It is recommended to use powers of 2 for easy control. Also increase number of sub-steps with amount of added mass points.")]
    [SerializeField] private int subSteps = 4;
    private const float GravitationalConstant = 9.81f;
    [Header("Config")] [SerializeField] public List<MassPoint> InitialParameters;
    [HideInInspector]
    public MassPoint[] RuntimeMassPoints;

    private void Start()
    {
        RuntimeMassPoints = new MassPoint[InitialParameters.Count];
        for (int i = 0; i < InitialParameters.Count; i++)
        {
            RuntimeMassPoints[i] = InitialParameters[i].Clone();
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime / subSteps;

        for (int step = 0; step < subSteps; step++)
        {
            RungeKutta4Step(deltaTime);
        }

        UpdatePositions();
    }

    private void RungeKutta4Step(float dt)
    {
        int n = RuntimeMassPoints.Length;

        // Store initial state
        float[] angles = new float[n];
        float[] velocities = new float[n];
        for (int i = 0; i < n; i++)
        {
            angles[i] = RuntimeMassPoints[i].Angle;
            velocities[i] = RuntimeMassPoints[i].AngularVelocity;
        }

        // k1
        float[] k1_v = GetAccelerations();
        float[] k1_a = new float[n];
        for (int i = 0; i < n; i++) k1_a[i] = velocities[i];

        // k2
        for (int i = 0; i < n; i++)
        {
            RuntimeMassPoints[i].Angle = angles[i] + k1_a[i] * dt * 0.5f;
            RuntimeMassPoints[i].AngularVelocity = velocities[i] + k1_v[i] * dt * 0.5f;
        }

        float[] k2_v = GetAccelerations();
        float[] k2_a = new float[n];
        for (int i = 0; i < n; i++) k2_a[i] = velocities[i] + k1_v[i] * dt * 0.5f;

        // k3
        for (int i = 0; i < n; i++)
        {
            RuntimeMassPoints[i].Angle = angles[i] + k2_a[i] * dt * 0.5f;
            RuntimeMassPoints[i].AngularVelocity = velocities[i] + k2_v[i] * dt * 0.5f;
        }

        float[] k3_v = GetAccelerations();
        float[] k3_a = new float[n];
        for (int i = 0; i < n; i++) k3_a[i] = velocities[i] + k2_v[i] * dt * 0.5f;

        // k4
        for (int i = 0; i < n; i++)
        {
            RuntimeMassPoints[i].Angle = angles[i] + k3_a[i] * dt;
            RuntimeMassPoints[i].AngularVelocity = velocities[i] + k3_v[i] * dt;
        }

        float[] k4_v = GetAccelerations();
        float[] k4_a = new float[n];
        for (int i = 0; i < n; i++) k4_a[i] = velocities[i] + k3_v[i] * dt;

        // Final update
        for (int i = 0; i < n; i++)
        {
            RuntimeMassPoints[i].Angle = angles[i] + (k1_a[i] + 2 * k2_a[i] + 2 * k3_a[i] + k4_a[i]) * dt / 6f;
            RuntimeMassPoints[i].AngularVelocity =
                velocities[i] + (k1_v[i] + 2 * k2_v[i] + 2 * k3_v[i] + k4_v[i]) * dt / 6f;
        }
    }

    private void UpdatePositions()
    {
        Vector2 anchorPoint = Vector2.zero;

        for (int i = 0; i < RuntimeMassPoints.Length; i++)
        {
            MassPoint mp = RuntimeMassPoints[i];

            Vector2 localPos = new Vector2(
                Mathf.Sin(mp.Angle),
                -Mathf.Cos(mp.Angle)
            ) * mp.AttachedRodLength;

            mp.Position = anchorPoint + localPos;
            anchorPoint = mp.Position;
        }
    }

    private float[] GetAccelerations()
    {
        int n = RuntimeMassPoints.Length;
        float[] accelerations = new float[n];

        if (n == 0) return accelerations;

        if (n == 1)
        {
            // Single pendulum: α = -(g/L) * sin(θ)
            MassPoint massPoint = RuntimeMassPoints[0];
            accelerations[0] = -(GravitationalConstant / massPoint.AttachedRodLength) * Mathf.Sin(massPoint.Angle);
            return accelerations;
        }

        if (n == 2)
        {
            float t1 = RuntimeMassPoints[0].Angle;
            float t2 = RuntimeMassPoints[1].Angle;
            float w1 = RuntimeMassPoints[0].AngularVelocity;
            float w2 = RuntimeMassPoints[1].AngularVelocity;
            float m1 = RuntimeMassPoints[0].Mass;
            float m2 = RuntimeMassPoints[1].Mass;
            float L1 = RuntimeMassPoints[0].AttachedRodLength;
            float L2 = RuntimeMassPoints[1].AttachedRodLength;

            float dt = t1 - t2;
            float den = (2 * m1 + m2) - m2 * Mathf.Cos(2 * dt);

            accelerations[0] = (-GravitationalConstant * (2 * m1 + m2) * Mathf.Sin(t1) -
                                m2 * GravitationalConstant * Mathf.Sin(t1 - 2 * t2) -
                                2 * Mathf.Sin(dt) * m2 * (w2 * w2 * L2 + w1 * w1 * L1 * Mathf.Cos(dt))) / (L1 * den);

            accelerations[1] = (2 * Mathf.Sin(dt) * (w1 * w1 * L1 * (m1 + m2) +
                                                     GravitationalConstant * (m1 + m2) * Mathf.Cos(t1) +
                                                     w2 * w2 * L2 * m2 * Mathf.Cos(dt))) / (L2 * den);

            return accelerations;
        }

        float[,] A = new float[n, n];
        float[] b = new float[n];

        for (int q = 0; q < n; q++)
        {
            for (int k = q; k < n; k++)
            {
                float Lq = RuntimeMassPoints[q].AttachedRodLength;
                float wq = RuntimeMassPoints[q].AngularVelocity;
                float tq = RuntimeMassPoints[q].Angle;

                float Lk = RuntimeMassPoints[k].AttachedRodLength;
                float wk = RuntimeMassPoints[k].AngularVelocity;
                float tk = RuntimeMassPoints[k].Angle;

                float mk = RuntimeMassPoints[k].Mass;

                // diagonal term
                A[q, q] += mk * Lq * Lq;

                for (int i = 0; i <= k; i++)
                {
                    float Li = RuntimeMassPoints[i].AttachedRodLength;
                    float ti = RuntimeMassPoints[i].Angle;
                    float wi = RuntimeMassPoints[i].AngularVelocity;
                    float mi = RuntimeMassPoints[i].Mass;

                    float cosTerm = Mathf.Cos(ti - tq);
                    float sinTerm = Mathf.Sin(ti - tq);

                    // coupling term
                    A[q, i] += mk * Lq * Li * cosTerm;

                    // gravity term
                    b[q] -= mk * GravitationalConstant * Lq * Mathf.Sin(tq);

                    // Coriolis / centrifugal terms
                    // b[q] -= mk * Lq * Li * wi * wq * sinTerm;
                    // b[q] -= mk * Lq * Li * wi * (wq - wi) * sinTerm;
                }
            }
        }

        accelerations = SolveLinearSystem(A, b);
        return accelerations;
    }
    private float[] SolveLinearSystem(float[,] A, float[] b)
    {
        int n = b.Length;
        float[,] M = (float[,])A.Clone();
        float[] x = new float[n];
        float[] B = (float[])b.Clone();

        // Gaussian elimination
        for (int k = 0; k < n; k++)
        {
            // pivot
            float max = Mathf.Abs(M[k, k]);
            int maxRow = k;
            for (int i = k + 1; i < n; i++)
            {
                if (Mathf.Abs(M[i, k]) > max)
                {
                    max = Mathf.Abs(M[i, k]);
                    maxRow = i;
                }
            }
            for (int j = k; j < n; j++)
            {
                float tmp = M[maxRow, j];
                M[maxRow, j] = M[k, j];
                M[k, j] = tmp;
            }
            float tmpB = B[maxRow];
            B[maxRow] = B[k];
            B[k] = tmpB;

            // elimination
            for (int i = k + 1; i < n; i++)
            {
                float factor = M[i, k] / M[k, k];
                B[i] -= factor * B[k];
                for (int j = k; j < n; j++)
                {
                    M[i, j] -= factor * M[k, j];
                }
            }
        }

        // back-substitution
        for (int i = n - 1; i >= 0; i--)
        {
            float sum = B[i];
            for (int j = i + 1; j < n; j++)
            {
                sum -= M[i, j] * x[j];
            }
            x[i] = sum / M[i, i];
        }

        return x;
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || RuntimeMassPoints == null) return;

        Vector2 previousPos = Vector2.zero;

        foreach (var massPoint in RuntimeMassPoints)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(previousPos, massPoint.Position);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(massPoint.Position, 0.1f * massPoint.Mass);

            previousPos = massPoint.Position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector2.zero, 0.05f);
    }
}
