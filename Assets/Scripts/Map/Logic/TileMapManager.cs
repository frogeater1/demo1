using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MFarm.Map
{
    public class TileMapManager : Singleton<TileMapManager>
    {
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
            
            string key = SceneManager.GetActiveScene().name  + gridPos.x + "x" + gridPos.y + "y";
            _tileDetailsDict.TryGetValue(key, out TileDetails details);
            return details;
        }
    }
}