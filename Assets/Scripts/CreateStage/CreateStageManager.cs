using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateStageManager : MonoBehaviour
{
    [SerializeField] private TileManager tileManager;//タイルを管理する機能
    [SerializeField] private ShowSelectBlockPanel showSelectBlockPanel;//現在選択中の筆を表示するためのパネル
    [SerializeField] private GameObject menuPanel;//メニュー画面
    [SerializeField] private Pallet pallet;//筆を選択するための機能
    [SerializeField] private PlayAlartManager playAlartManager;//パズルが不完全な時に警告を出すための機能
    [SerializeField] private GameObject alartPanel;//パズルが不完全な時に警告を出すパネル
    [SerializeField] private GameObject reallyGoBackPanel;//ステージセレクト画面へ移動するか確認するためのパネル
    [SerializeField] private GameObject uploadPanel;//アップロードする前の名前決めなどを行うパネル
    [SerializeField] private GameObject uploadFinishPanel;//アップロード終了後に制作を続けるかどうか尋ねるパズル

    [SerializeField] private ButtonNode playButton;//パズルを試すためのボタン
    [SerializeField] private ButtonNode uploadButton;//アップロードを行うためのボタン
    [SerializeField] private ButtonNode finishButton;//ステージセレクト画面へ戻るためのボタン
    [SerializeField] private ButtonNode playAraltOkButton;//パズルが不完全な時に警告を消すためのボタン
    [SerializeField] private InputField nameInputField;//名前を入力るInputFiled
    [SerializeField] private Text nameAlartText;//名前に不備がある時に出す警告文

    private bool isMenu;//メニュー画面が表示されているかどうか　
    private bool isMenuPrepare;//メニュー画面を切り替えいている途中かどうか
    private bool isShowAlart;//パズルの不備の表示をしているかどうか
    private bool isGoStageSelect;//ステージセレクト画面に戻るかどうか
    private bool canNotMenu;//メニュー画面を操作できないかどうか

    public bool IsShowAlart
    {
        get { return isShowAlart; }
        set
        {
            isShowAlart = value;
            if (isShowAlart)
            {
                //警告文を表示
                alartPanel.SetActive(true);
                playAlartManager.SetText();
            }
            else
            {
                //警告文を非表示にする
                alartPanel.SetActive(false);
                canNotMenu = false;
            }
        }
    }
    
    public void Initialize(string initialize_value)
    {
        isMenuPrepare = false;
        IsShowAlart = false;
        canNotMenu = false;

        playButton.Initialize();
        finishButton.Initialize();
        playAraltOkButton.Initialize();

        reallyGoBackPanel.SetActive(false);
        uploadPanel.SetActive(false);
        uploadFinishPanel.SetActive(false);

        if (initialize_value.Equals("Select"))
        {
            uploadButton.IsValid=false;
            tileManager.Initialize();
            showSelectBlockPanel.OpenMenu();
            pallet.Initialize();
            tileManager.InitializeStage();
        }
        else
        {
            if (initialize_value.Equals("GameClear"))
            {
                uploadButton.Initialize();
            }
        }
        CloseMenu();
    }

    public void Terminate()
    {
        if (isGoStageSelect)
        {
            tileManager.Terminate();
            isGoStageSelect = false;
        }
    }
    
    void Update()
    {
        if (!isMenuPrepare&&!canNotMenu)
        {
            if (Input.GetButtonDown("Pause"))
            {
                isMenuPrepare = true;
                if (isMenu)
                {
                    //メニュー画面を閉じる
                    CloseMenu();
                    SoundManager.PlaySE(SE_Enum.MENU);
                }
                else
                {
                    //メニュー画面へと移行
                    OpenMenu();
                    SoundManager.PlaySE(SE_Enum.MENU);
                }
                StartCoroutine(ChangeMenuState());
            }
        }
    }

    //メニュー画面を開く
    private void OpenMenu()
    {
        isMenu = true;
        showSelectBlockPanel.OpenMenu();
        menuPanel.SetActive(true);
        tileManager.IsValid = false;
    }

    //メニュー画面を閉じる
    private void CloseMenu()
    {
        isMenu = false;
        showSelectBlockPanel.CloseMenu();
        menuPanel.SetActive(false);
        pallet.CloseMenu();
        tileManager.IsValid = true;
    }

    //メニュー画面の開閉のインターバル
    private IEnumerator ChangeMenuState()
    {
        yield return new WaitForSeconds(0.1f);
        isMenuPrepare = false;
    }

    //制作したパズルで遊ぼうとする
    public void Play()
    {
        if (!playAlartManager.CheckStage(tileManager.BlockEnums))
        {
            //もしパズルに不備がないなら、遊ぶ
            SoundManager.PlaySE(SE_Enum.DECIDE2);
            InformationDeliveryUnit.Instance.BlockEnums = tileManager.BlockEnums;
            SceneChangeManager.GoGameFromCreateStage();
        }
        else
        {
            //不備がある時、警告を表示
            IsShowAlart = true;
            canNotMenu = true;
            SoundManager.PlaySE(SE_Enum.CANNOTPLAY);
        }
    }

    //パズルに変更が加えられた時にパズルをアップロードできないようにする
    public void ChangeCannotUpload()
    {
        uploadButton.IsValid=false;
    }

    //ステージセレクト画面に戻るかを尋ねるパネルを表示
    public void ShowReallyGoBack()
    {
        CloseMenu();
        tileManager.IsValid = false;
        reallyGoBackPanel.SetActive(true);
        canNotMenu = true;
    }

    //ステージセレクト画面に戻る
    public void GoBack()
    {
        isGoStageSelect = true;
        SceneChangeManager.GoSelect(SceneEnum.CreateStage);
    }

    //ステージセレクト画面に戻るかを尋ねるパネルを非表示にする
    public void NotGoBack()
    {
        OpenMenu();
        reallyGoBackPanel.SetActive(false);
        canNotMenu = false;
    }

    //制作したパズルをアップロードするためのパネルを表示する
    public void UploadPrepare()
    {
        CloseMenu();
        tileManager.IsValid = false;
        uploadPanel.SetActive(true);
        nameAlartText.enabled = false;
        nameInputField.text = "";
        canNotMenu = true;
    }

    //アップロードを取りやめる
    public void UploadCancel()
    {
        OpenMenu();
        uploadPanel.SetActive(false);
        canNotMenu = false;
    }

    //アップロードを実行する
    public void UploadOK()
    {
        string name = nameInputField.text;
        if(name.Trim().Length == 0)
        {
            //もし名前に不備がある時、アップロードを中止してその旨を表示
            nameAlartText.enabled = true;
            SoundManager.PlaySE(SE_Enum.CANNOTPLAY);
            return;
        }

        SoundManager.PlaySE(SE_Enum.UPLOAD);

        //ステージ公開処理
        //マップの情報をint型に直す
        int[,] mapData = new int[32,18];
        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 18; y++)
            {
                mapData[x, y] = (int)tileManager.BlockEnums[x, y];
            }
        }
        //アップロードする
        MyNCMBManager.PushMapInfo(name, mapData);
        //アップロードパネルを非表示にする
        uploadPanel.SetActive(false);
        uploadFinishPanel.SetActive(true);
        uploadButton.IsValid = false;

        //エクストラステージを読み込む
        MyNCMBManager.FetchList();
    }

    //パズル制作を続ける
    public void ContinueCreatePazzle()
    {
        OpenMenu();
        uploadFinishPanel.SetActive(false);
        canNotMenu = false;
    }
}
