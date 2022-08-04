using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Inventory
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Item itemPrefab;

        private Transform _itemParent;

        private readonly Dictionary<string, List<SceneItem>> _itemListDict = new();

        private void OnEnable()
        {
            // EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.AfterLoadScene += OnAfterLoadScene;
            EventHandler.BeforeUnloadScene += OnBeforeUnloadScene;
        }

        private void OnDisable()
        {
            // EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.AfterLoadScene -= OnAfterLoadScene;
            EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
        }

        private void OnBeforeUnloadScene()
        {
            GetSceneItems();
        }


        private void OnAfterLoadScene()
        {
            _itemParent =  GameObject.FindWithTag("ItemParent").transform;
            RecreateSceneItems();
        }

        public void InstantiateItemInScene(int itemID, Vector3 pos)
        {
            //instantiate会执行到那个物体的OnEnable()
            Item item = Instantiate(itemPrefab, pos, Quaternion.identity, _itemParent);
            item.itemID = itemID;
        }

        private void GetSceneItems()
        {
            List<SceneItem> list = new();
            foreach (Transform child in _itemParent)
            {
                child.TryGetComponent(out Item item);
                if (item)
                {
                    list.Add(new SceneItem { itemID = item.itemID, position = new SerializableVector3(item.transform.position) });
                }
            }
            
            _itemListDict[SceneManager.GetActiveScene().name] = list;
        }
        
        private async void RecreateSceneItems()
        {
            string scene_name = SceneManager.GetActiveScene().name;
            if (!_itemListDict.TryGetValue(scene_name, out List<SceneItem> currentSceneItems) || !currentSceneItems.Any()) return;
            int count = 0;
            foreach (SceneItem sceneItem in _itemListDict[scene_name])
            {
                InstantiateItemInScene(sceneItem.itemID, sceneItem.position.ToVector3());
                count++;
                if (count > 30) await UniTask.NextFrame();
            }
        }

        public void DropItemInScene(int itemID, Vector3 mouseWorldPos)
        {
            InstantiateItemInScene(itemID, mouseWorldPos);
        }
    }
}