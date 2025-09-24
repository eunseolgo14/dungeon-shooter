using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Boss_State
{ 
    Idle,
    NormalAtt,
    Summon,
    FeverAtt,
    TakeDamage,
    Die
}
public class BossManager : MonoBehaviour
{
    public Boss_State m_BossState = Boss_State.NormalAtt;

    GameObject m_RefHero = null;

    //--- 시작 후 잠시 대기
    float m_StartWait = 0.0f;

    //--- 일반 공격 업데이트 관련 변수
    public GameObject m_DemoCircle = null;
    Vector3 m_ShootPos = Vector3.zero;
    float m_Distance = 6.0f;
    Vector3 m_TargetDir = Vector3.zero;
    public GameObject m_BossBulletPrefab = null;
    int m_ShootCount = 5;
    float m_ShootCool = 0.2f;
    int m_NormalAttCount = 0;

    //--- 아이들 업데이트 관련 변수
    float m_WaitTime = 0.0f;
    //--- normalcase speed
    float m_ChaseSpeed = 10.0f;

    //--- 소환수 업데이트 관련 변수
    public List<Transform> m_Origin_SpawnPosList = new List<Transform>();
    public List<Transform> m_This_SpawnPosList;
    public GameObject m_WormPrefab = null;
    float m_SummonCool = 0.0f;
    public int m_SummonCount = 3;

    //--- 체력 관련 변수
    float m_CurHP = 2000;
    float m_MaxHP = 2000;
    public Image m_HpFillImg = null;

    //--- 추적 이동 관련 변수
    Vector3 m_OriginPos = Vector3.zero;

    //----- 애니메이션 연출용 변수
    Animator m_Anim;

    // Start is called before the first frame update
    void Start()
    {
        m_StartWait = 3.0f;


        for (int i = 0; i < m_Origin_SpawnPosList.Count; i++)
        {
            m_This_SpawnPosList.Add(m_Origin_SpawnPosList[i]);
        }

        m_SummonCount = 3;
        m_ShootCount = 5;
        //m_ShootPos = this.transform.position;
        m_WaitTime = Random.Range(0.5f, 3.0f);

        m_OriginPos = this.transform.position;

        if (m_RefHero == null)
        {
            m_RefHero = GameObject.FindGameObjectWithTag("Hero");
        }

        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f <= m_StartWait)
        {
            m_StartWait -= Time.deltaTime;
            return;
        }
        if (m_RefHero.GetComponent<HeroManager>().m_IsDie == true)
        {
            return;
        }
        switch (m_BossState)
        {
            case Boss_State.Idle:
                IdleUpdate();
                break;
            case Boss_State.NormalAtt:
                NormalAttUpdate();
                break;
            case Boss_State.Summon:
                SummonUpdate();
                break;
            case Boss_State.FeverAtt:
                break;
            case Boss_State.TakeDamage:
                break;
            case Boss_State.Die:
                break;
            default:
                break;
        }

