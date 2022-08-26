using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//画面外に落ちたゲームオーバーにするためのクラス
public class FallDeadCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.instance.StartCoroutine(GameManager.instance.RestartCoroutine());
        }
    }
}
