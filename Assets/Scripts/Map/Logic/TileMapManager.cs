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

        //两层字典<sceneName,dict<pos,tileDetails>>
        private readonly Dictionary<string, Dictionary<string, TileDetails>> _tileDetailsDict = new();

        public Grid currentGrid;

        // private Season _curSeason;


        private void OnEnable()
        {
            EventHandler.AfterLoadScene += OnAfterLoadScene;
            EventHandler.GameDateUpdate += OnGameDateUpdate;
        }

        private void OnDisable()
        {
            EventHandler.AfterLoadScene -= OnAfterLoadScene;
            EventHandler.GameDateUpdate -= OnGameDateUpdate;
        }

        private void OnAfterLoadScene()
        {
            currentGrid = FindObjectOfType<Grid>();
            _digTilemap = GameObject.FindGameObjectWithTag("DigTile").GetComponent<Tilemap>();
            _waterTilemap = GameObject.FindGameObjectWithTag("WaterTile").GetComponent<Tilemap>();

            SetCurSceneTileMaps();
        }

        private void OnGameDateUpdate(int year, int month, int day, Season season)
        {
            // _curSeason = season;
            //此处要更新所有场景的数据
            foreach (var v1 in _tileDetailsDict.Values)
            {
                foreach (var v2 in v1.Values)
                {
                    if (v2.daysSinceWatered > -1)
                        v2.daysSinceWatered = -1; //浇水只持续一天
                    if (v2.daysSinceDug > -1)
                    {
                        v2.daysSinceDug++;
                    }

                    if (v2.daysSinceDug > 5 && v2.seedItemID == -1)
                    {
                        v2.daysSinceDug = -1;
                        v2.canDig = true;
                    }
                }
            }

            ReSetCurSceneTileMaps();
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
            Dictionary<string, TileDetails> cur_scene_dict = new();
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                //字典的Key
                string key = "x" + tileProperty.tilePos.x + "y" + tileProperty.tilePos.y;

                cur_scene_dict.TryGetValue(key, out TileDetails tileDetails);
                tileDetails ??= new TileDetails
                {
                    pos = tileProperty.tilePos
                };

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

                cur_scene_dict[key] = tileDetails;
            }

            _tileDetailsDict.Add(mapData.sceneName, cur_scene_dict);
        }

        public TileDetails GetTileDetails(Vector3Int gridPos)
        {
            string key = "x" + gridPos.x + "y" + gridPos.y;
            _tileDetailsDict.TryGetValue(SceneManager.GetActiveScene().name, out var cur_scene_dict);
            if (cur_scene_dict == null) return null;
            cur_scene_dict.TryGetValue(key, out TileDetails details);
            return details;
        }

        //因为都是引用 似乎不需要set
        // private void SetTileDetails(TileDetails tileDetails)
        // {
        //     string key = SceneManager.GetActiveScene().name + tileDetails.pos.x + "x" + tileDetails.pos.y + "y";
        //     _tileDetailsDict[key] = tileDetails;
        // }

        public void Dig(TileDetails tileDetails)
        {
            tileDetails.canDig = false;
            tileDetails.canDropItem = false;
            tileDetails.daysSinceDug = 0;
            SetDigTile(tileDetails);
        }

        private void SetDigTile(TileDetails tileDetails)
        {
            var pos = new Vector3Int(tileDetails.pos.x, tileDetails.pos.y, 0);
            if (_digTilemap)
            {
                _digTilemap.SetTile(pos, digTile);
            }
#if DEBUGLOG
            else
            {
                Debug.LogError("没有正确获取到DigTileMap");
            }
#endif
        }

        public void Water(TileDetails tileDetails)
        {
            tileDetails.daysSinceWatered = 0;
            SetWaterTile(tileDetails);
        }

        public void SetWaterTile(TileDetails tileDetails)
        {
            var pos = new Vector3Int(tileDetails.pos.x, tileDetails.pos.y, 0);
            if (_waterTilemap)
            {
                _waterTilemap.SetTile(pos, waterTile);
            }
#if DEBUGLOG
            else
            {
                Debug.LogError("没有正确获取到WaterTileMap");
            }
#endif
        }

        private void SetCurSceneTileMaps()
        {
            string cur_scene_name = SceneManager.GetActiveScene().name;
            _tileDetailsDict.TryGetValue(cur_scene_name, out var cur_scene_dict);

            if (cur_scene_dict == null) return;
            foreach (TileDetails v in cur_scene_dict.Values)
            {
                if (v.daysSinceDug > -1)
                {
                    SetDigTile(v);
                }

                if (v.daysSinceWatered > -1)
                    SetWaterTile(v);
                //TOADD: 使用道具设置TileMap
                // if (tileDetails.seedItemID > -1)
                //     EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
            }
        }

        private void ReSetCurSceneTileMaps()
        {
            //等待GO支持?.
            if (_digTilemap) _digTilemap.ClearAllTiles();
            if (_waterTilemap) _waterTilemap.ClearAllTiles();
            SetCurSceneTileMaps();
        }
    }
}