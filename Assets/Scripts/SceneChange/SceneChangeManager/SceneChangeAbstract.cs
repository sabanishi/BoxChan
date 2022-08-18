using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneChangeAbstract : MonoBehaviour
{
    //ゲームが終わり、入力を受け付けさせなくする関数
    public abstract void StopActionForFinishDeal();

    //暗転中にゲームシーンを掃除する関数
    public abstract void DiscardDeal();

    //初期化
    public abstract void Initialize(int num);

    //暗転解除直後の処理
    public abstract void AfterOpenCurtainDeal();
}
