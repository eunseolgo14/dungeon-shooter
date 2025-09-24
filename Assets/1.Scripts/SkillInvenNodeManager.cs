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
                    m_PriceTxt.text = "���� : " + a_NewPrice.ToString("N0") + "��";
                    m_RemainCount.text = "���� :  " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString() + "��";
                }
            });
        }

        if (m_YesBtn != null)
        {
            //�ǸŹ�ư ������ ��
            m_YesBtn.onClick.AddListener(() =>
            {
                //refresh InvenScroll
                m_SellConfirmObj.gameObject.SetActive(false);
                //��ȭ ����
                float a_NewPrice = GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price;
                a_NewPrice *= 0.75f;

                GlobalValue.g_UserGold += (int)a_NewPrice;
                //�÷��̾� �����տ� ���� �׼� ����
                PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

                //������ ����
                GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount--;
                //�÷��̾� �����տ� ���� ���� ����
                string a_KeyBuff = string.Format("Skill_{0}_Count", (int)m_SkType);
                //string a_KeyBuff = string.Format("Skill_Item_{0}", (int)m_SkType); //<= �̷��� Ű ���� �ٸ��� ����� ������ �� �� ��,
                PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount);

                //ui����
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
            return; //enum���� ���� ���� ���� ���´� => ����
        }

        m_SkType = a_SkType;
        m_IconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
        m_CountTxt.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString();
        //m_HelpTxt.text = GlobalValue.m_SkDataList[(int)m_SkType].m_SkillExp;
        m_NameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillName + "]";
        m_RemainCount.text = "���� :  " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString() + "��";
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
