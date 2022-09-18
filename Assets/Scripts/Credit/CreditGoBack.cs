using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditGoBack : MonoBehaviour
{
    [SerializeField] private Vector3 normallySize;
    [SerializeField] private Vector3 inCursleSize;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite inCursleSprite;
    [SerializeField] private SpriteRenderer sprite;

    private Transform _transform;

    public void Start()
    {
        _transform = transform;
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        _transform = transform;
        _transform.localScale = normallySize;
        sprite.sprite = normallySprite;
    }

    public void OnMouseEnter()
    {
        _transform.localScale = inCursleSize;
        sprite.sprite = inCursleSprite;
    }

    public void OnMouseExit()
    {

        _transform.localScale = normallySize;
        sprite.sprite = normallySprite;
    }


    public void OnMouseClick()
    {
        SoundManager.PlaySE(SE_Enum.DECIDE2);
        SceneChangeManager.GoSelect(SceneEnum.Credit);
        OnMouseExit();
    }
}
