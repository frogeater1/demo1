using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Plant
{
    public class Crop : MonoBehaviour
    {
        public CropDetails CropDetails { get; set; }

        public TileDetails TileDetails { get; set; }

        private Sprite Sprite
        {
            //get => _spriteRenderer.sprite;
            set => _spriteRenderer.sprite = value;
        }

        private SpriteRenderer _spriteRenderer;
        // private BoxCollider2D _coll;
        
        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // _coll = GetComponent<BoxCollider2D>();
        }

        public void Init(TileDetails tileDetails)
        {
            TileDetails = tileDetails;
            CropDetails = CropManager.Instance.GetCropDetails(tileDetails.seedItemID);
            //成长阶段
            int growthStages = CropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = CropDetails.TotalGrowthDays;

            //倒序计算当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }

                dayCounter -= CropDetails.growthDays[i];
            }
            Sprite = CropDetails.growthSprites[currentStage];
        }
    }
}