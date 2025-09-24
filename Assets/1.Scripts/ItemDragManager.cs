using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDragManager : MonoBehaviour
{
    public SlotManager m_TargetSlot = null;
    public SlotManager[] m_SkillSlots = null;

    public GameObject m_MouseImg = null;
    int m_SaveIdx = -1; //<--- -1�� �ƴ϶�� ������ ��ŷ �� �巡�� ����
    public SkillType m_TargetType;

    //--- ��ȭ ���� �ɼ� ����
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
        //��ȭâ�� �����ִ°� �ƴ϶�� ������ �巡�� �� �� ���� ����
        if (m_StoreManager.m_StoreNow == StoreStatus.Browse)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) == true) //<--- ���콺 ��Ŭ�� ����
        {
            MouseBtnDown();
        }

        if (Input.GetMouseButton(0) == true) //<--- ���콺 ��Ŭ�� �ϰ��ִ� ����
        {
            MouseBtnPress();
        }

        if (Input.GetMouseButtonUp(0) == true) //<--- ���콺 ��Ŭ������ �� ����
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
        m_InitialTextObj.GetComponent<Text>().text = "��ȭ�� ���Ͻô� �нú� ��ų��\n�巡���ؼ� ��ĭ�� �����ּ���";
    }

    private void MouseBtnDown()
    {
        m_SaveIdx = -1;
        for (int ii = 0; ii < m_SkillSlots.Length; ii++)
        {
            //for���� ���鼭 1�� �нú� ��ų�� Ŭ���Ѱ��� 2�� �нú� ��ų�� �������� ����

            if (IsCollSlot(m_SkillSlots[ii]) == true) //IsCollSlot(m_ProductSlots[ii]) == true => �ٸ� ���� �ƴ϶� ������ �������� Ȯ��
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
        if (0 <= m_SaveIdx) //<-- 0���� �۴ٴ� ���� �� ���� Ŭ���ߴٴ� ��
        {
            m_MouseImg.transform.position = Input.mousePosition;
        }
    }


    private void MouseBtnUp()
    {
        if (m_SaveIdx < 0 || m_SkillSlots.Length <= m_SaveIdx) //<-- ó������ ������ ��ŷ�� ����� �̷������ ���� ��Ȳ
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

            //�ش� �нú� ��ų�� ���׷��̵� ������ �м��ϴ� �Լ�
            UpgradeRequirements(m_TargetType);

        }
        
       
        m_SaveIdx = -1; //<--- �������� �̷�����ٸ� �ٽ� �ʱ�ȭ
        m_MouseImg.gameObject.SetActive(false);
    }

    bool IsCollSlot(SlotManager a_CkCslot) //<--- �̰� �Լ��� ���� �� �θ� �Ź� ���콺 Ŭ�� ������ ���� ���� ������ �ۼ��� �ʿ� ����. 
    {
        //���콺�� ��� ���� UI���� �ִ����� �Ǵ��ϴ� �Լ�

        if (a_CkCslot == null)
        {
            return false;
        }

        Vector3[] v = new Vector3[4];
        a_CkCslot.GetComponent<RectTransform>().GetWorldCorners(v);
        //v[0]:���� �ϴ�(0,0), v[1]:���� ���, v[2]:���� ���(), v[3]:���� �ϴ�

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
        //���۰��� 6�̾�� ��
        a_grade = GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade - 1;

        if (a_grade <= 1) // ����� 1�� ���ų� ���� ��
        {
            m_InitialTextObj.GetComponent<Text>().text = "�̹� �ְ� ��޿� �����Ͽ�\n���̻��� ��޾��� �Ұ��մϴ�.";
            m_ShowTime = 3.0f;
            //�̹� �ְ� ���, ���̻��� ���� �Ұ�
            return;
        }

        m_InitialTextObj.gameObject.SetActive(false);
        m_HelpPanel.gameObject.SetActive(true);
        //m_ConfirmTextObj.gameObject.SetActive(true);

        //���׷��̵�� �ʿ��� ��ȭ ����
        a_NewPrice = GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Up_Price + (GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Up_Price * (7 - a_grade));

        //���� 0�϶� 100 + (100 * 1) = 200
        //���� 2�ϋ� 100 + (100 * 2) = 300 ���

        //���׷��̵� ���� ���� ����
        a_Random = Random.Range(0, 6); //0~5
        float a_Chance = ((float)a_grade / 6.0f) * 100.0f;
        a_RandColor = Random.Range(0, 3); //0.1.2 3����

        //UI�� �ȳ��Ѵ�.
        m_GemIconImage.sprite = m_GemImgs[a_RandColor]; //�ʿ� ������ ��
        a_GemCount =(Random.Range(8, 16)) - GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade;
        m_GemTxt.text = "X " + a_GemCount;//�ʿ� ������ ����
        m_CoinTxt.text = "X " + a_NewPrice; //�ʿ� ������ ����
        m_HelpTxt .text = "["+ a_Chance.ToString("N0") + "% Ȯ��]�� " + a_grade + "��� �°�"; //����Ȯ���� ��ȭ ���

    }

    void TryUpgrade(SkillType a_Type)
    {
        m_HelpPanel.gameObject.SetActive(false);
        m_InitialTextObj.gameObject.SetActive(true);

        Text a_HelpTxt = m_InitialTextObj.GetComponent<Text>();



        //���� �������� �Ǵ�
        if (GlobalValue.g_UserGold < a_NewPrice)
        {
            a_HelpTxt.text = "���� ��尡 \n ������� �ʽ��ϴ�.";
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
        //������ �������� �Ǵ�
        // a_RandColor[0] = blue / [1] = green / [2] = red
        if (a_RandColor == 0)
        {
            if (GlobalValue.g_UserBlueGem < a_GemCount)
            {
                a_HelpTxt.text = "�Ķ� ������ ������ \n ������� �ʽ��ϴ�.";
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
                a_HelpTxt.text = "�ʷ� ������ ������ \n ������� �ʽ��ϴ�.";
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
                a_HelpTxt.text = "���� ������ ������ \n ������� �ʽ��ϴ�.";
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
            //��޾� ����
            a_HelpTxt.text = "������ ����!" + "\n" + a_grade + "����� �Ǿ����ϴ�.";
            GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade--;
            //m_ShowTime = 3.0f;
            
        }
        else  //��޾� ����
        {
            //�ٽ� 7������� �ʱ�ȭ
            GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade = 7;
            a_HelpTxt.text = "��޾� ����..." + "\n" + "�ٽ� " + "7����� �Ǿ����ϴ�.";
           // m_ShowTime = 3.0f;
        }

        m_TargetSlot.m_MyImg.sprite = null;
        m_TargetSlot.m_CurItemIdx = -1;
        m_ShowTime = 3.0f;

        //�÷��̾� �����տ� ����� ��� ����
        string a_KeyBuff = string.Format("Skill_{0}_Grade", (int)a_Type);
        PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)a_Type].m_PassiveSk_Grade);

        m_StoreManager.RefreshInfoHud();

        switch (m_TargetType)
        {
            case SkillType.Skill_6:
                //���� ������ ����
                break;

            case SkillType.Skill_7:
                //����ü�� 20%�̸��϶� źȯ ������ ����
                break;

            case SkillType.Skill_8:
                //1.5�� ���� źâ
                break;

            case SkillType.Skill_9:
                //15%ū ����
                break;

            case SkillType.Skill_10:
                //20%���� ����
                break;

            case SkillType.Skill_11:
                //���� ������������ ü�� ������
                break;

            default:
                break;
        }
    }

    
}
