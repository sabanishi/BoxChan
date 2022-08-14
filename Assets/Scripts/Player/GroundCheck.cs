using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private bool isGround = false;
    private bool isGroudEnter, isGroundStay, isGroundExit;
    [SerializeField] private Transform _transform;

    public void Update()
    {
        _transform.localPosition = new Vector3(0, 0, 0);
    }

    public bool IsGround()
    {
        if (isGroundExit)
        {
            isGround = false;
        }
        else if (isGroudEnter || isGroundStay)
        {
            isGround = true;
        }

        isGroudEnter = false;
        isGroundStay = false;
        isGroundExit = false;
        return isGround;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Block"||collision.tag=="Box")
        {
            isGroudEnter = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Block" || collision.tag == "Box")
        {
            isGroundStay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Block" || collision.tag == "Box")
        {
            isGroundExit = true;

        }
    }
}
