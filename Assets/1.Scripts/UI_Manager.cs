using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum StageState
{ 
    Stage_1,
    BossStage
}
public class UI_Manager : MonoBehaviour
{
    //--- ��ź���� ǥ�� UI���� ����
    [Header("---- Bullet Panel ----")]

    //--- ��ź���� ǥ���ϱ����� FillŸ�� �̹�����
    public Image[] m_BulletImgs; //�Ϸ�
    //--- ������ ������ ���� ������
    public Button m_ReloadHelpBtn = null; //�Ϸ�
    public GameObject m_RemainBulletObj; //�Ϸ�
    public GameObject m_ReloadGageBarObj;
    public Image m_ReloadGageFillImg; //�Ϸ�

    private bool m_isReloadBtnDown;

    //--- �κ��丮 ��ũ�Ѻ� ���� ����
    [Header("---- Inventory Scroll ----")]
    //public Button m_InvenBtn = null;
    public GameObject m_InvenScrollView = null;
    //public Transform m_InvenContent;
    public GameObject m_SkInvenNode;

    //--- ���� ��ȭ ǥ�� UI���� ����
    [Header("---- Acquisition UI ----")]
    public Text m_BlueGemTxt = null;
    public Text m_GreenGemTxt = null;
    public Text m_RedGemTxt = null;
    public Text m_GoldCoinTxt = null;
    //Text[] m_AcquisitionTxt = null;

    //--- ���� ���� ǥ�ÿ� ����
    //bool isTouch = false;
    public GameObject m_ChestHud;
    public Text m_ChestHelpTxt;
    public Image m_ChestIconImg;

    [Header("---- Progress Bar ----")]
    public Image m_ProgressFill = null;
    public Text m_Timer = null;
    EnemyManager[] m_EnemyHps;
    float m_TotalProgress = 0.0f;
    float m_CurProgress = 0.0f;

    public StageState m_Stage;
    public Button m_BackBtn = null;

    [Header("---- Before Start ----")]
    public GameObject m_Panel = null;
    public GameObject m_InfoTab = null;
    public Text m_CountDownTxt = null;
    public Button m_CloseInfoBtn = null;
    float m_CountDownTimer = 3.0f;
    bool m_IsCountDown = false;
    bool m_IsStart = false;

