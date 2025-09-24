using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDragManager : MonoBehaviour
{
    public SlotManager m_TargetSlot = null;
    public SlotManager[] m_SkillSlots = null;

    public GameObject m_MouseImg = null;
    int m_SaveIdx = -1; //<--- -1이 아니라면 아이템 픽킹 후 드래그 상태
    public SkillType m_TargetType;

    //--- 강화 관련 옵션 변수
    public GameObject m_HelpPanel = null;
    public GameObject m_InitialTextObj = null;

    public GameObject m_ConfirmTextObj = null;
    public Image m_GemIconImage = null;
    public Image m_IconImage = null;
    public Sprite[] m_GemImgs;
    public Text m_CoinTxt = null;
    public Text m_GemTxt = null;

    public Button m_ConfirmBtn = null;
    public Button m_CancelBtn = null;

    public Text m_HelpTxt = null;
    float m_ShowTime = 0.0f;

    StoreManager m_StoreManager;

    void Start()
    {
        m_StoreManager = GameObject.FindObjectOfType<StoreManager>();

        if (m_ConfirmBtn != null)
        {
            m_ConfirmBtn.onClick.AddListener( ()=>
            {
                TryUpgrade(m_TargetType);
            });
        }
        if (m_CancelBtn != null)
        {
            m_CancelBtn.onClick.AddListener(() =>
            {
                m_TargetSlot.m_MyImg.sprite = null;
                m_TargetSlot.m_CurItemIdx = -1;

                m_HelpPanel.gameObject.SetActive(false);
                m_InitialTextObj.gameObject.SetActive(true);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        //강화창이 열려있는게 아니라면 아이템 드래그 할 수 없게 리턴
        if (m_StoreManager.m_StoreNow == StoreStatus.Browse)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) == true) //<--- 마우스 좌클릭 순간
        {
            MouseBtnDown();
        }

        if (Input.GetMouseButton(0) == true) //<--- 마우스 좌클릭 하고있는 동안
        {
            MouseBtnPress();
        }

        if (Input.GetMouseButtonUp(0) == true) //<--- 마우스 좌클릭에서 뗀 순간
        {
            MouseBtnUp();
        }

        if (0.0f < m_ShowTime)
        {
            m_ShowTime -= Time.deltaTime;
            if (m_ShowTime <= 0.0f)
            {
                ResetUpgradePanel();
            }
        }
    }

    public void ResetUpgradePanel()
    {
        m_TargetSlot.m_MyImg.sprite = null;
        m_TargetSlot.m_CurItemIdx = -1;
        m_HelpPanel.gameObject.SetActive(false);
        m_InitialTextObj.GetComponent<Text>().text = "강화를 원하시는 패시브 스킬을\n드래그해서 빈칸에 놓아주세요";
    }

    private void MouseBtnDown()
    {
        m_SaveIdx = -1;
        for (int ii = 0; ii < m_SkillSlots.Length; ii++)
        {
            //for문을 돌면서 1번 패시브 스킬을 클릭한건지 2번 패시브 스킬을 집은건지 감지

            if (IsCollSlot(m_SkillSlots[ii]) == true) //IsCollSlot(m_ProductSlots[ii]) == true => 다른 곳이 아니라 슬롯을 누른것을 확인
            {
                m_SaveIdx = ii;
                //Transform a_ChildImg = m_MouseImg.transform.Find("MsIconImg");

                if (m_MouseImg != null)
                {
                    m_MouseImg.GetComponent<Image>().sprite = m_SkillSlots[ii].m_MyImg.sprite;
                }

                m_MouseImg.gameObject.SetActive(true);
                break;
            }
        }
    }

    void MouseBtnPress()
    {
        if (0 <= m_SaveIdx) //<-- 0보다 작다는 것은 빈 곳을 클릭했다는 것
        {
            m_MouseImg.transform.position = Input.mousePosition;
        }
    }


    private void MouseBtnUp()
    {
        if (m_SaveIdx < 0 || m_SkillSlots.Length <= m_SaveIdx) //<-- 처음부터 아이템 피킹이 제대로 이루어지지 않은 상황
        {
            return;
        }

        //int a_BuyIdx = -1;

        if (IsCollSlot(m_TargetSlot) == true) 
        {
            Sprite a_MsIconImg = null;
            //Transform a_ChildImg = m_MouseImg.transform.Find("MsIconImg");

            if (m_MouseImg != null)
            {
                a_MsIconImg = m_MouseImg.GetComponent<Image>().sprite;
            }

            m_TargetSlot.m_MyImg.sprite = a_MsIconImg;
            //m_TargetSlot.gameObject.SetActive(true);
            m_TargetSlot.m_CurItemIdx = m_SaveIdx;

            int a_TempIdx = -1;
            a_TempIdx = ((int)GlobalValue.g_HeroType * 2) + 6 + m_SaveIdx;
            m_TargetType = (SkillType)a_TempIdx;

            //해당 패시브 스킬의 업그레이드 조건을 분석하는 함수
            UpgradeRequirements(m_TargetType);

        }
        
       
        m_SaveIdx = -1; //<--- 장착까지 이루어졌다면 다시 초기화
        m_MouseImg.gameObject.SetActive(false);
    }

    bool IsCollSlot(SlotManager a_CkCslot) //<--- 이걸 함수로 따로 빼 두면 매번 마우스 클릭 지점이 슬롯 영역 안인지 작성할 필요 없다. 
    {
        //마우스가 어느 슬롯 UI위에 있는지를 판단하는 함수

        if (a_CkCslot == null)
        {
            return false;
        }

        Vector3[] v = new Vector3[4];
        a_CkCslot.GetComponent<RectTransform>().GetWorldCorners(v);
        //v[0]:좌측 하단(0,0), v[1]:좌측 상단, v[2]:우측 상단(), v[3]:우측 하단

        if (v[0].x <= Input.mousePosition.x && Input.mousePosition.x <= v[2].x &&
            v[0].y <= Input.mousePosition.y && Input.mousePosition.y <= v[2].y)
        {
            return true;
        }

        return false;

    }


    int a_grade = 0;
    int a_NewPrice = 0;
    int a_Random = 0;
    int a_RandColor = 0;
    int a_GemCount = 0;

    public void UpgradeRequirements(SkillType a_Type)
    {
        //시작값은 6이어야 함
        a_grade = GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade - 1;

        if (a_grade <= 1) // 등급이 1과 같거나 작을 때
        {
            m_InitialTextObj.GetComponent<Text>().text = "이미 최고 등급에 도달하여\n더이상의 등급업은 불가합니다.";
            m_ShowTime = 3.0f;
            //이미 최고 등급, 더이상의 업글 불가
            return;
        }

        m_InitialTextObj.gameObject.SetActive(false);
        m_HelpPanel.gameObject.SetActive(true);
        //m_ConfirmTextObj.gameObject.SetActive(true);

        //업그레이드시 필요한 재화 산출
        a_NewPrice = GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Up_Price + (GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Up_Price * (7 - a_grade));

        //레벨 0일땐 100 + (100 * 1) = 200
        //레벨 2일떈 100 + (100 * 2) = 300 등등

        //업그레이드 성공 비율 산출
        a_Random = Random.Range(0, 6); //0~5
        float a_Chance = ((float)a_grade / 6.0f) * 100.0f;
        a_RandColor = Random.Range(0, 3); //0.1.2 3개만

        //UI에 안내한다.
        m_GemIconImage.sprite = m_GemImgs[a_RandColor]; //필요 보석의 색
        a_GemCount =(Random.Range(8, 16)) - GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade;
        m_GemTxt.text = "X " + a_GemCount;//필요 보석의 갯수
        m_CoinTxt.text = "X " + a_NewPrice; //필요 동전의 갯수
        m_HelpTxt .text = "["+ a_Chance.ToString("N0") + "% 확률]로 " + a_grade + "등급 승격"; //성공확률과 강화 등급

    }

    void TryUpgrade(SkillType a_Type)
    {
        m_HelpPanel.gameObject.SetActive(false);
        m_InitialTextObj.gameObject.SetActive(true);

        Text a_HelpTxt = m_InitialTextObj.GetComponent<Text>();



        //돈이 부족한지 판단
        if (GlobalValue.g_UserGold < a_NewPrice)
        {
            a_HelpTxt.text = "보유 골드가 \n 충분하지 않습니다.";
            m_TargetSlot.m_MyImg.sprite = null;
            m_TargetSlot.m_CurItemIdx = -1;
            m_ShowTime = 3.0f;
            return;
        }
        else
        {
            GlobalValue.g_UserGold -= a_NewPrice;
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
            GlobalValue.LoadGlobalValueData();
            m_StoreManager.RefreshInfoHud();

        }
        //보석이 부족한지 판단
        // a_RandColor[0] = blue / [1] = green / [2] = red
        if (a_RandColor == 0)
        {
            if (GlobalValue.g_UserBlueGem < a_GemCount)
            {
                a_HelpTxt.text = "파란 보석의 갯수가 \n 충분하지 않습니다.";
                m_TargetSlot.m_MyImg.sprite = null;
                m_TargetSlot.m_CurItemIdx = -1;
                m_ShowTime = 3.0f;
                return;
            }
            else
            {
                GlobalValue.g_UserBlueGem -= a_GemCount;
                PlayerPrefs.SetInt("BlueGem", GlobalValue.g_UserBlueGem);
                GlobalValue.LoadGlobalValueData();
                m_StoreManager.RefreshInfoHud();
            }
        }
        else if (a_RandColor == 1)
        {
            if (GlobalValue.g_UserGreenGem < a_GemCount)
            {
                a_HelpTxt.text = "초록 보석의 갯수가 \n 충분하지 않습니다.";
                m_TargetSlot.m_MyImg.sprite = null;
                m_TargetSlot.m_CurItemIdx = -1;
                m_ShowTime = 3.0f;
                return;
            }
            else
            {
                GlobalValue.g_UserGreenGem -= a_GemCount;
                PlayerPrefs.SetInt("GreenGem", GlobalValue.g_UserGreenGem);
                GlobalValue.LoadGlobalValueData();
                m_StoreManager.RefreshInfoHud();
            }
        }
        else
        {
            if (GlobalValue.g_UserRedGem < a_GemCount)
            {
                a_HelpTxt.text = "붉은 보석의 갯수가 \n 충분하지 않습니다.";
                m_TargetSlot.m_MyImg.sprite = null;
                m_TargetSlot.m_CurItemIdx = -1;
                m_ShowTime = 3.0f;
                return;
            }
            else
            {
                GlobalValue.g_UserRedGem -= a_GemCount;
                PlayerPrefs.SetInt("RedGem", GlobalValue.g_UserRedGem);
                GlobalValue.LoadGlobalValueData();
                m_StoreManager.RefreshInfoHud();
            }
        }

        if (a_grade > a_Random) //0= 100%, 1= 83%, 2= 66%, 3= 50%, 4= 33%, 5= 16%;
        {
            //등급업 성공
            a_HelpTxt.text = "레벨업 성공!" + "\n" + a_grade + "등급이 되었습니다.";
            GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade--;
            //m_ShowTime = 3.0f;
            
        }
        else  //등급업 실패
        {
            //다시 7등급으로 초기화
            GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade = 7;
            a_HelpTxt.text = "등급업 실패..." + "\n" + "다시 " + "7등급이 되었습니다.";
           // m_ShowTime = 3.0f;
        }

        m_TargetSlot.m_MyImg.sprite = null;
        m_TargetSlot.m_CurItemIdx = -1;
        m_ShowTime = 3.0f;

        //플레이어 프리팹에 변경된 등급 저장
        string a_KeyBuff = string.Format("Skill_{0}_Grade", (int)a_Type);
        PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade);

        m_StoreManager.RefreshInfoHud();

        switch (m_TargetType)
        {
            case SkillType.Skill_6:
                //폭발 데미지 적음
                break;

            case SkillType.Skill_7:
                //잔존체력 20%미만일때 탄환 데미지 적음
                break;

            case SkillType.Skill_8:
                //1.5배 많은 탄창
                break;

            case SkillType.Skill_9:
                //15%큰 피통
                break;

            case SkillType.Skill_10:
                //20%빠른 연사
                break;

            case SkillType.Skill_11:
                //다음 스테이지에서 체력 재충전
                break;

            default:
                break;
        }
    }

    
}
