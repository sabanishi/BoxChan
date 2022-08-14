using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageGenerator : MonoBehaviour
{
    public static StageGenerator instance;

    [SerializeField]private string NotBoxName = "notBox";
    [SerializeField]private Block NotBoxBlockPrefab;

    [SerializeField] private string PlainBoxName = "plainBox";
    [SerializeField] private Box PlainBoxPrefab;

    [SerializeField]private string StartName = "start";
    [SerializeField]private StartBlock StartBlockPrefab;

    [SerializeField] private string DamageBoxName = "damageBox";
    [SerializeField] private DamageBox DamageBoxPrefab;

    private StartBlock _startBlock;
    private Transform _transform;

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

    //初期化
    public Player Initialize()
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
                }
            }
        }
        return stageObjects;
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
            default:
                Debug.Log(blockEnum + "が無い");
                break;
        }
        return null;
    }
}