        if (0.0f < m_HeadsUpTime)
        {
            m_HeadsUpTime -= Time.deltaTime;

            if (m_HeadsUpTime <= 0.1f)
            {
                for (int i = 0; i < m_HeadsUpMarks.Length; i++)
                {
                    m_HeadsUpMarks[i].SetActive(false);
                }
            }
            if (m_HeadsUpTime <= 0.0f)
            {
                SummonWorm(m_HeadsUpMarks);
            }
        }
        
    }

    //랜덤 시간동안 대기
    void IdleUpdate()
    {
        //
        if (m_WaitTime <= 0.0f)
        {
            return;
        }

        m_WaitTime -= Time.deltaTime;
        NormalBack();

        if (m_WaitTime <= 0.0f)
        {
            this.transform.position = m_OriginPos;

            if (5 <= m_NormalAttCount)
            {
                m_NormalAttCount = 0;
                m_BossState = Boss_State.Summon;
            }
            else
            {
                m_BossState = Boss_State.NormalAtt;
            }

            m_WaitTime = Random.Range(0.5f, 3.0f);
           
        }
    }

    //일반 공격 업데이트 함수
    void NormalAttUpdate()
    {
        if (0.0f < m_ShootCool)
        {
            m_ShootCool -= Time.deltaTime;

            if (m_ShootCool <= 0.0f)
            {
                NormalFire();
                NormalChase();
            }
        }

    }
    void NormalFire()
    {
        //플레이어를 향하는 방향벡터 구하기
        m_TargetDir = m_RefHero.transform.position - this.transform.position;
        m_TargetDir.Normalize();

        //총알을 발사할 스폰 포스 생성하기
        m_ShootPos = this.transform.position + (m_TargetDir * m_Distance);
        m_DemoCircle.transform.position = m_ShootPos ;

        if (m_BossBulletPrefab != null)
        {
            //잡아둔 스폰 포스에 총알 생성
            GameObject a_Bullet = Instantiate(m_BossBulletPrefab) as GameObject;
            a_Bullet.transform.position = m_ShootPos;

            //플레이어를 향해 몸채 방향 수정
            float a_CacAngle = Mathf.Atan2(m_TargetDir.y, m_TargetDir.x) * Mathf.Rad2Deg;
            a_Bullet.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CacAngle);

            BulletManager a_BulletMgr = a_Bullet.GetComponent<BulletManager>();

            a_BulletMgr.m_FlyDir = Vector3.right; //(this.transform.right하니까 이상한 방향임)
            a_BulletMgr.m_BulletType = BulletType.EnemyBullet;
            a_BulletMgr.m_StartPos = a_Bullet.transform.position;
            a_BulletMgr.m_Reach = 20.0f;
            a_BulletMgr.m_Speed = 20.0f;

            //쿨타임 재충전
            m_ShootCool = 0.2f;
            //발사 카운트 차감
            m_ShootCount--;

            if (m_ShootCount <= 0)
            {
                m_ShootCount = 5;
                m_NormalAttCount++;
                m_BossState = Boss_State.Idle;
            }

        }
    }
    void NormalChase()
    {
        //m_TargetDir = m_RefHero.transform.position - this.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, m_RefHero.transform.position,
            m_ChaseSpeed * Time.deltaTime);
    }

    void NormalBack()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_OriginPos , 50.0f * Time.deltaTime);
    }
    //궁극기 공격 업데이트 함수
    void FeverAttUpdate()
    {

    }

    //소환수 업데이트 함수
    void SummonUpdate()
    {

        if (0.0f <= m_SummonCool)
        {
            m_SummonCool -= Time.deltaTime;

            if (m_SummonCool <= 0.0f)
            {
                if (m_SummonCount <= 0)
                {
                    m_SummonCount = 3;//재충전
                    m_SummonCool = 3.0f;
                    m_BossState = Boss_State.Idle;
                }
                else
                {
                    //m_HeadsUpTime = 0.5f;
                    //SummonWorm();
                    HeadsUp();
                }
            }
        }

        
    }

    public  GameObject m_MarkerPrefab = null;
    float m_HeadsUpTime = 0.0f;
    GameObject[] m_HeadsUpMarks = null;
    void HeadsUp()
    {
        int a_Rand = 0;
        GameObject a_Marker = null;

        for (int i = 0; i < 10; i++)
        {
            a_Rand = Random.Range(0, m_This_SpawnPosList.Count);

            a_Marker = Instantiate(m_MarkerPrefab) as GameObject;

            //랜덤 스폰 좌표 넘겨줌
            a_Marker.transform.position = m_This_SpawnPosList[a_Rand].position;
            m_This_SpawnPosList.RemoveAt(a_Rand);
        }

        m_HeadsUpMarks = GameObject.FindGameObjectsWithTag("HeadsUpMark");
        m_HeadsUpTime = 0.5f;
        //m_SummonCount--;
    }


    void SummonWorm(GameObject[] a_PosList)
    {
        if (a_PosList == null)
        {
            return;
        }

        GameObject a_Worm = null;

        for (int i = 0; i < a_PosList.Length; i++)
        {
            a_Worm = Instantiate(m_WormPrefab) as GameObject;
            a_Worm.transform.position = a_PosList[i].transform.position;
            Destroy(a_PosList[i].gameObject);
        }

        m_SummonCool = 3.0f;
        m_SummonCount--;

        m_This_SpawnPosList.Clear();
        //사용한 위치 리스트 복귀
        for (int i = 0; i < m_Origin_SpawnPosList.Count; i++)
        {
            m_This_SpawnPosList.Add(m_Origin_SpawnPosList[i]);
        }


    }

    //피격 함수
    public void TakeDamage(float a_Damage)
    {
        if (0 < m_CurHP)
        {
            m_Anim.SetTrigger("isHit");
            m_CurHP -= a_Damage;
            m_HpFillImg.fillAmount = m_CurHP / m_MaxHP;

            if (m_CurHP <= 0)
            {
                //사망처리
                Die();
            }


        }
    }

    void Die()
    {
        m_Anim.SetTrigger("isDead");
    }
    //애니메이션에 심을 이벤트 용 함수
    public void Die_Disappear()
    {
        Destroy(gameObject);



        //게임 완료 씬으로 이동
        SceneManager.LoadScene(8);
    }
}
