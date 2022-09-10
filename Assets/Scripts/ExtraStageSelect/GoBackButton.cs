using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoBackButton : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private Vector3 normallySize;
    [SerializeField] private Vector3 inCursleSize;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite inCursleSprite;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private ExtraStageSelectManager parent;

    private bool isValid;
    private bool isEnter;

    public void SetIsValidTrue()
    {
        isValid = true;
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        isValid = false;
        isEnter = false;
        _transform.localScale = normallySize;
        sprite.sprite = normallySprite;
    }

    public void Update()
    {
        if (isValid && isEnter&&!parent.IsSelect)
        {
            isEnter = false;
            _transform.localScale = inCursleSize;
            sprite.sprite = inCursleSprite;
        }
    }

    public void OnMouseEnter()
    {
        if (!isValid||parent.IsSelect)
        {
            isEnter = true;
        }
        else
        {
            _transform.localScale = inCursleSize;
            sprite.sprite = inCursleSprite;
        }
    }

    public void OnMouseExit()
    {
        _transform.localScale = normallySize;
        sprite.sprite = normallySprite;
        isEnter = false;
    }

    public void OnMouseClick()
    {
        if (!isValid||parent.IsSelect) return;
        SoundManager.PlaySE(SE_Enum.DECIDE2);
        SceneChangeManager.GoSelect(SceneEnum.ExtraStageSelect);
    }
}
