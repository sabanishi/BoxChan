using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectScene : SceneChangeAbstract
{
    [SerializeField] private SelectManager selectManager;

    public override void Initialize(string initialize_value)
    {
        selectManager.Initialize(initialize_value);
    }

    public override void BeforeCloseCurtainDeal()
    {
        selectManager.IsValid = false;
    }

    public override void AfterOpenCurtainDeal()
    {
        selectManager.AfterOpenCurtainDeal();
    }

    public override void Terminate()
    {
        selectManager.FinishDeal();
    }
}
