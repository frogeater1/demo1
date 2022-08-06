using System.Collections;
using System.Collections.Generic;
using MFarm.Inventory;
using UnityEngine;

namespace MFarm.Plant
{
    public class Crop : MonoBehaviour
    {
        private CropDetails _cropDetails;

        private TileDetails _tileDetails;

        public int _beHarvestCount;

        private Sprite Sprite
        {
            //get => _spriteRenderer.sprite;
            set => _spriteRenderer.sprite = value;
        }

        private SpriteRenderer _spriteRenderer;

        private Animator _animator;
        // private BoxCollider2D _coll;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // _coll = GetComponent<BoxCollider2D>();
        }

        public void Init(TileDetails tileDetails)
        {
            _tileDetails = tileDetails;
            _cropDetails = CropManager.Instance.GetCropDetails(tileDetails.seedItemID);
            //成长阶段
            int growthStages = _cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = _cropDetails.TotalGrowthDays;

            //倒序计算当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }

                dayCounter -= _cropDetails.growthDays[i];
            }

            Sprite = _cropDetails.growthSprites[currentStage];
        }

        public void BeHarvested(int toolID)
        {
            _beHarvestCount += _cropDetails.GetToolEffect(toolID);
            //_animator = GetComponentInChildren<Animator>();
            if (_beHarvestCount < _cropDetails.needUseToolCount)
            {
                //动画

                //粒子

                //声音
            }
            else
            {
                SpawnProduce();
            }
        }

        private void SpawnProduce()
        {
            //直接添加到背包
            for (int i = 0; i < _cropDetails.producedItemIDs.Length; i++)
            {
                var amount = Random.Range(_cropDetails.producedMinAmounts[i], _cropDetails.producedMaxAmounts[i]);
                if (_cropDetails.generateAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(_cropDetails.producedItemIDs[i], amount);
                }
                else
                {
                    for (int j = 0; j < amount; j++)
                        ItemManager.Instance.DropItemRandomInScene(_cropDetails.producedItemIDs[i], new Vector3(transform.position.x, transform.position.y + 1f, 0));
                }
            }
        }
    }
}