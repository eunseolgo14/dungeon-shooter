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
    //--- 잔탄수를 표시 UI관련 변수
    [Header("---- Bullet Panel ----")]

    //--- 잔탄수를 표시하기위한 Fill타입 이미지들
    public Image[] m_BulletImgs; //완료
    //--- 재장전 구현을 위한 변수들
    public Button m_ReloadHelpBtn = null; //완료
    public GameObject m_RemainBulletObj; //완료
    public GameObject m_ReloadGageBarObj;
    public Image m_ReloadGageFillImg; //완료

    private bool m_isReloadBtnDown;

    //--- 인벤토리 스크롤뷰 관련 변수
    [Header("---- Inventory Scroll ----")]
    //public Button m_InvenBtn = null;
    public GameObject m_InvenScrollView = null;
    //public Transform m_InvenContent;
    public GameObject m_SkInvenNode;

    //--- 습득 재화 표시 UI관련 변수
    [Header("---- Acquisition UI ----")]
    public Text m_BlueGemTxt = null;
    public Text m_GreenGemTxt = null;
    public Text m_RedGemTxt = null;
    public Text m_GoldCoinTxt = null;
    //Text[] m_AcquisitionTxt = null;

    //--- 보물 상자 표시용 변수
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
        

        //GlobalValue.LoadGlobalValueData();이미 StoreManager에서 불려왔을텐데 또 필요한가...?
        
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
                m_BulletImgs[i] = a_TmpImgs[i]; //<--- 5까지만 들어감
            }
        }


        EventTrigger a_Trigger = m_ReloadHelpBtn.GetComponent<EventTrigger>();

        EventTrigger.Entry a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerDown;
        a_entry.callback.AddListener((data) =>
        {
            OnReloadBtnDown((PointerEventData)data);
        });
        a_Trigger.triggers.Add(a_entry);//<--- list안에 위에서 할당된 eventID/callback가 찹찹찹 담겨 있어서 그 데이터 정보를 리스트의 한 노드로 저장!

        a_entry = new EventTrigger.Entry();
        a_entry.eventID = EventTriggerType.PointerUp;
        a_entry.callback.AddListener((data) =>
        {
            OnReloadBtnUp((PointerEventData)data);
        });
        a_Trigger.triggers.Add(a_entry);
        //right button 대기

        if (m_BackBtn != null)
        {
            Scene m_CurScene = SceneManager.GetActiveScene();
            GlobalValue.g_CurScene = m_CurScene.buildIndex;

            //스테이지 1에서 로비씬으로 가는것 (4)
            //보스씬에서 로비씬으로 가는 것 (5)
            m_BackBtn.onClick.AddListener(() =>
            {
                //로비씬으로 회귀
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


               print("시간 다시 풀기");
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

            //게이지 다 차면 게이지 바 닫기, 안내메세지 닫기
            if (0.9f <= m_ReloadGageFillImg.fillAmount)
            {
                m_ReloadGageFillImg.fillAmount = 1.0f;
                //게이지 충전 바 끄기
                m_ReloadGageBarObj.gameObject.SetActive(false);
                //재장전 안내 메세지 버튼 끄기
                m_ReloadHelpBtn.gameObject.SetActive(false);

                //잔탄 출력 UI를 다시 켜기
                m_RemainBulletObj.gameObject.SetActive(true);
                
                //불렛 카운트 30으로 재장전
                HeroManager.m_BulletCount = 30;
                m_isReloadBtnDown = false;
                //불렛 허드 잔탄수 UI재출력
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

    //갱신되어야하는 UI항목들 리스트
    //1. 총알 잔탄
    //2. 체력
    //3. 재화
    //4. 인벤토리

    //1. 총알 잔탄
    public void RefreshBulletHud()
    {
        //fill amount로 재구현
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

    //3. 습득 재화
    public void RefreshAcquisition()
    {
        //GlobalValue.LoadGlobalValueData();
        //글로벌벨루로부터 다음의 항목을 받아와 UI에 출력
        
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
        if (m_ReloadGageFillImg.gameObject.activeSelf == true) //<---이미 켜져있으면 리턴
        {
            //return;
        }

        //잔탄 출력 UI를 끄기
        m_RemainBulletObj.gameObject.SetActive(false);
        //재장전 안내 메세지 버튼 켜기
        m_ReloadHelpBtn.gameObject.SetActive(true);
        //재장전 출력 게이지바 활성화
        m_ReloadGageBarObj.gameObject.SetActive(true);
        //m_ReloadGageFillImg.fillAmount = 0.0f; //<--- 여기서 리로드 장전이 안 되게 막히는 중
        

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

    //글로벌 벨류에서 스킬을 보유 현황을 가져와 스킬창에 한번 띄워주는 함수
    //시작에 한번
    //스킬 사용 후 변경사항이 생기면 한번

    public Transform m_SkillItemContent = null;
    public GameObject m_SkillItemBtnPrefab = null;

    public void LoadSkillItem()
    {
        
        //float a_BackUpTime = 0.0f;
        //새로 로딩하기 전 스크롤뷰에 있는 모든 스킬버튼 싹 지워두기
        SkillBtnManager[] a_SkillList = GameObject.FindObjectsOfType<SkillBtnManager>();

        //a_SkillList의 번호로 1이지만 m_SkillInfoList에서는 2인 녀석을 담는다고 가정
        //결과적으로 m_SkillInfoList의 2에 접근
        if (0 < a_SkillList.Length)
        {
            for (int i = 0; i < a_SkillList.Length; i++)
            {
                //남아있는 리메인 타임이 초기값인 0보다 크다 = 쿨타임이 돌아가던 중이었다
                if (0.0f < a_SkillList[i].m_RemainTime)
                {
                    //쿨 타임 남은 이 녀석 스킬의 고유 번호 저장
                    //int a_SkipIdx = (int)a_SkillList[i].m_SkType;
                    //저장된 고유 번호 스킬번의 남은 시간 있다고 알려주기
                    GlobalValue.m_SkillInfoList[(int)a_SkillList[i].m_SkType].m_BackUpTime = a_SkillList[i].m_RemainTime;
                }

                Destroy(a_SkillList[i].gameObject);

            }
        }
        //지우지 않고 남기는 것의 문제점 => 남은 애와 새로 생성된 애 사이의 순서가 이상해짐
        // 싹 지웠다가 싹 다시 깔았다가 쿨타임 돌아가던 애는 다시 돌리고 그래야할듯

        //새로 불러와서 생성
        GlobalValue.LoadGlobalValueData();

        GameObject a_SkillObj = null;
        SkillBtnManager a_SkillBtnMgr = null;

        for (int ii = 0; ii < 6; ii++)
        {
            //보유 갯수가 0개 이하이거나, 쿨타임 돌아가서 삭제하지 않은 녀석은
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
