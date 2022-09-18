using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNodeparent : MonoBehaviour
{
    [SerializeField] private SelectNode goTitleNode;
    [SerializeField] private GameObject boardNode;
    [SerializeField] private SelectNode playNode;
    [SerializeField] private SelectNode createNode;

    //初期化
    public void Initialize()
    {
        NodePosReset();
        StartAnimation();
    }

    //ノードの位置を初期化
    private void NodePosReset()
    {
        goTitleNode.transform.localPosition = new Vector3(11, -3.65f, 0);

    }

    //開始時アニメーション
    private void StartAnimation()
    {
        goTitleNode.Initialize();
        playNode.Initialize();
        createNode.Initialize();


    }
}
