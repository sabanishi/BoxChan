using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBox : Box
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite speciallySprirt;

    public void SetSprite(bool isSpecial)
    {
        if (isSpecial)
        {
            sprite.sprite = speciallySprirt;
        }
        else
        {
            sprite.sprite = normallySprite;
        }
    }
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.CompareTag("PlayerGroundCheck"))
        {
            //必要な変数の用意
            Player player = collision.transform.parent.gameObject.GetComponent<Player>();
            Vector3 playerPos = player.gameObject.transform.position;
            Vector3 thisPos = transform.position;
            CapsuleCollider2D playerCollider= player.GetComponent<CapsuleCollider2D>();
            Vector2 playerSize = playerCollider.size / 2.5f;
            Vector2 playerOffset= playerCollider.offset;
            Vector2 thisSize = GetComponent<BoxCollider2D>().size/2;

            //プレイヤーが上かどうかを確認する
            float playerY = playerPos.y + playerSize.y + playerOffset.y;
            float thisY = thisPos.y - thisSize.y;
            if (thisY <= playerY)
            {
                player.HighJump();
            }
        }
    }
}
