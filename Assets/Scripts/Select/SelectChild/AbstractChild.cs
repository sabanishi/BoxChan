using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class AbstractChild: MonoBehaviour
{
    public SelectManager parent;
    public abstract void ClickDeal(string nodeName);

    public abstract void Reset();

    //targetをtime秒でposまで移動させる
    protected void SetMoveObj(SelectNode target, Vector3 pos,float delay,bool isValid)
    {
        if (isValid)
        {
            target.transform.DOLocalMove(pos, 0.5f).SetDelay(delay).OnComplete(target.SetIsValidTrue).SetLink(target.gameObject);
        }
        else
        {
            target.transform.DOLocalMove(pos, 0.5f).SetDelay(delay).SetLink(target.gameObject);
            target.SetIsvalidFalse();
        }

    }
}
