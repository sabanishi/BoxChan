using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShowSelectBlockPanel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer childSprite;

    [SerializeField] private Sprite openMenu;
    [SerializeField] private Sprite closeMenu;

    private bool isValid;
    private bool isEnter;
    private bool isExit;

    public void Update()
    {
        
        if (isValid)
        {
            if (isEnter)
            {
                isEnter = false;
                OnEnterClick();
            }
            if (isExit)
            {
                isExit = false;
                OnExitClick();
            }
        }
    }

    public void OnEnterClick()
    {
        if (isValid)
        {
            sprite.color = new Color(1, 1, 1, 0.2f);
            childSprite.color = new Color(1, 1, 1, 0.2f);
        }
        else
        {
            isEnter = true;
            isExit = false;
        }
    }

    public void OnExitClick()
    {
        if (isValid)
        {
            sprite.color = new Color(1, 1, 1, 1);
            childSprite.color = new Color(1, 1, 1, 1);
        }
        else
        {
            isEnter = false;
            isExit = true;
        }
    }

    public void CloseMenu()
    {
        isValid = true;
        sprite.sprite = closeMenu;
    }

    public void OpenMenu()
    {
        OnExitClick();
        isValid = false;
        sprite.sprite = openMenu;
    }
}
