using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int _gameSecond, _gameMinute, _gameHour, _gameDay, _gameMonth, _gameYear;
    private Season _season;

    public bool gameClockPause;

    private float _tikTime;

    private void Awake()
    {
        NewGameTime();
    }

    private void Start()
    {
        EventHandler.CallGameMinuteUpdate(_gameMinute);
        EventHandler.CallGameHourUpdate(_gameHour);
        EventHandler.CallGameDateUpdate(_gameYear, _gameMonth, _gameDay,_season);
    }

    private void Update()
    {
        //暂停直接return;
        if (gameClockPause) return;

        _tikTime += Time.deltaTime;
        if (_tikTime >= Settings.TiksInSecond)
        {
            _tikTime -= Settings.TiksInSecond;
            UpdateGameClock();
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameClock();
            }
        }
    }

    private void NewGameTime()
    {
        _tikTime = 0;
        _gameSecond = 0;
        _gameMinute = 0;
        _gameHour = 0;
        _gameDay = 1;
        _gameMonth = 1;
        _gameYear = 1;
        _season = Season.Spring;
        gameClockPause = false;
    }

    private void UpdateGameClock()
    {
        // print("seconds:" + _gameSecond + "  minute:" + _gameMinute);
        _gameSecond++;
        if (_gameSecond == Settings.SecondsInMinute)
        {
            _gameMinute++;
            _gameSecond = 0;
            if (_gameMinute == Settings.MinutesInHour)
            {
                _gameHour++;
                _gameMinute = 0;
                if (_gameHour == Settings.HoursInDay)
                {
                    _gameDay++;
                    _gameHour = 0;
                    if (_gameDay == Settings.DaysInMonth)
                    {
                        _gameMonth++;
                        _gameDay = 1;
                        //季节直接根据月份赋值
                        _season = (Season)((_gameMonth - 1) / Settings.MonthInSeason);
                        if (_gameMonth == Settings.MonthInYear)
                        {
                            _gameYear++;
                            _gameMonth = 1;
                        }
                    }
                    EventHandler.CallGameDateUpdate(_gameYear, _gameMonth, _gameDay, _season);
                }
                EventHandler.CallGameHourUpdate(_gameHour);
            }
            EventHandler.CallGameMinuteUpdate(_gameMinute);
        }
    }
}