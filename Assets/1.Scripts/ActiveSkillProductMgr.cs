using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveSkillProductMgr : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType = SkillType.SkillCount;

    //public Text m_LvTxt;
    public Image m_SkIconImg;
    public Text m_SkillNameTxt;
    public Text m_HelpTxt;
    //public Text m_BuyText;

    //버튼 클릭시 구매 의사 확인
    public Button m_ThisButton = null;
    public Image m_AskIconImg = null;
    public Text m_AskNameTxt = null;
    public Text m_AskPriceTxt = null;
    public GameObject m_SelectObj = null;
    public Button m_BuyBtn = null;
    public Button m_CancelBtn = null;

    float m_ShowTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {

        if (m_ThisButton != null)
        {
            m_ThisButton.onClick.AddListener(AskBuy);
        }

        if (m_BuyBtn != null)
        {
            
            m_BuyBtn.onClick.AddListener(ProceedBuy);
            
            
        }

        if (m_CancelBtn != null)
        {
            m_CancelBtn.onClick.AddListener(() =>
            {
                m_SelectObj.SetActive(false);
            });
        }
    }
    private void OnEnable()
    {
        //모든 아이템 버튼 선택 해제 후 시작
        ActiveSkillProductMgr[] a_productList = GameObject.FindObjectsOfType<ActiveSkillProductMgr>();
        if (0 < a_productList.Length)
        {
            for (int i = 0; i < a_productList.Length; i++)
            {
                a_productList[i].m_SelectObj.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_ShowTime)
        {
            m_ShowTime -= Time.deltaTime;

            if (m_ShowTime <= 0.0f)
            {
                m_ShowTime = 0.0f;
                m_AskPriceTxt.text = "가격: " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price.ToString() + "원";
            }
        }
    }

    public void InitData(SkillType a_SkType)
    {
        if (a_SkType < SkillType.Skill_0 || SkillType.SkillCount <= a_SkType)
        {
            Debug.Log((int)a_SkType);
            return; //enum형의 범위 밖의 값이 들어온다 => 리턴
        }

        m_SkType = a_SkType;
        m_SkIconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
        m_SkillNameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillName + "]";
        m_HelpTxt.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillExp;
    }

    void AskBuy()
    {
        //다른켜진 아이템 헷갈리니 나 끄고 시작
        ActiveSkillProductMgr[] a_productList = GameObject.FindObjectsOfType<ActiveSkillProductMgr>();
        if (0 < a_productList.Length)
        {
            for (int i = 0; i < a_productList.Length; i++)
            {
                a_productList[i].m_SelectObj.SetActive(false);
            }
        }

        //선택 화면 창이 연결은 되어 있지만 켜져있지는 않은 상태
        if (m_SelectObj != null && m_SelectObj.activeSelf == false)
        {
            m_SelectObj.SetActive(true);

            //확인창에 다시한번 상세 정보 표시
            m_AskIconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
            m_AskNameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillName + "]";
            m_AskPriceTxt.text = "가격: " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price.ToString() + "원";
        }
    }

    void ProceedBuy()
    {

        if (CheckBag() == false)
        {
            return;
        }
        //플레이어의 보유 재산을 확인, 구매 희망하는 아이템보다 보유 재산 적음
        if (GlobalValue.g_UserGold < GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price)
        {
            m_AskPriceTxt.text = "<color=#FF004F>잔액 부족</color>";
            m_ShowTime = 2.0f;
        }
        else //(GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price <= GlobalValue.g_UserGold)
        {
            //돈을 차감
            GlobalValue.g_UserGold -= GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price;

            //플레이어 프리팹에 차감 액수 저장
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

            //해당 아이템의 보유 갯수 증가
            GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount++;

            //플레이어 프리팹에 증가 갯수 저장
            string a_KeyBuff = string.Format("Skill_{0}_Count", (int)m_SkType);
            PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount);

            //해당 아이템 보유가 처음이라면 => 노드 생성
            if (GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount <= 1)
            {
                SkillInvenNodeManager[] a_InvenSkillList = GameObject.FindObjectsOfType<SkillInvenNodeManager>();

                if (0 < a_InvenSkillList.Length)
                {
                    for (int i = 0; i < a_InvenSkillList.Length; i++)
                    {
                        Destroy(a_InvenSkillList[i].gameObject);
                    }
                }

                StoreManager a_StoreMgr = GameObject.FindObjectOfType<StoreManager>();
                a_StoreMgr.RefreshSkillInven();

            }
            else //해당 아이템을 1개이상 보유 중이면 => 갯수 증가
            {
                //씬 내에서 SkillInvenNodeManager를 검색 => 보유 갯수 갱신
                SkillInvenNodeManager[] a_InvenSkillList = GameObject.FindObjectsOfType<SkillInvenNodeManager>();

                if (0 < a_InvenSkillList.Length)
                {
                    for (int i = 0; i < a_InvenSkillList.Length; i++)
                    {
                        //구매를 누른 이 아이템에 해당하는 보유 아이템의 UI갱신
                        if (a_InvenSkillList[i].m_SkType == m_SkType)
                        {
                            a_InvenSkillList[i].InitData(m_SkType);
                        }
                    }
                }

            }


            GameObject.FindObjectOfType<StoreManager>().RefreshInfoHud();
            //GameObject.FindObjectOfType<SkillInvenNodeManager>().RefreshState();


            m_AskPriceTxt.text = "<color=#009AAE>구매 성공</color>";
            m_ShowTime = 2.0f;
        }
        m_SelectObj.SetActive(false);
    }

    bool CheckBag()
    {
        //1.현재 보유중인 스킬의 수를 얻는다
        int a_Buf = 0;

        for (int i = 0; i < GlobalValue.m_SkillInfoList.Count; i++)
        {
            if (0 < GlobalValue.m_SkillInfoList[i].m_MyCount)
            {
                a_Buf += GlobalValue.m_SkillInfoList[i].m_MyCount;
            }
        }

        print(a_Buf);

        //2.20을 초과하지 않았으면 (19까지) 구매 가능
        if (a_Buf < 20)
        {
            return true;
        }
        //3. 20과 같거나 그보다 크다면 구매 불가
        else
        {
            return false;
        }
    }
}
