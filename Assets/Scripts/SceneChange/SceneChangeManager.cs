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
        GoTitle(SceneEnum.None);
    }

    public static void GoTitle(SceneEnum from)
    {
        if (instance.isChangeing) return;

        if (from == SceneEnum.Select)
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.titleScene,""));
        }
        else
        {
            instance.StartCoroutine(instance.GoStageCoroutine(null, instance.titleScene,""));
        }
    }

    public static void GoSelect(SceneEnum fromScene)
    {
        if (instance.isChangeing) return;

        if (fromScene.Equals(SceneEnum.Title))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.titleScene, instance.seletScene,"Title"));
        }else if (fromScene.Equals(SceneEnum.Game))
        {
            instance.StartCoroutine(instance.GoStageCoroutine(instance.gameScene, instance.seletScene,"Game"));
        }
        else
        {
            Debug.Log(fromScene + "が用意されていない");
        }
    }

    public static void GoGame(string stageName)
    {
        if (instance.isChangeing) return;

        instance.StartCoroutine(instance.GoStageCoroutine(instance.seletScene, instance.gameScene,stageName));
    }

    private IEnumerator GoStageCoroutine(SceneChangeAbstract from,SceneChangeAbstract to,string initialize_value)
    {
        isChangeing = true;
        //入力受け付け停止
        if (from != null)
        {
            from.BeforeCloseCurtainDeal();
            //暗転
            yield return StartCoroutine(CloseCurtainCoroutine());
        }

        //場面転換
        if (from != null)
        {
            from.Terminate();
            from.gameObject.SetActive(false);
        }
        to.gameObject.SetActive(true);
        to.Initialize(initialize_value);

        if (from != null)
        {
            //暗転解除
            yield return StartCoroutine(OpenCurtainCoroutine());
        }
        to.AfterOpenCurtainDeal();

        isChangeing = false;
    }


    //荷物の山を上から降らせる
    public static IEnumerator CloseCurtainCoroutine()
    {
        yield return instance._curtainManager.StartCoroutine(instance._curtainManager.AppearLetterCoroutine());
    }

    //荷物の山を下に落とす
    public static IEnumerator OpenCurtainCoroutine()
    {
        yield return instance._curtainManager.StartCoroutine(instance._curtainManager.DisappearLetterCoroutine());
    }
}
