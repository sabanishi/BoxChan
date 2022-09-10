using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : SceneChangeAbstract
{
    [SerializeField] private GameManager gameManager;

    private string _initialize_value;

    public override void Initialize(string initialize_value)
    {
        _initialize_value = initialize_value;
        gameManager.Initialize(initialize_value);
    }

    public override void AfterOpenCurtainDeal()
    {
        if (!_initialize_value.Equals("CreateStage"))
        {
            SoundManager.PlayBGM(BGM_Enum.GAME);
        }
        gameManager.CanAcceptInput = true;
        SoundManager.PlaySE(SE_Enum.HUE);
    }

    public override void BeforeCloseCurtainDeal()
    {

    }

    public override void Terminate()
    {
        gameManager.Terminate();
    }
}
