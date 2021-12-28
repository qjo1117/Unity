using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{
	const float scale = 5.0f;

	const float viewerMoveThresholdForChunkUpdate = 25.0f;
	const float sqrViewMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

	public LODInfo[] detailLevels;
	public static float maxViewDst;

	public Transform viewer;
	public Material mapMaterial;

	public static Vector2 viewerPosition;
	private Vector2 viewerPositionOld;
	private static MapGenerator mapGenerator;

	int chunkSize;
	int chunkVisibleInViewDst;

	// Mesh를 저장하기 위한 Dictionary
	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

	// 보일 Object를 저장하기 위한 자료구조
	static List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();

	/*
	 * 공간을 배열을 담는 듯이 정의하고 Dist를 구해서
	 * Bound에 포함된 Object들을 보여주고 나머지는 비활성화시켜준다.
	 * 
	 * 이걸로 Update 연산은 조금 늘어나지만 크게 그려지는 Terrain Rendering 연산을 감소시켜준다.
	*/

	private void Start()
	{
		mapGenerator = FindObjectOfType<MapGenerator>();

		maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		chunkSize = MapGenerator.mapChunkSize;
		chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

		UpdateVisibleChunks();
	}

	private void Update()
	{
		viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

		if((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewMoveThresholdForChunkUpdate) {
			viewerPositionOld = viewerPosition;
			UpdateVisibleChunks();
		}
	}



	void UpdateVisibleChunks()
	{
		for (int i = 0; i < terrainChunkVisibleLastUpdate.Count; ++i) {
			terrainChunkVisibleLastUpdate[i].SetVisible(false);
		}
		terrainChunkVisibleLastUpdate.Clear();

		int currentChunckCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
		int currentChunckCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

		for (int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; ++yOffset) {
			for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; ++xOffset) {
				Vector2 viewedChunkCoord = new Vector2(currentChunckCoordX + xOffset, currentChunckCoordY + yOffset);

				if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
					terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
				}
				else {
					terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
				}
			}
		}
	}

	public class TerrainChunk
	{
		GameObject meshObject;
		Vector2 position;
		Bounds bounds;

		MeshRenderer meshRenderer;
		MeshFilter meshFilter;

		LODInfo[] detailLevels;
		LODMesh[] lodMeshs;

		MapData mapData;
		bool mapDataReceived;
		int previousLODIndex = -1;

		public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
		{
			this.detailLevels = detailLevels;

			position = coord * size;
			Vector3 positionV3 = new Vector3(position.x, 0.0f, position.y);

			bounds = new Bounds(position, Vector2.one * size);

			meshObject = new GameObject("Terrain Chunk");
			meshRenderer = meshObject.AddComponent<MeshRenderer>();
			meshFilter = meshObject.AddComponent<MeshFilter>();
			meshRenderer.material = material;

			meshObject.transform.position = positionV3 * scale;
			meshObject.transform.parent = parent;
			meshObject.transform.localScale = Vector3.one * scale;
			SetVisible(false);

			lodMeshs = new LODMesh[detailLevels.Length];
			for (int i = 0; i < detailLevels.Length; ++i) {
				lodMeshs[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
			}

			mapGenerator.RequestMapData(position, OnMapDataReceived);
		}

		private void OnMapDataReceived(MapData mapData)
		{
			this.mapData = mapData;
			mapDataReceived = true;

			int chunkSize = MapGenerator.mapChunkSize;
			Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, chunkSize, chunkSize);
			meshRenderer.material.mainTexture = texture;

			UpdateTerrainChunk();
		}

		private void OnMeshDataReceived(MeshData meshData)
		{
			meshFilter.mesh = meshData.CreateMesh();
		}

		public void UpdateTerrainChunk()
		{
			if(mapDataReceived == false) {
				return;
			}

			float viewerDstFromNearstEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			bool visible = viewerDstFromNearstEdge <= maxViewDst;

			if(visible) {
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; ++i) {
					if(viewerDstFromNearstEdge > detailLevels[i].visibleDstThreshold) {
						lodIndex = i + 1;
					}
					else {
						break;
					}
				}

				if(lodIndex != previousLODIndex) {
					LODMesh lodMesh = lodMeshs[lodIndex];
					if(lodMesh.hasMesh) {
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
					}
					else if (!lodMesh.hasRequestedMesh) {
						lodMesh.RequestMesh(mapData);
					}
				}

				terrainChunkVisibleLastUpdate.Add(this);
			}
			
			SetVisible(visible);
		}

		public void SetVisible(bool visible)
		{
			meshObject.SetActive(visible);
		}

		public bool isVisible()
		{
			return meshObject.activeSelf;
		}
	}

	class LODMesh
	{
		public Mesh mesh;
		public bool hasRequestedMesh;
		public bool hasMesh;
		int lod;

		System.Action updateCallback;

		public LODMesh(int lod, System.Action updateCallback)
		{
			this.lod = lod;
			this.updateCallback = updateCallback;
		}

		void OnMeshDataReceived(MeshData meshData)
		{
			mesh = meshData.CreateMesh();
			hasMesh = true;

			updateCallback();
		}

		public void RequestMesh(MapData mapData)
		{
			hasRequestedMesh = true;
			mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
		}
	}


	[System.Serializable]
	public struct LODInfo
	{
		public int lod;
		public float visibleDstThreshold;
	}
}
