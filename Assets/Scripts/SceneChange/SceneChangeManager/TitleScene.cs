using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleScene : SceneChangeAbstract
{
    [SerializeField] private GameObject ClickStartText;
    private bool isAction;
    private Sequence sequence;

    private void Update()
    {


        if (isAction && Input.GetMouseButtonDown(0))
        {
            SceneChangeManager.GoSelect(SceneEnum.Title);
        }
    }

    public override void Initialize(string initialize_value)
    {
        ClickStartText.transform.localPosition = new Vector3(3.6f, -3.1f, 0);
        isAction = true;
        sequence = DOTween.Sequence();
        sequence.Append(ClickStartText.transform.DOLocalMove(new Vector3(3.6f, -3), 0.1f).SetDelay(0.35f)).SetLoops(-1,LoopType.Yoyo);
        sequence.Play();
    }

    public override void BeforeCloseCurtainDeal()
    {
        isAction = false;
    }

    public override void AfterOpenCurtainDeal()
    {

    }

    public override void Terminate()
    {
        sequence.Kill();
    }
}
