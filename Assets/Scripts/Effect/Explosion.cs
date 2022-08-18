using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            Destroy(gameObject);
        }
    }
}
