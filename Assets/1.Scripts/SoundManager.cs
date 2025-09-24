using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : GlobalSingleton<SoundManager>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_ClipList = new Dictionary<string, AudioClip>();

    float m_BGM_Volume = 0.2f;

    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;


    //--- 효과음 최적화를 위한 버퍼 변수
    int m_EffSdCount = 5; //<--- 지금은 5개의 레이어로 플레이
    int m_SoundCount = 0; //<--- 죄채 5개까지 재생

    //최대 5개로 생성할 빈게임 오브젝트 담을 리스트
    List<GameObject> m_SoundObjList = new List<GameObject>();
    //5개의 게임 오브젝트에 부착될 오디오소스 컴포넌트
    AudioSource[] m_AudioSourceList = new AudioSource[10];
    float[] m_EffVolume = new float[10];
    //--- 효과음 최적화를 위한 버퍼 변수

    protected override void Init() //<--- Awake()함수 대신 사용
    {

        base.Init(); // base. : => 부모쪽의 인잇함수를 호출, 
                     //G_Singleton에서 virtual void Awake에서
                     //부모의 Init(); 함수가 호출되고 있다

        //virtual이란?
        //함수를 가상함수로 만드는 키워드, 

        //부모의 가상 어웨이크 함수에서 인잇이 불렸다면
        //자녀 스크립트에서 불릴때 어웨이크가 호출되

        LoadChildGameObj();
    }

    void Start()
    {

        AudioClip a_GAudioClip = null;
        object[] temp = Resources.LoadAll("Sound"); //<--- 리소스 폴더 안, Sound안 모든것을 로드

        for (int ii = 0; ii < temp.Length; ii++)
        {
            a_GAudioClip = temp[ii] as AudioClip;
            if (m_ClipList.ContainsKey(a_GAudioClip.name) == true)
            {
                continue; //<--- 같은 이름의 키가 있다면 다시 딕셔너리에 추가할 필요 없으니까
            }

            m_ClipList.Add(a_GAudioClip.name, a_GAudioClip);
            //해당 클립의 이름을 키로, 해당 파일 자체 값을 딕셔너리에 저장한다
        }

        //Debug.Log("사운드 클립 로딩 완료, 총 " + m_ClipList.Count + "개의 사운드 클립 저장됨");
    }


    public void LoadChildGameObj()
    {

        //GlobalSingleton에 의해 빈 게임 오브젝트가 생성 되었고,
        //a_go.AddComponent<T>();에 의해 this가 컴포넌트로 부착됨
        //그 this 오브젝트에 AudioSource 컴포넌트를 붙인다
        if (this == null)
        {
            return;
        }
        else
        {
            m_AudioSrc = this.gameObject.AddComponent<AudioSource>(); //<--- 동적으로 컴포넌트를 부착하는 방법
        }
        
        //나 자신의 게임 오브젝트 아래 5개의 레이어 생성 
        for (int ii = 0; ii < m_EffSdCount; ii++)
        {
            GameObject a_SoundObj = new GameObject();
            a_SoundObj.transform.SetParent(this.transform);
            a_SoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSource = a_SoundObj.AddComponent<AudioSource>();
            a_AudioSource.playOnAwake = false;
            a_AudioSource.loop = false;
            a_SoundObj.name = "SFX_Object_With_AudioSource";

            m_AudioSourceList[ii] = a_AudioSource;
            m_SoundObjList.Add(a_SoundObj);
        }
        //5개의 레이어 생성 코드
    }

    //씬의 배경음악 함수
    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudopClip = null;

        if (m_ClipList.ContainsKey(a_FileName) == true) //메게변수로 들어온 이름의 키의 클립이 존재한다면
        {
            a_GAudopClip = m_ClipList[a_FileName] as AudioClip;
            //a_GAudopClip에 그 클립을 오디오 클립으로 저장
        }
        else //m_ADClipList딕셔너리에 해당 키의 값이 없음
        {
            //폴더에서 이름을 가진 클립 로드
            a_GAudopClip = Resources.Load("Sound/" + a_FileName) as AudioClip;
            //m_ADClipList딕셔너리에 추가
            m_ClipList.Add(a_FileName, a_GAudopClip);
        }

        if (m_AudioSrc == null) //이게 널이 아닐때 리턴을 해야한다. == 무언가 이미 들어있을 때(ㅣ작할때 비워놨는데 무언가 들어있다== 오류)
        {
            return;
        }
        //오디오 소스 클립이 널이라 
        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName) // 널 레퍼런스 오류
        {

            //이유: m_AudioSrc.clip != null || m_AudioSrc.clip.name == a_FileName라고 씀
            //오디오 클립은 널이어야 하지만  m_AudioSrc.clip.name != a_FileName이어야하는데 
            //널이어도 무조건 걸려버려서 
            return;
        }


        m_AudioSrc.clip = a_GAudopClip;
        m_AudioSrc.volume = fVolume * m_SoundVolume;
        m_BGM_Volume = fVolume;
        m_AudioSrc.loop = true;
        m_AudioSrc.Play();
    }

    //총알 발사 효과음 함수
    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;

        if (m_ClipList.ContainsKey(a_FileName) == true)
        {
            //Debug.Log(a_FileName + "을(를) 발견, 재생합니다.");
            a_GAudioClip = m_ClipList[a_FileName] as AudioClip;
        }
        else
        {
            //Debug.Log(a_FileName + "을(를) 발견하지못해 다시 로딩합니다.");
            a_GAudioClip = Resources.Load("Sound/" + a_FileName) as AudioClip;
            m_ClipList.Add(a_FileName, a_GAudioClip);
        }

        if (a_GAudioClip == null)
        {
            //Debug.Log(a_FileName + "을(를) 로딩하지 못해 재생할 수 없습니다.");
            return;
        }

        if (m_AudioSourceList[m_SoundCount] != null)
        {
            //m_SndSrcList[m_SoundCount].volume = fVolume * m_SoundVolume;
            m_AudioSourceList[m_SoundCount].PlayOneShot(a_GAudioClip, fVolume * m_BGM_Volume);
            m_EffVolume[m_SoundCount] = fVolume;

            m_SoundCount++;
            if (m_EffSdCount <= m_SoundCount)
            {
                m_SoundCount = 0;
            }
        }
    }

   
}
