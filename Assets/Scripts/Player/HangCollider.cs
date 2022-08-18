using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangCollider : MonoBehaviour
{
    public Box collisionBox { private set; get; }
    public bool isHang { get; set; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHang&&collision.tag == "Box")
        {
            if (collisionBox != null)
            {
                collisionBox.IsMoveTriangle = false;
                collisionBox = null;
            }
            
            Box thisCollisionBox= collision.GetComponent<Box>();
            if (thisCollisionBox.myBlockEnum.Equals(BlockEnum.DamageBox))
            {
                return;
            }
            collisionBox = thisCollisionBox;
            collisionBox.IsMoveTriangle = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Box")
        {
            if (collisionBox != null)
            {
                Box thisCollisionBox = collision.GetComponent<Box>();
                if (thisCollisionBox.myBlockEnum.Equals(BlockEnum.DamageBox))
                {
                    return;
                }
                if (collisionBox == thisCollisionBox)
                {
                    collisionBox.IsMoveTriangle = false;
                    collisionBox = null;
                }
            }
        }
    }
}
