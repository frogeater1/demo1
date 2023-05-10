using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MFarm.Plant
{
    public class CropManager : Singleton<CropManager>
    {
        [Header("所有农作物数据")]
        public CropData_SO cropData;

        private Transform _cropParent;

        private Season CurSeason => TimeManager.Instance.CurSeason;

        private void OnEnable()
        {
            EventHandler.Sow += OnSow;
            EventHandler.AfterLoadScene += OnAfterLoadScene;
        }


        private void OnDisable()
        {
            EventHandler.Sow -= OnSow;
            EventHandler.AfterLoadScene -= OnAfterLoadScene;
        }

        private void OnSow(int seedItemID, TileDetails tileDetails)
        {
            tileDetails.seedItemID = seedItemID;
            tileDetails.growthDays = 0;
            CreateCropItem(tileDetails);
        }

        private void OnAfterLoadScene()
        {
            _cropParent = GameObject.FindGameObjectWithTag("CropParent").transform;
        }

        public CropDetails GetCropDetails(int itemID)
        {
            return cropData.cropDetailsList.Find(i => i.seedItemID == itemID);
        }

        /// <summary>
        /// Check的时候还没种上,需要传递种子id和地图信息,在Onsow之后则不需要再单独传递种子id
        /// </summary>
        public bool CheckCanSow(int itemID, TileDetails tileDetails)
        {
            var crop_details = GetCropDetails(itemID);
            return tileDetails.daysSinceDug > -1 && tileDetails.seedItemID == -1 && crop_details != null && crop_details.seasons.Contains(CurSeason);
        }
        
        public void CreateCropItem(TileDetails tileDetails)
        {
            var cropDetails = CropManager.Instance.GetCropDetails(tileDetails.seedItemID);
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            //倒序计算当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }

                dayCounter -= cropDetails.growthDays[i];
            }

            var sprite = cropDetails.growthSprites[currentStage];
            var prefab = cropDetails.growthPrefabs[currentStage];

            //+0.5是为了在格子正中间
            Vector3 pos = new Vector3(tileDetails.pos.x + 0.5f, tileDetails.pos.y + 0.5f, 0);
            Crop cropInstance = Instantiate(prefab, pos, Quaternion.identity, _cropParent);
            cropInstance.Init(tileDetails, cropDetails, sprite);
        }

        public bool CheckCanHarvest(int toolID, TileDetails tileDetails)
        {
            var crop_details = GetCropDetails(tileDetails.seedItemID);
            return tileDetails.growthDays >= crop_details?.TotalGrowthDays && crop_details.ToolMatched(toolID);//任何值(包括null)跟null的比较大小(<,>,>=,<=)永远返回false
        }
        
    }
}