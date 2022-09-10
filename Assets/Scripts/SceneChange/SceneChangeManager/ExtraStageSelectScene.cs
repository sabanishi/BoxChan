using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraStageSelectScene : SceneChangeAbstract
{
    [SerializeField] private ExtraStageSelectManager manager;

    private bool isLoadFinish;
    public bool IsLoadFinish
    {
        get { return isLoadFinish; }
        set { isLoadFinish = value; }
    }

    public override void AfterOpenCurtainDeal()
    {
        manager.AfterOpenCurtainDeal();
    }

    public override void BeforeCloseCurtainDeal()
    {
        
    }

    public override void Initialize(string initialize_value)
    {
        isLoadFinish = false;
        manager.Initialize(initialize_value);
    }

    public override void Terminate()
    {
        
    }
}
