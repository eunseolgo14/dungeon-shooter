using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Monstate
{
    Idle,
    Trace,
    Attack,
    Die
}



public class EnemyManager : MonoBehaviour
{
    GameObject m_RefHero = null;

    #region 첫번째 필드 몬스터 관련 변수

    //----- IdleUpdate 이동 관련 변수
    float m_WaitTime = 0.2f;
    float m_TargetDist = 0.0f;
    float m_AttackDist = 3.0f;

    //----- TraceUpdate 이동 관련 변수
    float m_MinX = 0.0f;
    float m_MinY = 0.0f;
    float m_MaxX = 0.0f;
    float m_MaxY = 0.0f;

    Vector3 m_OriginPos = Vector3.zero;

    public float speed = 7.0f;
    public int idx = 0;
    public Monstate m_State = Monstate.Idle;

    //--- 공격 업데이트용 변수 
    public GameObject m_BulletPrefab = null;
    public Transform m_ShootPos = null;

    //float m_ShootCool = 0.01f;
    float m_ShootCool = 0.3f;
    float m_Reach = 10.0f;
    BulletManager m_EBullerMgr = null;
    float m_Speed = 15.0f;

    //체력 표시 관련
    public float m_MaxHp = 100.0f;
    public float m_CurHp = 100.0f;
    public Image m_HpFillImg;

    //사망 보상 아이템드롭 과련 변수
    public GameObject m_DropObj;
    public Sprite[] m_IconImgs;
    Vector3 m_DropPos;

    // 폭탄 투하 관련 변수
    public List<Transform> a_SpawnPosList = new List<Transform>();
    public Transform m_BombDropPos;
    public GameObject m_BombPrefab = null;
    float m_BombCool = 5.0f;

    //--- 애니메이션 관련
    Animator m_MonAnim = null;
    SpriteRenderer m_MonSprite = null;
    float m_FaceRightX = 1.73f;
    float m_FaceLeftX = -1.73f;
    //flip x true시 shoot pos의 위치
    //원래 Vector3(-1.73933065,-0.117148407,0.00635284372)

    #endregion

    //--- 시작 후 잠시 대기
    float m_StartWait = 0.0f;


    //--- 미사일 스킬 사용시 추적 당하고 있는지를 담을 변수
    [HideInInspector]public GameObject m_HomingMissile = null;
    private void Start()
    {
        m_StartWait = 3.0f;

        m_OriginPos = this.transform.position;
        m_RefHero = GameObject.FindGameObjectWithTag("Hero");
        m_MonAnim = GetComponentInChildren<Animator>();
        m_MonSprite = GetComponentInChildren<SpriteRenderer>();


        m_WaitTime = Random.Range(0.7f, 3.0f);
        //m_TraceDist = 15.0f;
        m_AttackDist = 6.0f;


        //몬스터에게 부여된 번호에 따라 자신의 활동 영역 나누기
        int a_Idx = -1;
        string a_Num = this.gameObject.name.Split('_')[1];
        int.TryParse(a_Num, out a_Idx);

        switch (a_Idx)
        {
            case 1:
                m_MinX = -18.0f;
                m_MinY = 2.0f;
                m_MaxX = -1.0f;
                m_MaxY = 11.0f;
                break;

            case 2:
                m_MinX = -1.0f;
                m_MinY = 2.0f;
                m_MaxX = 17.0f;
                m_MaxY = 11.0f;
                break;

            case 3:
                m_MinX = -18.0f;
                m_MinY = -7.0f;
                m_MaxX = -1.0f;
                m_MaxY = 2.0f;
                break;

            case 4:
                m_MinX = -1.0f;
                m_MinY = -7.0f;
                m_MaxX = 17.0f;
                m_MaxY = 2.0f;
                break;
        }
    }
    private void Update()
    {
        //스테이지 진입 후 3초간 대기
        if (0.0f <= m_StartWait)
        {
            m_StartWait -= Time.deltaTime;
            return;
        }
        if (m_RefHero.GetComponent<HeroManager>().m_IsDie == true)
        {
            return;
        }

        if (m_State == Monstate.Die)
        {
            return;
        }
        switch (m_State)
        {
            case Monstate.Idle:

                IdleUpdate();
                BombUpdate();


                break;

            case Monstate.Trace:

                TraceUpdate();

                break;

            case Monstate.Attack:

                AttackUpdadte();
                break;

            case Monstate.Die:



                break;

            default:
                break;
        }

    }

