using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyInfoTipManager : MonoBehaviour
{
    //Info Tip Object
    public GameObject m_InfoTipObj = null;
    public Text m_InfoTxt = null;
    //몇번째 이미지위에 호버링 중인지
    int m_Idx = -1;

    int m_HeroIdx = -1;
    LobbyManager m_LobbyMgr;

    Vector3[] m_PositionList = { new Vector3(), new Vector3(), new Vector3() };

    void Start()
    {

        m_LobbyMgr = GameObject.FindObjectOfType<LobbyManager>();
        //LoadGunInfo();

        if (m_Idx == -1)
        {
            int.TryParse(this.gameObject.name.Split('_')[1], out m_Idx);
        }



        //print(m_Idx);

        //포인트 오버의 이벤트 타입 엔트리를 저장
        EventTrigger a_Trigger = this.GetComponent<EventTrigger>();
        EventTrigger.Entry a_Entry = new EventTrigger.Entry();
        a_Entry.eventID = EventTriggerType.PointerEnter;

        //콜백 함수 연결
        a_Entry.callback.AddListener((data) =>
        {
            //인포팁 제 위치에 활성화
            ShowInfoTip();
        });

        a_Trigger.triggers.Add(a_Entry);

        //포인트 엑시트의 이벤트 타입 엔트리를 저장
        a_Entry = new EventTrigger.Entry();
        a_Entry.eventID = EventTriggerType.PointerExit;

        //콜백 함수 연결
        a_Entry.callback.AddListener((data) =>
        {
            //인포팁 비활성화 후 원위치 복귀
            HideInfoTip();

            //꺼졌다가 켜졌다가가 반복했던 이유:
            //인포팁 오브젝트의 레이케스트를 끄지 않아서 포인트엔터 => 인포팁 열림 => 레이케스트 막음
            //=> 인포팁 열리자마자 포인트 엑시트 가 반복
            //해결: 인포팁의 레이케스트 타겟을 다 꺼주었더니 정장적으로 엔터, 호버링, 엑시트 전부 감지
        });

        a_Trigger.triggers.Add(a_Entry);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_InfoTipObj.gameObject.activeSelf == true)
        {
            m_InfoTipObj.transform.position = Input.mousePosition + new Vector3(215.0f, -130.0f, 0.0f);
        }
    }


    void ShowInfoTip()
    {
        //print("ShowInfoTip 호출 : " + m_Idx);
        if (m_InfoTipObj.gameObject.activeSelf == false)
        {
            //print(m_LobbyMgr.m_HeroIdx + " : " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_GunName);
            InitInfo();
            m_InfoTipObj.gameObject.SetActive(true);
            m_InfoTipObj.transform.position = Input.mousePosition;
        }

        
    }

    void HideInfoTip()
    {

        if (m_InfoTipObj.gameObject.activeSelf == true)
        {
            m_InfoTipObj.gameObject.SetActive(false);
        }

        //텍스트 초기화
        if (m_InfoTxt != null)
        {
            m_InfoTxt.text = "";
        }
    }

    //List<Gun_Info> m_GunInfoList = new List<Gun_Info>();
    //void LoadGunInfo()
    //{
    //    Gun_Info a_GunInfo = new Gun_Info();

    //    for (int i = 0;  i < 3; i++)
    //    {
    //        a_GunInfo.SetGunInfo((GunType)i);
    //        m_GunInfoList.Add(a_GunInfo);
    //    }
    //}

    void InitInfo()
    {
        m_HeroIdx = m_LobbyMgr.m_HeroIdx;
        

        //올바른텍스트 출력
        if (m_InfoTxt != null)
        {
            string a_Mess = "";

            switch (m_Idx)
            {
                case 0:
                    {
                        //print(m_HeroIdx + " : " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_GunName);

                        a_Mess = "이름: " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_GunName + "\n" +
                                 "사거리: " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_Reach + "미터\n" +
                                 "장탄수: " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_MagazineSize + "발";
                       

                    }
                    break;

                case 1:
                    {
                        switch (m_LobbyMgr.m_HeroIdx)
                        {
                            case 0:
                                a_Mess = "[철갑옷]" + "\n" + "폭발로 입는 피폭 데미지" + "\n" + "20% 적게 시작한다";
                                break;

                            case 1:
                                a_Mess = "[큰탄창]" + "\n" + "다른 히어로보다 1.05배 많은" + "\n" + "탄환 장전 가능하다";
                                break;

                            case 2:
                                a_Mess = "[빠른 총알]" + "\n" + "다른 히어로보다 연사속도가" + "\n" + "20% 빠르게 시작한다";
                                break;
                        }
                        
                    }
                    break;

                case 2:

                    switch (m_LobbyMgr.m_HeroIdx)
                    {
                        case 0:
                            a_Mess = "[구사일생]" + "\n" + "체력이 20% 미만일 때" + "\n" + "피격 데미지 줄어든다";
                            break;

                        case 1:
                            a_Mess = "[초인적 힘]" + "\n" + "최대체력이 15%" + "\n" + "높은 상태에서 시작한다";
                            break;

                        case 2:
                            a_Mess = "[재충전]" + "\n" + "다음 스테이지로 넘어갈 때" + "\n" + "50퍼센트 체력 충전된다";
                            break;
                    }
                    
                    break;
            }

            m_InfoTxt.text = a_Mess;
        }
    }
}
