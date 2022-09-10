using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Letter : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Vector3 normalSize;
    [SerializeField] private Vector3 isTouchSize;
    [SerializeField] private Text nameText;

    private ExtraStageDataNode data;
    private ExtraStageSelectManager parent;
    private bool isEnter;
    private bool isExit;
    private Vector3 stablePos;

    public ExtraStageDataNode Data
    {
        get { return data; }
    }

    public void SetStablePos(Vector3 pos)
    {
        stablePos = pos;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void Initialize(ExtraStageDataNode _data,ExtraStageSelectManager _parent)
    {
        data = _data;
        parent = _parent;
        nameText.text = data.MapName;
    }

    public void Update()
    {
        if (isEnter&&!parent.IsAnimationPlay)
        {
            isEnter = false;
            if (!isExit)
            {
                OnEnter();
            }
        }
    }

    public void OnEnter()
    {
        if (!parent.IsAnimationPlay)
        {
            spriteRenderer.sprite = sprite2;
            _transform.localScale = isTouchSize;
        }
        else
        {
            isEnter = true;
            isExit = false;
        }
    }

    public void OnExit()
    {
        spriteRenderer.sprite = sprite1;
        _transform.localScale = normalSize;
        isExit = true;
    }

    public void OnClick()
    {
        if (parent.IsAnimationPlay) return;
        OnExit();
        SoundManager.PlaySE(SE_Enum.DECIDE2);
        parent.StartCoroutine(parent.Selectletter(this));
    }

    public void GoSelectPos(Vector3 selectPos,Vector3 scale,float time)
    {
        spriteRenderer.sortingOrder = 8;
        var sequence = DOTween.Sequence();
        sequence
            .Append(_transform.DOLocalMove(selectPos,time).SetEase(Ease.OutSine))
            .Join(_transform.DOScale(scale,time).SetEase(Ease.OutSine).SetSpeedBased());
    }

    public void GoStablePos(float time)
    {
        var sequence = DOTween.Sequence();
        sequence
            .Append(_transform.DOLocalMove(stablePos, time).SetEase(Ease.OutSine))
            .Join(_transform.DOScale(normalSize, time).SetEase(Ease.OutSine).SetSpeedBased()).OnComplete(
            ()=> { spriteRenderer.sortingOrder = 2; });
    }
}
