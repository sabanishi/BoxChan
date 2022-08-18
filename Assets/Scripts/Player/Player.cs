using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private float speed;//移動速度
    [SerializeField] private float jumpSpeed;//ジャンプ高度
    [SerializeField]private Animator _animator;
    [SerializeField]private Transform _transform;
    [SerializeField]private GroundCheck _groundCheck;//接地判定
    [SerializeField]private HangCollider _hangCollider;//自身の目の前のBoxを調べる
    [SerializeField]private SpriteRenderer _hangBoxSprite;//持っているハコの画像

    private bool canOperate;
    public bool CanOperate
    {
        get { return canOperate; }
        set { this.canOperate = value; }
    }

    private float beforeYSpeed = 0;
    private bool isHang;//ハコをつかんでいるかどうか
    private BlockEnum hangBlockEnum;//掴んでいるハコの種類
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

    //speedの変更
    public void SetVelocity(Vector2 speed)
    {
        rb.velocity = speed;
    }
    //speedの取得
    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        canOperate = true;
    }

    private void Update()
    {
        if (GameManager.instance.IsTimeStop) return;
        if (canOperate)
        {
            Move();
            Hang();
        }
        
        Animation();
        YSpeedDeal();
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
            if (BlockManager.CanPutBox(_transform.localPosition,isRight))
            {
                if (Input.GetButtonDown("Hang"))
                {
                    BlockManager.PutBox(this, isRight);
                }
            }
        }
        else
        {
            //ハコを持ち上げる
            if (box!=null&&Input.GetButtonDown("Hang"))
            {
                BlockManager.GrabBox(this, box);
            }
        }
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
                speedVector = new Vector3(speedVector.x,jumpSpeed, 0);
            }
        }
        rb.velocity = speedVector;
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

    //画像切り替え
    private void Animation()
    {
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
        _animator.SetFloat("ySpeed",rb.velocity.y);
        _animator.SetBool("IsAir", !_groundCheck.IsGround());
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
    }
}
