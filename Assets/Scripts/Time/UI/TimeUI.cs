using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimeUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform _dayNightTransform;

    [SerializeField]
    private RectTransform _clockParent;

    [SerializeField]
    private TextMeshProUGUI _dateText;

    [SerializeField]
    private TextMeshProUGUI _clockText;

    [SerializeField]
    private Image _seasonImage;
    
    [SerializeField]
    private Sprite[] _seasonSprites;

    private Image[] _clockBlocks;
    private int _curHour;
    private int _curMinute;

    private void Awake()
    {
        _clockBlocks = _clockParent.GetComponentsInChildren<Image>();
    }
    private void OnEnable()
    {
        EventHandler.GameMinuteUpdate += OnGameMinuteUpdate;
        EventHandler.GameHourUpdate += OnGameHourUpdate;
        EventHandler.GameDateUpdate += OnGameDateUpdate;
    }


    private void OnDisable()
    {
        EventHandler.GameMinuteUpdate -= OnGameMinuteUpdate;
        EventHandler.GameHourUpdate -= OnGameHourUpdate;
        EventHandler.GameDateUpdate -= OnGameDateUpdate;
    }

    private void OnGameMinuteUpdate(int minute)
    {
        _curMinute = minute;
        _clockText.text = HelperFunc.GetClockText(_curHour,_curMinute);
    }
    private void OnGameHourUpdate(int hour)
    {
        _curHour = hour;
        //分钟更新就会触发text刷新了这里不需要刷新text
        UpdateClockBlock(hour);
        DayNightImageRotate(hour);
    }
    private void OnGameDateUpdate(int year, int month, int day, Season season)
    {
        _dateText.text = HelperFunc.GetDateText(year, month, day);
        _seasonImage.sprite = _seasonSprites[(int)season];
    }


    private void UpdateClockBlock(int hour)
    {
        int index = hour / 4;
        if (index == 0)
        {
            foreach (var img in _clockBlocks)
            {
                img.enabled = false;
            }
        }
        _clockBlocks[index].enabled = true;
    }

    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, 15 * hour - 90);
        _dayNightTransform.DORotate(target, 1f);
    }
}