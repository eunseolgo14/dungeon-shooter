using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBtnManager : MonoBehaviour
{
    public Button m_ThisBtn = null;
    public Image m_SkillIconImg = null;
    public Text m_MyCount = null;
    public GameObject m_SelectMark = null;

    //---  ���콺 ȣ���� �� ���� â ���� ����
    public GameObject m_ToolTipFab = null;
    public GameObject m_ToolTipObj = null;
    public Transform m_ThisCanvasTr = null;
    Vector3 m_RightPos = new Vector3(128.0f, 0.0f, 0.0f);
    Vector3 m_LeftPos = new Vector3(-128.0f, 0.0f, 0.0f);


    public SkillType m_SkType = SkillType.SkillCount;

    //--- ��ų ��� ���� ����
    UseSkillManager m_SkillMrg = null;
    public GameObject m_CoolTimeObj = null;
    public Image m_CoolGraph = null;
    float m_TotalCoolTime = 0.0f;
    public float m_RemainTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_SkillMrg = GameObject.FindObjectOfType<UseSkillManager>();

        m_ThisBtn.onClick.AddListener(() =>
        {
            m_SkillMrg.UseSkill(m_SkType);

            //����� ��ų�� ���� ����
            GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount--;

            //�� �ڽ��� ������ 0���� �Ǹ� ����(������ ����)
            if (GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount <= 0 && m_ToolTipObj != null)
            {
                Destroy(m_ToolTipObj);
                Destroy(this.gameObject);
            }

            //�������� ���� Ű���� ����
            string a_KeyBuff = string.Format("Skill_{0}_Count", (int)m_SkType);
             
            //������ ��ų ���� ���ÿ� ����
            PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount);

            //����� ���� �۷ι����翡 �ҷ��͵α�
            GlobalValue.LoadGlobalValueData();

            //UI�� �ٲ� ���� ���
            m_MyCount.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString();

            SetCoolTime(GlobalValue.m_SkillInfoList[(int)m_SkType].m_CoolTime);


        });

        //���콺 ������ ������ ����
        EventTrigger a_Trigger = this.GetComponent<EventTrigger>();

        EventTrigger.Entry a_Entry = new EventTrigger.Entry();
        a_Entry.eventID = EventTriggerType.PointerEnter;
        a_Entry.callback.AddListener((data) =>
        {
            if (m_ToolTipObj == null)
            {
                if (0.0f < m_RemainTime)
                {
                    return;
                }

                m_SelectMark.SetActive(true);
                CreatToolTip();
            }
            
        });

        a_Trigger.triggers.Add(a_Entry);

        //���콺 ����Ʈ�� �������� ����
        a_Entry = new EventTrigger.Entry();
        a_Entry.eventID = EventTriggerType.PointerExit;
        a_Entry.callback.AddListener((data) =>
        {
            if (m_ToolTipObj != null)
            {
                m_SelectMark.SetActive(false);
                Destroy(m_ToolTipObj);
            }
        });

        a_Trigger.triggers.Add(a_Entry);
    }

   
    void Update()
    {
        CoolTimeUpdate();
    }

    public void InitSkillBtn(SkillType a_SkType)
    {
        if (a_SkType < SkillType.Skill_0 || SkillType.SkillCount <= a_SkType)
        {
            return; //enum���� ���� ���� ���� ���´� => ����
        }

        m_SkType = a_SkType;
        m_SkillIconImg.sprite = GlobalValue.m_SkillInfoList[(int)m_SkType].m_IconImg;
        m_MyCount.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString();
    }

    public void CreatToolTip()
    {
        if (m_ToolTipFab != null || m_ToolTipObj == null)
        {
            
            if (m_ThisCanvasTr == null)
            {
              
                m_ThisCanvasTr = GameObject.Find("Skill_HUD").GetComponent<Transform>();
                if (m_ThisCanvasTr == null)
                {
                    Debug.Log("��ų ��� ã�� ����");
                }
            }

            m_ToolTipObj = Instantiate(m_ToolTipFab) as GameObject;
            m_ToolTipObj.transform.SetParent(m_ThisCanvasTr, false);

            if ((int)m_SkType < 3) //<--- ������ ��ġ��
            {
                m_ToolTipObj.transform.position = (this.transform.position + m_RightPos);//<-- ������ ���
            }
            else
            {
                m_ToolTipObj.transform.position = (this .transform.position + m_LeftPos);//<-- ������ ���
            }

            ToolTipManager a_ToolMgr = m_ToolTipObj.GetComponent<ToolTipManager>();

            if (a_ToolMgr != null)
            {
                a_ToolMgr.InitToolTip(m_SkType);
            }
        }
    }

    public void SetCoolTime(float a_CoolTime)
    {
        m_TotalCoolTime = a_CoolTime;
        m_RemainTime = m_TotalCoolTime;

        this.GetComponent<Button>().interactable = false;

        if (m_CoolTimeObj != null)
        {
            m_CoolTimeObj.SetActive(true);
            m_CoolGraph.fillAmount = 1.0f;
        }

    }

    void CoolTimeUpdate()
    {
        if (m_RemainTime <= 0.0f)
        {
            return;
        }

        m_RemainTime -= Time.deltaTime;

        m_CoolGraph.fillAmount = m_RemainTime / m_TotalCoolTime;

        //��Ÿ���� �� ���Ҵ��� Ȯ��
        if (m_RemainTime <= 0.0f)
        {
            //���� m_MyCount�� 0�� �Ǹ�
            if (GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount <= 0 && m_ToolTipObj != null)
            {
                m_SelectMark.SetActive(false);
                //��ư �ڽ� ����
                Destroy(this.gameObject);
                //������ ���� ����
                Destroy(m_ToolTipObj);

            }
            else //0�� �ƴ϶��
            {
                m_CoolTimeObj.SetActive(false);
                this.GetComponent<Button>().interactable = true;
            }
        }
    }
}
