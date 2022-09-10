using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    [SerializeField] private GameObject _markForPutBox;//ハコを置ける位置を示す三角形

    private BlockEnum[,] stageBlockEnums;//ステージ全体をBlockEnumで表した二次元配列
    private GameObject[,] stageBlockObjects;//ステージ全体をGameObjectで表した二次元配列
    private WarpBox[,] warpBoxs = new WarpBox[4, 2];//ワープバコを格納する配列

    public BlockEnum[,] GetStageBlockEnums()
    {
        return stageBlockEnums;
    }

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
    }

    //初期化処理
    public void Initialize(BlockEnum[,] _stageBlockEnums, GameObject[,] _stageBlockObjects)
    {
        //BlockEnum、GameObjectの配列への代入
        stageBlockEnums = _stageBlockEnums;
        stageBlockObjects = _stageBlockObjects;
        //ワープバコの配列の初期化
        InitializeWarpBoxArrangement();
        //全てのワープバコについて、ワープ可能かのチェック
        CanWarpCheck();
    }

    //破壊処理
    public void DeleteBlock()
    {
        //ワープバコの配列を削除
        for (int x = 0; x < warpBoxs.GetLength(0); x++)
        {
            for (int y = 0; y < warpBoxs.GetLength(1); y++)
            {
                warpBoxs[x, y] = null;
            }
        }

        //stageBlockEnumsかstageBlockObjectsが無い時、ここで早期リターンする
        if (stageBlockEnums == null && stageBlockObjects == null) return;

        for(int x = 0; x < stageBlockEnums.GetLength(0); x++)
        {
            for(int y = 0; y < stageBlockEnums.GetLength(1); y++)
            {
                stageBlockEnums[x, y] = BlockEnum.None;
            }
        }

        foreach(Transform tf in transform)
        {
            Destroy(tf.gameObject);
        }
    }

    //playerPosの位置でハコを置けるかどうか
    public static bool CanPutBox(Vector3 playerPos,bool isRight)
    {
        //まだinstanceの初期化が済んでいない時、falseを返す
        if (instance == null)
        {
            return false;
        }

        //向いている方向に応じて位置に補正をかける
        Vector3 pos = playerPos;
        if (isRight)
        {
            pos += new Vector3(0.1f, 0, 0);
        }
        else
        {
            pos -= new Vector3(0.1f, 0, 0);
        }

        //該当箇所の特定
        int x = (int)Math.Round(pos.x, MidpointRounding.AwayFromZero);
        int y = (int)Math.Round(pos.y, MidpointRounding.AwayFromZero);
        if (isRight) { x++; }
        else { x--; }

        //右向いていて、右半分にいない時、
        //または左向いていて、左半分にいない時、falseを返す
        if ((isRight && pos.x % 1 > 0.5f)||(!isRight&&pos.x%1 < 0.5f))
        {
            //目印の三角形を非表示にする
            DissapearMarks();
            return false;
        }

        //画面外の時、falseを返す
        if (x < 0 || x >= instance.stageBlockEnums.GetLength(0)||
            y < 0 || y >= instance.stageBlockEnums.GetLength(1))
        {
            //目印の三角形を非表示にする
            DissapearMarks();
            return false;
        }

        //該当箇所がゴールで、配達バコを持っている時、trueを返す
        if (instance.stageBlockEnums[x, y] == BlockEnum.Goal
            && GameManager.instance._player.HangBlockEnum.Equals(BlockEnum.DeliverBox))
        {
            //ゴールの三角形を表示
            GameManager.instance._goal.IsMoveTriangle = true;
            //置ける位置の三角形を非表示にする
            instance._markForPutBox.SetActive(false);
            return true;
        }

        //該当箇所に何かが置かれている時、falseを返す
        if (instance.stageBlockEnums[x, y] != BlockEnum.None)
        {
            //目印の三角形を非表示にする
            DissapearMarks();
            return false;
        }

        //該当箇所の真下に何も置かれていない(BlockEnumがNoneまたはGoal)時、falseを返す
        if (y<=0 || instance.stageBlockEnums[x, y-1] == BlockEnum.None||instance.stageBlockEnums[x,y-1]==BlockEnum.Goal)
        {
            //目印の三角形を非表示にする
            instance._markForPutBox.SetActive(false);
            //ゴールの三角形を非表示にする
            GameManager.instance._goal.IsMoveTriangle = false;
            return false;
        }

        //置ける位置の三角形を表示する
        instance._markForPutBox.SetActive(true);
        instance._markForPutBox.transform.localPosition=new Vector3(x, y-0.225f, 0);
        //ゴールの三角形を非表示にする
        GameManager.instance._goal.IsMoveTriangle = false;
        return true;
    }

    //playerの現在位置にハコを置く
    public static void PutBox(Player player,bool isRight)
    {
        Vector3 playerPos = player.transform.position;
        //向いている方向に応じて位置に補正をかける
        Vector3 pos = playerPos;
        if (isRight)
        {
            pos += new Vector3(0.1f, 0, 0);
        }
        else
        {
            pos -= new Vector3(0.1f, 0, 0);
        }
        //該当箇所の特定
        int x = (int)Math.Round(pos.x, MidpointRounding.AwayFromZero);
        int y = (int)Math.Round(pos.y, MidpointRounding.AwayFromZero);
        if (isRight){ x++;}
        else{ x--;}

        //配達バコをゴールに置く場合
        if (instance.stageBlockEnums[x, y] == BlockEnum.Goal&&player.HangBlockEnum.Equals(BlockEnum.DeliverBox))
        {
            //ゴールの三角形を非表示にする
            GameManager.instance._goal.IsMoveTriangle = false;

            //プレイヤーの状態の設定
            player.IsHang = false;
            player.HangBlockEnum = BlockEnum.None;
            player.SetHangBoxSprite(null);
            player.SetVelocity(new Vector2(0, player.GetVelocity().y));

            //配達バコの提出
            GameManager.instance.SubmitDeliverBox();
            return;
        }

        //ハコの作成
        Box box = StageGenerator.instance.CreateBlock(player.HangBlockEnum, x, y).GetComponent<Box>();

        if (box != null)
        {
            //配列の設定
            instance.stageBlockEnums[x, y] = player.HangBlockEnum;
            instance.stageBlockObjects[x, y] = box.gameObject;

            //置ける位置の目印の三角形を非表示にする
            instance._markForPutBox.SetActive(false);

            //プレイヤーの状態の設定
            player.IsHang = false;
            player.HangBlockEnum = BlockEnum.None;
            player.SetHangBoxSprite(null);
            player.SetVelocity(new Vector2(0, player.GetVelocity().y));

            //ワープバコを配列に追加する
            instance.AddWarpBox(box);
            //各ワープバコがワープできるかの更新
            instance.CanWarpCheck();
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

        //ワープバコの処理
        instance.DestoryWarpBox(box);
        instance.CanWarpCheck();

        //上のブロックを落下させる
        bool isFinishDeal = true;//落下後の処理をするかどうか
        y++;
        while(y<instance.stageBlockEnums.GetLength(1)&&instance.stageBlockEnums[x, y] != BlockEnum.None&&instance.stageBlockEnums[x,y]!=BlockEnum.NotBox)
        {
            instance.stageBlockEnums[x, y - 1] = instance.stageBlockEnums[x, y];
            instance.stageBlockEnums[x, y] = BlockEnum.None;
            instance.stageBlockObjects[x, y - 1] = instance.stageBlockObjects[x, y];
            instance.stageBlockObjects[x, y] = null;

            if (isFinishDeal)
            {
                player.CanOperate = false;
                isFinishDeal = false;
                instance.stageBlockObjects[x,y-1].transform.DOLocalMove(new Vector3(x, y-1, 0),0.2f)
                .SetEase(Ease.InSine).OnComplete(instance.FinishFallAnimation).SetLink(instance.stageBlockObjects[x,y-1]);
            }else{
                instance.stageBlockObjects[x,y-1].transform.DOLocalMove(new Vector3(x, y-1, 0),0.2f)
                    .SetEase(Ease.InSine).SetLink(instance.stageBlockObjects[x, y - 1]);
            }
            y++;
        }
    }

    //置ける位置とゴールの目印の非表示
    public static void DissapearMarks()
    {
        //目印の三角形を非表示にする
        instance._markForPutBox.SetActive(false);
        //ゴールの三角形を非表示にする
        GameManager.instance._goal.IsMoveTriangle = false;
    }

    //落下アニメーションが終わり、再度操作可能にする
    public void FinishFallAnimation()
    {
        GameManager.instance._player.CanOperate = true;
    }

    //目印の三角形を非表示にする
    public static void DissapearTriangle()
    {
        instance._markForPutBox.SetActive(false);
    }

    //ワープバコの配列の初期化
    private void InitializeWarpBoxArrangement()
    {
        //全てのワープバコについて調べる(xが同じの時、その二つはペアとなる)
        for (int x = 0; x < stageBlockEnums.GetLength(0); x++)
        {
            for (int y = 0; y < stageBlockEnums.GetLength(1); y++)
            {
                //ワープバコ1-4について調べる
                for (int warpTypeNum = 0; warpTypeNum <= 3; warpTypeNum++)
                {
                    //数字をBlockEnumに変換する
                    BlockEnum warpEnum = ConvertWarpEnumFormInt(warpTypeNum);
                    //もしstageBlockEnum[x,y]がワープバコだった時、
                    if (stageBlockEnums[x, y].Equals(warpEnum))
                    {
                        //0番目が空いているなら
                        if (object.ReferenceEquals(warpBoxs[warpTypeNum, 0], null))
                        {
                            //0番目に代入
                            warpBoxs[warpTypeNum, 0] = stageBlockObjects[x, y].GetComponent<WarpBox>();
                        }
                        else
                        {
                            //0番目が空いていない時、1番目に代入
                            warpBoxs[warpTypeNum, 1] = stageBlockObjects[x, y].GetComponent<WarpBox>();
                        }
                    }
                }
            }
        }
    }

    //ワープバコがワープ可能かの更新
    private void CanWarpCheck()
    {
        for (int x = 0; x < warpBoxs.GetLength(0); x++)
        {
            for (int y = 0; y < warpBoxs.GetLength(1); y++)
            {
                if (warpBoxs[x, y] != null)
                {
                    warpBoxs[x, y].CheckIsWarp(this);
                }
            }
        }
    }


    //ワープバコを配列に追加する
    private void AddWarpBox(Box box)
    {
        for (int warpTypeNum = 0; warpTypeNum <= 3; warpTypeNum++)
        {
            //数字をBlockEnumに変換
            BlockEnum warpType = ConvertWarpEnumFormInt(warpTypeNum);
            WarpBox warpBox;
            if (box.myBlockEnum.Equals(warpType))
            {
                if (object.ReferenceEquals(instance.warpBoxs[warpTypeNum, 0], null))
                {
                    warpBox = box.GetComponent<WarpBox>();
                    instance.warpBoxs[warpTypeNum, 0] = warpBox;
                    if (!object.ReferenceEquals(instance.warpBoxs[warpTypeNum, 1], null))
                    {
                        warpBox.SetPair(instance.warpBoxs[warpTypeNum, 1]);
                        instance.warpBoxs[warpTypeNum, 1].SetPair(warpBox);
                    }
                }
                else
                {
                    warpBox = box.GetComponent<WarpBox>();
                    instance.warpBoxs[warpTypeNum, 1] = warpBox;
                    if (!object.ReferenceEquals(instance.warpBoxs[warpTypeNum, 0], null))
                    {
                        warpBox.SetPair(instance.warpBoxs[warpTypeNum, 0]);
                        instance.warpBoxs[warpTypeNum, 0].SetPair(warpBox);
                    }
                }
            }
        }
    }

    //ワープバコの削除
    private void DestoryWarpBox(Box box)
    {
        for (int warpTypeNum = 0; warpTypeNum <= 3; warpTypeNum++)
        {
            BlockEnum warpType = ConvertWarpEnumFormInt(warpTypeNum);
            if (box.myBlockEnum.Equals(warpType))
            {
                ((WarpBox)box).CanWarp = false;
                if (object.ReferenceEquals(warpBoxs[warpTypeNum, 0], box))
                {
                    warpBoxs[warpTypeNum, 0] = null;
                    if (!object.ReferenceEquals(warpBoxs[warpTypeNum, 1], null))
                    {
                        warpBoxs[warpTypeNum, 1].SetPair(null);
                    }
                }
                else if (object.ReferenceEquals(warpBoxs[warpTypeNum, 1], box))
                {
                    warpBoxs[warpTypeNum, 1] = null;
                    if (!object.ReferenceEquals(warpBoxs[warpTypeNum, 0], null))
                    {
                        warpBoxs[warpTypeNum, 0].SetPair(null);
                    }
                }
            }
        }
    }

    //数字をBlockEnumに変換する
    private BlockEnum ConvertWarpEnumFormInt(int warpTypeNum)
    {
        BlockEnum warpEnum = BlockEnum.None;
        switch (warpTypeNum)
        {
            case 0: warpEnum = BlockEnum.WarpBox1; break;
            case 1: warpEnum = BlockEnum.WarpBox2; break;
            case 2: warpEnum = BlockEnum.WarpBox3; break;
            case 3: warpEnum = BlockEnum.WarpBox4; break;
            default: Debug.Log(warpTypeNum + "が用意されていない"); break;
        }
        return warpEnum;
    }
}
