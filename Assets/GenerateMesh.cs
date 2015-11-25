using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMesh : MonoBehaviour {
    new public GameObject gameObject;

    void Start () {
		
        gameObject = GameObject.Find("Face");
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        Mesh faceMesh = GetComponent<MeshFilter>().mesh;
        faceMesh.Clear();
        
		Vector3 origin = new Vector3 (0, 0, 0);
		float gr = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;//golen ratio (a+b is to a as a is to b)
		float centerDist = (Mathf.Sqrt ((gr * gr) + 1));
		int smoothLevel = 1;

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
       
		List<int> trianglesIndices = new List<int> () {	
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

		//faceMesh.triangles = trianglesIndices.ToArray();
		//faceMesh.vertices
		//faceMesh.vertices = newVertices.ToArray();
		//trianglesIndices = newTriangleIndices;
		//faceMesh.triangles = trianglesIndices.ToArray ();

		for (int i = 0; i < smoothLevel; i++)//recurs a number of times = the smoothLevel, each loop quadruples the number of vertices
		{
			List<int> newTriangleIndices = new List<int>();
			List<Vector3> newVertices = new List<Vector3>();//to be placed over facemesh.vertices
		
			for (int j = 0; j < trianglesIndices.Count; j+=3)//for every third index in trianglesVertices 
			{
				int sm = j*2;

				Vector3 vertexZero 	= faceMesh.vertices[trianglesIndices[j]];
				Vector3 vertexOne 	= faceMesh.vertices[trianglesIndices[j+1]];
				Vector3 vertexTwo = faceMesh.vertices[trianglesIndices[j+2]];
				Vector3 vertexThree 	= GetMidpoint(vertexZero, 	vertexOne);		//gets the midpoints of the 
				Vector3 vertexFour 	= GetMidpoint(vertexOne, 	vertexTwo); 	//three indices of each triangle
				Vector3 vertexFive 	= GetMidpoint(vertexTwo, 	vertexZero);

				newVertices.AddRange(new [] {
					vertexZero,
					vertexOne,
					vertexTwo,
					vertexThree,
					vertexFour,
					vertexFive
				});


				newTriangleIndices.AddRange(new [] {	
					0+sm,3+sm,5+sm,
					3+sm,1+sm,4+sm,
					5+sm,4+sm,2+sm,
					5+sm,3+sm,4+sm});
			}
			faceMesh.vertices = newVertices.ToArray();//replace the previous set of vertices with the smoothed one
			trianglesIndices = newTriangleIndices;
			faceMesh.triangles = trianglesIndices.ToArray ();
			for (int k = 0; k < faceMesh.vertices.Length; k++) {
				faceMesh.vertices[k] = Roundify(faceMesh.vertices[k], origin, centerDist);
				//float debug = Vector3.Distance(faceMesh.vertices[k], origin);
				//float z = 0f;
			}
		}
	}
	
	
	void Update () {
		
	}

	Vector3 GetMidpoint(Vector3 vectorA, Vector3 vectorB) {
		Vector3 midpointVectorAB = new Vector3();
		midpointVectorAB.x = (vectorA.x + vectorB.x) / 2;
		midpointVectorAB.y = (vectorA.y + vectorB.y) / 2;
		midpointVectorAB.z = (vectorA.z + vectorB.z) / 2;
		return midpointVectorAB;
	}

	Vector3 Roundify(Vector3 vertex,Vector3 center, float distance)
	{
		Vector3 offset = vertex - center;
		offset.Normalize ();
		return offset * distance;
		//Vector3 bug = (vertex - center).normalized * distance + center;
		//return bug;
	}
}

    
