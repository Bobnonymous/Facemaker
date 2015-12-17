using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerateNose : MonoBehaviour {

    public GameObject Nose;
    public Mesh NoseMesh;

    float gr = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;//golen ratio (a+b is to a as a is to b)
    Vector3 origin = new Vector3(0, 0, 0);

    void Start()
    {
        Nose = GameObject.Find("Nose");
        Nose.AddComponent<MeshFilter>();
        Nose.AddComponent<MeshRenderer>();

        NoseMesh = GetComponent<MeshFilter>().mesh;
        NoseMesh.Clear();

        NoseMesh.vertices = new Vector3[] {
            new Vector3( gr,   0,  0),
            new Vector3( gr-1,   -gr,  -gr/4),
            new Vector3( gr-1,   -gr,   gr/4),
            new Vector3( gr*1.5f, -gr,   0)
        };

        List<int> noseTrianglesIndices = new List<int>() {
            0,  1,  2,
            0,  3,  1,
            0,  2,  3,
            1,  3,  2};
        NoseMesh.triangles = noseTrianglesIndices.ToArray();
    }

    void Update () {}
}
