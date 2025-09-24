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


    //--- ȿ���� ����ȭ�� ���� ���� ����
    int m_EffSdCount = 5; //<--- ������ 5���� ���̾�� �÷���
    int m_SoundCount = 0; //<--- ��ä 5������ ���

    //�ִ� 5���� ������ ����� ������Ʈ ���� ����Ʈ
    List<GameObject> m_SoundObjList = new List<GameObject>();
    //5���� ���� ������Ʈ�� ������ ������ҽ� ������Ʈ
    AudioSource[] m_AudioSourceList = new AudioSource[10];
    float[] m_EffVolume = new float[10];
    //--- ȿ���� ����ȭ�� ���� ���� ����

    protected override void Init() //<--- Awake()�Լ� ��� ���
    {

        base.Init(); // base. : => �θ����� �����Լ��� ȣ��, 
                     //G_Singleton���� virtual void Awake����
                     //�θ��� Init(); �Լ��� ȣ��ǰ� �ִ�

        //virtual�̶�?
        //�Լ��� �����Լ��� ����� Ű����, 

        //�θ��� ���� �����ũ �Լ����� ������ �ҷȴٸ�
        //�ڳ� ��ũ��Ʈ���� �Ҹ��� �����ũ�� ȣ���

        LoadChildGameObj();
    }

    void Start()
    {

        AudioClip a_GAudioClip = null;
        object[] temp = Resources.LoadAll("Sound"); //<--- ���ҽ� ���� ��, Sound�� ������ �ε�

        for (int ii = 0; ii < temp.Length; ii++)
        {
            a_GAudioClip = temp[ii] as AudioClip;
            if (m_ClipList.ContainsKey(a_GAudioClip.name) == true)
            {
                continue; //<--- ���� �̸��� Ű�� �ִٸ� �ٽ� ��ųʸ��� �߰��� �ʿ� �����ϱ�
            }

            m_ClipList.Add(a_GAudioClip.name, a_GAudioClip);
            //�ش� Ŭ���� �̸��� Ű��, �ش� ���� ��ü ���� ��ųʸ��� �����Ѵ�
        }

        //Debug.Log("���� Ŭ�� �ε� �Ϸ�, �� " + m_ClipList.Count + "���� ���� Ŭ�� �����");
    }


    public void LoadChildGameObj()
    {

        //GlobalSingleton�� ���� �� ���� ������Ʈ�� ���� �Ǿ���,
        //a_go.AddComponent<T>();�� ���� this�� ������Ʈ�� ������
        //�� this ������Ʈ�� AudioSource ������Ʈ�� ���δ�
        if (this == null)
        {
            return;
        }
        else
        {
            m_AudioSrc = this.gameObject.AddComponent<AudioSource>(); //<--- �������� ������Ʈ�� �����ϴ� ���
        }
        
        //�� �ڽ��� ���� ������Ʈ �Ʒ� 5���� ���̾� ���� 
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
        //5���� ���̾� ���� �ڵ�
    }

    //���� ������� �Լ�
    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudopClip = null;

        if (m_ClipList.ContainsKey(a_FileName) == true) //�ްԺ����� ���� �̸��� Ű�� Ŭ���� �����Ѵٸ�
        {
            a_GAudopClip = m_ClipList[a_FileName] as AudioClip;
            //a_GAudopClip�� �� Ŭ���� ����� Ŭ������ ����
        }
        else //m_ADClipList��ųʸ��� �ش� Ű�� ���� ����
        {
            //�������� �̸��� ���� Ŭ�� �ε�
            a_GAudopClip = Resources.Load("Sound/" + a_FileName) as AudioClip;
            //m_ADClipList��ųʸ��� �߰�
            m_ClipList.Add(a_FileName, a_GAudopClip);
        }

        if (m_AudioSrc == null) //�̰� ���� �ƴҶ� ������ �ؾ��Ѵ�. == ���� �̹� ������� ��(�����Ҷ� ������µ� ���� ����ִ�== ����)
        {
            return;
        }
        //����� �ҽ� Ŭ���� ���̶� 
        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName) // �� ���۷��� ����
        {

            //����: m_AudioSrc.clip != null || m_AudioSrc.clip.name == a_FileName��� ��
            //����� Ŭ���� ���̾�� ������  m_AudioSrc.clip.name != a_FileName�̾���ϴµ� 
            //���̾ ������ �ɷ������� 
            return;
        }


        m_AudioSrc.clip = a_GAudopClip;
        m_AudioSrc.volume = fVolume * m_SoundVolume;
        m_BGM_Volume = fVolume;
        m_AudioSrc.loop = true;
        m_AudioSrc.Play();
    }

    //�Ѿ� �߻� ȿ���� �Լ�
    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
        {
            return;
        }

        AudioClip a_GAudioClip = null;

        if (m_ClipList.ContainsKey(a_FileName) == true)
        {
            //Debug.Log(a_FileName + "��(��) �߰�, ����մϴ�.");
            a_GAudioClip = m_ClipList[a_FileName] as AudioClip;
        }
        else
        {
            //Debug.Log(a_FileName + "��(��) �߰��������� �ٽ� �ε��մϴ�.");
            a_GAudioClip = Resources.Load("Sound/" + a_FileName) as AudioClip;
            m_ClipList.Add(a_FileName, a_GAudioClip);
        }

        if (a_GAudioClip == null)
        {
            //Debug.Log(a_FileName + "��(��) �ε����� ���� ����� �� �����ϴ�.");
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
