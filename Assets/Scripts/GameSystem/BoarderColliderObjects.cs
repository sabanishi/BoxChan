using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームシーンにおいて、端からプレイヤーが落ちないようにするためのCollider
public class BoarderColliderObjects : MonoBehaviour
{
    [SerializeField] private BoxCollider2D up;
    [SerializeField] private BoxCollider2D down;
    [SerializeField] private BoxCollider2D right;
    [SerializeField] private BoxCollider2D left;

    public void SetBorderLine(float width,float height)
    {
        up.size = new Vector2(width, 1);
        up.offset = new Vector2(width / 2-0.5f, height+5);

        down.size = new Vector2(width, 1);
        down.offset = new Vector2(width / 2 - 0.5f,-5);

        right.size = new Vector2(1, height+10);
        right.offset = new Vector2(-1, height / 2 - 0.5f);

        left.size = new Vector2(1, height+10);
        left.offset=new Vector2(width, height/2 -0.5f);
    }
}