    void Start()
    {
        Time.timeScale = 0.0f;
        m_CountDownTxt.text = "3";

        if (m_Stage == StageState.Stage_1)
        {
            GameObject a_Tmp4 = GameObject.Find("Gem_HUD").gameObject;
            m_BlueGemTxt = a_Tmp4.transform.Find("BlueGem_Txt").GetComponent<Text>();
            m_GreenGemTxt = a_Tmp4.transform.Find("GreenGem_Txt").GetComponent<Text>();
            m_RedGemTxt = a_Tmp4.transform.Find("RedGem_Txt").GetComponent<Text>();
            m_GoldCoinTxt = a_Tmp4.transform.Find("Coin_Txt").GetComponent<Text>();

            RefreshAcquisition();
        }
        

        //GlobalValue.LoadGlobalValueData();�̹� StoreManager���� �ҷ������ٵ� �� �ʿ��Ѱ�...?
        
        LoadSkillItem();

        GameObject a_Tmp2 = GameObject.Find("Bullet_HUD");
        if (a_Tmp2 != null)
        {
            m_ReloadHelpBtn = a_Tmp2.transform.Find("Reload_Btn").GetComponent<Button>();
            m_ReloadGageBarObj = a_Tmp2.transform.Find("ReloadGageBar").gameObject;
            m_ReloadGageFillImg = m_ReloadGageBarObj.transform.Find("ReloadBarFill").GetComponent<Image>();
            m_ReloadGageFillImg.fillAmount = 0.0f;
        }

        GameObject a_Tmp1 = GameObject.Find("Bullet_Gage");
        if (a_Tmp1 != null)
        {
            m_RemainBulletObj = a_Tmp1;

            Image[] a_TmpImgs = a_Tmp1.GetComponentsInChildren<Image>();
            for (int i = 0; i < 6; i++)
            {
                m_BulletImgs[i] = a_TmpImgs[i]; //<--- 5������ ��
            }
        }


        EventTrigger a_Trigger = m_ReloadHelpBtn.GetComponent<EventTrigger>();

        EventTrigger.Entry a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerDown;
        a_entry.callback.AddListener((data) =>
        {
            OnReloadBtnDown((PointerEventData)data);
        });
        a_Trigger.triggers.Add(a_entry);//<--- list�ȿ� ������ �Ҵ�� eventID/callback�� ������ ��� �־ �� ������ ������ ����Ʈ�� �� ���� ����!

        a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerUp;
        a_entry.callback.AddListener((data) =>
        {
            OnReloadBtnUp((PointerEventData)data);
        });
        a_Trigger.triggers.Add(a_entry);
        //right button ���

        if (m_BackBtn != null)
        {
            Scene m_CurScene = SceneManager.GetActiveScene();
            GlobalValue.g_CurScene = m_CurScene.buildIndex;

            //�������� 1���� �κ������ ���°� (4)
            //���������� �κ������ ���� �� (5)
            m_BackBtn.onClick.AddListener(() =>
            {
                //�κ������ ȸ��
                SceneManager.LoadScene(2);

            });
        }

        if (m_CloseInfoBtn != null)
        {
            m_CloseInfoBtn.onClick.AddListener(() =>
           {
               
               m_InfoTab.SetActive(false);
               m_CountDownTxt.gameObject.SetActive(true);
               //StartCoroutine("CountDown");


               print("�ð� �ٽ� Ǯ��");
               Time.timeScale = 1.0f;
               m_IsCountDown = true;
           });
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsCountDown == true)
        {
            NormalCountDown();
        }

        if (m_IsStart == false)
        {
            return;
        }

        if (m_ReloadHelpBtn.gameObject.activeSelf == true && m_isReloadBtnDown == true)
        {
            m_ReloadGageFillImg.fillAmount += Time.deltaTime;

            //������ �� ���� ������ �� �ݱ�, �ȳ��޼��� �ݱ�
            if (0.9f <= m_ReloadGageFillImg.fillAmount)
            {
                m_ReloadGageFillImg.fillAmount = 1.0f;
                //������ ���� �� ����
                m_ReloadGageBarObj.gameObject.SetActive(false);
                //������ �ȳ� �޼��� ��ư ����
                m_ReloadHelpBtn.gameObject.SetActive(false);

                //��ź ��� UI�� �ٽ� �ѱ�
                m_RemainBulletObj.gameObject.SetActive(true);
                
                //�ҷ� ī��Ʈ 30���� ������
                HeroManager.m_BulletCount = 30;
                m_isReloadBtnDown = false;
                //�ҷ� ��� ��ź�� UI�����
                RefreshBulletHud();
            }
        }
        else if (m_ReloadHelpBtn.gameObject.activeSelf == true && m_isReloadBtnDown == false)
        {
            m_ReloadGageFillImg.fillAmount = 0.0f;
        }

        if (m_Stage == StageState.Stage_1)
        {
            TimerUpdate();
        }

    }

    //���ŵǾ���ϴ� UI�׸�� ����Ʈ
    //1. �Ѿ� ��ź
    //2. ü��
    //3. ��ȭ
    //4. �κ��丮

    //1. �Ѿ� ��ź
    public void RefreshBulletHud()
    {
        //fill amount�� �籸��
        float a_CacHp = 0.0f;

        //m_BulletImgs.Length = 6,
        for (int i = 0; i < m_BulletImgs.Length; i++)
        {
            a_CacHp = ((float)HeroManager.m_BulletCount / 5.0f) - (float)i;

            if (a_CacHp < 0.0f)
            {
                a_CacHp = 0.0f;
            }
            if (1.0f < a_CacHp)
            {
                a_CacHp = 1.0f;
            }
            m_BulletImgs[i].fillAmount = a_CacHp;
        }

    }

