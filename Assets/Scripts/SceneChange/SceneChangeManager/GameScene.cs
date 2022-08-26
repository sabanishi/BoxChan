using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : SceneChangeAbstract
{
    [SerializeField] private GameManager gameManager;

    public override void Initialize(string initialize_value)
    {
        gameManager.Initialize(initialize_value);
    }

    public override void AfterOpenCurtainDeal()
    {
        gameManager.CanAcceptInput = true;
    }

    public override void BeforeCloseCurtainDeal()
    {

    }

    public override void Terminate()
    {
        gameManager.Terminate();
    }
}
