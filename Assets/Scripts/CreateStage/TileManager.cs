using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private ShowSelectBlockPanel selectBlockNode;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform _transform;
    [SerializeField] private CreateStageManager parent;

    private bool isValid;
    private PalletNode nowSelectPallet;

    private Tile[,] tiles;
    private BlockEnum[,] blockEnums;

    public bool IsValid
    {
        get { return isValid; }
        set
        {
            isValid = value;
            if (isValid)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    public PalletNode NowSelectPallet
    {
        set
        {
            nowSelectPallet = value;
        }
    }

    public BlockEnum[,] BlockEnums
    {
        get { return blockEnums; }
    }

    public void Initialize()
    {
        tiles = new Tile[32, 18];
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y] = Instantiate(tilePrefab,_transform);
                tiles[x, y].Initialize(new Vector2Int(x, y),this);
                tiles[x, y].transform.localPosition = new Vector3(x, y, 0);
            }
        }

        blockEnums = new BlockEnum[32, 18];
        for (int x = 0; x < blockEnums.GetLength(0); x++)
        {
            for (int y = 0; y < blockEnums.GetLength(1); y++)
            {
                blockEnums[x, y] = BlockEnum.None;
            }
        }
    }

    public void Terminate()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Destroy(tiles[x, y].gameObject);
                tiles[x, y] = null;
            }
        }
    }

    public void InitializeStage()
    {
        for(int x = 0; x < tiles.GetLength(0); x++)
        {
            blockEnums[x, 0] = nowSelectPallet.GetBlockEnum();
            tiles[x,0].SetSprite(nowSelectPallet.GetSprite());
            tiles[x,0].SetBeforeSprite(nowSelectPallet.GetSprite());
        }
    }

    private void OpenMenu()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y].SetColliderEnable(false);
            }
        }
    }

    private void CloseMenu()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y].SetColliderEnable(true);
            }
        }
    }

    public void PutBlock(int x,int y,Tile tile,bool isClick)
    {
        if (nowSelectPallet.GetBlockEnum().Equals(BlockEnum.None)) return;
        if (blockEnums[x, y].Equals(nowSelectPallet.GetBlockEnum())) return;
        if (nowSelectPallet.GetBlockEnum().Equals(BlockEnum.Eraser) && blockEnums[x, y].Equals(BlockEnum.None)) return;

        if (Input.GetMouseButton(0) || isClick)
        {
            //クリックされているので実際に入れ替える
            tile.SetAlpha(1);
            if (nowSelectPallet.GetBlockEnum().Equals(BlockEnum.Eraser))
            {
                tile.SetDefaultSprite();
                blockEnums[x, y] = BlockEnum.None;
            }
            else
            {
                blockEnums[x, y] = nowSelectPallet.GetBlockEnum();
                tile.SetSprite(nowSelectPallet.GetSprite());
                tile.SetBeforeSprite(nowSelectPallet.GetSprite());
            }
            SoundManager.PlaySE(SE_Enum.PA);
            parent.ChangeCannotUpload();

        }
        else
        {
            //クリックされていないので入れ替えない
            if (nowSelectPallet.GetBlockEnum().Equals(BlockEnum.Eraser))
            {
                //消しゴムの時
                tile.SetAlpha(0.2f);
            }
            else
            {
                //消しゴム以外
                tile.SetAlpha(0.6f);
                tile.SetBeforeSprite(tile.GetSprite());
                tile.SetSprite(nowSelectPallet.GetSprite());
            }
        }
    }

    public void MouseExit(int x, int y, Tile tile)
    {
        if (nowSelectPallet.GetBlockEnum().Equals(BlockEnum.None)) return;
        if (!blockEnums[x, y].Equals(nowSelectPallet.GetBlockEnum()))
        {
            tile.ResetSprite();
        }
        tile.SetAlpha(1);
    }

    public void ShowSelectBlockNode()
    {
        selectBlockNode.OnExitClick();
    }

    public void DissapearSelectBlockNode()
    {
        selectBlockNode.OnEnterClick();
    }
}
