using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  MFarm.Transition
{
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string targetScene;
        public Vector3 targetPos;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventHandler.CallTransition(targetScene, targetPos);
            }
        }
    }
}
