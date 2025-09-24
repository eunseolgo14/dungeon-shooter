using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HeroManager : MonoBehaviour
{
    //--- Ű���� �Է��� ���� �÷��̾� �̵��� ���� ����
    float m_WalkForce = 5.0f;
    float m_MaxSpeed = 15.0f;
    Rigidbody2D m_Rig;

    //--- �Ѿ� �߻縦 ���� ����
    public GameObject m_BulletPrefab = null;
    public Transform m_ShootPos;
    public static int m_BulletCount = 0; //m_MagazineSize = 12; 
    float m_AttSpeed = 0.1f;        //<--- ���� �ӵ�
    float m_CacAtTIck = 0.0f;       //<--- ����� ��� ƽ �����
    float m_ShootRange = 0.0f;     //<--- ��Ÿ� //m_Reach


    //--- �߻� Ŭ�� ������ ���� �� �ڿ������� �̵�
    public Transform m_ArmPivot = null;

    //--- �Ѿ� ������ ��带 ���� ����

    //--- �ִϸ��̼� ���� ����
    Animator m_HeroAnim;
    public GameObject m_GunObj;
    Animator m_GunAnim;
    public GameObject m_ExploObj;
    Animator m_ExploAnim;

    //public Button m_ReloadHelpTxtObj;

    //--- �ǰ� �� ������ ������ ���� ����
    public bool m_IsFlashOn = false;
    public float m_FlashDur = 0.0f; //�ѹ� ������ �ð�
    public float m_PlayDur = 0.0f; //��ü ���� �ð�
    Color m_StartColor = new Color(255, 255, 255, 255); //�⺻ ��
    Color m_RedColor = new Color(255, 0, 0, 255); //����
    public SpriteRenderer m_HeroSpr = null;

    //--- ���ΰ� UI���� ����
    public Image m_HeroHpBarImg;
    public Image m_HeroCurGunImg;

    //--- ���ΰ� �ѱ� ��ü ���� �̹���
    public SpriteRenderer[] m_Hands = null;
    public Sprite[] m_HeroHands = null;

    public Sprite m_NormalGunSpr = null;
    public Sprite m_SpecialGunSpr = null;


    public Image m_TargetGunUIImg = null;


    //--- ���ΰ� ųī��Ʈ ���� ����
    public int m_KillCount = 0;
    float m_DieTime = 0.0f;
    public bool m_IsDie = false;
    float m_StartWait = 3.0f;

    void Start()
    {
        m_DieTime = 0.0f;

        //������ ������ Ǯ�Ƿ�
        GlobalValue.g_CurrHP = GlobalValue.g_MaxHP;

        //print("����� ü��:" + GlobalValue.g_CurrHP);
        Gun_Info a_DefaultGun = new Gun_Info();

        if (GlobalValue.g_HeroType == HeroType.FirstHero)//�ܽ���
        {
            a_DefaultGun.SetGunInfo(GunType.Hero1_Defalut);
        }

        GlobalValue.g_MyGunList.Insert(0, a_DefaultGun);
        GlobalValue.g_CurGun = a_DefaultGun;

        //������� �ѱ⿡ �°� ��Ÿ��� âź, ������ ����
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
                //��������� ����
                SceneManager.LoadScene(6);
            }
        }
        if (m_IsDie == true)
        {
            return;
        }

        //�ѱⰡ 2�� �̻��̶� cŰ�� ���� �ѱ� ��ü�� �Ϸ� �ϴ� ���
        if (Input.GetKeyDown(KeyCode.C) == true && 2 <= GlobalValue.g_MyGunList.Count)
        {
            ChangeGun();
        }

        if (0.0f < m_CacAtTIck)
        {
            //m_CacAtTIck�� �����Ǿ��ִٸ� �ð��� �帥��ŭ ��� ����ֱ�
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

        //Ű���� �Է°��� ���� �̵�
        if (Input.GetKey(KeyCode.LeftArrow) == true || Input.GetKey(KeyCode.RightArrow) == true)
        {
            float h = Input.GetAxis("Horizontal");
            m_Rig.AddForce(Vector3.right * h * m_WalkForce * Time.deltaTime, ForceMode2D.Impulse);

            //�ִ�ӷ� �����ʰ� ����
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

            //�ִ�ӷ� �����ʰ� ����
            if (m_MaxSpeed < Mathf.Abs(m_Rig.velocity.y))
            {
                m_Rig.velocity = new Vector2(m_Rig.velocity.x, (m_MaxSpeed * v));
            }
        }

        //Ű���忡�� �� ���ڸ��� �����̴� �� ���߱�
        if (Input.GetKeyUp(KeyCode.LeftArrow) == true || Input.GetKeyUp(KeyCode.RightArrow) == true)
        {
            m_Rig.velocity= new Vector2(0.0f, m_Rig.velocity.y);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) == true || Input.GetKeyUp(KeyCode.DownArrow) == true)
        {
            m_Rig.velocity = new Vector2(m_Rig.velocity.x, 0.0f);
        }

        m_HeroAnim.SetInteger("speed", (int)(m_Rig.velocity.x + m_Rig.velocity.y));


        //--- ���� �Ѿ� �߻� �ڵ�(���콺Ŭ��)
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            //m_CacAtTIck�� ������Ʈ���� ���̰� ���̴� 0���� �۾����� �߻�!
            if (m_CacAtTIck <= 0.0f)
            {
                //��ź���� 0�� �ƴϸ�
                if (0 < m_BulletCount)
                {
                    //���콺 ��Ŭ���� ������ ������ǥ�� �Լ��� ������.
                    ShootFire(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                    //m_ExploAnim.SetTrigger("isShoot");
                    //m_CacAtTIck�� �ٽ� 0.1�� ����
                    m_CacAtTIck = m_AttSpeed;
                    //��ź�� ����
                    m_BulletCount--;
                    //������ ��ź ���
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

    void ShootFire(Vector3 a_ClickPos) //a_TPos = ���콺�� Ŭ�� ����
    {
        float a_Dist = (transform.position - a_ClickPos).magnitude;
        //Ŭ���� ������ ���ΰ� �ڽŰ� ����ġ�԰����ٸ�
        if (a_Dist <= 10.04f)
        {
            return;
        }

        if (m_BulletPrefab != null)
        {
            //1. ������Ʈ�� �����Ѵ�
            GameObject a_Bullet = Instantiate(m_BulletPrefab) as GameObject;
            BulletManager a_BulletMgr = a_Bullet.GetComponent<BulletManager>();
            a_Bullet.transform.position = m_ShootPos.position;
            m_GunAnim.SetTrigger("isShoot");
            //2. �÷��̾��� ���� ��ǥ���� �ִ´�
            Vector3 a_CurPos = this.transform.position;
            Vector3 a_ShootDir = a_ClickPos - a_CurPos;
            a_ShootDir.z = 0.0f;

            //���⺤�ͷ� ����ȭ
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
            print(" ������ ���� ����");
        }

        SoundManager.Instance.PlayEffSound("GunFire_Short", 1.5f);
    }

    void RotateArm(Vector3 a_Dir)
    {
        //Debug.Log("�� ȸ�� + " + a_Dir);
        Quaternion a_CacAngle = Quaternion.LookRotation(a_Dir); //�Ű������� ���� Ŭ�� �������ϴ� ���͸� ���� ���ʹϾ� ����
        Vector3 a_CacVec = a_CacAngle.eulerAngles;
        int a_idx = 0;


        if (m_ArmPivot == null)
        {
            Debug.Log("�� ����");
            return;
        }

        if (a_Dir.x <= 0) //������ x�� ����, ���� Ŭ��
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 180.0f, transform.rotation.z));
            a_idx = 180;
        }
        else if(0 < a_Dir.x) //�溤�� x�� ���, ���� �� Ŭ��
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
            Debug.Log("���� ���� ��, ������ ����");
            return;
        }

        //�ǰ� ���� ����
        m_IsFlashOn = true;
        m_PlayDur = 2.0f;
        m_FlashDur = 0.2f;

        //�۷ι� ���翡�� ������ ���� ���(�������� �׻� ���ָ� �������� ����)
        GlobalValue.g_CurrHP -= a_Value;
        
        RefreshHpBar();

        //�÷��̾� ���� ü�� 0�Ʒ��� ������
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
      
        //fillAmount = currHP / maxHP ����
    }

    //��Ȱ��ȭ �� ������Ʈ�� �˻��Ϸ��� �θ��� Hero�� Transform�� ����Ͽ� Find�Լ��� �˻��� �� �ۿ� ���µ�
    //Find�Լ��� string�� �Ű������� �ޱ⶧���� ��ü�� '�̸�'�� ���ڿ��� �����صξ��
    //�̹� ������ ��ü�� ���°���, ��Ȱ������ Find�Լ��� �˾Ƴ� �� �ִ�.
    string m_SpecialBackUp = "";
    string m_DefaultBackUp = "";

    void ChangeGun()
    {
        RectTransform rect = m_TargetGunUIImg.GetComponent<RectTransform>();
        
        //�÷��̾ ����Ʈ �ѱ� ���� ���̶��
        if (GlobalValue.g_CurGun == GlobalValue.g_MyGunList[0])
        {
            //���� �������� �ѱ��� Ȱ��ȭ ����
            GameObject a_Default = GameObject.FindGameObjectWithTag("Default");
            //���� �������� �ѱ� �̸� ����(���߿� �˻����� ������ ���� ����)
            m_DefaultBackUp = a_Default.name; 
            a_Default.SetActive(false);

            //����� �ѱ� ���� ���̶� ���°� ����
            GlobalValue.g_CurGun = GlobalValue.g_MyGunList[1];

            GameObject a_Gun = null; 

            //��Ȱ��ȭ�� ����� �ѱⰡ ���̾��Ű �� �������� ���� => ��ü�� ���Ӱ� ����
            if (string.IsNullOrEmpty(m_SpecialBackUp) == true)
            {
                //GlobalValue.g_CurGun = GlobalValue.g_MyGunList[1];
                GameObject a_Special = Instantiate(GlobalValue.g_MyGunList[1].m_GunArmPrefab);

                //�˻��� ��� ������ Ư�� �ѱ� �̸� ����
                m_SpecialBackUp = a_Special.name;
                //Debug.Log("Ư�� �ѱ� �̸��� ����Ǿ����ϴ�(�̸�: " + m_SpecialBackUp + ")");

                a_Special.transform.SetParent(this.transform, false);

                a_Gun = a_Special;
            }
            else//��ü�� �ִµ� ��Ȱ ������ ��
            {
                GameObject a_HiddenGun = this.transform.Find(m_SpecialBackUp).gameObject;
                //���̾��Ű�� ��Ȱ��ȭ�� ������Ʈ �˻�, Ȱ��ȭ
                if(a_HiddenGun != null && a_HiddenGun.activeSelf == false)
                {
                    a_HiddenGun.SetActive(true);
                    a_Gun = a_HiddenGun;
                }
                
            }

            //�ٲ� �ѱ⿡�°� �� ���� �ʱ�ȭ ����
            //m_ShootPos = a_Gun.transform.Find("ShootPos");
            m_BulletPrefab = GlobalValue.g_MyGunList[1].m_BulletPrefab;
            m_GunObj = a_Gun;

            //m_GunAnim = a_Gun.GetComponent<Animator>();
            //m_ArmPivot = a_Gun.transform;
            //m_ShootPos = m_GunObj.transform.Find("ShootPos").transform;

            rect.sizeDelta = new Vector2(328.0f, 131.0f);
            m_TargetGunUIImg.sprite = m_SpecialGunSpr;


        }
        //�÷��̾ ����� ���� ���� ���̶��
        else if (GlobalValue.g_CurGun == GlobalValue.g_MyGunList[1])
        {

            GameObject a_Special = GameObject.FindGameObjectWithTag("Special");
            a_Special.SetActive(false);

            GameObject a_HiddenGun = this.transform.Find(m_DefaultBackUp).gameObject;
            if (a_HiddenGun != null && a_HiddenGun.activeSelf == false)
            {
                a_HiddenGun.SetActive(true);
            }

            //����Ʈ �ѱ� ����
            GlobalValue.g_CurGun = GlobalValue.g_MyGunList[0];
            m_BulletPrefab = GlobalValue.g_MyGunList[0].m_BulletPrefab;

            m_GunObj = a_HiddenGun;

            //m_GunAnim = m_GunObj.GetComponent<Animator>();
            //m_ArmPivot = m_GunObj.transform;
            //m_ShootPos = m_GunObj.transform.Find("ShootPos").transform;
            rect.sizeDelta = new Vector2(534.0f, 213.0f);
            m_TargetGunUIImg.sprite = m_NormalGunSpr;
        }

        //���� �ٲ���� ������ �ѱ� ������Ʈ�� ������Ʈ, �Ѿ� ������ �ٽ� ����
        //m_GunObj = GameObject.FindGameObjectWithTag("Gun");//Ȱ��ȭ�� ����
        m_GunAnim = m_GunObj.GetComponentInChildren<Animator>();
        m_ArmPivot = m_GunObj.transform;
        m_ShootPos = m_GunObj.transform.Find("ShootPos").transform;


    }

    public GameObject m_ClearDoor;
    public void Clear1stStage()//KillCount�� 4�� �Ǿ� �������� 1�� Ŭ���� ������ ȣ��
    {
        if (m_ClearDoor != null && m_ClearDoor.activeSelf == false)
        {
            Debug.Log("1�� �������� Ŭ����");
            m_ClearDoor.SetActive(true);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("trigger�浹");
    //}
}
