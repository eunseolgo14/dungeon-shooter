using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
{
    public Text m_DialogueTxt;
    string[] m_LineLists =
                        { "그래, 내가 졌어. 당신은 나보다 강한 자야.",
                          "나보다 강한 녀석을 만나는건 오랜만이라 무척이나 즐거웠어.",
                          "당신의 요구를 들어주도록 하지. 왜 인간이 되고싶다고 했지?",
                          "<color=grey>난 작은 시골 수족관 구석에 있는 잊혀진 펭귄이야.</color>",
                          "<color=grey>매일 날 보러오는 인간 손님이 있는데 그와 말을 하고싶어.</color>" ,
                          "<color=grey>지금보다 더 가까워지고싶어.</color>",
                          "그렇군. 하지만 인간이 된다고 네 뜻대로 흘러가지 않을거야." ,
                          "후회가 없길 빌어.",
                          "<color=grey>응, 후회하지 않을거야.</color>",
                          "강한자는 각오도 남다르군.",
                          "이 던전에 들어왔던 그 길 그대로 빠져나가면 돼.",
                          "동굴 밖으로 나가는 순간 당신은 인간이 되어있을거야.",
                          "즐거웠어."}; //length == 13


    public Button m_NextBtn;

    //순서에 맞는 대사 출력을 위한 변수
    int m_Idx = 0;
    float m_ShowTime = 2.0f;

    public GameObject m_MonSprObj;
    //몬스터 투명도 조절을 위한 변수
    SpriteRenderer m_MonSpr;
    float m_ColorA = 1.0f;
    public GameObject m_FinalEndingObj;

    //걸어서 떠나는 여성 오브젝트 변수
    public GameObject m_LadyObj;
    bool m_IsLadyWalk = false;

    //남성 오브젝트 걷기 연출 변수
    public GameObject m_ManObj;
    bool m_IsManWalk = false;

    //마지막 엔딩 화면 연출을 위한 변수
    public GameObject m_DialogueBoxObj;
    public GameObject m_ThankyouObj;

    //업데이트를 나눠쓰기위한 변수
    bool m_IsFading = false;
    bool m_IsEnding = false;

    //마지막 화면 다시하기, 돌아가기 버튼
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
                //타이틀 화면으로 복귀
                SceneManager.LoadScene(0);
            });
        }
        if (m_QuitBtn != null)
        {
            m_QuitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
            //유니티 에디터 상이라면 => play모드 끄기
            UnityEditor.EditorApplication.isPlaying = false;
#else
            //아니라면 실행파일종료
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
        
        //몬스터의 사라짐 효과 업데이트
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

        //여성과 남성의 걷기 연출 업데이트
        if (m_IsLadyWalk == true)
        {
            m_LadyObj.transform.Translate(Vector3.right * 0.06f);

            //남성의 이미지가 화면 밖으로 넘어가면
            if (10.0f <= m_LadyObj.transform.position.x)
            {
                m_IsLadyWalk = false;
            }
        }
        if (m_IsManWalk == true)
        {
            m_ManObj.transform.Translate(Vector3.right * 0.06f);

            //남성의 이미지가 화면 밖으로 넘어가면
            if (10.0f <= m_ManObj.transform.position.x)
            {
                m_IsManWalk = false;
                m_FinalEndingObj.gameObject.SetActive(false);
                m_DialogueBoxObj.gameObject.SetActive(false);
                m_ThankyouObj.gameObject.SetActive(true);
            }
            
        }
        

        //기본 업데이트는 대사 출력
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
                //최종 엔딩 연출 함수
                m_IsFading = true;
                m_NextBtn.gameObject.SetActive(false);
                
                return;
            }

            //증가된 순번으로 대사 교체
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
                //최종 엔딩 연출 함수
                m_IsManWalk = true;
                m_ManObj.GetComponent<Animator>().SetTrigger("IsLeave");
                return;
            }

            //증가된 순번으로 대사 교체
            m_DialogueTxt.text = m_ConvoLists[m_Idx];
        }

        
        //쇼타임 충전
        m_ShowTime = 1.0f;
    }


    string[] m_ConvoLists =
                        { "저기요, 혹시 날 기억하고 있나요?",
                          "당신이 매일 보러 온 수족관의 그 펭귄이에요.",
                          "늘 당신과 대화를 나누고 싶었는데 방금 인간이 되었어요.",
                          "<color=white>당신이 그 펭귄이라고요? 바보같은 거짓말 하지 마세요.</color>",
                          "<color=white>전 펭귄이 아닌 다른 그 어떤 생명체에도 관심이 없어요.</color>" ,
                          "<color=white>특히 지독한 인간에게는 더더욱이요.</color>",
                          "잠깐만요…! 저는 당신과 가까워지고 싶어서 인간이 된건데..." ,
                          "잠시만요! 저기요!"};

    //5번 인덱스에서 여성이 떠나는 연출

    //void FinalUpdate()
    //{
    //    print("FinalUpdate호출 중" + m_ShowTime);

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
