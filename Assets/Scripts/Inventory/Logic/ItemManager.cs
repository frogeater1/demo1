using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace MFarm.Inventory
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Item itemPrefab;

        private Transform _itemParent;
        private Transform _player;

        private readonly Dictionary<string, List<SceneItem>> _itemListDict = new();

        private void OnEnable()
        {
            EventHandler.AfterLoadScene += OnAfterLoadScene;
            EventHandler.BeforeUnloadScene += OnBeforeUnloadScene;
        }

        private void OnDisable()
        {
            EventHandler.AfterLoadScene -= OnAfterLoadScene;
            EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
        }

        private void OnBeforeUnloadScene()
        {
            GetSceneItems();
        }


        private void OnAfterLoadScene()
        {
            _itemParent = GameObject.FindWithTag("ItemParent").transform;
            _player = GameObject.FindWithTag("Player").transform;
            RecreateSceneItems();
        }

        public void InstantiateItemInScene(int itemID, Vector3 pos)
        {
            //instantiate会执行到那个物体的OnEnable()
            Item item = Instantiate(itemPrefab, pos, Quaternion.identity, _itemParent);
            item.ItemID = itemID;
        }

        private void GetSceneItems()
        {
            List<SceneItem> list = new();
            foreach (Transform child in _itemParent)
            {
                child.TryGetComponent(out Item item);
                if (item)
                {
                    list.Add(new SceneItem { itemID = item.ItemID, position = new SerializableVector3(item.transform.position) });
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

        public void DropItemRandomInScene(int itemID)
        {
            //TOBETTER:Drop动画,Player身上生成,随机飞到附近
            Vector2 pos = Random.insideUnitCircle;
            DropItemInScene(itemID, new Vector3(_player.position.x + pos.x, _player.position.y + pos.y, 0)).Forget();
        }

        public async UniTaskVoid DropItemInScene(int itemID, Vector3 toWorldPos)
        {
            var player_pos = _player.position;
            var from_pos = new Vector3(player_pos.x, player_pos.y + 1f, 0);
            Item item = Instantiate(itemPrefab, from_pos, Quaternion.identity, _itemParent);
            item.ItemID = itemID;
            item.coll.enabled = false;
            var ease = from_pos.y > toWorldPos.y ? Ease.Linear : Ease.InBack;//朝上扔的时候
            var uniTaskX =  item.transform.DOMoveX(toWorldPos.x, 0.4f).SetEase(Ease.Linear).ToUniTask();
            var uniTaskY =  item.transform.DOMoveY(toWorldPos.y, 0.4f).SetEase(ease).ToUniTask();
            await UniTask.WhenAll(uniTaskX, uniTaskY);
            item.coll.enabled = true;
        }

        public void DropItemRandomInScene(int itemID, Vector3 fromWorldPos)
        {
            Vector2 pos = Random.insideUnitCircle;
            DropItemInScene(itemID, fromWorldPos, new Vector3(fromWorldPos.x + pos.x, fromWorldPos.y + pos.y, 0)).Forget();
        }

        private async UniTaskVoid DropItemInScene(int itemID, Vector3 fromWorldPos, Vector3 toWorldPos)
        {
            Item item = Instantiate(itemPrefab, fromWorldPos, Quaternion.identity, _itemParent);
            item.ItemID = itemID;
            item.coll.enabled = false;
            //TOBETTER:Drop动画,fromPos生成,飞到toPos
            var ease = fromWorldPos.y > toWorldPos.y ? Ease.Linear : Ease.InBack;
            var uniTaskX = item.transform.DOMoveX(toWorldPos.x, 0.4f).SetEase(Ease.Linear).ToUniTask();
            var uniTaskY = item.transform.DOMoveY(toWorldPos.y, 0.4f).SetEase(ease).ToUniTask();
            await UniTask.WhenAll(uniTaskX, uniTaskY);
            item.coll.enabled = true;
        }
    }
}