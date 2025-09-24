using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManeger : MonoBehaviour
{
    public Image m_LoadingFill_Img = null;
    public Text m_LoadingProgress_Txt = null;
    public Text m_RandomStory_Txt = null;

    static string m_NextScene;
    public AsyncOperation op;

    string[] a_MessList = { "난 인간이 될거야될거야.\n반드시 인간이 되어서 꼭...",
                            "던전 안에서 보물 상자를 발견했다면 열어봐 도움이 될거야.\n_ 익명의 탐험가",
                            "그림자 괴물의 하수인들은 바닥에서 기어나온다는 걸 기억해.\n_ 익명의 탐험가",
                            "[TIP] 보석을 사용해서 고유 능력을 강화 시킬 수 있다.\n보다 강력해진 나만의 능력으로 적과 싸워보자!",
                            "인간이 되고싶다고?\n장담컨데 네 뜻대로 흘러가진 않을거야.\n_ 그림자 괴물"};

    // Start is called before the first frame update
    void Start()
    {
        m_LoadingFill_Img.fillAmount = 0;
        GlobalValue.LoadGlobalValueData();
        StartCoroutine(LoadScene());
        op = SceneManager.LoadSceneAsync(m_NextScene);

        if (m_RandomStory_Txt != null)
        {
            int a_Rand = Random.Range(0, a_MessList.Length);
            m_RandomStory_Txt.text = a_MessList[a_Rand];
        }
        
    }

    private void Update()
    {
    }
    public static void LoadScene(string sceneName)
    {
        m_NextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //수동으로 전환 연출할 것이기때문에 자동 화면 전환 꺼두기
        op.allowSceneActivation = false; 

        float timer = 0.0f;

        while (!op.isDone) //작업이 완료되기 전 까지 로딩 연출
        {
            yield return null;

            timer += Time.deltaTime;


            if (m_LoadingFill_Img.fillAmount < 0.8f)
            {
                m_LoadingFill_Img.fillAmount = Mathf.Lerp(m_LoadingFill_Img.fillAmount, op.progress, timer);

                if (m_LoadingFill_Img.fillAmount >= op.progress)
                {
                    timer = 0f;
                }

            }
            else //진행률이 90이상이면 100까지 수동으로 남은 시간 조정
            {
                m_LoadingFill_Img.fillAmount += Time.deltaTime * 0.1f;

                if (0.999f <= m_LoadingFill_Img.fillAmount)
                {
                    m_LoadingFill_Img.fillAmount = 1.0f;
                    op.allowSceneActivation = true;
                    yield break;
                }
            }

            //진행률 90퍼센트 미만이면 실제 진행률을 표기
            //if (op.progress < 0.6f)
            //{
            //    m_LoadingFill_Img.fillAmount = Mathf.Lerp(m_LoadingFill_Img.fillAmount, op.progress, timer);

            //    if (m_LoadingFill_Img.fillAmount >= op.progress)
            //    {
            //        timer = 0f;
            //    }

            //}
            //else //진행률이 90이상이면 100까지 수동으로 남은 시간 조정
            //{
            //    m_LoadingFill_Img.fillAmount = Mathf.Lerp(m_LoadingFill_Img.fillAmount, 1f, ( m_LoadingFill_Img.fillAmount));

            //    print(m_LoadingFill_Img.fillAmount);
            //    //m_LoadingFill_Img.fillAmount = Mathf.Lerp(m_LoadingFill_Img.fillAmount, 1f, timer * 0.5f);

            //    if (0.99999f <= m_LoadingFill_Img.fillAmount)
            //    {
            //        m_LoadingFill_Img.fillAmount = 1.0f;
            //        op.allowSceneActivation = true;
            //        yield break;
            //    }
            //}

            m_LoadingProgress_Txt.text = (m_LoadingFill_Img.fillAmount * 100.0f).ToString("N0") + "% 진행 중";
        }
    }
}
