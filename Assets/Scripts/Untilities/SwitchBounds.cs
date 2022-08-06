using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterLoadScene += OnAfterLoadScene;
    }

    private void OnDisable()
    {
        EventHandler.AfterLoadScene -= OnAfterLoadScene;
    }

    private void OnAfterLoadScene()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        var confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        var confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShape;

        //如果边界形状的点在运行时发生变化，则调用它
        confiner.InvalidatePathCache();
    }
}
