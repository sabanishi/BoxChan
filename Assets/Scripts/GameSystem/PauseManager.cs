using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject BlackBack;
    [SerializeField] private Transform MainCameraTF;
    [SerializeField] private GameObject PauseObj;
    [SerializeField] private GameObject GoOnObj;
    [SerializeField] private GameObject RestartObj;
    [SerializeField] private GameObject GoBackObj;
    [SerializeField] private GameObject ExplainObj;
    [SerializeField] private Text GoOnText;
    [SerializeField] private Text RestartText;
    [SerializeField] private Text GoBackText;

    private bool isPausePrepare;//ポーズ準備・片付けも含めて管理するフラグ
    private int pauseStateNum;//ポーズの状態を管理する変数

    //ポーズ画面の初期化
    public void PauseInitialize()
    {
        PauseObj.transform.localPosition = new Vector3(-1200,330,0);
        GoOnObj.transform.localPosition = new Vector3(1200,0,0);
        RestartObj.transform.localPosition = new Vector3(1200,-150,0);
        GoBackObj.transform.localPosition = new Vector3(1200,-300,0);
        ExplainObj.transform.localPosition = new Vector3(-1200,0,0);
        BlackBack.SetActive(false);
    }

    //ポーズ画面が始める際の一連の流れ
    public IEnumerator PauseStartCoroutine()
    {
        //ポーズ準備スタート
        isPausePrepare = true;

        BlackBack.SetActive(true);
        pauseStateNum = 1;
        DecideTextColor();

        //テキストの色を初期化
        TextColorInitialize();

        //「ポーズ画面」の文字を表示
        Hashtable PausemoveHash = new Hashtable();
        PausemoveHash.Add("position", new Vector3(-500, 330, 0));
        PausemoveHash.Add("time", 0.15f);
        PausemoveHash.Add("delay", 0f);
        PausemoveHash.Add("easeType", "easeOutQuart");
        PausemoveHash.Add("isLocal", true);
        iTween.MoveTo(PauseObj, PausemoveHash);

        yield return new WaitForSeconds(0.15f);

        CreateTextHashTable(0, GoOnObj);
        yield return new WaitForSeconds(0.1f);
        CreateTextHashTable(1, RestartObj);
        yield return new WaitForSeconds(0.1f);
        CreateTextHashTable(2, GoBackObj);
        CreateTextHashTable(3, ExplainObj);
        yield return new WaitForSeconds(0.1f);
        //ポーズ準備終わり
        isPausePrepare = false;
    }

    //ポーズ画面の制御
    public void PauseUpdate()
    {
        if (!isPausePrepare)
        {
            //準備・片付け中出ない時、入力を受け付ける
            DecideTextColor();
            if (Input.GetButtonDown("Pause"))
            {
                GoOnDeal();
            }
            else
            if (Input.GetButtonDown("Hang"))
            {
                switch (pauseStateNum)
                {
                    case 1:
                        //続ける
                        GoOnDeal();
                        break;
                    case 2:
                        //最初から
                        RestartDeal();
                        break;
                    case 3:
                        //セレクト画面へ
                        GoBackDeal();
                        break;
                    default:
                        Debug.Log(pauseStateNum + "がない");
                        break;
                }
            }
            else
            if (Input.GetButtonDown("Up"))
            {
                //一つ上に
                pauseStateNum--;
                if (pauseStateNum < 1)
                {
                    pauseStateNum = 3;
                }
            }else
            if (Input.GetButtonDown("Down"))
            {
                //一つ下に
                pauseStateNum++;
                if (pauseStateNum > 3)
                {
                    pauseStateNum = 1;
                }
            }

        }
    }

    //続ける
    private void GoOnDeal()
    {
        StartCoroutine(GoOnDealCoroutine());
    }

    //「続ける」処理の一連の流れ
    private IEnumerator GoOnDealCoroutine()
    {
        isPausePrepare = true;
        CreateBackHashTable(1, PauseObj);
        CreateBackHashTable(2, GoOnObj);
        CreateBackHashTable(3, RestartObj);
        CreateBackHashTable(4, GoBackObj);
        CreateBackHashTable(5, ExplainObj);
        yield return new WaitForSeconds(0.15f);

        GameManager.instance.IsPause = false;
        BlackBack.SetActive(false);
    }

    //最初から
    private void RestartDeal()
    {
        isPausePrepare = true;
        GameManager.instance.StartCoroutine(GameManager.instance.RestartCoroutine());
    }

    //セレクト画面へ
    private void GoBackDeal()
    {
        isPausePrepare = true;
        SceneChangeManager.GoSelect(SceneEnum.Game);
    }

    //テキストの色を決める
    private void DecideTextColor()
    {
        TextColorInitialize();
        
        switch (pauseStateNum)
        {
            case 1:
                GoOnText.color = new Color(((float)264)/255 ,((float)150)/255 ,((float)6)/255);
                GoOnObj.transform.localScale = new Vector3(1.3f, 1.3f, 1);
                break;
            case 2:
                RestartText.color = new Color(((float)264) / 255, ((float)150) / 255, ((float)6) / 255);
                RestartObj.transform.localScale = new Vector3(1.3f, 1.3f, 1);
                break;
            case 3:
                GoBackText.color = new Color(((float)264) / 255, ((float)150) / 255, ((float)6) / 255);
                GoBackObj.transform.localScale = new Vector3(1.3f, 1.3f, 1);
                break;
            default:
                Debug.Log(pauseStateNum + "が用意されていない");
                break;
        }
    }

    //テキストの色の初期化
    private void TextColorInitialize()
    {
        GoOnText.color = new Color(1, 1, 1);
        RestartText.color = new Color(1, 1, 1);
        GoBackText.color = new Color(1, 1, 1);

        GoOnObj.transform.localScale = new Vector3(1, 1, 1);
        RestartObj.transform.localScale = new Vector3(1, 1, 1);
        GoBackObj.transform.localScale = new Vector3(1, 1, 1);
    }

    //文字を出す
    private void CreateTextHashTable(int i, GameObject target)
    {
        Hashtable PausemoveHash = new Hashtable();
        if (i != 3)
        {
            PausemoveHash.Add("position", MainCameraTF.position + new Vector3(500, -i * 150, 0));
        }
        else
        {
            PausemoveHash.Add("position", MainCameraTF.position + new Vector3(-400,0, 0));
        }

        PausemoveHash.Add("isLocal", true);
        PausemoveHash.Add("time", 0.15f);
        PausemoveHash.Add("delay", 0f);
        PausemoveHash.Add("easeType", "easeOutQuart");
        iTween.MoveTo(target, PausemoveHash);
    }

    //文字を戻す
    private void CreateBackHashTable(int i, GameObject Target)
    {
        Hashtable PausemoveHash = new Hashtable();
        float x = 1200;
        if (i == 1) { x = -1200; }
        if (i == 5) { x = -1200; }
        float y = 0;
        if (i == 1) { y = 180; }
        if (i == 5) { y = 450; }
        PausemoveHash.Add("position", MainCameraTF.position + new Vector3(x, y - (i - 2) * 150, 0));
        PausemoveHash.Add("time", 0.15f);
        PausemoveHash.Add("delay", 0f);
        PausemoveHash.Add("easeType", "easeOutQuart");
        PausemoveHash.Add("isLocal", true);
        iTween.MoveTo(Target, PausemoveHash);
    }
}
