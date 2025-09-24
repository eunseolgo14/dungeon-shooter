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

    public RawImage[] m_SelectImg = null; //0���� ������ ���� â, 1���� ������ ��ȭ â
    public GameObject[] m_ScrollViews; //0���� ������ ���� â, 1���� ������ ��ȭ â

    public Text m_UserInfoTxt = null;

    public GameObject m_Item_ScContent; //<--- Scroll Content�� �ڳ�� ������ �θ� ��ü
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

    //�ʱ� ����, ���� ���Ž� ���� _ �ٸ� �� ��Ȳ�� ���� �Լ� ���� ����Ŷ� �������� ����
    GameObject a_SkillObj = null;
    SkillInvenNodeManager a_SkInvenNode = null;
    ItemDragManager m_DragMgr = null;

    [Header("-----Debug Mode Panel-----")]
    public Button D_AddGoldBtn = null;
    public Button D_AddGemBtn = null;


    //���� �����ִ� ���� â ���� Ȯ�ο� ����(�̰� browse�� ���߿� �巡�� �� �ǰ� ���� ����)
    public StoreStatus m_StoreNow = StoreStatus.Browse;

    void Start()
    {
        m_DragMgr = FindObjectOfType<ItemDragManager>();
        Hero_Info.SetHero(GlobalValue.g_HeroType);
        RefreshInfoHud();

        //��Ƽ�� ��ų ������ â Ȱ��ȭ �� ����
        m_ScrollViews[0].gameObject.SetActive(true);
        m_SelectImg[0].gameObject.SetActive(true);

        GameObject a_ItemObj = null;
        ActiveSkillProductMgr a_SkItemNode = null;

        //���� �Ǹ����� ��Ƽ�� ��ų �����۷ε��Ѵ�

        for (int ii = 0; ii < 6; ii++) //(0-5������ ��Ƽ��)��ų �������� ������ŭ ��ü�� ����
        {
            a_ItemObj = (GameObject)Instantiate(m_Item_NodeObj);

            //��ų �������� Ÿ�Կ� �´� �ʱⰪ �����ϴ� �Լ� ȣ�� 
            a_SkItemNode = a_ItemObj.GetComponent<ActiveSkillProductMgr>();
            a_SkItemNode.InitData(GlobalValue.m_SkillInfoList[ii].m_SkillType);
            //=> �� ��ų�� �´� �̹����� ���� ���´�(����� ���� ������ �������� �Լ�)

            //��ũ�Ѻ��� ����Ʈ�� �ڳ�� ����
            a_ItemObj.transform.SetParent(m_Item_ScContent.transform, false);

            //����� ��� => 0, 1, 2 ������ ��ų�� Ŭ�� �����ϵ���
            if (2 < (int)a_SkItemNode.m_SkType)
            {
                a_ItemObj.GetComponent<Button>().interactable = false;
            }
        }

        #region ���� �� ������ ī�װ� ��ư Ŭ�� ����
        if (m_ActiveItemBtn != null)
        {
            m_ActiveItemBtn.onClick.AddListener(() =>
            {
                //�ٸ� â ������ ����
                m_ScrollViews[1].gameObject.SetActive(false);
                //m_ScrollViews[2].gameObject.SetActive(false);
                m_SelectImg[1].gameObject.SetActive(false);
                //m_SelectImg[2].gameObject.SetActive(false);

                m_ScrollViews[0].gameObject.SetActive(true);
                m_SelectImg[0].gameObject.SetActive(true);

                //���� ���� == ������ ������
                m_StoreNow = StoreStatus.Browse;
            });
        }

        if (m_PassiveItemBtn != null)
        {
            m_PassiveItemBtn.onClick.AddListener(() =>
            {
                //�ٸ� â ������ ����
                m_ScrollViews[0].gameObject.SetActive(false);
                //m_ScrollViews[2].gameObject.SetActive(false);
                m_SelectImg[0].gameObject.SetActive(false);
                //m_SelectImg[2].gameObject.SetActive(false);
                m_DragMgr.ResetUpgradePanel();

                m_ScrollViews[1].gameObject.SetActive(true);
                m_SelectImg[1].gameObject.SetActive(true);

                //���� ���� == ��ȭ â ���� ��
                m_StoreNow = StoreStatus.Strengthen;
            });
        }

        #endregion

        #region ���� �κ��丮 â�� �нú�, ��Ƽ�� ��ų �ε��ؼ� ���

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
                //�������� 1���� �κ������ ���°� (4)
                //���������� �κ������ ���� �� (5)

                Scene m_CurScene = SceneManager.GetActiveScene();
                GlobalValue.g_CurScene = m_CurScene.buildIndex; //=> 3���� ���

                SceneManager.LoadScene(2);

            });
        }

        //--- ����׸��� ��ư �Լ� ���
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
        //��尪, ���� ũ��, ���� ���� ���� � ���� ������ ������ ��� ȣ��
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
            //���� ������ 0�� ���ϸ� ��� �������� �ʽ��ϴ�
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
