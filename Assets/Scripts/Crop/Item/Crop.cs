using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MFarm.Inventory;
using MFarm.Map;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//BUG: 1.树冠和animator冲突无法半透明 2.判断鼠标点击范围时会点到格子外面 3播倒下动画期间再次点击 4捡不起来道具 5.树桩没有碰撞体(可以获取树的coll赋值过来,但没必要)
namespace MFarm.Plant
{
    public class Crop : MonoBehaviour
    {
        private CropDetails _cropDetails;

        private TileDetails _tileDetails;

        // 因为Crop刷新是直接销毁对象再重新创建,所以只能存在创建时可以获取的数据

        private Sprite Sprite
        {
            set => _spriteRenderer.sprite = value;
        }

        private SpriteRenderer _spriteRenderer;

        private Animator _animator;

        private Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponentInChildren<Animator>();
                }

                return _animator;
            }
        }
        // private BoxCollider2D _coll;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            // _coll = GetComponent<BoxCollider2D>();
        }


        public void Init(TileDetails tileDetails, CropDetails cropDetails, Sprite sprite)
        {
            _tileDetails = tileDetails;
            _cropDetails = cropDetails;
            Sprite = sprite;
        }

        public void BeHarvested(int toolID)
        {
            _tileDetails.beUsedToolCount += _cropDetails.GetToolEffect(toolID);
            //_animator = GetComponentInChildren<Animator>();
            if (_tileDetails.beUsedToolCount < _cropDetails.needUseToolCount)
            {
                //摇晃动画
                if (Animator)
                {
                    Animator.SetTrigger("chopped");
                }

                //粒子

                //声音
            }
            else
            {
                SpawnProduce();
                _tileDetails.beHarvestedCount++;
                _tileDetails.beUsedToolCount = 0;
                //收获次数没了之后销毁物体
                if (_tileDetails.beHarvestedCount > _cropDetails.regrowTimes)
                {
                    _tileDetails.ResetCropInfo(); //重置tile里的农作物相关数据
                    Animator.SetTrigger("falldown");
                    UniTask.Void(async () =>
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(1.3f));
                        Destroy(gameObject);
                    });

                    if (_cropDetails.transferItemID > 0)
                    {
                        ItemManager.Instance.InstantiateItemInScene(_cropDetails.transferItemID, transform.position);
                    }
                }
                else
                {
                    _tileDetails.growthDays = _cropDetails.TotalGrowthDays - _cropDetails.daysToRegrow;
                    //可重复收获时,回到倒数第二阶段
                    Destroy(gameObject);
                    CropManager.Instance.CreateCropItem(_tileDetails);
                }
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
                        ItemManager.Instance.DropItemRandomInScene(_cropDetails.producedItemIDs[i], new Vector3(transform.position.x, transform.position.y + 1f, 0), _cropDetails.spawnRadius);
                }
            }
        }
    }
}