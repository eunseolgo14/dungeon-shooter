using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum StoreStatus
{ 
    Browse,
    Strengthen
}
public class StoreManager : MonoBehaviour
{
    [Header("-----Item Store Panel-----")]
    public Button m_BackBtn = null;

    public Button m_ActiveItemBtn = null;
    public Button m_PassiveItemBtn = null;
    //public Button m_NormalItemBtn = null;

    public RawImage[] m_SelectImg = null; //0번은 아이템 구매 창, 1번은 아이템 강화 창
    public GameObject[] m_ScrollViews; //0번은 아이템 구매 창, 1번은 아이템 강화 창

    public Text m_UserInfoTxt = null;

    public GameObject m_Item_ScContent; //<--- Scroll Content의 자녀로 생성할 부모 객체
    public GameObject m_Item_NodeObj;   //<--- Node Prefab

    [Header("-----Inventory Panel-----")]
    public Image[] m_PassiveSkillImgs;
    public Text[] m_SkillGrade;
    public GameObject m_Active_ScContent;
    public GameObject m_Skill_NodeObj;

    public Text m_GoldText = null;
    public Text m_RedGemText = null;
    public Text m_GreenGemText = null;
    public Text m_BlueGemText = null;
    public Text m_BagSizeText = null;

    //초기 생성, 최초 구매시 생성 _ 다른 두 상황을 위해 함수 따로 만들거라 전역으로 뺐다
    GameObject a_SkillObj = null;
    SkillInvenNodeManager a_SkInvenNode = null;
    ItemDragManager m_DragMgr = null;

    [Header("-----Debug Mode Panel-----")]
    public Button D_AddGoldBtn = null;
    public Button D_AddGemBtn = null;


    //현재 열려있는 상점 창 상태 확인용 변수(이게 browse면 나중에 드래그 안 되게 막기 위함)
    public StoreStatus m_StoreNow = StoreStatus.Browse;

