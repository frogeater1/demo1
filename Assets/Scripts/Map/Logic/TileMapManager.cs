using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace MFarm.Map
{
    public class TileMapManager : Singleton<TileMapManager>
    {
        [Header("种地瓦片切换信息")]
        public RuleTile digTile;

        public RuleTile waterTile;

        private Tilemap _digTilemap;

        private Tilemap _waterTilemap;

        [Header("地图信息")]
        public List<MapData_SO> mapDataList;

        private readonly Dictionary<string, TileDetails> _tileDetailsDict = new();

        public Grid currentGrid;


        private void OnEnable()
        {
            EventHandler.AfterLoadScene += OnAfterLoadScene;
        }

        private void OnDisable()
        {
            EventHandler.AfterLoadScene -= OnAfterLoadScene;
        }

        private void OnAfterLoadScene()
        {
            currentGrid = FindObjectOfType<Grid>();
            _digTilemap = GameObject.FindGameObjectWithTag("DigTile").GetComponent<Tilemap>();
            _waterTilemap = GameObject.FindGameObjectWithTag("WaterTile").GetComponent<Tilemap>();
        }

        private void Start()
        {
            foreach (MapData_SO mapData in mapDataList)
            {
                InitTileDetailsDict(mapData);
            }
        }

        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails;
                //字典的Key
                string key = mapData.sceneName + tileProperty.tilePos.x + "x" + tileProperty.tilePos.y + "y";
                if (_tileDetailsDict.ContainsKey(key))
                {
                    tileDetails = _tileDetailsDict[key];
                }
                else
                {
                    tileDetails = new TileDetails
                    {
                        pos = tileProperty.tilePos
                    };
                }

                switch (tileProperty.tileType)
                {
                    case TileType.canDig:
                        tileDetails.canDig = true;
                        break;
                    case TileType.canDropItem:
                        tileDetails.canDropItem = true;
                        break;
                    case TileType.canPlaceFurniture:
                        tileDetails.canPlaceFurniture = true;
                        break;
                    case TileType.NpcObstacle:
                        tileDetails.isNpcObstacle = true;
                        break;
                }

                _tileDetailsDict[key] = tileDetails;
            }
        }

        public TileDetails GetTileDetails(Vector3Int gridPos)
        {
            string key = SceneManager.GetActiveScene().name + gridPos.x + "x" + gridPos.y + "y";
            _tileDetailsDict.TryGetValue(key, out TileDetails details);
            return details;
        }

        public void SetDigTile(TileDetails tileDetails)
        {
            var pos = new Vector3Int(tileDetails.pos.x, tileDetails.pos.y, 0);
            if (_digTilemap)
            {
                _digTilemap.SetTile(pos,digTile);
                MarkDug(tileDetails);
            }
            // else
            // {
            //     Debug.LogError("没有正确获取到DigTileMap");
            // }
        }

        private void MarkDug(TileDetails tileDetails)
        {
            tileDetails.canDig = false;
            tileDetails.canDropItem = false;
            tileDetails.daysSinceDug = 0;
        }
         public void SetWaterTile(TileDetails tileDetails)
        {
            var pos = new Vector3Int(tileDetails.pos.x, tileDetails.pos.y, 0);
            if (_waterTilemap)
            {
                _waterTilemap.SetTile(pos,waterTile);
                MarkWatered(tileDetails);
            }
            // else
            // {
            //     Debug.LogError("没有正确获取到WaterTileMap");
            // }
        }

         private void MarkWatered(TileDetails tileDetails)
         {
             tileDetails.daysSinceWatered = 0;
         }
        
    }
}