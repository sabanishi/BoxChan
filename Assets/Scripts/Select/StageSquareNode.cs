using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSquareNode : MonoBehaviour
{
    [SerializeField] private SpriteRenderer squareSprite;
    [SerializeField] private Sprite normallySprite;
    [SerializeField] private Sprite isCursleSprite;
    [SerializeField] private int StageNumber;
    [SerializeField] private StageSelectManager parent;
    [SerializeField] private PolygonCollider2D pCollider;

    private bool isEnter;
    private Transform _transform;
    private bool isValid;

    private void Start()
    {
        _transform = transform;
    }

    public void Initialize()
    {
        _transform = transform;
        squareSprite.sprite = normallySprite;
        _transform.localScale = new Vector3(1, 1, 1);
    }

    public void Update()
    {
        if (isEnter&&parent.IsValidStageNum())
        {
            squareSprite.sprite = isCursleSprite;
            _transform.localScale = new Vector3(1.2f, 1.2f, 1);
            parent.DisplayStageInfo(StageNumber);
        }
        if (!parent.IsValidStageNum()&&isValid)
        {
            isValid = false;
            pCollider.enabled = false;
        }
        if (parent.IsValidStageNum() && !isValid)
        {
            isValid = true;
            pCollider.enabled = true;
        }
    }

    public void OnMouseEnter()
    {
        if (parent.IsValidStageNum())
        {
            squareSprite.sprite = isCursleSprite;
            _transform.localScale = new Vector3(1.2f, 1.2f, 1);
            parent.DisplayStageInfo(StageNumber);
        }
        else
        {
            isEnter = true;
        }
    }

    public void OnMouseExit()
    {
        if (parent.IsValidStageNum())
        {
            squareSprite.sprite = normallySprite;
            _transform.localScale = new Vector3(1,1, 1);
            parent.DissapearStageInfo(StageNumber);
        }
        else
        {
            isEnter = false;
        }
    }

    public void OnMouseClick()
    {
        if (!parent.IsValidStageNum()) return;
        parent.ClickNumber(StageNumber);
    }
}