    //3. ���� ��ȭ
    public void RefreshAcquisition()
    {
        //GlobalValue.LoadGlobalValueData();
        //�۷ι�����κ��� ������ �׸��� �޾ƿ� UI�� ���
        
        m_BlueGemTxt.text = GlobalValue.g_UserBlueGem.ToString();
        m_GreenGemTxt.text = GlobalValue.g_UserGreenGem.ToString();
        m_RedGemTxt.text = GlobalValue.g_UserRedGem.ToString();
        m_GoldCoinTxt.text =  GlobalValue.g_UserGold.ToString();

        
    }

    void OnReloadBtnDown(PointerEventData a_data)
    {
        m_isReloadBtnDown = true;
    }

    void OnReloadBtnUp(PointerEventData a_data)
    {
        m_isReloadBtnDown = false;
    }
    public void ShowRelaodBar()
    {
        if (m_ReloadGageFillImg.gameObject.activeSelf == true) //<---�̹� ���������� ����
        {
            //return;
        }

        //��ź ��� UI�� ����
        m_RemainBulletObj.gameObject.SetActive(false);
        //������ �ȳ� �޼��� ��ư �ѱ�
        m_ReloadHelpBtn.gameObject.SetActive(true);
        //������ ��� �������� Ȱ��ȭ
        m_ReloadGageBarObj.gameObject.SetActive(true);
        //m_ReloadGageFillImg.fillAmount = 0.0f; //<--- ���⼭ ���ε� ������ �� �ǰ� ������ ��
        

    }

    

    public void RefreshProgressBar()
    {
        m_EnemyHps = GameObject.FindObjectsOfType<EnemyManager>();
        m_CurProgress = 0.0f;
        m_TotalProgress = 400.0f;

        for (int i = 0; i < m_EnemyHps.Length; i++)
        {
            m_CurProgress += m_EnemyHps[i].m_CurHp;
        }

        if (m_ProgressFill != null)
        {
            m_ProgressFill.fillAmount = 1.0f - (float)m_CurProgress / m_TotalProgress;
        }
        
    }

    //float a_total = 0.0f;
    float a_Second = 0.0f;
    int a_Minute = 0;
    int a_Hour = 0;

    string a_SecString = "";
    string a_MinString = "";
    string a_HourString = "";
    
    void TimerUpdate()
    {
        a_Second += Time.deltaTime;
        

        if (60.0f <= a_Second)
        {
            a_Second = 0.0f;
            a_Minute++;
        }
        if (60 <= a_Minute)
        {
            a_Minute = 0;
            a_Hour++;
        }

        if (a_Second < 9.5f)
        {
            a_SecString = "0"+ a_Second.ToString("N0");
        }
        else if(10.0f <= a_Second)
        {
            a_SecString = a_Second.ToString("N0");
        }

        if (a_Minute < 10)
        {
            a_MinString = "0" + a_Minute.ToString();
        }
        else
        {
            a_MinString = a_Minute.ToString();
        }

        if (a_Hour < 10)
        {
            a_HourString = "0" + a_Hour.ToString();
        }
        else
        {
            a_HourString = a_Hour.ToString();
        }

        m_Timer.text = a_HourString + ":" + a_MinString + ":" + a_SecString;
    }

    //�۷ι� �������� ��ų�� ���� ��Ȳ�� ������ ��ųâ�� �ѹ� ����ִ� �Լ�
    //���ۿ� �ѹ�
    //��ų ��� �� ��������� ����� �ѹ�

    public Transform m_SkillItemContent = null;
    public GameObject m_SkillItemBtnPrefab = null;

