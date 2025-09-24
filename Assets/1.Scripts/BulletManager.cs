using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{ 
    HeroBullet,
    EnemyBullet,
    BossBullet
}
public class BulletManager : MonoBehaviour
{
    [HideInInspector] public Vector3 m_FlyDir = Vector3.zero;
    public float m_Speed = 20.0f;
    public float m_Reach = 0.0f;
    public Vector3 m_StartPos = Vector3.zero;

    //누가 발사한 총알인지 식별하기위한 타입형
    public  BulletType m_BulletType;

    //총알 이미지 좌우 반전용
    SpriteRenderer m_BulletSprite = null;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.CompareTag("HeroBullet") == true)
        {
            m_BulletType = BulletType.HeroBullet;
        }
        else if (this.gameObject.CompareTag("EnemyBullet") == true)
        {
            m_BulletType = BulletType.EnemyBullet;
        }
        else if (this.gameObject.CompareTag("SpecialBullet") == true)
        {
            m_BulletType = BulletType.HeroBullet;
            m_FlyDir = Vector3.right;
        }
        else
        {
            m_BulletType = BulletType.BossBullet;
        }
        //Destroy(this.gameObject, 5.0f);
    }

    void Update()
    {
        transform.Translate(m_FlyDir * Time.deltaTime * m_Speed);

        float a_Length = Vector3.Distance(transform.position, m_StartPos);
        if (m_Reach < a_Length)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.gameObject.CompareTag("SpecialBullet") == true)
        {
            print("충돌");
        }
        //플레이어가 발사한 총알
        if (m_BulletType == BulletType.HeroBullet)
        {
            //방패에 맞았다면 리턴
            if (collision.gameObject.name.Contains("Shield"))
            {
                //Destroy(gameObject);
                return;
            }
            //플레이어가 필드몹을 명중
            if (collision.tag == "Monster")
            {
                //
                EnemyManager a_EnMgr = collision.GetComponent<EnemyManager>();

                if (a_EnMgr.m_State == Monstate.Die)
                {
                    return;
                }
                a_EnMgr.TakeDamage(GlobalValue.g_CurGun.m_Damage);
                Destroy(gameObject);
            }
            //플레이어가 보스몹을 명중
            else if (collision.tag == "Boss")
            {
                BossManager a_BsMgr = collision.GetComponent<BossManager>();
                a_BsMgr.TakeDamage(GlobalValue.g_CurGun.m_Damage);
                Destroy(gameObject);
            }
        }

        //몹이 발사한 총알
        else if (m_BulletType == BulletType.EnemyBullet || m_BulletType == BulletType.BossBullet)
        {
           //방패에 맞았다면 리턴
            if (collision.gameObject.name.Contains("Shield"))
            {
                //Destroy(gameObject);
                return;
            }
            else if (collision.tag == "Hero")
            {
                GameObject a_Shield = GameObject.Find("Shield");

                if (a_Shield == null)
                {
                    if (m_BulletType == BulletType.EnemyBullet)
                    {
                        collision.GetComponent<HeroManager>().TakeDamage(5);
                    }
                    else if (m_BulletType == BulletType.BossBullet)
                    {
                        collision.GetComponent<HeroManager>().TakeDamage(2);
                    }
                }
                Destroy(gameObject);
            }
        }
        Destroy(this.gameObject);
    }

    public void FlipBullet()
    {
        m_BulletSprite = GetComponent<SpriteRenderer>();

        //총알이 날아가야할 방향벡터의 x값이 0보다 클 때
        // => 오른쪽으로 날아가야할 때
        if (0.0f < m_FlyDir.x)
        {
            if (m_BulletSprite != null && m_BulletSprite.flipX == false)
            {
                m_BulletSprite.flipX = true;
            }
        }
        else
        {
            return;
        }
    }
   
}
