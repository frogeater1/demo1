using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //逐渐半透明
    public void FadeOut()
    {
        Color targetColor = new(1, 1, 1, Settings.FadeTarget);
        _spriteRenderer.DOColor(targetColor, Settings.ItemFadeDuration);
    }
    
    //逐渐恢复不透明
    public void FadeIn()
    {
        Color targetColor = new(1, 1, 1, 1);
        _spriteRenderer.DOColor(targetColor, Settings.ItemFadeDuration);
    }
}
