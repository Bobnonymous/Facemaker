using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerateMesh : MonoBehaviour {
    
	public GameObject Face;
	public Mesh faceMesh;
   
    float gr = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;//golen ratio (a+b is to a as a is to b)
    Vector3 origin = new Vector3 (0, 0, 0);

    void Start() {

        Face = GameObject.Find("Face");
        Face.AddComponent<MeshFilter>();
        Face.AddComponent<MeshRenderer>();

        faceMesh = GetComponent<MeshFilter>().mesh;
        faceMesh.Clear();

        float radius = (Mathf.Sqrt((gr * gr) + 1));
        int smoothLevel = 3;

        // create 12 vertices of a icosahedron, via 3 intersecting rectangles
        faceMesh.vertices = new Vector3[] {
            new Vector3( gr,   0,  -1),  //rectangle x point 1
            new Vector3(-gr,   0,  -1),  //rectangle x point 2
            new Vector3(-gr,   0,   1),  //rectangle x point 3
            new Vector3( gr,   0,   1),  //rectangle x point 4
            new Vector3(  1, -gr,   0),  //rectangle y point 1
            new Vector3(  1,  gr,   0),  //rectangle y point 2
            new Vector3( -1,  gr,   0),  //rectangle y point 3
            new Vector3( -1, -gr,   0),  //rectangle y point 4
            new Vector3(  0,  -1, -gr),  //rectangle z point 1
            new Vector3(  0,   1, -gr),  //rectangle z point 2
            new Vector3(  0,   1,  gr),  //rectangle z point 3
            new Vector3(  0,  -1,  gr) };//rectangle z point 4

        List<int> trianglesIndices = new List<int>() {
            0,  9,  5,//triangles around point 0
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

        //Triangle smoothing loop, each loop quadruples the number of faces
        for (int i = 0; i < smoothLevel; i++)
        {
            List<int> newTriangleIndices = new List<int>();
            List<Vector3> newVertices = new List<Vector3>();//to be placed over facemesh.vertices

            for (int j = 0; j < trianglesIndices.Count; j += 3)//for every three indices, each set of three represents a triangle
            {
                int sm = j * 2;//start multiplier

                Vector3 vertexZero =    SetVectorDist (faceMesh.vertices[trianglesIndices[j]],      origin, radius);
                Vector3 vertexOne =     SetVectorDist (faceMesh.vertices[trianglesIndices[j + 1]],  origin, radius);
                Vector3 vertexTwo =     SetVectorDist (faceMesh.vertices[trianglesIndices[j + 2]],  origin, radius);

                Vector3 vertexThree =   SetVectorDist (GetMidpoint(vertexZero, vertexOne),  origin, radius);//gets the midpoints of the 
                Vector3 vertexFour =    SetVectorDist (GetMidpoint(vertexOne, vertexTwo),   origin, radius);//three indices of each triangle
                Vector3 vertexFive =    SetVectorDist (GetMidpoint(vertexTwo, vertexZero),  origin, radius);

                newVertices.AddRange(new[] {//adds the six new vertices to the list
					vertexZero,
                    vertexOne,
                    vertexTwo,
                    vertexThree,
                    vertexFour,
                    vertexFive
                });

                newTriangleIndices.AddRange(new[] {//arranges the vertices to create four new triangles in place of the starting triangle
					0+sm,3+sm,5+sm,
                    3+sm,1+sm,4+sm,
                    5+sm,4+sm,2+sm,
                    5+sm,3+sm,4+sm});
            }
            faceMesh.vertices = newVertices.ToArray();//replace the previous set of vertices with the smoothed one
            trianglesIndices = newTriangleIndices;
            faceMesh.triangles = trianglesIndices.ToArray();
        }


        //Mesh mutation
        List<Vector3> overwriteVertices = new List<Vector3>();
        Vector3 xMostVertex = new Vector3(0, 0, 0);//the vertex that is the furthest from the center on the x axis, this is the direction the face looks

        for (int i = 0; i < faceMesh.vertices.Length; i++) {//for each vertex in facemesh

            if (faceMesh.vertices[i].x > xMostVertex.x)
                xMostVertex.x = faceMesh.vertices[i].x;//finds x most

            Vector3 overwriteVertex = faceMesh.vertices[i];
            if (overwriteVertex.y >= 0)
            {
                
            }
            if (overwriteVertex.y < 0)
            {
                overwriteVertex.y = overwriteVertex.y * 1.3f;
            }
            overwriteVertices.Add(overwriteVertex);
        }
        faceMesh.vertices = overwriteVertices.ToArray();

        GameObject eye1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);eye1.name = "rightEye";
        eye1.transform.position = new Vector3(xMostVertex.x*0.85f, 0, -gr*0.3f);
        eye1.transform.localScale = new Vector3(gr/3, 0.5f, 1);
        GameObject eye2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);eye2.name = "leftEye";
        eye2.transform.position = new Vector3(xMostVertex.x*0.85f, 0, gr*0.3f);
        eye2.transform.localScale = new Vector3(gr/3, 0.5f, 1);

        Color fleshtone = new Color(240, 205, 180);
        Material material = new Material(Shader.Find("Diffuse"));
        material.color = fleshtone;
        Face.GetComponent<Renderer>().material = material;

        //Doesnt Work VVVVV
        faceMesh.uv = new Vector2[faceMesh.vertices.Length];
		for (int i = 0; i < faceMesh.uv.Length; i++) {
			faceMesh.uv[i] = new Vector2(faceMesh.vertices[i].x, faceMesh.vertices[i].z);

            faceMesh.Optimize();
        }
	}
	
	bool grabbed;

	void Update () {
		if (Input.GetMouseButton (0)) {
			RaycastHit raycastHit;
			if (Physics.Raycast (Camera.main.transform.position, Input.mousePosition, out raycastHit)) {
				grabbed = true;
				int hitTriangle = raycastHit.triangleIndex;
				Vector3 p0 = faceMesh.vertices[faceMesh.triangles[hitTriangle * 3 + 0]];
				Vector3 p1 = faceMesh.vertices[faceMesh.triangles[hitTriangle * 3 + 1]];
				Vector3 p2 = faceMesh.vertices[faceMesh.triangles[hitTriangle * 3 + 2]];
				SetVectorDist(p0, origin, 200);
				SetVectorDist(p0, origin, 200);
				SetVectorDist(p0, origin, 200);
			}
		}
	}

    //returns the midpoint of two Vector3s
	Vector3 GetMidpoint(Vector3 vectorA, Vector3 vectorB) {
		Vector3 midpointVectorAB = new Vector3();
		midpointVectorAB.x = (vectorA.x + vectorB.x) / 2;
		midpointVectorAB.y = (vectorA.y + vectorB.y) / 2;
		midpointVectorAB.z = (vectorA.z + vectorB.z) / 2;
		return midpointVectorAB;
	}

    //sets the distance between vertex and center to float distance
	Vector3 SetVectorDist(Vector3 vertex, Vector3 center, float distance)
	{
		Vector3 offset = vertex - center;
		offset.Normalize ();
		return offset * distance;
	}
}

    
