using UnityEngine;
using System.Collections;

public class GenerateMesh : MonoBehaviour {
    public GameObject gameObject;

    void Start () {
        gameObject = GameObject.FindGameObjectWithTag("Face");
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh faceMesh = GetComponent<MeshFilter>().mesh;
        faceMesh.Clear();
        float gr = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;//golen ratio (a+b is to a as a is to b)

        // create 12 vertices of a icosahedron, via 3 intersecting rectangles
        faceMesh.vertices = new Vector3[] { new Vector3( gr,   0,  -1),  //rectangle x point 1
                                            new Vector3(-gr,   0,  -1),  //rectangle x point 2
                                            new Vector3(-gr,   0,   1),  //rectangle x point 3
                                            new Vector3( gr,   0,   1),  //rectangle x point 4
                                            new Vector3(  1, -gr,  -1),  //rectangle y point 1
                                            new Vector3(  1,  gr,  -1),  //rectangle y point 2
                                            new Vector3( -1,  gr,  -1),  //rectangle y point 3
                                            new Vector3( -1, -gr,  -1),  //rectangle y point 4
                                            new Vector3(  0,  -1, -gr),  //rectangle z point 1
                                            new Vector3(  0,   1, -gr),  //rectangle z point 2
                                            new Vector3(  0,   1,  gr),  //rectangle z point 3
                                            new Vector3(  0,  -1,  gr) };//rectangle z point 4

        faceMesh.triangles = new int[] { 0,  9,  5,//triangles around point 0
                                         0,  5,  3,
                                         0,  3,  4,
                                         0,  4,  8,
                                         0,  8,  9,
                                         2, 10,  6,//triangles around point 2
                                         2,  6,  1,
                                         2,  1,  7,
                                         2,  7, 11,
                                         2, 11, 10,
                                         3, 10, 11,//triangles between ends
                                         3, 11,  4,
                                         4, 11,  7,
                                         4,  7,  8,
                                         8,  7,  1,
                                         8,  1,  9,
                                         9,  1,  6,
                                         9,  6,  5,
                                         5,  6, 10,
                                         5,  10, 3};
    }
	
	void Update () {
	
	}
}

    
