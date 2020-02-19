using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{

    public const float maxViewDst = 450;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunckSize;
    int chuncksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunck> terrainChunckDictionary = new Dictionary<Vector2, TerrainChunck>();
    List<TerrainChunck> terrainChuncksVisibleLastUpdate = new List<TerrainChunck>();

    private void Start()
    {
        chunckSize = MapGenerator.mapChunckSize - 1;
        chuncksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunckSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChuncks();
    }

    void UpdateVisibleChuncks()
    {

        for(int i = 0; i < terrainChuncksVisibleLastUpdate.Count; i++)
        {
            terrainChuncksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChuncksVisibleLastUpdate.Clear();

        int currentChunckCoordX = Mathf.RoundToInt(viewerPosition.x / chunckSize);
        int currentChunckCoordY = Mathf.RoundToInt(viewerPosition.y / chunckSize);

        for(int yOffset = -chuncksVisibleInViewDst; yOffset <= chuncksVisibleInViewDst; yOffset++)
        {
            for(int xOffset = -chuncksVisibleInViewDst; xOffset <= chuncksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunckCoord = new Vector2(currentChunckCoordX + xOffset, currentChunckCoordY + yOffset);

                if (terrainChunckDictionary.ContainsKey(viewedChunckCoord))
                {
                    terrainChunckDictionary[viewedChunckCoord].UpdateTerrainChunck();
                    if (terrainChunckDictionary[viewedChunckCoord].IsVisible())
                    {
                        terrainChuncksVisibleLastUpdate.Add(terrainChunckDictionary[viewedChunckCoord]);
                    }
                }
                else
                {
                    terrainChunckDictionary.Add(viewedChunckCoord, new TerrainChunck(viewedChunckCoord, chunckSize, transform));
                }
            }
        }
    }

    public class TerrainChunck
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunck(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f;
            meshObject.transform.parent = parent;
            //SetVisible(false);
        }

        public void UpdateTerrainChunck()
        {
            float viewerDstFromNearestEdge =  Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
