using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : SceneChangeAbstract
{
    [SerializeField] private GameManager gameManager;
    public override void StopActionForFinishDeal()
    {

    }

    public override void DiscardDeal()
    {

    }

    public override void Initialize(int num)
    {
        gameManager.Initialize(num);
    }

    public override void AfterOpenCurtainDeal()
    {
        gameManager.IsTimeStop = false;
    }
}
