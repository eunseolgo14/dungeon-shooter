using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject m_FirstVisitRoot_Obj = null;
    public GameObject m_ReVisitRoot_Obj = null;
    public GameObject m_LoadingBarRoot_Obj = null;

    //저장 데이터가 없을 시 최초 시작하는 버튼
    public Button m_Start_Btn = null;
    //저장 데이터가 있을 시 이어하는 버튼
    public Button m_Continue_Btn = null;
    //저장 데이터가 있을 시 리셋 후 다시 시작하는 버튼
    public Button m_NewGame_Btn = null;

    public Image m_LodingFill_Img = null;


    //typewritter연출용 변수
    string m_FullLetter = "DUNGEON\nSHOOTER";
    char[] m_LetterArray;
    float m_TimeDelay = 0.1f;
    float m_WaitTime = 0.0f;
    public Text m_TitleTxt = null;
    int m_Idx = 0;
    bool m_IsTyping = false;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM("Glitch");

        m_IsTyping = true;
        m_LetterArray = m_FullLetter.ToCharArray();

        //플레이어 프리팹에 저장된 키값이 없다면
        if (PlayerPrefs.HasKey("Name") == false && m_FirstVisitRoot_Obj != null)
        {
            //해당 플레이어는 게임에 최조 접속 => 최조 접속 루트 출력
            m_FirstVisitRoot_Obj.gameObject.SetActive(true);
            m_ReVisitRoot_Obj.gameObject.SetActive(false);
        }
        //플레이어 프리팹에 저장된 키 값이 있다면
        else if (PlayerPrefs.HasKey("Name") == true && m_ReVisitRoot_Obj != null)
        {
            //플레이어 프리팹에 저장된 데이타 있음 => 재접속 루트 출력
            m_ReVisitRoot_Obj.gameObject.SetActive(true);
            m_FirstVisitRoot_Obj.gameObject.SetActive(false);
        }
        

        if (m_Start_Btn != null)
        {
            m_Start_Btn.onClick.AddListener(() =>
            {
                //스토리 설명 씬으로 이동
                SceneManager.LoadScene(7);
                //LoadingSceneManeger.LoadScene("LobbyScene");
            });
        }

        if (m_Continue_Btn != null)
        {
            m_Continue_Btn.onClick.AddListener(() =>
            {
                LoadingSceneManeger.LoadScene("LobbyScene");
            });
        }

        if (m_NewGame_Btn != null)
        {
            m_NewGame_Btn.onClick.AddListener(() =>
            {
                //데이터 모두 삭제
                PlayerPrefs.DeleteAll();
                //스토리 설명 씬으로 이동
                SceneManager.LoadScene(7);
                //LoadingSceneManeger.LoadScene("LobbyScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsTyping == true)
        {
            TypeEffect();
        }
        else //m_IsTyping == false
        {
            if (0 < m_WaitTime)
            {
                m_WaitTime -= Time.deltaTime;
                if (m_WaitTime <= 0.0f)
                {
                    //텍스트 다시 초기화
                    m_TitleTxt.text = "";
                    //다시 타이핑 연출 시작
                    m_IsTyping = true;
                    m_TimeDelay = 0.1f;
                    m_Idx = 0;
                }
            }

        }
    }

    void TypeEffect()
    {
        if (0 < m_TimeDelay)
        {
            m_TimeDelay -= Time.deltaTime;

            if (m_TimeDelay <= 0.0f && m_TitleTxt != null)
            {
                m_TitleTxt.text += m_LetterArray[m_Idx];
                m_TimeDelay = 0.1f;
                m_Idx++;
                if (m_Idx == m_LetterArray.Length)
                {
                    m_IsTyping = false;
                    m_WaitTime = 3.0f;
                }
            }
        }
    }
}
