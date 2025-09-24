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
    //���° �̹������� ȣ���� ������
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

        //����Ʈ ������ �̺�Ʈ Ÿ�� ��Ʈ���� ����
        EventTrigger a_Trigger = this.GetComponent<EventTrigger>();
        EventTrigger.Entry a_Entry = new EventTrigger.Entry();
        a_Entry.eventID = EventTriggerType.PointerEnter;

        //�ݹ� �Լ� ����
        a_Entry.callback.AddListener((data) =>
        {
            //������ �� ��ġ�� Ȱ��ȭ
            ShowInfoTip();
        });

        a_Trigger.triggers.Add(a_Entry);

        //����Ʈ ����Ʈ�� �̺�Ʈ Ÿ�� ��Ʈ���� ����
        a_Entry = new EventTrigger.Entry();
        a_Entry.eventID = EventTriggerType.PointerExit;

        //�ݹ� �Լ� ����
        a_Entry.callback.AddListener((data) =>
        {
            //������ ��Ȱ��ȭ �� ����ġ ����
            HideInfoTip();

            //�����ٰ� �����ٰ��� �ݺ��ߴ� ����:
            //������ ������Ʈ�� �����ɽ�Ʈ�� ���� �ʾƼ� ����Ʈ���� => ������ ���� => �����ɽ�Ʈ ����
            //=> ������ �����ڸ��� ����Ʈ ����Ʈ �� �ݺ�
            //�ذ�: �������� �����ɽ�Ʈ Ÿ���� �� ���־����� ���������� ����, ȣ����, ����Ʈ ���� ����
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
        //print("ShowInfoTip ȣ�� : " + m_Idx);
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

        //�ؽ�Ʈ �ʱ�ȭ
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
        

        //�ùٸ��ؽ�Ʈ ���
        if (m_InfoTxt != null)
        {
            string a_Mess = "";

            switch (m_Idx)
            {
                case 0:
                    {
                        //print(m_HeroIdx + " : " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_GunName);

                        a_Mess = "�̸�: " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_GunName + "\n" +
                                 "��Ÿ�: " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_Reach + "����\n" +
                                 "��ź��: " + m_LobbyMgr.m_GunInfoList[m_LobbyMgr.m_HeroIdx].m_MagazineSize + "��";
                       

                    }
                    break;

                case 1:
                    {
                        switch (m_LobbyMgr.m_HeroIdx)
                        {
                            case 0:
                                a_Mess = "[ö����]" + "\n" + "���߷� �Դ� ���� ������" + "\n" + "20% ���� �����Ѵ�";
                                break;

                            case 1:
                                a_Mess = "[ūźâ]" + "\n" + "�ٸ� ����κ��� 1.05�� ����" + "\n" + "źȯ ���� �����ϴ�";
                                break;

                            case 2:
                                a_Mess = "[���� �Ѿ�]" + "\n" + "�ٸ� ����κ��� ����ӵ���" + "\n" + "20% ������ �����Ѵ�";
                                break;
                        }
                        
                    }
                    break;

                case 2:

                    switch (m_LobbyMgr.m_HeroIdx)
                    {
                        case 0:
                            a_Mess = "[�����ϻ�]" + "\n" + "ü���� 20% �̸��� ��" + "\n" + "�ǰ� ������ �پ���";
                            break;

                        case 1:
                            a_Mess = "[������ ��]" + "\n" + "�ִ�ü���� 15%" + "\n" + "���� ���¿��� �����Ѵ�";
                            break;

                        case 2:
                            a_Mess = "[������]" + "\n" + "���� ���������� �Ѿ ��" + "\n" + "50�ۼ�Ʈ ü�� �����ȴ�";
                            break;
                    }
                    
                    break;
            }

            m_InfoTxt.text = a_Mess;
        }
    }
}
