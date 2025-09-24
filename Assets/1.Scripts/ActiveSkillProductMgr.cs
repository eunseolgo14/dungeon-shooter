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

    //��ư Ŭ���� ���� �ǻ� Ȯ��
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
        //��� ������ ��ư ���� ���� �� ����
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
                m_AskPriceTxt.text = "����: " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price.ToString() + "��";
            }
        }
    }

    public void InitData(SkillType a_SkType)
    {
        if (a_SkType < SkillType.Skill_0 || SkillType.SkillCount <= a_SkType)
        {
            Debug.Log((int)a_SkType);
            return; //enum���� ���� ���� ���� ���´� => ����
        }

        m_SkType = a_SkType;
        m_SkIconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
        m_SkillNameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillName + "]";
        m_HelpTxt.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillExp;
    }

    void AskBuy()
    {
        //�ٸ����� ������ �򰥸��� �� ���� ����
        ActiveSkillProductMgr[] a_productList = GameObject.FindObjectsOfType<ActiveSkillProductMgr>();
        if (0 < a_productList.Length)
        {
            for (int i = 0; i < a_productList.Length; i++)
            {
                a_productList[i].m_SelectObj.SetActive(false);
            }
        }

        //���� ȭ�� â�� ������ �Ǿ� ������ ���������� ���� ����
        if (m_SelectObj != null && m_SelectObj.activeSelf == false)
        {
            m_SelectObj.SetActive(true);

            //Ȯ��â�� �ٽ��ѹ� �� ���� ǥ��
            m_AskIconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
            m_AskNameTxt.text = "[" + GlobalValue.m_SkillInfoList[(int)m_SkType].m_SkillName + "]";
            m_AskPriceTxt.text = "����: " + GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price.ToString() + "��";
        }
    }

    void ProceedBuy()
    {

        if (CheckBag() == false)
        {
            return;
        }
        //�÷��̾��� ���� ����� Ȯ��, ���� ����ϴ� �����ۺ��� ���� ��� ����
        if (GlobalValue.g_UserGold < GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price)
        {
            m_AskPriceTxt.text = "<color=#FF004F>�ܾ� ����</color>";
            m_ShowTime = 2.0f;
        }
        else //(GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price <= GlobalValue.g_UserGold)
        {
            //���� ����
            GlobalValue.g_UserGold -= GlobalValue.m_SkillInfoList[(int)m_SkType].m_ActiveSk_Price;

            //�÷��̾� �����տ� ���� �׼� ����
            PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

            //�ش� �������� ���� ���� ����
            GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount++;

            //�÷��̾� �����տ� ���� ���� ����
            string a_KeyBuff = string.Format("Skill_{0}_Count", (int)m_SkType);
            PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount);

            //�ش� ������ ������ ó���̶�� => ��� ����
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
            else //�ش� �������� 1���̻� ���� ���̸� => ���� ����
            {
                //�� ������ SkillInvenNodeManager�� �˻� => ���� ���� ����
                SkillInvenNodeManager[] a_InvenSkillList = GameObject.FindObjectsOfType<SkillInvenNodeManager>();

                if (0 < a_InvenSkillList.Length)
                {
                    for (int i = 0; i < a_InvenSkillList.Length; i++)
                    {
                        //���Ÿ� ���� �� �����ۿ� �ش��ϴ� ���� �������� UI����
                        if (a_InvenSkillList[i].m_SkType == m_SkType)
                        {
                            a_InvenSkillList[i].InitData(m_SkType);
                        }
                    }
                }

            }


            GameObject.FindObjectOfType<StoreManager>().RefreshInfoHud();
            //GameObject.FindObjectOfType<SkillInvenNodeManager>().RefreshState();


            m_AskPriceTxt.text = "<color=#009AAE>���� ����</color>";
            m_ShowTime = 2.0f;
        }
        m_SelectObj.SetActive(false);
    }

    bool CheckBag()
    {
        //1.���� �������� ��ų�� ���� ��´�
        int a_Buf = 0;

        for (int i = 0; i < GlobalValue.m_SkillInfoList.Count; i++)
        {
            if (0 < GlobalValue.m_SkillInfoList[i].m_MyCount)
            {
                a_Buf += GlobalValue.m_SkillInfoList[i].m_MyCount;
            }
        }

        print(a_Buf);

        //2.20�� �ʰ����� �ʾ����� (19����) ���� ����
        if (a_Buf < 20)
        {
            return true;
        }
        //3. 20�� ���ų� �׺��� ũ�ٸ� ���� �Ұ�
        else
        {
            return false;
        }
    }
}
