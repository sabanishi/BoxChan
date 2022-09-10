using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpBox : Box
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Sprite sprite3;
    [SerializeField] private Sprite sprite4;
    [SerializeField] private Sprite spriteNot1;
    [SerializeField] private Sprite spriteNot2;
    [SerializeField] private Sprite spriteNot3;
    [SerializeField] private Sprite spriteNot4;
    [SerializeField] private Sprite spriteOK1;
    [SerializeField] private Sprite spriteOK2;
    [SerializeField] private Sprite spriteOK3;
    [SerializeField] private Sprite spriteOK4;
    private int WarpNum;
    private WarpBox pairWarpBox;

    private bool isTouch;
    private bool canWarp;
    public bool CanWarp
    {
        get { return canWarp; }
        set
        {
            canWarp = value;
            if (!canWarp) isTouch = false;
            SetSprite();
        }
    }

    public void SetNumber(int num)
    {
        WarpNum = num;
        switch (num)
        {
            case 1:
                myBlockEnum = BlockEnum.WarpBox1;
                break;
            case 2:
                myBlockEnum = BlockEnum.WarpBox2;
                break;
            case 3:
                myBlockEnum = BlockEnum.WarpBox3;
                break;
            case 4:
                myBlockEnum = BlockEnum.WarpBox4;
                break;
            default:
                Debug.Log(num + "がない");
                break;
        }
    }

    private void SetSprite()
    {
        if (!canWarp)
        {
            switch (myBlockEnum)
            {
                case BlockEnum.WarpBox1:
                    sprite.sprite = spriteNot1;
                    break;
                case BlockEnum.WarpBox2:
                    sprite.sprite = spriteNot2;
                    break;
                case BlockEnum.WarpBox3:
                    sprite.sprite = spriteNot3;
                    break;
                case BlockEnum.WarpBox4:
                    sprite.sprite = spriteNot4;
                    break;
            }
            return;
        }

        if (isTouch)
        {
            switch (myBlockEnum)
            {
                case BlockEnum.WarpBox1:
                    sprite.sprite = spriteOK1;
                    break;
                case BlockEnum.WarpBox2:
                    sprite.sprite = spriteOK2;
                    break;
                case BlockEnum.WarpBox3:
                    sprite.sprite = spriteOK3;
                    break;
                case BlockEnum.WarpBox4:
                    sprite.sprite = spriteOK4;
                    break;
            }
            return;
        }

        switch (myBlockEnum)
        {
            case BlockEnum.WarpBox1:
                sprite.sprite = sprite1;
                break;
            case BlockEnum.WarpBox2:
                sprite.sprite = sprite2;
                break;
            case BlockEnum.WarpBox3:
                sprite.sprite = sprite3;
                break;
            case BlockEnum.WarpBox4:
                sprite.sprite = sprite4;
                break;
        }
    }

    public void Update()
    {
        if (isTouch)
        {
            if (Input.GetButtonDown("Warp"))
            {
                //プレイヤーをワープさせる
                if (GameManager.instance._player.CanOperate)
                {
                    GameManager.instance._player.WarpDeal(this, pairWarpBox);
                }
            }
        }
    }

    public void SetPair(WarpBox pair)
    {
        pairWarpBox = pair;
        if (BlockManager.instance != null)
        {
            CheckIsWarp(BlockManager.instance);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canWarp) return;
        if (collision.gameObject.transform.CompareTag("PlayerGroundCheck"))
        {
            isTouch = true;
            SetSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("PlayerGroundCheck"))
        {
            isTouch = false;
            SetSprite();
        }
    }

    //ワープできるかを更新する
    public void CheckIsWarp(BlockManager blockManager)
    {
        if (pairWarpBox == null)
        {
            CanWarp = false;
            return;
        }
        Vector2Int thisPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int pairPos = new Vector2Int((int)pairWarpBox.transform.position.x, (int)pairWarpBox.transform.position.y);

        //自分と相方の両方の上一マスが空いている時、trueを返す
        if(blockManager.GetStageBlockEnums()[thisPos.x,thisPos.y+1].Equals(BlockEnum.None)&&
            blockManager.GetStageBlockEnums()[pairPos.x, pairPos.y + 1].Equals(BlockEnum.None))
        {
            CanWarp = true;
        }
        else
        {
            CanWarp = false;
        }
    }
}
