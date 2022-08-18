using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager instance { get; private set; }
    [SerializeField] private CurtainManager _curtainManager;
    [SerializeField] private TitleScene titleScene;
    [SerializeField] private SelectScene seletScene;
    [SerializeField] private GameScene gameScene;

    private bool isChangeing;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //TODO:タイトル画面の呼び出し
        GoTitle(SceneEnum.None);
    }

    public static void GoTitle(SceneEnum from)
    {
        if (instance.isChangeing) return;

        if (from == SceneEnum.Select)
        {
            instance.StartCoroutine(instance.GoStageEnumerator(instance.seletScene, instance.titleScene, 0));
        }
        else
        {
            instance.StartCoroutine(instance.GoStageEnumerator(null, instance.titleScene, 0));
        }
    }

    public static void GoSelect(SceneEnum fromScene)
    {
        if (instance.isChangeing) return;

        if (fromScene.Equals(SceneEnum.Title))
        {
            instance.StartCoroutine(instance.GoStageEnumerator(instance.titleScene, instance.seletScene, 0));
        }else if (fromScene.Equals(SceneEnum.Game))
        {
            instance.StartCoroutine(instance.GoStageEnumerator(instance.gameScene, instance.seletScene, 0));
        }
        else
        {
            Debug.Log(fromScene + "が用意されていない");
        }
    }

    public static void GoGame(int stageNum)
    {
        if (instance.isChangeing) return;

        instance.StartCoroutine(instance.GoStageEnumerator(instance.seletScene, instance.gameScene,stageNum));
    }

    private IEnumerator GoStageEnumerator(SceneChangeAbstract from,SceneChangeAbstract to,int num)
    {
        isChangeing = true;
        //入力受け付け停止
        if (from != null)
        {
            from.StopActionForFinishDeal();
            //暗転
            yield return StartCoroutine(CloseCurtain());
        }

        //場面転換
        if (from != null)
        {
            from.DiscardDeal();
            from.gameObject.SetActive(false);
        }
        to.gameObject.SetActive(true);
        to.Initialize(num);

        if (from != null)
        {
            //暗転解除
            yield return StartCoroutine(OpenCurtain());
        }
        to.AfterOpenCurtainDeal();

        isChangeing = false;
    }


    //荷物の山を上から降らせる
    public static IEnumerator CloseCurtain()
    {
        yield return instance._curtainManager.StartCoroutine(instance._curtainManager.AppearLetter());
    }

    //荷物の山を下に落とす
    public static IEnumerator OpenCurtain()
    {
        yield return instance._curtainManager.StartCoroutine(instance._curtainManager.DisappearLetter());
    }
}
