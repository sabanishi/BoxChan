using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNode : MonoBehaviour
{
    [SerializeField] private Vector3 normallySize;
    [SerializeField] private Vector3 inCursleSize;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite inCursleSprite;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private ExtraStageSelectManager parent;
    [SerializeField] private Transform _transform;
    [SerializeField] private bool isPlay;

    public void OnEnter()
    {
        sprite.sprite = inCursleSprite;
        _transform.localScale = inCursleSize;
    }

    public void OnExit()
    {
        sprite.sprite = normallySprite;
        _transform.localScale = normallySize;
    }

    public void OnClick()
    {
        OnExit();
        if (isPlay)
        {
            //遊ぶ
            SoundManager.PlaySE(SE_Enum.DECIDE1);
            parent.PlaySelectStage();
        }
        else
        {
            //やめる
            SoundManager.PlaySE(SE_Enum.DECIDE2);
            parent.CancelSelectStage();
        }
    }
}
