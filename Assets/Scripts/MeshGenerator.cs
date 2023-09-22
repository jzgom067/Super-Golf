using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Master master;
    TrajectoryPrediction tp;

    LineRenderer line;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public float lastPointOffset;

    // Start is called before the first frame update
    void Start()
    {
        master = Master.instance;
        tp = master.GetComponent<TrajectoryPrediction>();
        line = tp.GetLine();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CreateShape();
        UpdateMesh();

        GetComponent<MeshRenderer>().enabled = line.enabled;
    }

    void CreateShape()
    {
        Vector3[] points = new Vector3[line.positionCount];

        line.GetPositions(points);

        int last = points.Length - 1;

        vertices = new Vector3[points.Length + 1];

        //vertices[0] = new Vector3(points[0].x, points[0].y - 10, points[0].z);
        vertices[0] = new Vector3(points[last].x, master.getPlayer().transform.position.y - lastPointOffset, points[last].z);

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i + 1] = points[i];
        }

        triangles = new int[(points.Length - 2) * 3];

        int j = 0;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            triangles[i + 2] = j + 2;

            j++;
        }

        int tLast = triangles.Length - 3;

        triangles[tLast] = 0;
        triangles[tLast + 1] = j;
        triangles[tLast + 2] = j + 1;
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
