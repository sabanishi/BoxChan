using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] private GameObject Triangle;
    public BlockEnum myBlockEnum;
    private bool isMoveTriangle;
    public bool IsMoveTriangle
    {
        get
        {
            return this.isMoveTriangle;
        }
        set
        {
            this.isMoveTriangle = value;
            if (isMoveTriangle)
            {
                Triangle.SetActive(true);
            }
            else
            {
                Triangle.SetActive(false);
            }
        }
    }
}
