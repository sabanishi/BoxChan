using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class CurtainManager : MonoBehaviour
{
    [SerializeField] private GameObject Curtain;

    public IEnumerator AppearLetter()
    {
        yield return new WaitForSeconds(0.3f);
        Curtain.transform.localPosition = new Vector3(0,-22, 0);
        Curtain.transform.DOLocalMove(new Vector3(0,6, 0),0.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.6f);
    }

    public IEnumerator DisappearLetter()
    {
        Curtain.transform.DOLocalMove(new Vector3(0,34, 0),0.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.5f);
    }
}
