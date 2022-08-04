using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TileMap : MonoBehaviour
{
    public MapData_SO mapData;

    public TileType tileType;

    private Tilemap _currentTilemap;


    private void OnEnable()
    {
        if (Application.IsPlaying(this)) return;
        
        _currentTilemap = GetComponent<Tilemap>();
        if (mapData)
            mapData.tileProperties.Clear();
    }

    private void OnDisable()
    {
        if(Application.IsPlaying(this)) return;

        _currentTilemap.GetComponent<Tilemap>();
        UpdateTileProperties();
#if UNITY_EDITOR
        if (mapData != null)
            EditorUtility.SetDirty(mapData);
#endif
    }

    private void UpdateTileProperties()
    {
        if(Application.IsPlaying(this)) return;

        _currentTilemap.CompressBounds();

        if (mapData)
        {
            //左下角到右上角
            BoundsInt cellBounds = _currentTilemap.cellBounds;
            Vector3Int startPos = cellBounds.min;
            Vector3Int endPos = cellBounds.max;

            for (int x = startPos.x; x < endPos.x; x++)
            {
                for (int y = startPos.y; y < endPos.y; y++)
                {
                    TileBase tile = _currentTilemap.GetTile(new Vector3Int(x, y, 0));

                    if (!tile) continue;
                    var newTile = new TileProperty
                    {
                        tilePos = new Vector2Int(x, y),
                        tileType = tileType
                    };

                    mapData.tileProperties.Add(newTile);
                }
            }
        }
        
    }
}