using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;//移動速度
    [SerializeField] private float jumpSpeed;//ジャンプ高度
    [SerializeField] private float highJumpSpeed;//ジャンプ台に乗った時のジャンプ高度
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _transform;
    [SerializeField] private GroundCheck _groundCheck;//接地判定
    [SerializeField] private HangCollider _hangCollider;//自身の目の前のBoxを調べる
    [SerializeField] private SpriteRenderer _hangBoxSprite;//持っているハコの画像
    [SerializeField] private SpriteRenderer _sprite;//Spriterenderer
    [SerializeField] private Sprite WaitSprite;
    [SerializeField] private Sprite JumpSprite1;
    [SerializeField] private Sprite JumpSprite2;

    private bool canOperate;//キー入力を受け付けるかどうか
    private float beforeYSpeed = 0;//1フレーム前のY軸速度
    private bool isHang;//ハコをつかんでいるかどうか
    private BlockEnum hangBlockEnum;//掴んでいるハコの種類
    private int isLeftThen1;//ゲームクリア時の演出に使う変数
    private bool isClearAnimation;//クリア時のアニメーションを行なっているかどうか

    //ゲッター/セッター
    public bool CanOperate
    {
        get { return canOperate; }
        set { this.canOperate = value; }
    }
    public bool IsHang
    {
        get{ return this.isHang;}
        set{
            this.isHang = value;
            _hangCollider.isHang = value;
        }

    }
    public BlockEnum HangBlockEnum
    {
        get { return this.hangBlockEnum;}
        set { this.hangBlockEnum = value; }
    }
    public void SetHangBoxSprite(Sprite sprite)
    {
        _hangBoxSprite.sprite = sprite;
    }
    public void SetVelocity(Vector2 speed)
    {
        rb.velocity = speed;
    }
    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }
    public int IsLeftThen1()
    {
        return isLeftThen1;
    }


    private void Start()
    {
        Initialize();
    }

    //初期化処理
    public void Initialize()
    {
        canOperate = true;
    }

    private void Update()
    {
        //ポーズ画面ではない場合
        if (!GameManager.instance.CanAcceptInput) return;
        if (canOperate)
        {
            Move();
            Hang();
        }

        Animation();
        YSpeedDeal();
    }

    //移動処理
    private void Move()
    {
        float vertical = Input.GetAxis("Horizontal");
        Vector3 speedVector = rb.velocity;
        if (vertical > 0)
        {
            speedVector = new Vector3(speed, rb.velocity.y, 0);
        }
        else if (vertical < 0)
        {
            speedVector = new Vector3(-speed, rb.velocity.y, 0);
        }
        else
        {
            speedVector = new Vector3(0, rb.velocity.y, 0);
        }
        if (_groundCheck.IsGround())
        {
            if (Input.GetButtonDown("Jump"))
            {
                SoundManager.PlaySE(SE_Enum.JUMP);
                speedVector = new Vector3(speedVector.x, jumpSpeed, 0);
            }
        }
        rb.velocity = speedVector;
    }

    //掴む処理
    private void Hang()
    {
        Box box = _hangCollider.collisionBox;
        if (isHang)
        {
            //左右どちらを向いているかの判定
            bool isRight = false;
            if (_transform.localScale.x == 1)
            {
                isRight = true;
            }
            //ハコを置く
            if (_groundCheck.IsGround() && BlockManager.CanPutBox(_transform.localPosition,isRight))
            {
                if (Input.GetButtonDown("Hang"))
                {
                    SoundManager.PlaySE(SE_Enum.GRAB);
                    BlockManager.PutBox(this, isRight);
                }
            }
            else
            {
                BlockManager.DissapearMarks();
            }
        }
        else
        {
            //ハコを持ち上げる
            if (box!=null&&Input.GetButtonDown("Hang"))
            {
                SoundManager.PlaySE(SE_Enum.GRAB2);
                BlockManager.GrabBox(this, box);
            }
        }
    }

    //画像切り替え
    private void Animation()
    {
        if (isClearAnimation) return;
        if (rb.velocity.x != 0)
        {
            _animator.SetBool("IsWalk", true);
            if (rb.velocity.x > 0)
            {
                _transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                _transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            _animator.SetBool("IsWalk", false);
        }

        _animator.SetBool("IsBox", isHang);
        _animator.SetFloat("ySpeed", rb.velocity.y);
        _animator.SetBool("IsAir", !_groundCheck.IsGround());
    }

    //Yスピードの処理(なんか引っ掛かったら上に余計にジャンプしないようにする処理)
    private void YSpeedDeal()
    {
        if (!_groundCheck.IsGround())
        {
            if (rb.velocity.y != jumpSpeed)
            {
                if (rb.velocity.y > beforeYSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, beforeYSpeed - Time.deltaTime * 9.8f);
                }
            }
        }
        beforeYSpeed = rb.velocity.y;
    }

    //シミュレートするかどうか(ポーズ画面での停止に使う)
    public void SetIsSimulate(bool isSimulate)
    {
        rb.simulated = isSimulate;
    }

    //任意のアニメーションを呼び出し
    public void PlayAnimation(string animationName)
    {
        _animator.Play(animationName);
    }

    //ゲームオーバー時のアニメーション
    public void DamageAnimation(bool isRight)
    {
        PlayAnimation("DamageAnimation");
        GetComponent<CapsuleCollider2D>().enabled = false;
        Vector2 velocity = new Vector2();
        if (isRight)
        {
            velocity.x = speed*1.5f;
        }
        else
        {
            velocity.x = -speed*1.5f;
        }
        velocity.y = jumpSpeed * 1.7f;
        rb.velocity = velocity;
        rb.gravityScale = 3.0f;
        SetHangBoxSprite(null);
        canOperate = false;
    }

    //ジャンプ台に乗った時のジャンプ
    public void HighJump()
    {
        SoundManager.PlaySE(SE_Enum.HIGHJUMP);
        rb.velocity = new Vector3(rb.velocity.x, highJumpSpeed, 0);
    }

    //ワープ処理
    public void WarpDeal(WarpBox fromBox,WarpBox toBox)
    {
        GetComponent<SpriteRenderer>().sortingLayerName = "WarpPlayer";
        _hangBoxSprite.sortingLayerName = "WarpPlayer";
        SetVelocity(new Vector2(0, 0));
        GetComponent<CapsuleCollider2D>().enabled = false;
        rb.simulated = false;
        SoundManager.PlaySE(SE_Enum.WARP);
        canOperate = false;
        _transform.DOLocalMove(fromBox.transform.localPosition, 0.4f).SetLink(gameObject).OnComplete(
            () =>
            {
                _transform.localPosition = toBox.transform.localPosition;
                _transform.DOLocalMove(toBox.transform.localPosition + new Vector3(0, 1, 0), 0.4f).SetDelay(0.1f).SetLink(gameObject).OnComplete(
                    () =>
                    {
                        GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                        _hangBoxSprite.sortingLayerName = "Player";
                        canOperate = true;
                        GetComponent<CapsuleCollider2D>().enabled = true;
                        rb.simulated = true;
                    });
            });
    }

    //クリアアニメーション
    public void ClearDeal(Vector3 goalPos,GameObject submitString)
    {
        rb.simulated = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        CanOperate = false;
        isLeftThen1 = 1;
        isClearAnimation = true;
        _animator.enabled = false;
        if (_transform.localPosition.x < goalPos.x)
        {
            //もし右なら-1
            isLeftThen1 = -1;
        }
        _transform.localScale = new Vector3(-isLeftThen1, 1, 1);
        _sprite.sprite = JumpSprite1;
        //プレイヤーがゴールより左
        _transform.DOLocalMove(new Vector3(goalPos.x + 0.5f*isLeftThen1, goalPos.y + 1.27f, 0), 0.2f).SetLink(gameObject).OnComplete(
            () =>
            {
                _sprite.sprite = JumpSprite2;
                _transform.DOLocalMove(new Vector3(goalPos.x + 0.24f*isLeftThen1, goalPos.y + 0.9f, 0), 0.2f).SetLink(gameObject).OnComplete(
                    () =>
                    {
                        _sprite.sprite = WaitSprite;
                    });
            });
    }
}
