using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MFarm.Transition
{ 

public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;
        
        private CanvasGroup _fadeCanvasGroup;
        //TOBETTER:为什么跟cursorcanvas在一起
        private bool _isFading;

        private void OnEnable()
        {
            EventHandler.Transition += OnTransition;
        }

        private void OnDisable()
        {
            EventHandler.Transition -= OnTransition;
        }

        private void OnTransition(string targetScene, Vector3 targetPos)
        {
            if(!_isFading)
                Transition(targetScene, targetPos).Forget();
        }
        
        private async void Start()
        {
            await LoadSceneAndSetActive(startSceneName);
            _fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
            EventHandler.CallAfterLoadScene();
        }

        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景名字</param>
        /// <param name="targetPos">目标位置</param>
        /// <returns></returns>
        private async UniTask Transition(string sceneName, Vector3 targetPos)
        {
            EventHandler.CallBeforeUnloadScene();
            await Fade(1);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()).ToUniTask().Forget();
            await LoadSceneAndSetActive(sceneName);
            EventHandler.CallMoveToPostition(targetPos);
            await Fade(0);
            EventHandler.CallAfterLoadScene();
        }

        /// <summary>
        /// 加载场景并且设置成当前活动场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private async UniTask LoadSceneAndSetActive(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            SceneManager.SetActiveScene(newScene);
        }

        //可以用dotween做,这里是异步做法
        private async UniTask Fade(float targetAlpha)
        {
            _isFading = true;
            _fadeCanvasGroup.blocksRaycasts = true;
            float speed = Mathf.Abs(_fadeCanvasGroup.alpha - targetAlpha) / Settings.LoadingFadeDuration;
            
            while (!Mathf.Approximately(_fadeCanvasGroup.alpha, targetAlpha))
            {
                _fadeCanvasGroup.alpha = Mathf.MoveTowards(_fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                //必须进入下一帧才能得到正确的deltaTime
                await UniTask.NextFrame();
            }
            _fadeCanvasGroup.blocksRaycasts = false;
            _isFading = false;
        }
    }
}


