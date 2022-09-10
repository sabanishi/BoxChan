using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexChangeButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform _transform;
    [SerializeField] private ExtraStageSelectManager parent;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Vector3 normalSize;
    [SerializeField] private Vector3 isTouchSize;
    [SerializeField] private bool isUp;

    private bool isEnter;
    private bool isExit;
    private bool isValid;

    public bool IsValid
    {
        get { return isValid; }
        set
        {
            isValid = value;
            if (isValid)
            {
                spriteRenderer.color = new Color(1, 1, 1, 1);
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            }
            isExit = false;
        }
    }

    public void Update()
    {
        if (isEnter && !parent.IsAnimationPlay)
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
        if (!isValid) return;
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
        if (!isValid) return;
        if (parent.IsAnimationPlay) return;
        SoundManager.PlaySE(SE_Enum.DECIDE2);
        if (isUp)
        {
            parent.IndexNum++;
        }
        else
        {
            parent.IndexNum--;
        }

        OnExit();
    }
}
