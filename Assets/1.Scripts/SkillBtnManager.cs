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

    //---  마우스 호버링 중 툴팁 창 띄우는 변수
    public GameObject m_ToolTipFab = null;
    public GameObject m_ToolTipObj = null;
    public Transform m_ThisCanvasTr = null;
    Vector3 m_RightPos = new Vector3(128.0f, 0.0f, 0.0f);
    Vector3 m_LeftPos = new Vector3(-128.0f, 0.0f, 0.0f);


    public SkillType m_SkType = SkillType.SkillCount;

    //--- 스킬 사용 관련 변수
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

            //사용한 스킬의 갯수 차감
            GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount--;

            //나 자신의 갯수가 0개가 되면 삭제(툴팁도 삭제)
            if (GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount <= 0 && m_ToolTipObj != null)
            {
                Destroy(m_ToolTipObj);
                Destroy(this.gameObject);
            }

            //아이템의 고유 키값을 발행
            string a_KeyBuff = string.Format("Skill_{0}_Count", (int)m_SkType);
             
            //차감된 스킬 갯수 로컬에 저장
            PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount);

            //저장된 정도 글로벌벨루에 불러와두기
            GlobalValue.LoadGlobalValueData();

            //UI에 바뀐 갯수 출력
            m_MyCount.text = GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount.ToString();

            SetCoolTime(GlobalValue.m_SkillInfoList[(int)m_SkType].m_CoolTime);


        });

        //마우스 오버시 참으로 변경
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

        //마우스 엑시트시 거짓으로 변경
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
            return; //enum형의 범위 밖의 값이 들어온다 => 리턴
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
                    Debug.Log("스킬 허드 찾지 못함");
                }
            }

            m_ToolTipObj = Instantiate(m_ToolTipFab) as GameObject;
            m_ToolTipObj.transform.SetParent(m_ThisCanvasTr, false);

            if ((int)m_SkType < 3) //<--- 좌측에 위치함
            {
                m_ToolTipObj.transform.position = (this.transform.position + m_RightPos);//<-- 우측에 출력
            }
            else
            {
                m_ToolTipObj.transform.position = (this .transform.position + m_LeftPos);//<-- 우측에 출력
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

        //쿨타임이 다 돌았는지 확인
        if (m_RemainTime <= 0.0f)
        {
            //만약 m_MyCount가 0이 되면
            if (GlobalValue.m_SkillInfoList[(int)m_SkType].m_MyCount <= 0 && m_ToolTipObj != null)
            {
                m_SelectMark.SetActive(false);
                //버튼 자신 삭제
                Destroy(this.gameObject);
                //툴팁도 같이 삭제
                Destroy(m_ToolTipObj);

            }
            else //0이 아니라면
            {
                m_CoolTimeObj.SetActive(false);
                this.GetComponent<Button>().interactable = true;
            }
        }
    }
}
