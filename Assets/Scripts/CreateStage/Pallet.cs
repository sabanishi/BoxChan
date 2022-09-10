using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pallet : MonoBehaviour
{
    [SerializeField] private SpriteRenderer selectSprite;//現在選択中の筆の画像を表示するSpriteRenderer
    [SerializeField] private PalletNode StartNode;//最初に選択されている筆Node
    [SerializeField] private PalletNode[] palletNodes;//Palletにある全ての筆Node
    [SerializeField] private TileManager tileManager;

    private PalletNode selectNode;//現在選択中の筆
    public PalletNode SelectNode
    {
        get { return selectNode; }
        set
        {
            //もし既に筆が選択されている時、その筆の選択を解除する
            if (!object.Equals(selectNode,null))
            {
                selectNode.FinishSelect();
            }

            selectNode = value;
            selectSprite.sprite = selectNode.GetSprite();
            selectNode.StartSelect();
            tileManager.NowSelectPallet = selectNode;
        }
    }

    public void Initialize()
    {
        SelectNode = StartNode;
    }

    public void CloseMenu()
    {
        foreach(var palletNode in palletNodes)
        {
            palletNode.OnExit();
        }
    }
}
