using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HeroManager : MonoBehaviour
{
    //--- 키보드 입력을 통한 플레이어 이동을 위한 변수
    float m_WalkForce = 5.0f;
    float m_MaxSpeed = 15.0f;
    Rigidbody2D m_Rig;

    //--- 총알 발사를 위한 변수
    public GameObject m_BulletPrefab = null;
    public Transform m_ShootPos;
    public static int m_BulletCount = 0; //m_MagazineSize = 12; 
    float m_AttSpeed = 0.1f;        //<--- 공격 속도
    float m_CacAtTIck = 0.0f;       //<--- 기관총 방사 틱 만들기
    float m_ShootRange = 0.0f;     //<--- 사거리 //m_Reach


    //--- 발사 클릭 지점을 향해 팔 자연스럽게 이동
    public Transform m_ArmPivot = null;

    //--- 총알 재장전 허드를 위한 변수

    //--- 애니메이션 변수 설정
    Animator m_HeroAnim;
    public GameObject m_GunObj;
    Animator m_GunAnim;
    public GameObject m_ExploObj;
    Animator m_ExploAnim;

    //public Button m_ReloadHelpTxtObj;

    //--- 피격 후 데미지 연출을 위한 변수
    public bool m_IsFlashOn = false;
    public float m_FlashDur = 0.0f; //한번 깜빡임 시간
    public float m_PlayDur = 0.0f; //전체 연출 시간
    Color m_StartColor = new Color(255, 255, 255, 255); //기본 색
    Color m_RedColor = new Color(255, 0, 0, 255); //적색
    public SpriteRenderer m_HeroSpr = null;

    //--- 주인공 UI관련 변수
    public Image m_HeroHpBarImg;
    public Image m_HeroCurGunImg;

    //--- 주인공 총기 교체 관련 이미지
    public SpriteRenderer[] m_Hands = null;
    public Sprite[] m_HeroHands = null;

    public Sprite m_NormalGunSpr = null;
    public Sprite m_SpecialGunSpr = null;


    public Image m_TargetGunUIImg = null;


    //--- 주인공 킬카운트 관련 변수
    public int m_KillCount = 0;
    float m_DieTime = 0.0f;
    public bool m_IsDie = false;
    float m_StartWait = 3.0f;

    void Start()
    {
        m_DieTime = 0.0f;

        //시작은 언제나 풀피로
        GlobalValue.g_CurrHP = GlobalValue.g_MaxHP;

        //print("히어로 체력:" + GlobalValue.g_CurrHP);
        Gun_Info a_DefaultGun = new Gun_Info();

        if (GlobalValue.g_HeroType == HeroType.FirstHero)//햄스터
        {
            a_DefaultGun.SetGunInfo(GunType.Hero1_Defalut);
        }

        GlobalValue.g_MyGunList.Insert(0, a_DefaultGun);
        GlobalValue.g_CurGun = a_DefaultGun;

        //히어로의 총기에 맞게 사거리와 창탄, 데미지 조정
        m_BulletCount = GlobalValue.g_CurGun.m_MagazineSize;
        m_ShootRange = GlobalValue.g_CurGun.m_Reach;

        m_NormalGunSpr = a_DefaultGun.m_GunIconSprite;

        m_Rig = this.GetComponent<Rigidbody2D>();
        m_HeroAnim = this.GetComponent<Animator>();
        //m_BulletCount = 30;

        if (m_GunObj != null)
        {
            m_GunAnim = m_GunObj.GetComponent<Animator>();
        }
        if (m_ExploObj != null)
        {
            m_ExploAnim = m_ExploObj.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_StartWait)
        {
            m_StartWait -= Time.deltaTime;
            return;
        }
        if (0.0f < m_DieTime)
        {
            m_DieTime -= Time.unscaledDeltaTime;

            if (m_DieTime <= 0.0f)
            {
                //사망씬으로 연결
                SceneManager.LoadScene(6);
            }
        }
        if (m_IsDie == true)
        {
            return;
        }

        //총기가 2개 이상이라 c키를 눌러 총기 교체를 하려 하는 경우
        if (Input.GetKeyDown(KeyCode.C) == true && 2 <= GlobalValue.g_MyGunList.Count)
        {
            ChangeGun();
        }

        if (0.0f < m_CacAtTIck)
        {
            //m_CacAtTIck이 충전되어있다면 시간이 흐른만큼 계속 깎아주기
            m_CacAtTIck -= Time.deltaTime;
        }

        if (m_IsFlashOn == true)
        {
            if (0.0f < m_PlayDur && 0.0f < m_FlashDur)
            {
                m_PlayDur -= Time.deltaTime;
                m_FlashDur -= Time.deltaTime;

                if (m_FlashDur <= 0.0f)
                {
                    if (m_HeroSpr.color == m_RedColor)
                    {
                        m_HeroSpr.color = m_StartColor;
                    }
                    else
                    {
                        m_HeroSpr.color = m_RedColor;
                    }

                    m_FlashDur = 0.2f;
                }

                if (m_PlayDur <= 0.0f)
                {
                    m_IsFlashOn = false;
                    m_HeroSpr.color = m_StartColor;
                    m_PlayDur = 2.0f;
                }
            }

        }

        //키보드 입력값에 맞춰 이동
        if (Input.GetKey(KeyCode.LeftArrow) == true || Input.GetKey(KeyCode.RightArrow) == true)
        {
            float h = Input.GetAxis("Horizontal");
            m_Rig.AddForce(Vector3.right * h * m_WalkForce * Time.deltaTime, ForceMode2D.Impulse);

            //최대속력 넘지않게 제한
            if (m_MaxSpeed < Mathf.Abs(m_Rig.velocity.x))
            {
                m_Rig.velocity = new Vector2((m_MaxSpeed * h), m_Rig.velocity.y);
            }

            if (h < 0.0f)
            {
                this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            }
            else
            {
                this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow) == true || Input.GetKey(KeyCode.DownArrow) == true)
        {
            float v = Input.GetAxis("Vertical");
            m_Rig.AddForce(Vector3.up * v * m_WalkForce * Time.deltaTime, ForceMode2D.Impulse);

            //최대속력 넘지않게 제한
            if (m_MaxSpeed < Mathf.Abs(m_Rig.velocity.y))
            {
                m_Rig.velocity = new Vector2(m_Rig.velocity.x, (m_MaxSpeed * v));
            }
        }

        //키보드에서 손 떼자마자 움직이던 것 멈추기
        if (Input.GetKeyUp(KeyCode.LeftArrow) == true || Input.GetKeyUp(KeyCode.RightArrow) == true)
        {
            m_Rig.velocity= new Vector2(0.0f, m_Rig.velocity.y);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) == true || Input.GetKeyUp(KeyCode.DownArrow) == true)
        {
            m_Rig.velocity = new Vector2(m_Rig.velocity.x, 0.0f);
        }

        m_HeroAnim.SetInteger("speed", (int)(m_Rig.velocity.x + m_Rig.velocity.y));


        //--- 연발 총알 발사 코드(마우스클릭)
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            //m_CacAtTIck이 업데이트에서 깎이고 깎이다 0보다 작아지면 발사!
            if (m_CacAtTIck <= 0.0f)
            {
                //잔탄수가 0이 아니면
                if (0 < m_BulletCount)
                {
                    //마우스 우클릭한 지점의 월드좌표를 함수에 보낸다.
                    ShootFire(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                    //m_ExploAnim.SetTrigger("isShoot");
                    //m_CacAtTIck에 다시 0.1초 충전
                    m_CacAtTIck = m_AttSpeed;
                    //잔탄수 차감
                    m_BulletCount--;
                    //차감된 잔탄 출력
                    GameObject.FindObjectOfType<UI_Manager>().RefreshBulletHud();
                }
                else if (m_BulletCount <= 0)
                {
                    GameObject.FindObjectOfType<UI_Manager>().ShowRelaodBar();
                }
            }

            //m_ArmPivot.rotation = Quaternion.Euler(new Vector3(0.0f, this.gameObject.transform.rotation.eulerAngles.y, 0.0f));
        }
        if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            m_ArmPivot.rotation = Quaternion.Euler(new Vector3(0.0f, this.gameObject.transform.rotation.eulerAngles.y, 0.0f));
        }

        
    }

    void ShootFire(Vector3 a_ClickPos) //a_TPos = 마우스의 클릭 지점
    {
        float a_Dist = (transform.position - a_ClickPos).magnitude;
        //클릭한 지점이 주인공 자신과 지나치게가깝다면
        if (a_Dist <= 10.04f)
        {
            return;
        }

        if (m_BulletPrefab != null)
        {
            //1. 오브젝트를 생성한다
            GameObject a_Bullet = Instantiate(m_BulletPrefab) as GameObject;
            BulletManager a_BulletMgr = a_Bullet.GetComponent<BulletManager>();
            a_Bullet.transform.position = m_ShootPos.position;
            m_GunAnim.SetTrigger("isShoot");
            //2. 플레이어의 현재 좌표값을 넣는다
            Vector3 a_CurPos = this.transform.position;
            Vector3 a_ShootDir = a_ClickPos - a_CurPos;
            a_ShootDir.z = 0.0f;

            //방향벡터로 정규화
            a_ShootDir.Normalize();
            a_ShootDir.z = 0.0f;
            RotateArm(a_ShootDir);
            a_BulletMgr.m_FlyDir = a_ShootDir;
            a_BulletMgr.m_BulletType = BulletType.HeroBullet;
            a_BulletMgr.m_StartPos = a_Bullet.transform.position;
            a_BulletMgr.m_Reach = GlobalValue.g_CurGun.m_Reach;

            
            if (GlobalValue.g_CurGun != GlobalValue.g_MyGunList[0])
            {
                //a_BulletMgr.m_FlyDir = Vector3.forward;
                float a_CacAngle = Mathf.Atan2(a_ShootDir.y, a_ShootDir.x) * Mathf.Rad2Deg;
                a_Bullet.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CacAngle);
            }

        }
        else
        {
            print(" 프리팹 연결 끊김");
        }

        SoundManager.Instance.PlayEffSound("GunFire_Short", 1.5f);
    }

    void RotateArm(Vector3 a_Dir)
    {
        //Debug.Log("팔 회전 + " + a_Dir);
        Quaternion a_CacAngle = Quaternion.LookRotation(a_Dir); //매개변수로 받은 클릭 지점향하는 벡터를 보는 쿼터니언값 추출
        Vector3 a_CacVec = a_CacAngle.eulerAngles;
        int a_idx = 0;


        if (m_ArmPivot == null)
        {
            Debug.Log("팔 없음");
            return;
        }

        if (a_Dir.x <= 0) //빙벡의 x가 음수, 왼쪽 클릭
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180.0f, transform.rotation.z));
            a_idx = 180;
        }
        else if(0 < a_Dir.x) //방벡의 x가 양수, 오른 쪽 클릭
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0.0f, transform.rotation.z));
            a_idx = 0;
        }

        m_ArmPivot.rotation = Quaternion.Euler(new Vector3(m_ArmPivot.transform.rotation.x, m_ArmPivot.transform.rotation.y + a_idx, -a_CacVec.x));

    }

  

    public void TakeDamage(int a_Value)
    {
        if (this.GetComponentInChildren<ShieldManager>() != null)
        {
            Debug.Log("방패 착용 중, 데미지 없음");
            return;
        }

        //피격 연출 시작
        m_IsFlashOn = true;
        m_PlayDur = 2.0f;
        m_FlashDur = 0.2f;

        //글로벌 벨루에서 데미지 누적 계산(데미지는 항상 음주만 들어오도록 주의)
        GlobalValue.g_CurrHP -= a_Value;
        
        RefreshHpBar();

        //플레이어 잔존 체력 0아래로 떨어짐
        if (GlobalValue.g_CurrHP <= 0)
        {
            m_IsDie = true;
            m_HeroAnim.SetTrigger("IsDie");
            m_ArmPivot.gameObject.SetActive(false);
            m_DieTime = 2.5f;
        }
    }

    public void RefreshHpBar()
    {
        m_HeroHpBarImg.fillAmount = GlobalValue.g_CurrHP / GlobalValue.g_MaxHP;
      
        //fillAmount = currHP / maxHP 실행
    }

    //비활성화 된 오브젝트를 검색하려면 부모인 Hero의 Transform을 사용하여 Find함수로 검색할 수 밖에 없는데
    //Find함수는 string을 매개변수로 받기때문에 객체의 '이름'을 문자열로 저장해두어야
    //이미 생성된 객체가 없는건지, 비활인지를 Find함수로 알아낼 수 있다.
    string m_SpecialBackUp = "";
    string m_DefaultBackUp = "";

    void ChangeGun()
    {
        RectTransform rect = m_TargetGunUIImg.GetComponent<RectTransform>();
        
        //플레이어가 디폴트 총기 착용 중이라면
        if (GlobalValue.g_CurGun == GlobalValue.g_MyGunList[0])
        {
            //현재 장착중인 총기의 활성화 끄기
            GameObject a_Default = GameObject.FindGameObjectWithTag("Default");
            //지금 장착중인 총기 이름 저장(나중에 검색으로 끄집어 내기 위함)
            m_DefaultBackUp = a_Default.name; 
            a_Default.SetActive(false);

            //스페셜 총기 장착 중이라 상태값 변경
            GlobalValue.g_CurGun = GlobalValue.g_MyGunList[1];

            GameObject a_Gun = null; 

            //비활성화된 스페셜 총기가 하이어라키 상에 존재하지 않음 => 객체를 새롭게 생성
            if (string.IsNullOrEmpty(m_SpecialBackUp) == true)
            {
                //GlobalValue.g_CurGun = GlobalValue.g_MyGunList[1];
                GameObject a_Special = Instantiate(GlobalValue.g_MyGunList[1].m_GunArmPrefab);

                //검색용 백업 변수에 특수 총기 이름 저장
                m_SpecialBackUp = a_Special.name;
                //Debug.Log("특수 총기 이름이 저장되었습니다(이름: " + m_SpecialBackUp + ")");

                a_Special.transform.SetParent(this.transform, false);

                a_Gun = a_Special;
            }
            else//객체는 있는데 비활 상태일 뿐
            {
                GameObject a_HiddenGun = this.transform.Find(m_SpecialBackUp).gameObject;
                //하이어라키상 비활성화된 오브젝트 검색, 활성화
                if(a_HiddenGun != null && a_HiddenGun.activeSelf == false)
                {
                    a_HiddenGun.SetActive(true);
                    a_Gun = a_HiddenGun;
                }
                
            }

            //바뀐 총기에맞게 상세 정보 초기화 적용
            //m_ShootPos = a_Gun.transform.Find("ShootPos");
            m_BulletPrefab = GlobalValue.g_MyGunList[1].m_BulletPrefab;
            m_GunObj = a_Gun;

            //m_GunAnim = a_Gun.GetComponent<Animator>();
            //m_ArmPivot = a_Gun.transform;
            //m_ShootPos = m_GunObj.transform.Find("ShootPos").transform;

            rect.sizeDelta = new Vector2(328.0f, 131.0f);
            m_TargetGunUIImg.sprite = m_SpecialGunSpr;


        }
        //플레이어가 스페셜 무기 착용 중이라면
        else if (GlobalValue.g_CurGun == GlobalValue.g_MyGunList[1])
        {

            GameObject a_Special = GameObject.FindGameObjectWithTag("Special");
            a_Special.SetActive(false);

            GameObject a_HiddenGun = this.transform.Find(m_DefaultBackUp).gameObject;
            if (a_HiddenGun != null && a_HiddenGun.activeSelf == false)
            {
                a_HiddenGun.SetActive(true);
            }

            //디폴트 총기 장착
            GlobalValue.g_CurGun = GlobalValue.g_MyGunList[0];
            m_BulletPrefab = GlobalValue.g_MyGunList[0].m_BulletPrefab;

            m_GunObj = a_HiddenGun;

            //m_GunAnim = m_GunObj.GetComponent<Animator>();
            //m_ArmPivot = m_GunObj.transform;
            //m_ShootPos = m_GunObj.transform.Find("ShootPos").transform;
            rect.sizeDelta = new Vector2(534.0f, 213.0f);
            m_TargetGunUIImg.sprite = m_NormalGunSpr;
        }

        //총이 바뀌었기 때문에 총기 오브젝트와 컴포넌트, 총알 프리팹 다시 연결
        //m_GunObj = GameObject.FindGameObjectWithTag("Gun");//활성화만 연결
        m_GunAnim = m_GunObj.GetComponentInChildren<Animator>();
        m_ArmPivot = m_GunObj.transform;
        m_ShootPos = m_GunObj.transform.Find("ShootPos").transform;


    }

    public GameObject m_ClearDoor;
    public void Clear1stStage()//KillCount가 4가 되어 스테이지 1을 클리어 했을때 호출
    {
        if (m_ClearDoor != null && m_ClearDoor.activeSelf == false)
        {
            Debug.Log("1번 스테이지 클리어");
            m_ClearDoor.SetActive(true);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("trigger충돌");
    //}
}
