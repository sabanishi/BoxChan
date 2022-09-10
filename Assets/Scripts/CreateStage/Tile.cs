using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Sprite defaultSprite;

    private Vector2Int position;
    private TileManager parent;
    private Sprite beforeSprite;

    public void Initialize(Vector2Int pos, TileManager _parent)
    {
        position = pos;
        parent = _parent;
        beforeSprite = defaultSprite;
    }

    public void OnClick()
    {
        parent.PutBlock(position.x, position.y, this, true);
    }
    public void OnEnter()
    {
        parent.PutBlock(position.x, position.y, this, false);
        if(position.x >= 0 && position.x <5 &&
            position.y > 14 && position.y <= 17)
        {
            parent.DissapearSelectBlockNode();
        }
    }

    public void OnExit()
    {
        parent.MouseExit(position.x, position.y, this);
        if (position.x >= 0 && position.x < 5 &&
            position.y > 14 && position.y <= 17)
        {
            parent.ShowSelectBlockNode();
        }
    }

    public void SetAlpha(float a)
    {
        sprite.color = new Color(1, 1, 1, a);
    }

    public void SetSprite(Sprite _sprite)
    {
        sprite.sprite = _sprite;
    }

    public Sprite GetSprite()
    {
        return sprite.sprite;
    }

    public void SetBeforeSprite(Sprite _sprite)
    {
        beforeSprite = _sprite;
    }

    public void ResetSprite()
    {
        sprite.sprite = beforeSprite;
    }

    public void SetColliderEnable(bool enable)
    {
        boxCollider.enabled = enable;
    }

    public void SetDefaultSprite()
    {
        sprite.sprite = defaultSprite;
        SetBeforeSprite(defaultSprite);
    }
}
