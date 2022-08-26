using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneChangeAbstract : MonoBehaviour
{
    //暗転中にシーンの初期化をする
    public abstract void Initialize(string initialize_value);

    //暗転開始直前の処理
    public abstract void BeforeCloseCurtainDeal();

    //暗転解除直後の処理
    public abstract void AfterOpenCurtainDeal();

    //暗転中にシーンを終わらせる
    public abstract void Terminate();
}
