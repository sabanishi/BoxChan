using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectLetter : MonoBehaviour
{
    [SerializeField] private GameObject entireNode;
    [SerializeField] private Animator topAnimator;
    [SerializeField] private Animator bottomAnimator;
    [SerializeField] private Text nameText;
    [SerializeField] private Text timeText;
    [SerializeField] private Transform blockParent;
    [SerializeField] private GameObject infoCard;
    [SerializeField] private ShowPazzleNode showPazzleNodePrefab;
    [SerializeField] private BlockImage blockImage;

    [SerializeField] private Sprite topDefaultSprite;
    [SerializeField] private Sprite bottomDefaultSprite;
    [SerializeField] private SpriteRenderer topSprite;
    [SerializeField] private SpriteRenderer bottomSprite;

    [System.Serializable][SerializeField]
    private struct BlockImage
    {
        public Sprite Start, Goal, NotBox, PlainBox,DeliverBox,DamageBox
            ,JumpBox,WarpBox1,WarpBox2,WarpBox3,WarpBox4;
    }

    public void Reset()
    {
        DestroyBlock();
        entireNode.SetActive(false);
        infoCard.transform.localPosition = new Vector3(0, -4.7f, 0);
        infoCard.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        topSprite.sprite = topDefaultSprite;
        bottomSprite.sprite = bottomDefaultSprite;
    }


    private ExtraStageDataNode dataNode;

    public void Initialzie(ExtraStageDataNode _dataNode)
    {
        DestroyBlock();
        infoCard.transform.localPosition = new Vector3(0, -4.7f, 0);
        dataNode = _dataNode;
        DisplayStageInfo();
        CreateBlock();
    }

    //既に存在するブロックの破壊
    private void DestroyBlock()
    {
        foreach(Transform tf in blockParent)
        {
            Destroy(tf.gameObject);
        }
    }

    private void CreateBlock()
    {
        BlockEnum[,] blockEnums = dataNode.MapData;
        Vector3 startPos = new Vector3(-12.4f, -6.8f, 0);
        for(int x = 0; x < blockEnums.GetLength(0); x++)
        {
            for(int y = 0; y < blockEnums.GetLength(1); y++)
            {
                if (!blockEnums[x, y].Equals(BlockEnum.None))
                {
                    ShowPazzleNode node = Instantiate(showPazzleNodePrefab, blockParent);
                    node.gameObject.transform.localPosition = startPos + new Vector3(0.8f * x, 0.8f * y, 0);
                    node.Initialize(SelectBlockSprite(blockEnums[x, y]));
                }
            }
        }
    }

    private Sprite SelectBlockSprite(BlockEnum blockEnum)
    {
        switch (blockEnum)
        {
            case BlockEnum.Start:
                return blockImage.Start;
            case BlockEnum.Goal:
                return blockImage.Goal;
            case BlockEnum.NotBox:
                return blockImage.NotBox;
            case BlockEnum.PlainBox:
                return blockImage.PlainBox;
            case BlockEnum.DeliverBox:
                return blockImage.DeliverBox;
            case BlockEnum.DamageBox:
                return blockImage.DamageBox;
            case BlockEnum.JumpBox:
                return blockImage.JumpBox;
            case BlockEnum.WarpBox1:
                return blockImage.WarpBox1;
            case BlockEnum.WarpBox2:
                return blockImage.WarpBox2;
            case BlockEnum.WarpBox3:
                return blockImage.WarpBox3;
            case BlockEnum.WarpBox4:
                return blockImage.WarpBox4;
            default:
                break;
        }
        return null;
    }

    private void DisplayStageInfo()
    {
        nameText.text = dataNode.MapName;
        float timeNum = SaveData.GetStageDataFromStagename(dataNode.ObjectID);
        
        if (timeNum == -1)
        {
            timeText.text = "未クリア";
        }
        else
        {
            timeText.text = Util.ConvertTimeFormat(timeNum);
        }
    }

    //手紙を開く処理
    public IEnumerator Show()
    {
        entireNode.SetActive(true);
        //手紙を開くアニメーション
        topAnimator.Play("OpenTopAnimation");
        bottomAnimator.Play("OpenBottomAnimation");

        yield return new WaitForSeconds(0.2f);

        //カード移動
        var sequence = DOTween.Sequence();
        sequence
            .Append(infoCard.transform.DOLocalMove(new Vector3(0, 1.5f, 0), 0.2f).SetEase(Ease.OutSine))
            .Join(infoCard.transform.DOScale(new Vector3(0.95f,0.95f,1),0.2f).SetEase(Ease.OutSine).SetSpeedBased());

        yield return new WaitForSeconds(0.2f);
    }

    //手紙を閉じる処理
    public IEnumerator Fold()
    {
        //カード移動
        var sequence = DOTween.Sequence();
        sequence
            .Append(infoCard.transform.DOLocalMove(new Vector3(0,-4.7f, 0), 0.2f).SetEase(Ease.OutSine))
            .Join(infoCard.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 0.2f).SetEase(Ease.OutSine).SetSpeedBased());

        yield return new WaitForSeconds(0.2f);

        //手紙を閉じるアニメーション
        topAnimator.Play("CloseTopAnimation");
        bottomAnimator.Play("CloseBottomAnimation");

        yield return new WaitForSeconds(0.25f);

        entireNode.SetActive(false);
    }
}
