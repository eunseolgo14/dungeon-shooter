using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StorySceneManager : MonoBehaviour
{
    //순차적으로 활성화시킬 게임 오브젝트
    public List<GameObject> m_StoryCutList;

    //활성화 시킬 오브젝트의 인덱스 번호
    int m_Idx = 0;

    //딜레이 타임
    float m_DelayTime = 0.0f;

    //다음 버튼
    public Button m_NextBtn;


    void Start()
    {
        SoundManager.Instance.PlayBGM("Walking");

        m_DelayTime = 0.7f;

        if (m_NextBtn != null )
        {
            m_NextBtn.onClick.AddListener(() =>
            {
               LoadingSceneManeger.LoadScene("LobbyScene");
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (4 < m_Idx)
        {
            return;
        }

        if (0.0f < m_DelayTime)
        {
            m_DelayTime -= Time.deltaTime;

            if (m_DelayTime <= 0.0f)
            {
                if (0 < m_StoryCutList.Count && m_StoryCutList[m_Idx].activeSelf == false)
                {
                    m_StoryCutList[m_Idx].SetActive(true);
                    m_Idx++;
                    m_DelayTime = 2.5f;
                }
                
            }
        }   
    }
}
