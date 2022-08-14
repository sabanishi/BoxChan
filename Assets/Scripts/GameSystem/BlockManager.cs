using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    [SerializeField] private GameObject _markForPutBox;

    private BlockEnum[,] stageBlockEnums;
    private GameObject[,] stageBlockObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //初期化処理
    public void Initialize(BlockEnum[,] _stageBlockEnums,GameObject[,] _stageBlockObjects)
    {
        stageBlockEnums = _stageBlockEnums;
        stageBlockObjects = _stageBlockObjects;
    }

    //playerPosの位置でハコを置けるかどうか
    public static bool CanPutBox(Vector3 playerPos,bool isRight)
    {
        //まだinstanceの初期化が済んでいない時、falseを返す
        if (instance == null)
        {
            return false;
        }

        //該当箇所の特定
        int x = (int)Math.Round(playerPos.x,MidpointRounding.AwayFromZero);
        int y = (int)Math.Round(playerPos.y,MidpointRounding.AwayFromZero);
        if (isRight) { x++; }
        else { x--; }

        //画面外の時、falseを返す
        if (x < 0 || x >= instance.stageBlockEnums.GetLength(0)||
            y < 0 || y >= instance.stageBlockEnums.GetLength(1))
        {
            //目印の三角形を非表示にする
            instance._markForPutBox.SetActive(false);
            return false;
        }

        //該当箇所に何かが置かれている時、falseを返す
        if (instance.stageBlockEnums[x, y] != BlockEnum.None)
        {
            //目印の三角形を非表示にする
            instance._markForPutBox.SetActive(false);
            return false;
        }

        //該当箇所の真下に何も置かれていない時、falseを返す
        if (y<=0 || instance.stageBlockEnums[x, y-1] == BlockEnum.None)
        {
            //目印の三角形を非表示にする
            instance._markForPutBox.SetActive(false);
            return false;
        }

        //目印の三角形を表示する
        instance._markForPutBox.SetActive(true);
        instance._markForPutBox.transform.localPosition=new Vector3(x, y-0.225f, 0);

        return true;
    }

    //playerの現在位置にハコを置く
    public static void PutBox(Player player,bool isRight)
    {
        Vector3 playerPos = player.transform.position;
        //該当箇所の特定
        int x = (int)Math.Round(playerPos.x, MidpointRounding.AwayFromZero);
        int y = (int)Math.Round(playerPos.y, MidpointRounding.AwayFromZero);
        if (isRight){ x++;}
        else{ x--;}

        //ハコの作成
        Box box = StageGenerator.instance.CreateBlock(player.HangBlockEnum, x, y).GetComponent<Box>();

        if (box != null)
        {
            //配列の設定
            instance.stageBlockEnums[x, y] = player.HangBlockEnum;
            instance.stageBlockObjects[x, y] = box.gameObject;

            //目印の三角形を非表示にする
            instance._markForPutBox.SetActive(false);

            //プレイヤーの状態の設定
            player.IsHang = false;
            player.HangBlockEnum = BlockEnum.None;
            player.SetHangBoxSprite(null);
            player.SetVelocity(new Vector2(0, player.GetVelocity().y));
        }
        else
        {
            Debug.Log("BlockManagerでboxが作れていない:" + player.HangBlockEnum);
        }
    }


    //playerの現在位置でハコを掴む
    public static void GrabBox(Player player,Box box)
    {
        //ハコの位置の特定
        int x = (int)Math.Round(box.gameObject.transform.position.x, MidpointRounding.AwayFromZero);
        int y = (int)Math.Round(box.gameObject.transform.position.y, MidpointRounding.AwayFromZero);

        //プレイヤーの状態の変更
        player.IsHang = true;
        player.HangBlockEnum = instance.stageBlockEnums[x, y];
        player.SetHangBoxSprite(instance.stageBlockObjects[x, y].GetComponent<SpriteRenderer>().sprite);
        player.SetVelocity(new Vector2(0,player.GetVelocity().y));
        
        //該当箇所の削除
        instance.stageBlockEnums[x, y] = BlockEnum.None;
        Destroy(instance.stageBlockObjects[x, y]);
        instance.stageBlockObjects[x, y] = null;

        //上のブロックを落下させる
        bool isFinishDeal = true;//落下後の処理をするかどうか
        y++;
        while(instance.stageBlockEnums[x, y] != BlockEnum.None&&instance.stageBlockEnums[x,y]!=BlockEnum.NotBox)
        {
            instance.stageBlockEnums[x, y - 1] = instance.stageBlockEnums[x, y];
            instance.stageBlockEnums[x, y] = BlockEnum.None;
            instance.stageBlockObjects[x, y - 1] = instance.stageBlockObjects[x, y];
            instance.stageBlockObjects[x, y] = null;

            Hashtable moveHash = new Hashtable();
            moveHash.Add("position", new Vector3(x, y-1, 0));
            moveHash.Add("isLocal", true);
            moveHash.Add("easetype", "easeInSine");
            moveHash.Add("time", 0.2f);
            if (isFinishDeal)
            {
                player.CanOperate = false;
                moveHash.Add("oncomplete", "FinishFallAnimation");
                moveHash.Add("oncompletetarget", instance.gameObject);
                isFinishDeal = false;
            }
            iTween.MoveTo(instance.stageBlockObjects[x,y-1],moveHash);
            y++;
        }
    }

    //落下アニメーションが終わり、再度操作可能にする
    public void FinishFallAnimation()
    {
        GameManager.instance._player.CanOperate = true;
    }
}