    #region 첫번째 필드 몬스터 고유 특징 함수 (폭탄 투하)

    public GameObject m_PosGroup;
    void BombUpdate()
    {

        if (0.0f < m_BombCool)
        {
            m_BombCool -= Time.deltaTime;
            return;
        }


        FindEmpty();

    }
    void FindEmpty()
    {
        int a_Random = -1;
        //씬에 있는 폭탄 찾아 배열에 저장

        GameObject[] a_Bombs = GameObject.FindGameObjectsWithTag("Bomb");

        if (1 <= a_Bombs.Length) //현재 씬에 존재하는 폭탄의 갯수가 5개 이상이면 더 발사 금지
        {
            return;
        }

        if (a_Bombs.Length == 0) //첫 생성 폭탄임
        {
            a_Random = Random.Range(0, a_SpawnPosList.Count);
        }
        else //두번째 이후의 생성 폭탄임
        {
            for (int i = 0; i < 1; i++)
            {
                a_Random = Random.Range(0, a_SpawnPosList.Count);

                //print("난수를 생성합니다" + a_Random);

                for (int ii = 0; ii < a_Bombs.Length; ii++)
                {
                    if (a_SpawnPosList[a_Random].position == a_Bombs[ii].transform.position)
                    {
                        //print(a_Random + " :해당 위치에 이미 폭탄 존재합니다. 다시 합니다");
                        i--;
                    }
                }
            }
        }
        
       

        //print("중복 없이 걸러진 수: " + a_Random);


        //정상적 랜덤값이 들어았다면 랜덤 좌표 넘겨줌
        if (a_Random != -1)
        {
            GameObject a_Bomb = Instantiate(m_BombPrefab, this.transform.position, Quaternion.identity) as GameObject;
            a_Bomb.GetComponent<BombManager>().m_Target = a_SpawnPosList[a_Random];
        }
        else
        {
            return;
        }

        m_BombCool = 5.0f;
    }
   
    #endregion


    void IdleUpdate()
    {
        //해당 몬스터의 수비 범위 안에 있으면
        if (m_MinX <= m_RefHero.transform.position.x && m_RefHero.transform.position.x < m_MaxX)
            if (m_MinY <= m_RefHero.transform.position.y && m_RefHero.transform.position.y < m_MaxY)
            {
                m_State = Monstate.Trace;
                m_MonAnim.SetTrigger("IsWalk");
                return;
            }

        if (0.0f < m_WaitTime)
        {
            //쿨타임 차감
            m_WaitTime -= Time.deltaTime;

            if (m_WaitTime <= 0.0f)
            {
                if (m_MonSprite.flipX == false)
                {
                    m_MonSprite.flipX = true;
                    m_ShootPos.transform.localPosition = new Vector3(m_FaceRightX, 0.0f, 0.0f);
                }
                else
                {
                    m_MonSprite.flipX = false;
                    m_ShootPos.transform.localPosition = new Vector3(m_FaceLeftX, 0.0f, 0.0f);
                }

                m_WaitTime = Random.Range(0.7f, 3.0f);
            }
        }
    }


    void TraceUpdate()
    {
        m_TargetDist = Vector3.Distance(m_RefHero.transform.position, this.transform.position);

        //해당 몬스터의 수비 범위 안에 있다가 밖으로 나가면
        if (m_MinX >= m_RefHero.transform.position.x || m_RefHero.transform.position.x > m_MaxX ||
            m_MinY >= m_RefHero.transform.position.y || m_RefHero.transform.position.y > m_MaxY)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_OriginPos, speed * Time.deltaTime);
            Vector3 a_MvDir = m_OriginPos - transform.position;

