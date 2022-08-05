using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

public class AnimatorOverride : MonoBehaviour
{
    [Header("各身体部位动画列表")]
    public List<AnimatorType> animatorTypes;

    private readonly Dictionary<string, Animator> _animatorsDict = new();

    private void Awake()
    {
        var animators = GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            _animatorsDict.Add(animator.name, animator);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelect += OnItemSelect;
        EventHandler.BeforeUnloadScene += OnBeforeUnloadScene;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelect -= OnItemSelect;
        EventHandler.BeforeUnloadScene -= OnBeforeUnloadScene;
    }

    private void OnBeforeUnloadScene()
    {
        SwitchAnimator(BodyState.None);
    }
    
    private void OnItemSelect(ItemDetails details,int slotIndex)
    {
        BodyState cur_state = details == null
            ? BodyState.None
            : details.itemType switch
            {
                //TOADD
                ItemType.Seed => BodyState.Hold,
                ItemType.Commodity => BodyState.Hold,
                ItemType.HoeTool => BodyState.Hoe,
                ItemType.WaterTool=> BodyState.Water,
                _ => BodyState.None
            };
        SwitchAnimator(cur_state);
    }

    private void SwitchAnimator(BodyState state)
    {
        foreach (var animator_type in animatorTypes.Where(e => e.BodyState == state))
        {
            _animatorsDict[animator_type.bodyPart.ToString()].runtimeAnimatorController = animator_type.overrideController;
        }
    }
}