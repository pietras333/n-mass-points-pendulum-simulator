using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PendulumSolver), typeof(MeshFilter), typeof(MeshRenderer))]
public class PendulumFullRenderer : MonoBehaviour
{
    [Header("Trail Settings")]
    public int maxTrailPoints = 500;
    public float trailPointScale = 0.1f;
    public float maxVelocity = 10f;
    public Gradient velocityGradient;     // velocity-based coloring

    [Header("Rod Settings")]
    public Color rodColor = Color.magenta; // vibrant rod color
    public float rodWidth = 0.05f;

    [Header("Mass Point Settings")]
    public float sphereRadius = 0.1f;

    private PendulumSolver solver;

    private List<List<Vector3>> pointsPerMass = new List<List<Vector3>>();
    private List<List<Color>> colorsPerMass = new List<List<Color>>();
    private List<GameObject> massSpheres = new List<GameObject>();
    private Mesh mesh;

    void Awake()
    {
        solver = GetComponent<PendulumSolver>();

        mesh = new Mesh();
        mesh.name = "Pendulum Full Mesh";
        GetComponent<MeshFilter>().mesh = mesh;

        var mr = GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Custom/VertexColor"));

        // Create spheres for each mass
        int n = solver.InitialParameters.Count;
        Debug.Log(n);
        for (int i = 0; i < n; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = Vector3.one * sphereRadius;
            sphere.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));
            massSpheres.Add(sphere);
        }
    }

    void Update()
    {
        if (solver.RuntimeMassPoints == null || solver.RuntimeMassPoints.Length == 0) return;   
        
        int n = solver.RuntimeMassPoints.Length;

        // initialize per mass
        while (pointsPerMass.Count < n)
        {
            pointsPerMass.Add(new List<Vector3>());
            colorsPerMass.Add(new List<Color>());
        }

        // record positions and colors
        for (int i = 0; i < n; i++)
        {
            var mp = solver.RuntimeMassPoints[i];
            Vector3 pos = new Vector3(mp.Position.x, mp.Position.y, i * 0.01f); // z-offset
            pointsPerMass[i].Add(pos);

            float speedNorm = Mathf.Clamp01(Mathf.Abs(mp.AngularVelocity) / maxVelocity);
            colorsPerMass[i].Add(velocityGradient.Evaluate(speedNorm));

            if (pointsPerMass[i].Count > maxTrailPoints)
            {
                pointsPerMass[i].RemoveAt(0);
                colorsPerMass[i].RemoveAt(0);
            }
        }

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();

        int offset = 0;

        // --- Trails per mass point ---
        for (int i = 0; i < pointsPerMass.Count; i++)
        {
            var pts = pointsPerMass[i];
            var cols = colorsPerMass[i];

            for (int j = 0; j < pts.Count; j++)
            {
                vertices.Add(pts[j]);
                colors.Add(cols[j]);

                if (j < pts.Count - 1)
                {
                    indices.Add(offset + j);
                    indices.Add(offset + j + 1);
                }
            }

            offset += pts.Count;
        }

        // --- Rods connecting masses (single line connecting current positions) ---
        var solverPoints = solver.RuntimeMassPoints;
        for (int i = 0; i < solverPoints.Length; i++)
        {
            Vector3 start = i == 0 ? Vector3.zero : new Vector3(solverPoints[i - 1].Position.x, solverPoints[i - 1].Position.y, 0);
            Vector3 end = new Vector3(solverPoints[i].Position.x, solverPoints[i].Position.y, 0);

            vertices.Add(start);
            colors.Add(rodColor);
            vertices.Add(end);
            colors.Add(rodColor);

            indices.Add(offset);
            indices.Add(offset + 1);

            offset += 2;
        }

        // --- Mass points as small spheres at current positions ---
        // (draw as vertices, could be replaced with actual sphere meshes if needed)
        // Update sphere positions
        for (int i = 0; i < solverPoints.Length; i++)
        {
            if (i < massSpheres.Count && massSpheres[i] != null)
            {
                Vector3 pos = new Vector3(solverPoints[i].Position.x, solverPoints[i].Position.y, i * 0.01f);
                massSpheres[i].transform.position = pos;
                float speedNorm = Mathf.Clamp01(Mathf.Abs(solverPoints[i].AngularVelocity) / maxVelocity);
                massSpheres[i].GetComponent<Renderer>().material.color = velocityGradient.Evaluate(speedNorm);

            }
        }


        mesh.vertices = vertices.ToArray();
        mesh.colors = colors.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
    }
}
