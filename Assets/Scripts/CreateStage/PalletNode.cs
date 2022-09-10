using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletNode : MonoBehaviour
{
    [SerializeField] private BlockEnum blockEnum;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private Pallet parent;
    [SerializeField] private SpriteRenderer sprite;

    [SerializeField]private Transform _transform;

    public bool isSelect;
    
    public void OnEnter()
    {
        if (isSelect) return;
        _transform.localScale = new Vector3(2.4f, 2.4f, 1);
        backSprite.color = new Color(1, 0, 0);
    }

    public void OnExit()
    {
        if (isSelect) return;
        _transform.localScale = new Vector3(2, 2, 1);
        backSprite.color = new Color(1, 1, 1);
    }

    public void OnClick()
    {
        if (isSelect) return;
        SoundManager.PlaySE(SE_Enum.DECIDE3);
        parent.SelectNode = this;
    }

    //筆が選択された時の処理
    public void StartSelect()
    {
        isSelect = true;
        _transform.localScale = new Vector3(2.4f, 2.4f, 1);
        backSprite.color = new Color(1, 1, 0);
    }

    //筆の選択が解除された時の処理
    public void FinishSelect()
    {
        isSelect = false;
        OnExit();
    }

    public Sprite GetSprite()
    {
        return sprite.sprite;
    }

    public BlockEnum GetBlockEnum()
    {
        return blockEnum;
    }
}
