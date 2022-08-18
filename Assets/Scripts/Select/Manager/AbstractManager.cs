using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class AbstractManager : MonoBehaviour
{
    public SelectManager parent;
    public abstract void ClickDeal(string nodeName);

    public abstract void Reset();

    //targetをtime秒でposまで移動させる
    protected void SetMoveObj(SelectNode target, Vector3 pos,float delay,bool isValid)
    {
        if (isValid)
        {
            target.transform.DOLocalMove(pos, 0.5f).SetDelay(delay).OnComplete(target.SetIsValidTrue);
        }
        else
        {
            target.transform.DOLocalMove(pos, 0.5f).SetDelay(delay);
            target.SetIsvalidFalse();
        }

    }
}
