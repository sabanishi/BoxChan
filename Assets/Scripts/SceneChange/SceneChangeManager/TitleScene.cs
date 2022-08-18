using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : SceneChangeAbstract
{
    private bool isAction;

    public override void Initialize(int num)
    {
        isAction = true;
    }

    public override void DiscardDeal()
    {
        
    }

    public override void StopActionForFinishDeal()
    {
        isAction = false;
    }

    public void TentatieAction()
    {
        SceneChangeManager.GoSelect(SceneEnum.Title);
    }

    public override void AfterOpenCurtainDeal()
    {

    }
}
