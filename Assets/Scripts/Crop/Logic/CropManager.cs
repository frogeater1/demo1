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

        public Crop normalCropPrefab;

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
            CreateCropItem(seedItemID, tileDetails);
        }

        private void OnAfterLoadScene()
        {
            _cropParent = GameObject.FindGameObjectWithTag("CropParent").transform;
        }

        public CropDetails GetCropDetails(int itemID)
        {
            return cropData.cropDetailsList.Find(i => i.seedItemID == itemID);
        }

        public bool CheckCanSow(int itemID, TileDetails tileDetails)
        {
            var crop_details = GetCropDetails(itemID);
            return tileDetails.daysSinceDug > -1 && tileDetails.seedItemID == -1 && crop_details != null && crop_details.seasons.Contains(CurSeason);
        }

        private void CreateCropItem(int seedItemID, TileDetails tileDetails)
        {
            //+0.5是为了在格子正中间
            Vector3 pos = new Vector3(tileDetails.pos.x + 0.5f, tileDetails.pos.y + 0.5f, 0);
            Crop cropInstance = Instantiate(normalCropPrefab, pos, Quaternion.identity, _cropParent);
            cropInstance.Init(seedItemID, tileDetails);
        }
    }
}