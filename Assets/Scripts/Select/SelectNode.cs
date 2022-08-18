using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : MonoBehaviour
{
    [SerializeField] private Vector3 normallySize;
    [SerializeField] private Vector3 inCursleSize;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite inCursleSprite;
    [SerializeField] private AbstractManager parent;
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
        if (!isValid)
        {
            isEnter = true;
        }
        else
        {
            if (parent.parent.IsValid)
            {
                _transform.localScale = inCursleSize;
                sprite.sprite = inCursleSprite;
            }
        }
       
    }

    public void OnMouseExit()
    {
        if (!isValid)
        {
            isEnter = false;
        }
        else
        {
            if (parent.parent.IsValid)
            {
                _transform.localScale = normallySize;
                sprite.sprite = normallySprite;
            }
        }
    }

    public void OnMouseClick()
    {
        if (!isValid || !parent.parent.IsValid) return;
        parent.ClickDeal(nodeName);
    }
}
