using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPazzleNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public void Initialize(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
