using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LobbyManager : MonoBehaviour
{
    [Header("----- Hero Select Panel -----")]
    public Image m_HeroIconImg = null;
    public Sprite[] m_HeroSprites;
    public Button m_NextHeroBtn = null;
    public Button m_PrevHeroBtn = null;
    public Text m_HeroNameTxt = null;

    [Header("----- Selected Hero Info Panel -----")]
    public Image m_DefaultWeaponImgs; //3칸 이미지 슬랏
    public Image m_PassiveSkillImg_1; 
    public Image m_PassiveSkillImg_2; 
    public Sprite[] m_DefaultSkillSprites; //9개의 스프라이트 재료
    public Text m_SelectedHeroInfoTxt = null;

    public Button m_ConfirmBtn = null;
    public int m_HeroIdx = 0;

    [Header("----- Info Tip Panel -----")]
    public GameObject m_InfoTipObj = null;
    public Text m_InfoTxt = null;


    [Header("----- Dungeon Map Panel -----")]
    int m_ChosenStage = -1;
    public GameObject m_DungeonMapPanelObj = null;
    public GameObject m_SelectPanelObj = null;
    public RawImage[] m_SelectBoxImgs = null;
    public Text m_StageNumTxt = null;
    public Text m_StageHelpTxt = null;
    public Button m_SelectSatgeBtn = null;
    public Button m_BackToSelectBtn = null;
    //public RawImage m_PrevImg = null;
    public GameObject m_BossRoot = null;
    public GameObject m_1stRoot = null;


    // help 텍스트 ui연출용 변수
    float m_Showtime = 0.0f;
  
    bool m_IsSelect = false;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM("Lobby");

        //전투 스테이지에서 뒤로 온거라면 지도부터 열리게
        if (2 < GlobalValue.g_CurScene)
        {
            m_DungeonMapPanelObj.gameObject.SetActive(true);
            m_SelectPanelObj.gameObject.SetActive(false);
        }

        //GlobalValue.LoadGlobalValueData();
        
        RefreshPanel();
        LoadGunInfo();

        if (m_NextHeroBtn != null)
        {
            m_NextHeroBtn.onClick.AddListener(ClickNext);
        }

        if (m_PrevHeroBtn != null)
        {
            m_PrevHeroBtn.onClick.AddListener(ClickPrev);
        }

        if (m_ConfirmBtn != null)
        {
            m_ConfirmBtn.onClick.AddListener(() =>
            {
                if (0 == m_HeroIdx) //햄스터가 아닌 다른 영웅 선택 => 리턴
                {

                    m_DungeonMapPanelObj.gameObject.SetActive(true);
                    m_SelectPanelObj.gameObject.SetActive(false);
                    PlayerPrefs.SetString("Name", "Penguin");
                    
                }
                else
                {
                    if (m_SelectedHeroInfoTxt != null)
                    {
                        m_SelectedHeroInfoTxt.text =
                            "<color=red><size=80>해당 캐릭터는 개발 중에 있습니다." + "\n"
                          + "다른 캐릭터를 선택해주세요.</size></color>";
                        m_Showtime = 1.5f;
                    }

                    return;
                }

            });
        }

        if (m_BackToSelectBtn != null)
        {
            m_BackToSelectBtn.onClick.AddListener(() =>
           {
               m_SelectPanelObj.gameObject.SetActive(true);

               //붉은 선택 영역 해제
               for (int i = 0; i < m_SelectBoxImgs.Length; i++)
               {
                   m_SelectBoxImgs[i].enabled = false;
               }
               //안내 멘트 초기화
               m_StageHelpTxt.text = "<color=white>이 스테이지를\n" + "선택하시겠습니까?</color>";
               m_DungeonMapPanelObj.gameObject.SetActive(false);
           });
        }
        if (m_SelectSatgeBtn != null)
        {
            m_SelectSatgeBtn.onClick.AddListener(SelectStageClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_Showtime)
        {
            m_Showtime -= Time.deltaTime;
            if (m_Showtime <= 0.0f)
            {
                m_Showtime = 0.0f;
                if (m_DungeonMapPanelObj.gameObject.activeSelf == false)
                {
                    RefreshPanel();
                }
                else
                {
                    m_StageHelpTxt.text = "이 스테이지를\n" + "선택하시겠습니까?";
                }
                
            }
        }
    }

    // 히어로 선택 판넬 이전/다음 버튼 
    void ClickNext()
    {
        if (m_HeroIdx < 2) //0,1,2 까지만
        {
            m_HeroIdx++;
            GlobalValue.g_HeroType = (HeroType)m_HeroIdx;
            //GameManager.inst.m_HeroType = (GameManager.HeroType)m_HeroIdx;
            RefreshPanel();
        }
        else
        {
            //0으로 초기화
            m_HeroIdx = 0;
            GlobalValue.g_HeroType = (HeroType)m_HeroIdx;
            //GameManager.inst.m_HeroType = (GameManager.HeroType)m_HeroIdx;
            RefreshPanel();
        }

    }

    void ClickPrev()
    {
        if (m_HeroIdx < 1)
        {
            m_HeroIdx = 2;
            GlobalValue.g_HeroType = (HeroType)m_HeroIdx;
            //GameManager.inst.m_HeroType = (GameManager.HeroType)m_HeroIdx;
            RefreshPanel();
        }
        else
        {
            m_HeroIdx--;
            GlobalValue.g_HeroType = (HeroType)m_HeroIdx;
            //GameManager.inst.m_HeroType = (GameManager.HeroType)m_HeroIdx;
            RefreshPanel();
        }
    }

    //플레이어가 캐릭터 보기를 바꿀 때 미리보기 이미지 수정하는 함수
    void RefreshPanel()
    {
        Hero_Info.SetHero((HeroType)m_HeroIdx);

        if (m_HeroIconImg != null && m_HeroNameTxt != null)
        {
            m_HeroIconImg.sprite = Hero_Info.m_HeroImg;//m_HeroSprites[m_HeroIdx];
            m_HeroNameTxt.text = "<" + Hero_Info.m_HeroName + ">";//GameManager.inst.m_HeroList[m_HeroIdx].m_HeroName +" >";

            m_DefaultWeaponImgs.sprite = Hero_Info.m_HeroWeapImg;
            m_PassiveSkillImg_1.sprite = Hero_Info.m_HeroPassImg_1;
            m_PassiveSkillImg_2.sprite = Hero_Info.m_HeroPassImg_2;


            if (m_SelectedHeroInfoTxt != null)
            {
                m_SelectedHeroInfoTxt.text = Hero_Info.m_Purpose;
            }

        }
    }

    public void ClickStage()
    {
        GameObject a_ClickBtn = EventSystem.current.currentSelectedGameObject;
        RawImage a_TargetImg = a_ClickBtn.transform.GetComponentInChildren<RawImage>();
        int.TryParse(a_ClickBtn.gameObject.name.Split('_')[0], out m_ChosenStage); // 1.1_Btn => [0]:1.1 / [1]:Btn
        for (int a_ii = 0; a_ii < m_SelectBoxImgs.Length; a_ii++)
        {
            if (m_SelectBoxImgs[a_ii].enabled == true) //이미 어떤버튼이 켜져있음
            {
                if (m_SelectBoxImgs[a_ii] == a_TargetImg) //방금 누른 그녀석 이미 켜진 녀석임
                {
                    a_TargetImg.enabled = false;
                    m_StageNumTxt.text = "";
                    m_IsSelect = false;
                    if (m_1stRoot.gameObject.activeSelf == true || m_BossRoot.gameObject.activeSelf == true)
                    {
                        m_StageHelpTxt.text = "이 스테이지를\n" + "선택하시겠습니까?";
                        m_1stRoot.gameObject.SetActive(false);
                        m_BossRoot.gameObject.SetActive(false);
                        //m_MonImg.GetComponent<Image>().sprite = null;
                    }
                    return;
                }
                else //어딘가 버튼이 선택되어있지만 방금 클릭한 녀석은 아님
                {
                    m_SelectBoxImgs[a_ii].enabled = false;
                    a_TargetImg.enabled = true;
                    if (m_ChosenStage == 0) //TryParse의 결과가 false, 즉 float형으로 변환 불가한 문자열 == store
                    {
                        if (m_1stRoot.gameObject.activeSelf == true || m_BossRoot.gameObject.activeSelf == true)
                        {
                            m_StageHelpTxt.text = "이 스테이지를\n" + "선택하시겠습니까?";
                            m_1stRoot.gameObject.SetActive(false);
                            m_BossRoot.gameObject.SetActive(false);
                            //m_MonImg.GetComponent<Image>().sprite = null;
                        }
                        m_StageNumTxt.text = "상점";
                    }
                    else
                    {
                        m_StageNumTxt.text = m_ChosenStage.ToString() + "번 방";

                        if (m_ChosenStage == 1)
                        {
                            if (m_BossRoot.gameObject.activeSelf == true)
                            {
                                m_BossRoot.gameObject.SetActive(false);
                            }

                            m_StageHelpTxt.text = "";
                            m_1stRoot.gameObject.SetActive(true);
                            //m_MonImg.GetComponent<Image>().sprite = m_MonSprite[0];
                        }
                        else if (m_ChosenStage == 4)
                        {
                            if (m_1stRoot.gameObject.activeSelf == true)
                            {
                                m_1stRoot.gameObject.SetActive(false);
                            }

                            m_StageHelpTxt.text = "";
                            m_BossRoot.gameObject.SetActive(true);
                            //m_MonImg.GetComponent<Image>().sprite = m_MonSprite[1];
                        }
                        else
                        {
                            m_StageHelpTxt.text = "이 스테이지를\n" + "선택하시겠습니까?";
                            m_1stRoot.gameObject.SetActive(false);
                            m_BossRoot.gameObject.SetActive(false);
                        }
                    }
                    m_IsSelect = true;
                    return;
                }
            }
        }


        a_TargetImg.enabled = true;
        if (m_ChosenStage == 0)
        {
            m_StageNumTxt.text = "상점";
        }
        else
        {
            m_StageNumTxt.text = m_ChosenStage.ToString() + "번 방";

            if (m_ChosenStage == 1)
            {
                m_StageHelpTxt.text = "";
                m_1stRoot.gameObject.SetActive(true);
            }
            else if (m_ChosenStage == 4)
            {
                m_StageHelpTxt.text = "";
                m_BossRoot.gameObject.SetActive(true);
                //m_MonImg.GetComponent<Image>().sprite = m_MonSprite[1];
            }
        }
        m_IsSelect = true;
    }

    void SelectStageClick()
    {
        //m_ChosenStage가 0이라면 상점씬
        //m_ChosenStage번의 씬으로 이동
        

        if (m_IsSelect == false)
        {
            m_StageHelpTxt.text = "<color=#ff0000>원하는 장소를\n" + "선택해주세요.</color>";
            m_Showtime = 1.5f;
            return;
        }
        if (m_ChosenStage == 2 || m_ChosenStage == 3 || 4 < m_ChosenStage)
        {
            m_StageHelpTxt.text = "<color=#ff0000>아직 스테이지가\n오픈되지 않았습니다.\n" + "(1번, 4번 가능)</color>";
            m_Showtime = 1.5f;
            return;
        }
       


        GlobalValue.g_StageNum = m_ChosenStage;
        GlobalValue.g_HeroType = (HeroType)m_HeroIdx;

        switch (m_ChosenStage)
        {
            case 0:
                SceneManager.LoadScene(3);
                break;

            case 1:
                SceneManager.LoadScene(4);
                break;

            case 4:
                SceneManager.LoadScene(5);
                break;

            default:
                break;
        }
        
    }

    public List<Gun_Info> m_GunInfoList = new List<Gun_Info>();
    void LoadGunInfo()
    {
        Gun_Info a_GunInfo;

        for (int i = 0; i < 3; i++)
        {
            a_GunInfo = new Gun_Info();//이걸 반복문 안에 넣지 않아서 값이 계속 덮어씌워짐,
            a_GunInfo.SetGunInfo((GunType)i);
            m_GunInfoList.Add(a_GunInfo);
            
        }
    }
}
