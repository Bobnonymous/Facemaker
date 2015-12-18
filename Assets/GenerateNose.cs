using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerateNose : MonoBehaviour {

    public GameObject Nose;
    public Mesh noseMesh;

    float gr = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;//golen ratio (a+b is to a as a is to b)
    
    void Start()
    {
        Nose = GameObject.Find("Nose");
        Nose.AddComponent<MeshFilter>();
        Nose.AddComponent<MeshRenderer>();

        noseMesh = GetComponent<MeshFilter>().mesh;
        noseMesh.Clear();

        noseMesh.vertices = new Vector3[] {
            new Vector3( gr,   1,  0),
            new Vector3( gr-1,   -gr*0.6f,  -gr*0.75f),
            new Vector3( gr-1,   -gr*0.6f,   gr*0.75f),
            new Vector3( gr*1.5f, -gr*0.6f,   0)
        };

        List<int> noseTrianglesIndices = new List<int>() {
            0,  1,  2,
            0,  3,  1,
            0,  2,  3,
            1,  3,  2};
        noseMesh.triangles = noseTrianglesIndices.ToArray();

        //Set Colour
        Material material = new Material(Shader.Find("Standard"));
        Color fleshtone = new Color(10, 205, 180);
        material.SetColor("fleshtone", fleshtone);

        Nose.GetComponent<Renderer>().material = material;

        noseMesh.RecalculateBounds();
        noseMesh.RecalculateNormals();
        noseMesh.Optimize();
    }

    void Update () {}
}
