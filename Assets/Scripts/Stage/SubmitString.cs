using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitString : MonoBehaviour
{
    [SerializeField] private GameObject DeliverBoxSprite;

    public void Initialize()
    {
        transform.localScale = new Vector3(0.5f, 1, 1);
        gameObject.SetActive(true);
        DeliverBoxSprite.SetActive(false);
    }

    public void AppearDeliverBox()
    {
        DeliverBoxSprite.SetActive(true);
    }
}
