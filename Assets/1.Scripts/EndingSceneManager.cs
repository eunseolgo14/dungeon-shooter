using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
{
    public Text m_DialogueTxt;
    string[] m_LineLists =
                        { "�׷�, ���� ����. ����� ������ ���� �ھ�.",
                          "������ ���� �༮�� �����°� �������̶� ��ô�̳� ��ſ���.",
                          "����� �䱸�� ����ֵ��� ����. �� �ΰ��� �ǰ�ʹٰ� ����?",
                          "<color=grey>�� ���� �ð� ������ ������ �ִ� ������ ����̾�.</color>",
                          "<color=grey>���� �� �������� �ΰ� �մ��� �ִµ� �׿� ���� �ϰ�;�.</color>" ,
                          "<color=grey>���ݺ��� �� ���������;�.</color>",
                          "�׷���. ������ �ΰ��� �ȴٰ� �� ���� �귯���� �����ž�." ,
                          "��ȸ�� ���� ����.",
                          "<color=grey>��, ��ȸ���� �����ž�.</color>",
                          "�����ڴ� ������ ���ٸ���.",
                          "�� ������ ���Դ� �� �� �״�� ���������� ��.",
                          "���� ������ ������ ���� ����� �ΰ��� �Ǿ������ž�.",
                          "��ſ���."}; //length == 13


    public Button m_NextBtn;

    //������ �´� ��� ����� ���� ����
    int m_Idx = 0;
    float m_ShowTime = 2.0f;

    public GameObject m_MonSprObj;
    //���� ���� ������ ���� ����
    SpriteRenderer m_MonSpr;
    float m_ColorA = 1.0f;
    public GameObject m_FinalEndingObj;

    //�ɾ ������ ���� ������Ʈ ����
    public GameObject m_LadyObj;
    bool m_IsLadyWalk = false;

    //���� ������Ʈ �ȱ� ���� ����
    public GameObject m_ManObj;
    bool m_IsManWalk = false;

    //������ ���� ȭ�� ������ ���� ����
    public GameObject m_DialogueBoxObj;
    public GameObject m_ThankyouObj;

    //������Ʈ�� ������������ ����
    bool m_IsFading = false;
    bool m_IsEnding = false;

    //������ ȭ�� �ٽ��ϱ�, ���ư��� ��ư
    public Button m_ReplayBtn;
    public Button m_QuitBtn;
    void Start()
    {
        SoundManager.Instance.PlayBGM("Arguement");

        if (m_NextBtn != null)
        {
            m_NextBtn.onClick.AddListener(NextLine);
        }

        if (m_ReplayBtn != null)
        {
            m_ReplayBtn.onClick.AddListener(() =>
            {
                //Ÿ��Ʋ ȭ������ ����
                SceneManager.LoadScene(0);
            });
        }
        if (m_QuitBtn != null)
        {
            m_QuitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
            //����Ƽ ������ ���̶�� => play��� ����
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //�ƴ϶�� ������������
             Application.Quit();
#endif
            });
        }

        m_NextBtn.gameObject.SetActive(false);
        m_DialogueTxt.text = m_LineLists[m_Idx];
        m_MonSpr = m_MonSprObj.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //������ ����� ȿ�� ������Ʈ
        if (m_IsFading == true)
        {
            m_ColorA -= 0.007f;
            m_MonSpr.color = new Color(255, 255, 255, m_ColorA);
          

            if (m_MonSpr.color.a <= 0.01f)
            {
                m_IsFading = false;

                m_MonSprObj.gameObject.SetActive(false);

                m_IsEnding = true;
                m_Idx = 0;
                m_DialogueTxt.text = m_ConvoLists[m_Idx];
                m_ShowTime = 1.0f;
                m_FinalEndingObj.gameObject.SetActive(true);
            }
        }

        //������ ������ �ȱ� ���� ������Ʈ
        if (m_IsLadyWalk == true)
        {
            m_LadyObj.transform.Translate(Vector3.right * 0.06f);

            //������ �̹����� ȭ�� ������ �Ѿ��
            if (10.0f <= m_LadyObj.transform.position.x)
            {
                m_IsLadyWalk = false;
            }
        }
        if (m_IsManWalk == true)
        {
            m_ManObj.transform.Translate(Vector3.right * 0.06f);

            //������ �̹����� ȭ�� ������ �Ѿ��
            if (10.0f <= m_ManObj.transform.position.x)
            {
                m_IsManWalk = false;
                m_FinalEndingObj.gameObject.SetActive(false);
                m_DialogueBoxObj.gameObject.SetActive(false);
                m_ThankyouObj.gameObject.SetActive(true);
            }
            
        }
        

        //�⺻ ������Ʈ�� ��� ���
        MonSayUpdate();
    }

    void MonSayUpdate()
    {
        if (0.0f < m_ShowTime)
        {
            m_ShowTime -= Time.deltaTime;

            if (m_ShowTime <= 0.0f)
            {
                if (m_NextBtn.gameObject.activeSelf == false)
                {
                    m_NextBtn.gameObject.SetActive(true);
                }
            }
        }

        
    }

    void NextLine()
    {
        m_NextBtn.gameObject.SetActive(false);
        m_Idx++;

        if (m_IsEnding == false)
        {

            if (m_Idx == 13)
            {
                //���� ���� ���� �Լ�
                m_IsFading = true;
                m_NextBtn.gameObject.SetActive(false);
                
                return;
            }

            //������ �������� ��� ��ü
            m_DialogueTxt.text = m_LineLists[m_Idx];
        }
        else //(m_IsEnding == true)
        {
            if (m_Idx == 6)
            {
                m_LadyObj.GetComponent<SpriteRenderer>().flipX = false;
                m_LadyObj.GetComponent<Animator>().SetTrigger("IsLeave");
                m_IsLadyWalk = true;
            }
            if (m_Idx == 8)
            {
                //���� ���� ���� �Լ�
                m_IsManWalk = true;
                m_ManObj.GetComponent<Animator>().SetTrigger("IsLeave");
                return;
            }

            //������ �������� ��� ��ü
            m_DialogueTxt.text = m_ConvoLists[m_Idx];
        }

        
        //��Ÿ�� ����
        m_ShowTime = 1.0f;
    }


    string[] m_ConvoLists =
                        { "�����, Ȥ�� �� ����ϰ� �ֳ���?",
                          "����� ���� ���� �� �������� �� ����̿���.",
                          "�� ��Ű� ��ȭ�� ������ �;��µ� ��� �ΰ��� �Ǿ����.",
                          "<color=white>����� �� ����̶���? �ٺ����� ������ ���� ������.</color>",
                          "<color=white>�� ����� �ƴ� �ٸ� �� � ����ü���� ������ �����.</color>" ,
                          "<color=white>Ư�� ������ �ΰ����Դ� �������̿�.</color>",
                          "��񸸿䡦! ���� ��Ű� ��������� �; �ΰ��� �Ȱǵ�..." ,
                          "��ø���! �����!"};

    //5�� �ε������� ������ ������ ����

    //void FinalUpdate()
    //{
    //    print("FinalUpdateȣ�� ��" + m_ShowTime);

    //    if (0.0f < m_ShowTime)
    //    {
    //        m_ShowTime -= Time.deltaTime;

    //        if (m_ShowTime <= 0.0f)
    //        {
    //            if (m_NextBtn.gameObject.activeSelf == false)
    //            {
    //                m_NextBtn.gameObject.SetActive(true);
    //            }
    //        }
    //    }
    //}
}
