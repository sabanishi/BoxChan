using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScene : SceneChangeAbstract
{
    [SerializeField] private SelectManager selectManager;


    public override void DiscardDeal()
    {
        
    }

    public override void StopActionForFinishDeal()
    {
        selectManager.IsValid = false;
    }

    public override void Initialize(int num)
    {
        selectManager.Initialize();
    }

    public override void AfterOpenCurtainDeal()
    {
        selectManager.AfterOpenCurtainDeal();
    }
}
