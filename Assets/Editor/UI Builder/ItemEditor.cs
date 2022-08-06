using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class ItemEditor : EditorWindow
{
    private ItemData_SO _dataBase;
    private List<ItemDetails> _itemDataList = new();
    private VisualTreeAsset _itemRowTemplate;
    private ScrollView _itemDetailsSection;
    private ItemDetails _activeItem;

    //默认预览图片
    private Sprite _defaultIcon;

    private VisualElement _iconPreview;
    //获得VisualElement
    private ListView _itemListView;

    [MenuItem("M STUDIO/ItemEditor")]
    public static void ShowExample()
    {
        var wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        //// VisualElements objects can contain other VisualElement following a tree hierarchy.
        // VisualElement label = new Label("Hello World! From C#");
        // root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //拿到模版数据
        _itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        //拿默认Icon图片
        _defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //变量赋值
        _itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        _itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        _iconPreview = _itemDetailsSection.Q<VisualElement>("Icon");


        //获得按键
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;
        //加载数据
        LoadDataBase();

        //生成ListView
        GenerateListView();
    }

    #region 按键事件
    private void OnDeleteClicked()
    {
        _itemDataList.Remove(_activeItem);
        _itemListView.Rebuild();
        _itemDetailsSection.visible = false;
    }

    private void OnAddItemClicked()
    {
        ItemDetails newItem = new()
        {
            itemName = "NEW ITEM",
            itemID = 1001 + _itemDataList.Count
        };
        _itemDataList.Add(newItem);
        _itemListView.Rebuild();
    }
    #endregion

    private void LoadDataBase()
    {
        // string[] dataArray = AssetDatabase.FindAssets("ItemDataList_SO");
        // if (dataArray.Length >= 1)
        // {
        //     string path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
        //     _dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
        // }
        
        _dataBase = AssetDatabase.LoadAssetAtPath<ItemData_SO>("Assets/GameData/Inventory/ItemData_SO.asset");
        Debug.Assert(_dataBase != null, nameof(_dataBase) + " == null");
        _itemDataList = _dataBase.itemDetailsList;
        //如果不标记则无法保存数据
        EditorUtility.SetDirty(_dataBase);

        //Debug.Log(_itemDataList[0].itemID);
    }

    private void GenerateListView()
    {
        _itemListView.fixedItemHeight = 50;  //根据需要高度调整数值
        _itemListView.itemsSource = _itemDataList;
        _itemListView.makeItem = () => _itemRowTemplate.CloneTree();
        _itemListView.bindItem = (e, i) =>
        {
            if (i < _itemDataList.Count)
            {
                e.Q<VisualElement>("Icon").style.backgroundImage = _itemDataList[i].itemIcon == null ? _defaultIcon.texture : _itemDataList[i].itemIcon.texture;
                e.Q<Label>("Name").text = _itemDataList[i] == null ? "NO ITEM" : _itemDataList[i].itemName;
            }
        };

        _itemListView.onSelectionChange += OnListSelectionChange;

        //右侧信息面板不可见
        _itemDetailsSection.visible = false;
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        _activeItem = selectedItem.First() as ItemDetails;
        GetItemDetails();

        _itemDetailsSection.visible = true;
    }

    private void GetItemDetails()
    {
        _itemDetailsSection.MarkDirtyRepaint();

        _itemDetailsSection.Q<IntegerField>("ItemID").value = _activeItem.itemID;
        _itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemID = evt.newValue;
        });

        _itemDetailsSection.Q<TextField>("ItemName").value = _activeItem.itemName;
        _itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemName = evt.newValue;
            _itemListView.Rebuild();
        });

        _iconPreview.style.backgroundImage = _activeItem.itemIcon == null ? _defaultIcon.texture : _activeItem.itemIcon.texture;
        _itemDetailsSection.Q<ObjectField>("ItemIcon").value = _activeItem.itemIcon;
        _itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            _activeItem.itemIcon = newIcon;

            _iconPreview.style.backgroundImage = newIcon == null ? _defaultIcon.texture : newIcon.texture;
            _itemListView.Rebuild();
        });

        //其他所有变量的绑定
        _itemDetailsSection.Q<ObjectField>("ItemSprite").value = _activeItem.itemOnWorldSprite;
        _itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemOnWorldSprite = (Sprite)evt.newValue;
        });

        _itemDetailsSection.Q<EnumField>("ItemType").Init(_activeItem.itemType);
        _itemDetailsSection.Q<EnumField>("ItemType").value = _activeItem.itemType;
        _itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemType = (ItemType)evt.newValue;
        });

        _itemDetailsSection.Q<TextField>("Description").value = _activeItem.itemDescription;
        _itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemDescription = evt.newValue;
        });

        _itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = _activeItem.itemUseRadius;
        _itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemUseRadius = evt.newValue;
        });

        _itemDetailsSection.Q<Toggle>("CanPickedup").value = _activeItem.canPickedUp;
        _itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            _activeItem.canPickedUp = evt.newValue;
        });

        _itemDetailsSection.Q<Toggle>("CanDropped").value = _activeItem.canDropped;
        _itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            _activeItem.canDropped = evt.newValue;
        });

        _itemDetailsSection.Q<Toggle>("CanCarried").value = _activeItem.canHolded;
        _itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            _activeItem.canHolded = evt.newValue;
        });

        _itemDetailsSection.Q<IntegerField>("Price").value = _activeItem.itemPrice;
        _itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            _activeItem.itemPrice = evt.newValue;
        });

        _itemDetailsSection.Q<Slider>("SellPercentage").value = _activeItem.sellPercentage;
        _itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            _activeItem.sellPercentage = evt.newValue;
        });
    }
}