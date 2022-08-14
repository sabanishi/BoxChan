using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : Box
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //TODO:プレイヤーまたはGameManagerに死亡関数を実装して、それを呼び出す
        }
    }
}
