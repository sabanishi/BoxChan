using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : MonoBehaviour
{
    [SerializeField] private Vector3 normallySize;
    [SerializeField] private Vector3 inCursleSize;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite inCursleSprite;
    [SerializeField] private AbstractChild parent;
    [SerializeField] private string nodeName;
    [SerializeField] private SpriteRenderer sprite;

    private bool isValid;
    private bool isEnter;
    public void SetIsValidTrue()
    {
        isValid = true;
    }
    public void SetIsvalidFalse()
    {
        isValid = false;
    }

    private Transform _transform;

    public void Start()
    {
        _transform = transform;
    }

    public void Update()
    {
        if (isValid&&isEnter&&parent.parent.IsValid)
        {
            isEnter = false;
            _transform.localScale = inCursleSize;
            sprite.sprite = inCursleSprite;
        }
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        isValid = false;
        isEnter = false;
        _transform = transform;
        _transform.localScale = normallySize;
        sprite.sprite = normallySprite;
    }

    public void OnMouseEnter()
    {
        if (!isValid|| !parent.parent.IsValid)
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

        if (parent.parent.IsValid)
        {
            _transform.localScale = normallySize;
            sprite.sprite = normallySprite;
        }
        isEnter = false;
    }
    

    public void OnMouseClick()
    {
        if (!isValid || !parent.parent.IsValid) return;
        parent.ClickDeal(nodeName);
        SoundManager.PlaySE(SE_Enum.DECIDE2);
    }
}
