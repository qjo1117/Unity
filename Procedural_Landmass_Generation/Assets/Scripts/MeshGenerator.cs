using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
	{
		AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width - 1) / -2.0f;
		float topLeftZ = (height - 1) / 2.0f;

		int meshSimplicationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplicationIncrement + 1;

		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height; y += meshSimplicationIncrement) {
			for (int x = 0; x < width; x += meshSimplicationIncrement) {
				// 현재 xz를 평면으로 깔아주고 y의 높이를 지정해준다.
				meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
				// uvs의 좌표를 x,y기준으로 셋팅을 해준다.
				meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

				if(x < width - 1 && y < height - 1) {
					meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
					meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
				} 

				vertexIndex += 1;
			}
		}

		return meshData;
	}
}


public class MeshData
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	private int triangleIndex = 0;

	public MeshData(int meshWidth, int meshHeight)
	{
		vertices = new Vector3[meshWidth * meshHeight];
		triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
		uvs = new Vector2[meshWidth * meshHeight];
	}

	public void AddTriangle(int a, int b, int c)
	{
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	Vector3[] CalculateNormals()
	{
		Vector3[] vertexNormalis = new Vector3[vertices.Length];
		int triangleCount = triangles.Length / 3;
		for (int i = 0; i < triangleCount; ++i) {
			int normalTriangleIndex = i * 3;
			int vertexIndexA = triangles[normalTriangleIndex];
			int vertexIndexB = triangles[normalTriangleIndex + 1];
			int vertexIndexC = triangles[normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormaliFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormalis[vertexIndexA] += triangleNormal;
			vertexNormalis[vertexIndexB] += triangleNormal;
			vertexNormalis[vertexIndexC] += triangleNormal;
		}

		for (int i = 0; i < vertexNormalis.Length; ++i) {
			vertexNormalis[i].Normalize();
		}

		return vertexNormalis;
	}

	Vector3 SurfaceNormaliFromIndices(int indexA,int indexB,int indexC)
	{
		Vector3 pointA = vertices[indexA];
		Vector3 pointB = vertices[indexB];
		Vector3 pointC = vertices[indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross(sideAB, sideAC).normalized;
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.normals = CalculateNormals();
		return mesh;
	}
}