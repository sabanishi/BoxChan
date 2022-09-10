using UnityEngine;
using System.Collections;

public class CreateStageScene : SceneChangeAbstract
{
    [SerializeField] private CreateStageManager manager;
    public override void Initialize(string initialize_value)
    {
        manager.Initialize(initialize_value);
    }

    public override void AfterOpenCurtainDeal()
    {
        
    }

    public override void BeforeCloseCurtainDeal()
    {

    }

    public override void Terminate()
    {
        manager.Terminate();
    }
}
