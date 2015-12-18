//Robert Cole - C12391946 - GameEngines 1 Assignment
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerateMesh : MonoBehaviour {
    
	public GameObject Face;
	public Mesh faceMesh;
    //MeshFilter meshFilter;
   
    float gr = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;//golen ratio (a+b is to a as a is to b)
    Vector3 origin = new Vector3 (0, 0, 0);

    void Start() {

        Face = GameObject.Find("Face");
        Face.AddComponent<MeshFilter>();
        Face.AddComponent<MeshRenderer>();

        faceMesh = GetComponent<MeshFilter>().mesh;
        faceMesh.Clear();

        float radius = (Mathf.Sqrt((gr * gr) + 1));//the radius is the diagonal of the rectangle with height = 1 and width = gr
        int smoothLevel = 4;

        // create 12 vertices of an icosahedron, via 3 intersecting rectangles
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
            List<Vector3> newVertices = new List<Vector3>();//these lists will overwrite the previous vertices & triangles
            List<int> newTriangleIndices = new List<int>();

            for (int j = 0; j < trianglesIndices.Count; j += 3)//every set of three indices is a distinct triangle
            {
                int sm = j * 2;//start multiplier

                Vector3 vertexZero =    SetVectorDist (faceMesh.vertices[trianglesIndices[j    ]],  origin, radius);
                Vector3 vertexOne =     SetVectorDist (faceMesh.vertices[trianglesIndices[j + 1]],  origin, radius);
                Vector3 vertexTwo =     SetVectorDist (faceMesh.vertices[trianglesIndices[j + 2]],  origin, radius);

                Vector3 vertexThree =   SetVectorDist (GetMidpoint(vertexZero, vertexOne),  origin, radius);//get the midpoints of the three sides
                Vector3 vertexFour =    SetVectorDist (GetMidpoint(vertexOne,  vertexTwo),  origin, radius);
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


        //Mesh mutation, based on some basic facial proportions
        List<Vector3> overwriteVertices = new List<Vector3>();
        for (int i = 0; i < faceMesh.vertices.Length; i++) {

            Vector3 overwriteVertex = faceMesh.vertices[i];

            if (overwriteVertex.y >= 0){//shaping the upper half of the face
                overwriteVertex.y += gr*0.3f;
            }
            if (overwriteVertex.y < 0) {//shaping the lower half of the face
                overwriteVertex.x += (overwriteVertex.y * -0.25f);
                overwriteVertex.y *= 1.4f;
            }
            overwriteVertex.z *= 0.9f;
            overwriteVertices.Add(overwriteVertex);
        }
        faceMesh.SetVertices(overwriteVertices);
        
        //Add some eyes
        AddeEyes(Face, faceMesh);

        //Set Colour
        Material material = new Material(Shader.Find("Standard"));
        Color fleshtone = new Color(10, 205, 180);
        material.SetColor("fleshtone", fleshtone);

        Face.GetComponent<Renderer>().material = material;

        /*Color32[] meshColor = new Color32[faceMesh.triangles.Length];
        for (int i = 0; i < meshColor.Length; i++) {
            meshColor[i] = fleshtone;
        }*/
        
        faceMesh.RecalculateBounds();
        faceMesh.RecalculateNormals();
        faceMesh.Optimize();

        MeshCollider meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
    }
	
    public RaycastHit raycastHit;
    List<Vector3> overwriteVertices = new List<Vector3>();

    void Update () {
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        Camera camera = cameraObject.GetComponent<Camera>();
        overwriteVertices = faceMesh.vertices.ToList();

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            Physics.Raycast(camera.transform.position, camera.transform.forward, out raycastHit);
            //Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition));
           
            Vector3 hitPoint = raycastHit.point;
            Debug.Log(hitPoint + "hitpoint");
            Debug.Log(raycastHit.triangleIndex + "index of triangle");
            
            for (int i = 0; i < overwriteVertices.Count; i++) {
                if (Vector3.Distance(overwriteVertices[i], hitPoint) < 0.3f) {
					//expand if left mouse button, contract if right mouse button
                    float distance = Vector3.Distance(overwriteVertices[i], hitPoint);
                    if(Input.GetMouseButton(0))
                        overwriteVertices[i] *= 1.05f;
                    if (Input.GetMouseButton(1))
                        overwriteVertices[i] *= 0.95f;
                }
            }
            faceMesh.SetVertices(overwriteVertices);
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

	//Create two eye shapes and add the to the face
    void AddeEyes(GameObject Face, Mesh faceMesh) {
        Vector3 xMostVertex = new Vector3(); //Vertex furthest from the center on the x axis, this is the direction the face looks
            for (int i = 0; i < faceMesh.vertices.Length; i++) { 
            if (faceMesh.vertices[i].x > xMostVertex.x)
                xMostVertex.x = faceMesh.vertices[i].x;
        }

        GameObject rightEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightEye.name = "rightEye";
        rightEye.transform.parent = Face.transform;
        rightEye.transform.position = new Vector3(xMostVertex.x * 0.85f, 0, -gr * 0.3f);
        rightEye.transform.localScale = new Vector3(gr / 3, 0.5f, 1);
        GameObject leftEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//rightEye.GetComponent<Material> ().SetColor ("rightEye",Color.red);
        leftEye.name = "leftEye";
        leftEye.transform.parent = Face.transform;
        leftEye.transform.position = new Vector3(xMostVertex.x * 0.85f, 0, gr * 0.3f);
        leftEye.transform.localScale = new Vector3(gr / 3, 0.5f, 1);
    }
}

    