            FaceTarget(a_MvDir.normalized);

            if (Vector3.Distance(transform.position, m_OriginPos) <= 0.5f)
            {
                transform.position = m_OriginPos;
                m_State = Monstate.Idle;
                m_MonAnim.SetTrigger("IsIdle");
            }
            //transform.(m_OriginPos * Time.deltaTime * 10.0f);
            return;
        }

        //공격거리 안이면 공격상태로 변경, 공격을 시작하기
        if (m_TargetDist < m_AttackDist)
        {
            m_State = Monstate.Attack;
            m_MonAnim.SetTrigger("IsAttack");
            return;
        }

        //추적에 앞서추적 경로 탐색
        PathFinding.inst.OriginObj = this.transform.position;
        PathFinding.inst.TargetObj = m_RefHero.transform.position;

        PathFinding.inst.PathFindingFunc();

        Vector3 a_TargetPos = Vector3.zero;

        if ((PathFinding.inst.FinalNodeList == null || PathFinding.inst.FinalNodeList.Count == 0))
        {
            return;
        }


        if (idx <= PathFinding.inst.FinalNodeList.Count - 1)
        {
            a_TargetPos = new Vector3(PathFinding.inst.FinalNodeList[idx].x, PathFinding.inst.FinalNodeList[idx].y, 0.0f);

            Vector3 a_MoveDir = a_TargetPos - this.transform.position;

            FaceTarget(a_MoveDir.normalized);


            transform.position = Vector3.MoveTowards(transform.position, a_TargetPos, speed * Time.deltaTime);

        }


        if ((a_TargetPos.x - 0.2f < transform.position.x && transform.position.x < a_TargetPos.x + 0.2f) &&
            (a_TargetPos.y - 0.2f < transform.position.y && transform.position.y < a_TargetPos.y + 0.2f))
        {
            transform.position = a_TargetPos;
            idx++;

            return;
        }


    }


    void AttackUpdadte()
    {
        //플레이어가 공격거리 밖으로 나감 => 추적상태로 전환
        m_TargetDist = Vector3.Distance(m_RefHero.transform.position, this.transform.position);

        if (m_AttackDist < m_TargetDist)
        {

            m_State = Monstate.Trace;
            m_MonAnim.SetTrigger("IsWalk");
            return;
        }

        if (0 < m_ShootCool)
        {
            m_ShootCool -= Time.deltaTime;

            if (m_ShootCool <= 0.0f)
            {
                GameObject a_Bullet = Instantiate(m_BulletPrefab) as GameObject;
                a_Bullet.transform.position = m_ShootPos.position;

                Vector3 a_TargetDir = m_RefHero.transform.position - this.m_ShootPos.position;

                m_EBullerMgr = a_Bullet.GetComponent<BulletManager>();
                a_TargetDir.Normalize();

                FaceTarget(a_TargetDir);



                m_EBullerMgr.m_FlyDir = a_TargetDir;
                m_EBullerMgr.m_BulletType = BulletType.EnemyBullet;
                m_EBullerMgr.m_StartPos = a_Bullet.transform.position;
                m_EBullerMgr.m_Reach = this.m_Reach;
                m_EBullerMgr.m_Speed = this.m_Speed;
                m_EBullerMgr.FlipBullet();

                m_ShootCool = 0.3f;
            }
        }
    }

    public void TakeDamage(float a_Value)
    {
        if (m_State == Monstate.Die)
        {
            return;
        }

        m_MonAnim.SetTrigger("IsHit");
        m_CurHp -= a_Value;
        UI_Manager a_UIMgr = GameObject.FindObjectOfType<UI_Manager>();
        a_UIMgr.RefreshProgressBar();

        if (m_CurHp <= 0)
        {
            MonsterDie();
            DropCoin();
        }

        if (m_HpFillImg != null)
        {
            m_HpFillImg.fillAmount = m_CurHp / m_MaxHp;
        }
    }

    public void MonsterDie()
    {
        m_HpFillImg.fillAmount = 0;
        m_MonAnim.SetTrigger("IsDead");
        m_DropPos = this.transform.position;
        DropRandomItem();
        m_State = Monstate.Die;

        //킬카운트 증가, 킬카운트 4가 되면 스케이지 클리어 함수 호출
        HeroManager a_HeroMgr = m_RefHero.GetComponent<HeroManager>();
        a_HeroMgr.m_KillCount++;
        if (a_HeroMgr.m_KillCount == 4)
        {
            a_HeroMgr.Clear1stStage();
        }

        Destroy(gameObject, 0.6f);
    }

    public Transform m_Canvas = null;
    public GameObject m_CoinUpObj = null;
    Vector3 m_StartPos = Vector3.zero;
    
    GameObject m_CoinUpClone = null;
    CoinUpTxtManager m_CoinMgr = null;
    void DropCoin()
    {
        if (m_Canvas == null || m_CoinUpObj == null)
        {
            return;
        }

        GlobalValue.g_UserGold += 100;
        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

        GameObject.FindObjectOfType<UI_Manager>().RefreshAcquisition();

        m_CoinUpClone = GameObject.Instantiate(m_CoinUpObj);
        m_CoinUpClone.transform.SetParent(m_Canvas);

        m_CoinMgr = m_CoinUpClone.GetComponent<CoinUpTxtManager>();
        if (m_CoinMgr != null)
        {
            m_CoinMgr.InitCoinTxt(100.0f, Color.red);
        }
        m_StartPos = new Vector3(1467.0f, 467.0f, 0.0f);
        m_CoinUpClone.transform.position = m_StartPos;


    }

    void DropRandomItem()
    {
        if (m_State == Monstate.Die)
        {
            return;
        }

        int a_Rand = Random.Range(0, 3); //0 ~ 2
        if (m_DropObj != null)
        {
            GameObject m_DropItem = Instantiate(m_DropObj) as GameObject;
            m_DropItem.transform.position = m_DropPos;

            m_DropItem.GetComponent<SpriteRenderer>().sprite = m_IconImgs[a_Rand];

            switch (a_Rand)
            {
                case 0: //<--- blue gem
                    m_DropItem.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
                    m_DropItem.gameObject.tag = "BlueGem";
                    break;

                case 1: //<--- green gem
                    m_DropItem.transform.localScale = new Vector3(4.5f, 4.5f, 4.5f);
                    m_DropItem.gameObject.tag = "GreenGem";
                    break;

                case 2: //<--- red gem
                    m_DropItem.transform.localScale = new Vector3(5.2f, 5.2f, 5.2f);
                    m_DropItem.gameObject.tag = "RedGem";
                    break;

                default:
                    break;
            }

        }
    }

    public void FaceTarget(Vector3 a_Dir)
    {
        //대상이 몬스터의 오른쪽에 있음 => 좌우 반전 필요
        if (0.0f < a_Dir.x)
        {
            if (m_MonSprite.flipX == false)
            {
                m_MonSprite.flipX = true;
                m_ShootPos.transform.localPosition = new Vector3(m_FaceRightX, 0.0f, 0.0f);
                
            }
            else
            {
                //이미 반전 되어 있음
                return;
            }
                
        }
        //대상이 몬스터의 왼쪽에 있음 => 좌우 반전 불필요
        else
        {
            if (m_MonSprite.flipX == false)
            {
                return;
            }

            else
            {
                m_MonSprite.flipX = false;
                m_ShootPos.transform.localPosition = new Vector3(m_FaceLeftX, 0.0f, 0.0f);
                
            }
            
        }

       
        //플레이어 추적
        //플레이어 향해 총알 발사
        //타겟 사라진 후 제자리로 돌아갈 떄
    }
    
}
