using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInvenNodeManager : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType = SkillType.SkillCount;

    public Image m_IconImg = null;
    public Text m_CountTxt = null;
    public Text m_NameTxt = null;
    public Button m_SellBtn = null;

    public GameObject m_SellConfirmObj = null;
    public Text m_PriceTxt = null;
    public Text m_RemainCount = null;
    public Button m_YesBtn = null;
    public Button m_NoBtn = null;

    void Start()
    {
        if (m_SellBtn != null)
        {
            m_SellBtn.onClick.AddListener(() =>
            {
                if (m_SellConfirmObj != null)
                {
                    m_SellConfirmObj.gameObject.SetActive(true);
                    float a_NewPrice = GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price;
                    a_NewPrice *= 0.75f;
                    m_PriceTxt.text = "가격 : " + a_NewPrice.ToString("N0") + "원";
                    m_RemainCount.text = "수량 :  " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString() + "개";
                }
            });
        }

        if (m_YesBtn != null)
        {
            //판매버튼 눌렀을 때
            m_YesBtn.onClick.AddListener(() =>
            {
                //refresh InvenScroll
                m_SellConfirmObj.gameObject.SetActive(false);
                //재화 증가
                float a_NewPrice = GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price;
                a_NewPrice *= 0.75f;

                GlobalValue.g_UserGold += (int)a_NewPrice;
                //플레이어 프리팹에 차감 액수 저장
                PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

                //아이템 차감
                GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount--;
                //플레이어 프리팹에 증가 갯수 저장
                string a_KeyBuff = string.Format("Skill_{0}_Count", (int)m_SkType);
                //string a_KeyBuff = string.Format("Skill_Item_{0}", (int)m_SkType); //<= 이렇게 키 값을 다르게 만들어 저장이 안 된 것,
                PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount);

                //ui갱신
                GameObject.FindObjectOfType<StoreManager>().RefreshInfoHud();
                RefreshState();
            });
        }

        if (m_NoBtn != null)
        {
            m_NoBtn.onClick.AddListener(() =>
            {
                m_SellConfirmObj.gameObject.SetActive(false);
            });
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitData(SkillType a_SkType)
    {
        if (a_SkType < SkillType.Skill_0 || SkillType.SkillCount <= a_SkType)
        {
            return; //enum형의 범위 밖의 값이 들어온다 => 리턴
        }

        m_SkType = a_SkType;
        m_IconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
        m_CountTxt.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString();
        //m_HelpTxt.text = GlobalValue.m_SkDataList[(int)m_SkType].m_SkillExp;
        m_NameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillName + "]";
        m_RemainCount.text = "수량 :  " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString() + "개";
    }

    public void RefreshState()
    {


        if (GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_CountTxt.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString();
            
        }
    }
}