    public void LoadSkillItem()
    {
        
        //float a_BackUpTime = 0.0f;
        //���� �ε��ϱ� �� ��ũ�Ѻ信 �ִ� ��� ��ų��ư �� �����α�
        SkillBtnManager[] a_SkillList = GameObject.FindObjectsOfType<SkillBtnManager>();

        //a_SkillList�� ��ȣ�� 1������ m_SkillInfoList������ 2�� �༮�� ��´ٰ� ����
        //��������� m_SkillInfoList�� 2�� ����
        if (0 < a_SkillList.Length)
        {
            for (int i = 0; i < a_SkillList.Length; i++)
            {
                //�����ִ� ������ Ÿ���� �ʱⰪ�� 0���� ũ�� = ��Ÿ���� ���ư��� ���̾���
                if (0.0f < a_SkillList[i].m_RemainTime)
                {
                    //�� Ÿ�� ���� �� �༮ ��ų�� ���� ��ȣ ����
                    //int a_SkipIdx = (int)a_SkillList[i].m_SkType;
                    //����� ���� ��ȣ ��ų���� ���� �ð� �ִٰ� �˷��ֱ�
                    GlobalValue.m_SkillInfoList[(int)a_SkillList[i].m_SkType].m_BackUpTime = a_SkillList[i].m_RemainTime;
                }

                Destroy(a_SkillList[i].gameObject);

            }
        }
        //������ �ʰ� ����� ���� ������ => ���� �ֿ� ���� ������ �� ������ ������ �̻�����
        // �� �����ٰ� �� �ٽ� ��Ҵٰ� ��Ÿ�� ���ư��� �ִ� �ٽ� ������ �׷����ҵ�

        //���� �ҷ��ͼ� ����
        GlobalValue.LoadGlobalValueData();

        GameObject a_SkillObj = null;
        SkillBtnManager a_SkillBtnMgr = null;

        for (int ii = 0; ii < 6; ii++)
        {
            //���� ������ 0�� �����̰ų�, ��Ÿ�� ���ư��� �������� ���� �༮��
            if (GlobalValue.m_SkillInfoList[ii].m_MyCount <= 0)
            {
                continue;
            }
            else
            {
                a_SkillObj = (GameObject)Instantiate(m_SkillItemBtnPrefab);
                a_SkillBtnMgr = a_SkillObj.GetComponent<SkillBtnManager>();
                a_SkillBtnMgr.InitSkillBtn(GlobalValue.m_SkillInfoList[ii].m_SkillType);
                a_SkillObj.transform.SetParent(m_SkillItemContent, false);
            }

            if (0.0f < GlobalValue.m_SkillInfoList[ii].m_BackUpTime)
            {
                //a_SkillObj = (GameObject)Instantiate(m_SkillItemBtnPrefab);
                //a_SkillBtnMgr = a_SkillObj.GetComponent<SkillBtnManager>();

               // a_SkillBtnMgr.InitSkillBtn(GlobalValue.m_SkillInfoList[ii].m_SkillType);
                a_SkillBtnMgr.SetCoolTime(GlobalValue.m_SkillInfoList[ii].m_BackUpTime);

                //a_SkillObj.transform.SetParent(m_SkillItemContent, false);

                GlobalValue.m_SkillInfoList[ii].m_BackUpTime = 0.0f;
            }


            //else if (0.0f < GlobalValue.m_SkillInfoList[ii].m_BackUpTime)
            //{
            //    a_SkillObj = (GameObject)Instantiate(m_SkillItemBtnPrefab);
            //    a_SkillBtnMgr = a_SkillObj.GetComponent<SkillBtnManager>();

            //    a_SkillBtnMgr.InitSkillBtn(GlobalValue.m_SkillInfoList[ii].m_SkillType);
            //    a_SkillBtnMgr.SetCoolTime(GlobalValue.m_SkillInfoList[ii].m_BackUpTime);

            //    a_SkillObj.transform.SetParent(m_SkillItemContent, false);
                
            //    GlobalValue.m_SkillInfoList[ii].m_BackUpTime = 0.0f;
            //}
            
        }
    }

   
    public void NormalCountDown()
    {
        m_CountDownTimer -= Time.unscaledDeltaTime;

        m_CountDownTxt.text = m_CountDownTimer.ToString("N0");


        if (m_CountDownTimer < 0.0f)
        {
            m_CountDownTxt.gameObject.SetActive(false);
            m_Panel.SetActive(false);
            m_IsStart = true;
        }
    }
}
