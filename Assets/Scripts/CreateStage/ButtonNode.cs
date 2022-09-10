using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonNode : MonoBehaviour
{
    [SerializeField] private string Name;//どのボタンか識別するための名前
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite image1;//通常時の画像
    [SerializeField] private Sprite image2;//カーソルが合っている時の画像
    [SerializeField] private Sprite image3;//選択できない時の画像
    [SerializeField] private CreateStageManager parent;
    [SerializeField] private float scale;//通常時のサイズ
    [SerializeField] private float bigScale;//カーソルが合っている時のサイズ
    [SerializeField]private bool isValid;//有効かどうか
    public bool IsValid
    {
        get { return isValid; }
        set
        {
            isValid = value;
            if (!isValid)
            {
                //有効出ない時、画像を変更
                sprite.sprite = image3;
            }
        }
    }

    public void Initialize()
    {
        sprite.sprite = image1;
        isValid = true;
    }

    public void OnEnter()
    {
        if (!isValid) return;
        if (!Name.Equals("PlayAlartOkButton") && parent.IsShowAlart) return;

        sprite.sprite = image2;
        transform.localScale = new Vector3(bigScale,bigScale, 1);
    }

    public void OnExit()
    {
        if (!isValid) return;
        if (!Name.Equals("PlayAlartOkButton") && parent.IsShowAlart) return;

        sprite.sprite = image1;
        transform.localScale = new Vector3(scale, scale, 1);
    }

    public void OnClick()
    {
        if (!isValid) return;
        OnExit();
        if (!Name.Equals("PlayAlartOkButton") && parent.IsShowAlart) return;

        if (!Name.Equals("UploadYesButton")&&!Name.Equals("PlayButton"))
        {
            SoundManager.PlaySE(SE_Enum.DECIDE2);
        }

        switch (Name)
        {
            case "FinishButton":
                //ステージセレクト画面に戻るかどうか尋ねるパネルを表示する
                parent.ShowReallyGoBack();
                break;
            case "PlayButton":
                //制作したパズルで遊ぶ
                parent.Play();
                break;
            case "UploadButton":
                //制作したパズルをアップロードするためのパネルを表示する
                parent.UploadPrepare();
                break;
            case "PlayAlartOkButton":
                //パズルに不備があることを示す警告文を非表示にする
                parent.IsShowAlart = false;
                break;
            case "ReallyGoBackYesButton":
                //ステージセレクト画面に戻る
                parent.GoBack();
                break;
            case "ReallyGoBackNoButton":
                //ステージセレクト画面に戻るかどうか尋ねるパネルを非表示にする
                parent.NotGoBack();
                break;
            case "UploadYesButton":
                //パズルをアップロードする
                parent.UploadOK();
                break;
            case "UploadNoButton":
                //パズルのアップロードを取りやめる
                parent.UploadCancel();
                break;
            case "UploadFinishYesButton":
                //パズル制作を続ける
                parent.ContinueCreatePazzle();
                break;
            case "UploadFinishNoButton":
                //ステージセレクト画面に戻る
                parent.GoBack();
                break;
            default:
                Debug.Log(Name + "がない");
                break;
        }
    }
}