    void Start()
    {
        m_DragMgr = FindObjectOfType<ItemDragManager>();
        Hero_Info.SetHero(GlobalValue.g_HeroType);
        RefreshInfoHud();

        //액티브 스킬 아이템 창 활성화 후 시작
        m_ScrollViews[0].gameObject.SetActive(true);
        m_SelectImg[0].gameObject.SetActive(true);

        GameObject a_ItemObj = null;
        ActiveSkillProductMgr a_SkItemNode = null;

        //좌측 판매중인 액티브 스킬 아이템로딩한다

        for (int ii = 0; ii < 6; ii++) //(0-5까지가 액티브)스킬 아이템의 갯수만큼 객체를 생성
        {
            a_ItemObj = (GameObject)Instantiate(m_Item_NodeObj);

            //스킬 아이템의 타입에 맞는 초기값 저장하는 함수 호출 
            a_SkItemNode = a_ItemObj.GetComponent<ActiveSkillProductMgr>();
            a_SkItemNode.InitData(GlobalValue.m_SkillInfoList[ii].m_SkillType);
            //=> 각 스킬에 맞는 이미지와 가격 들어온다(노드의 내용 정보를 가져오는 함수)

            //스크롤뷰의 컨텐트의 자녀로 생성
            a_ItemObj.transform.SetParent(m_Item_ScContent.transform, false);

            //디버그 모드 => 0, 1, 2 세가지 스킬만 클릭 가능하도록
            if (2 < (int)a_SkItemNode.m_SkType)
            {
                a_ItemObj.GetComponent<Button>().interactable = false;
            }
        }

        #region 상점 씬 아이템 카테고리 버튼 클릭 연결
        if (m_ActiveItemBtn != null)
        {
            m_ActiveItemBtn.onClick.AddListener(() =>
            {
                //다른 창 모조리 끈다
                m_ScrollViews[1].gameObject.SetActive(false);
                //m_ScrollViews[2].gameObject.SetActive(false);
                m_SelectImg[1].gameObject.SetActive(false);
                //m_SelectImg[2].gameObject.SetActive(false);

                m_ScrollViews[0].gameObject.SetActive(true);
                m_SelectImg[0].gameObject.SetActive(true);

                //현재 상태 == 아이템 눈팅중
                m_StoreNow = StoreStatus.Browse;
            });
        }

        if (m_PassiveItemBtn != null)
        {
            m_PassiveItemBtn.onClick.AddListener(() =>
            {
                //다른 창 모조리 끈다
                m_ScrollViews[0].gameObject.SetActive(false);
                //m_ScrollViews[2].gameObject.SetActive(false);
                m_SelectImg[0].gameObject.SetActive(false);
                //m_SelectImg[2].gameObject.SetActive(false);
                m_DragMgr.ResetUpgradePanel();

                m_ScrollViews[1].gameObject.SetActive(true);
                m_SelectImg[1].gameObject.SetActive(true);

                //현재 상태 == 강화 창 오픈 중
                m_StoreNow = StoreStatus.Strengthen;
            });
        }

        #endregion

        #region 우측 인벤토리 창에 패시브, 액티브 스킬 로딩해서 출력

        if (m_PassiveSkillImgs != null)
        {
            m_PassiveSkillImgs[0].sprite = Hero_Info.m_HeroPassImg_1;
            m_PassiveSkillImgs[1].sprite = Hero_Info.m_HeroPassImg_2;
        }

        RefreshSkillInven();


        #endregion

        if (m_BackBtn != null)
        {
            m_BackBtn.onClick.AddListener(() =>
            {
                //스테이지 1에서 로비씬으로 가는것 (4)
                //보스씬에서 로비씬으로 가는 것 (5)

                Scene m_CurScene = SceneManager.GetActiveScene();
                GlobalValue.g_CurScene = m_CurScene.buildIndex; //=> 3번이 담김

                SceneManager.LoadScene(2);

            });
        }

        //--- 디버그모드용 버튼 함수 대기
        if (D_AddGoldBtn != null)
        {
            D_AddGoldBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_UserGold += 1000;

                PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

                GlobalValue.LoadGlobalValueData();
                RefreshInfoHud();

            });
        }

        if (D_AddGemBtn != null)
        {
            D_AddGemBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_UserGreenGem += 10;
                GlobalValue.g_UserRedGem += 10;
                GlobalValue.g_UserBlueGem += 10;

                PlayerPrefs.SetInt("GreenGem", GlobalValue.g_UserGreenGem);
                PlayerPrefs.SetInt("RedGem", GlobalValue.g_UserRedGem);
                PlayerPrefs.SetInt("BlueGem", GlobalValue.g_UserBlueGem);

                GlobalValue.LoadGlobalValueData();
                RefreshInfoHud();


            });

        }
    }

        // Update is called once per frame
    void Update()
    {
        
    }
    
    public void RefreshInfoHud()
    {
        //골드값, 가방 크기, 보유 보석 갯수 등에 변동 사항이 생겼을 경우 호출
        if (m_GoldText != null)
        {
            m_GoldText.text = GlobalValue.g_UserGold.ToString();
        }
        if (m_RedGemText != null)
        {
            m_RedGemText.text = "X " + GlobalValue.g_UserRedGem.ToString();
        }
        if (m_GreenGemText != null)
        {
            m_GreenGemText.text = "X " + GlobalValue.g_UserGreenGem.ToString();
        }
        if (m_BlueGemText != null)
        {
            m_BlueGemText.text = "X " + GlobalValue.g_UserBlueGem.ToString();
        }
        if (m_BagSizeText != null)
        {
            int a_Buf = 0;

            for (int i = 0; i < GlobalValue.m_SkillInfoList.Count; i++)
            {
                if (0 < GlobalValue.m_SkillInfoList[i].m_MyCount)
                {
                    a_Buf += GlobalValue.m_SkillInfoList[i].m_MyCount;
                }
                
            }
            m_BagSizeText.text = "["+ a_Buf + "/20]";
        }

        if (m_SkillGrade != null)
        {
            m_SkillGrade[0].text = "[" + GlobalValue.m_SkillInfoList[6].m_PassiveSk_Grade.ToString() + "]";
            m_SkillGrade[1].text = "[" + GlobalValue.m_SkillInfoList[7].m_PassiveSk_Grade.ToString() + "]";
        }
    }
    public void RefreshSkillInven()
    {
        for (int ii = 0; ii < 6; ii++)
        {
            //보유 갯수가 0개 이하면 노드 생성하지 않습니다
            if (GlobalValue.m_SkillInfoList[ii].m_MyCount <= 0)
            {
                continue;
            }

            a_SkillObj = (GameObject)Instantiate(m_Skill_NodeObj);
            a_SkInvenNode = a_SkillObj.GetComponent<SkillInvenNodeManager>();
            a_SkInvenNode.InitData(GlobalValue.m_SkillInfoList[ii].m_SkillType);
            a_SkillObj.transform.SetParent(m_Active_ScContent.transform, false);
        }
    }
}
