using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageGenerator : MonoBehaviour
{
    public static StageGenerator instance;
    private Transform _transform;

    [SerializeField]private string NotBoxName = "notBox";
    [SerializeField]private Block NotBoxBlockPrefab;

    [SerializeField] private string PlainBoxName = "plainBox";
    [SerializeField] private Box PlainBoxPrefab;

    [SerializeField]private string StartName = "start";
    [SerializeField]private StartBlock StartBlockPrefab;

    [SerializeField] private string DamageBoxName = "damageBox";
    [SerializeField] private DamageBox DamageBoxPrefab;

    [SerializeField] private string DeliverBoxName = "deliverBox";
    [SerializeField] private DeliverBox DeliverBoxPrefab;

    [SerializeField] private string JumpBoxName = "jumpBox";
    [SerializeField] private JumpBox JumpBoxPrefab;

    [SerializeField] private string GoalName = "goal";
    [SerializeField] private Goal GoalPrefab;

    [SerializeField] private string WarpName1 = "warpBox1";
    [SerializeField] private string WarpName2 = "warpBox2";
    [SerializeField] private string WarpName3 = "warpBox3";
    [SerializeField] private string WarpName4 = "warpBox4";
    [SerializeField] private WarpBox WarpBoxprefab;

    private StartBlock _startBlock;//プレイヤーの開始場所
    public Goal _goal { get; private set; }//ゴール
    public int deliveryboxNum { get; private set; }//配達バコの総数(初期配置)

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            _transform = transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //プレイヤーの作成
    public Player CreatePlayer()
    {
        Player player = null;
        //プレイヤーの作成
        if (_startBlock != null)
        {
            player = _startBlock.Initialize();
        }
        return player;
    }

    //GameManagerから呼び出され、TileMapを基にステージを作成し、二次元配列を返す
    public (BlockEnum[,],GameObject[,]) CreateStageFromTilemap(Tilemap _tileMap)
    {
        _transform = transform;

        BlockEnum[,] blockEnums = GenerateStage(_tileMap);

        GameObject[,] blockObjects = CreateStage(blockEnums);
        (BlockEnum[,], GameObject[,]) tuple = (blockEnums, blockObjects);

        return tuple;
    }

    //TileMapからBlockEnumの二次元配列を作成する
    private BlockEnum[,] GenerateStage(Tilemap _tileMap)
    {
        _tileMap.CompressBounds();
        BlockEnum[,] blockEnums;
        BoundsInt bounds = _tileMap.cellBounds;
        TileBase[] allBlocks = _tileMap.GetTilesBlock(bounds);
        
        blockEnums = new BlockEnum[bounds.size.x, bounds.size.y];
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tileBase = allBlocks[x + y * bounds.size.x];
                if (tileBase != null)
                {
                    if (tileBase.name == NotBoxName)
                    {
                        blockEnums[x, y] = BlockEnum.NotBox;
                    }
                    else if (tileBase.name == StartName)
                    {
                        blockEnums[x, y] = BlockEnum.Start;
                    }else if (tileBase.name == PlainBoxName)
                    {
                        blockEnums[x, y] = BlockEnum.PlainBox;
                    }else if (tileBase.name == DamageBoxName)
                    {
                        blockEnums[x, y] = BlockEnum.DamageBox;
                    }else if (tileBase.name == DeliverBoxName)
                    {
                        blockEnums[x, y] = BlockEnum.DeliverBox;
                    }else if (tileBase.name == GoalName)
                    {
                        blockEnums[x, y] = BlockEnum.Goal;
                    }else if (tileBase.name == JumpBoxName)
                    {
                        blockEnums[x, y] = BlockEnum.JumpBox;
                    }else if (tileBase.name == WarpName1)
                    {
                        blockEnums[x, y] = BlockEnum.WarpBox1;
                    }
                    else if (tileBase.name == WarpName2)
                    {
                        blockEnums[x, y] = BlockEnum.WarpBox2;
                    }
                    else if (tileBase.name == WarpName3)
                    {
                        blockEnums[x, y] = BlockEnum.WarpBox3;
                    }
                    else if (tileBase.name == WarpName4)
                    {
                        blockEnums[x, y] = BlockEnum.WarpBox4;
                    }
                }
            }
        }
        return blockEnums;
    }

    //blockEnumsを基にステージを作成
    public GameObject[,] CreateStage(BlockEnum[,] blockEnums)
    {
        GameObject[,] stageObjects = new GameObject[blockEnums.GetLength(0), blockEnums.GetLength(1)];
        deliveryboxNum = 0;
        WarpBox[] warpBoxs = new WarpBox[4];
        for(int x = 0; x < blockEnums.GetLength(0); x++)
        {
            for(int y = 0; y < blockEnums.GetLength(1); y++)
            {
                if (blockEnums[x, y] != BlockEnum.None)
                {
                    stageObjects[x,y]=CreateBlock(blockEnums[x, y],x,y);
                    //blockEnum[x,y]がStart地点の時、それをNoneに差し替える
                    if (blockEnums[x, y] == BlockEnum.Start)
                    {
                        blockEnums[x, y] = BlockEnum.None;
                    }
                    //配達バコの数を数える
                    if (blockEnums[x, y] == BlockEnum.DeliverBox)
                    {
                        deliveryboxNum++;
                    }

                    //ワープボックスの相方の設定
                    SetWarpBoxPair(blockEnums[x, y], stageObjects[x, y], warpBoxs);
                }
            }
        }
        return stageObjects;
    }

    //ワープボックスの設定
    private void SetWarpBoxPair(BlockEnum blockEnum,GameObject warpObject,WarpBox[] warpBoxs)
    {
        WarpBox warp;
        //ワープボックスの相方の設定
        if (blockEnum == BlockEnum.WarpBox1)
        {
            warp = warpObject.GetComponent<WarpBox>();
            if (warpBoxs[0] == null)
            {
                warpBoxs[0] = warp;
            }
            else
            {
                warpBoxs[0].SetPair(warp);
                warp.SetPair(warpBoxs[0]);
            }
        }
        else if (blockEnum == BlockEnum.WarpBox2)
        {
            warp = warpObject.GetComponent<WarpBox>();
            if (warpBoxs[1] == null)
            {
                warpBoxs[1] = warp;
            }
            else
            {
                warpBoxs[1].SetPair(warp);
                warp.SetPair(warpBoxs[1]);
            }
        }else if (blockEnum == BlockEnum.WarpBox3)
        {
            warp = warpObject.GetComponent<WarpBox>();
            if (warpBoxs[2] == null)
            {
                warpBoxs[2] = warp;
            }
            else
            {
                warpBoxs[2].SetPair(warp);
                warp.SetPair(warpBoxs[2]);
            }
        }else if (blockEnum == BlockEnum.WarpBox4)
        {
            warp = warpObject.GetComponent<WarpBox>();
            if (warpBoxs[3] == null)
            {
                warpBoxs[3] = warp;
            }
            else
            {
                warpBoxs[3].SetPair(warp);
                warp.SetPair(warpBoxs[3]);
            }
        }
    }

    //BlockEnumを基にブロックを作成
    public GameObject CreateBlock(BlockEnum blockEnum,int x,int y)
    {
        Vector3 pos = new Vector3(x,y,0);
        switch (blockEnum)
        {
            case BlockEnum.Start:
                StartBlock start = Instantiate(StartBlockPrefab, _transform);
                start.transform.position = pos;
                _startBlock = start;
                return start.gameObject;
            case BlockEnum.NotBox:
                Block notBox = Instantiate(NotBoxBlockPrefab, _transform);
                notBox.transform.position = pos;
                break;
            case BlockEnum.PlainBox:
                Box plainBox = Instantiate(PlainBoxPrefab, _transform);
                plainBox.transform.position = pos;
                return plainBox.gameObject;
            case BlockEnum.DamageBox:
                DamageBox damageBox = Instantiate(DamageBoxPrefab, _transform);
                damageBox.transform.position = pos;
                return damageBox.gameObject;
            case BlockEnum.DeliverBox:
                DeliverBox deliverBox = Instantiate(DeliverBoxPrefab, _transform);
                deliverBox.transform.position = pos;
                return deliverBox.gameObject;
            case BlockEnum.Goal:
                Goal goal = Instantiate(GoalPrefab, _transform);
                goal.transform.position = pos;
                _goal = goal;
                return goal.gameObject;
            case BlockEnum.JumpBox:
                JumpBox jumpBox = Instantiate(JumpBoxPrefab, _transform);
                jumpBox.transform.position = pos;
                return jumpBox.gameObject;
            case BlockEnum.WarpBox1:
                WarpBox warpBox1 = Instantiate(WarpBoxprefab, _transform);
                warpBox1.transform.position = pos;
                warpBox1.SetNumber(1);
                return warpBox1.gameObject;
            case BlockEnum.WarpBox2:
                WarpBox warpBox2 = Instantiate(WarpBoxprefab, _transform);
                warpBox2.transform.position = pos;
                warpBox2.SetNumber(2);
                return warpBox2.gameObject;
            case BlockEnum.WarpBox3:
                WarpBox warpBox3 = Instantiate(WarpBoxprefab, _transform);
                warpBox3.transform.position = pos;
                warpBox3.SetNumber(3);
                return warpBox3.gameObject;
            case BlockEnum.WarpBox4:
                WarpBox warpBox4 = Instantiate(WarpBoxprefab, _transform);
                warpBox4.transform.position = pos;
                warpBox4.SetNumber(4);
                return warpBox4.gameObject;
            default:
                Debug.Log(blockEnum + "が無い");
                break;
        }
        return null;
    }
}
