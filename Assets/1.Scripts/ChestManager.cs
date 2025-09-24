using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    //플레이어와 상자 접촉 상태 여부 변수
    bool m_IsTouch = false;

    //--- 접총 상태 유지중 UI 연출용 변수
    public GameObject m_ChestHud = null;
    public Transform m_ChestIcon = null;
    public GameObject m_HelpTxtObj = null;

    //택스트 깜빡임연출용 시간 변수
    float m_TestTimer = 0.5f;
    //텍스트 오브젝트 켜짐 여부 변수
    bool m_TextIsOnOff = false;
    //이미지 회전 연출영 변수
    float m_IconRotY = 0.0f;

    //--- 상자 오픈 후 연출용 변수
    //상자 열림 여부 확인용 변수
    bool m_IsChestOpen = false;
    //상자 이미지 접근용 변수
    public SpriteRenderer m_ChestRender = null;
    //상자 오픈 후 변경할 열린 상자 이미지
    public Sprite m_OpenChestSpr = null;

    //--- 획득 아이템 정보 표시용 UI와 연출용 변수
    float m_FlyTime = 0.0f;
    public GameObject m_ItemPanelBG;
    public Transform m_ItemInfoPanelTr;
    Vector3 m_TargetPosition = Vector3.zero;
    Vector3 m_Position_1 = new Vector3(-350.0f, 130.0f, 0.0f);
    Vector3 m_Position_2 = new Vector3(0.0f, 130.0f, 0.0f);
    Vector3 m_StartPosition =  new Vector3(-1400.0f, 130.0f, 0.0f);
    bool m_IsPanelArrive = false;
    public Image m_GunItemImg = null;
    public Image m_SkillItemImg = null;
    public GameObject m_GunHelpTxt = null;


    //히어로 컨트롤러 가져오기
    HeroManager m_HeroMgr;

    // Start is called before the first frame update
    void Start()
    {
        m_HeroMgr = GameObject.FindObjectOfType<HeroManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsTouch == false)
        {
            return;
        }

        if (m_IsTouch == true && Input.GetKeyDown(KeyCode.I) == true)
        {
            Time.timeScale = 0.0f;

            //시간 잠시 멈춤
            print("시간 일시 정지");

            ChestOpen();
        }

        if (m_IsChestOpen == true)
        {
            //m_IsChestOpen이 true라는 것은 deltaTime사용 불가능
            m_FlyTime += Time.unscaledTime;
        }

        if (m_IsPanelArrive == true && Input.GetKeyDown(KeyCode.Return) == true)
        {
            //상자 여닫기 상태 불가로 처리
            m_IsChestOpen = false;
            //뿌연 배경 지우기
            m_ItemPanelBG.gameObject.SetActive(false);
            //상자 콜리도 비활 처리 => 충돌 막기
            GetComponent<BoxCollider2D>().enabled = false;
            //Debug.Log("밀어 넣을게요...!");
            //시간 멈춤 풀기
            Time.timeScale = 1.0f;
            //아이템 인포 판넬 다시 밀어넣기
            m_ItemInfoPanelTr.localPosition = m_StartPosition;
            //변경 사항 저장
        }

        HudUpdate();
        ItemInfoTabUpdate();
        print("time: " + Time.timeScale);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss")
        {
            return;
        }

        m_IsTouch = true;
        m_ChestHud.gameObject.SetActive(m_IsTouch);
        //m_HelpTxtObj.gameObject.SetActive(true);
        m_ChestIcon.rotation = Quaternion.Euler(Vector3.zero);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss")
        {
            return;
        }

        m_IsTouch = false;
        m_ChestHud.gameObject.SetActive(m_IsTouch);
    }

    //플레이어가 체스트와 접촉해 있는 동안 연출을 할 업데이트 함수
    void HudUpdate()
    {

        //오류 방지를 위한 예외 처리
        if (m_ChestHud.gameObject.activeSelf == false || m_ChestHud == null)
        {
            return;
        }

        if (m_ChestIcon == null || m_HelpTxtObj == null)
        {
            return;
        }

        if (0 < m_TestTimer)
        {
            //타이머 차감
            m_TestTimer -= Time.deltaTime;

            if (m_TestTimer <= 0.0f)
            {
                m_TextIsOnOff = m_HelpTxtObj.gameObject.activeSelf;
                m_HelpTxtObj.gameObject.SetActive(!m_TextIsOnOff);
                m_TestTimer = 0.5f;
            }
        }

        m_IconRotY += Time.deltaTime * 200;

        m_ChestIcon.rotation = Quaternion.Euler(new Vector3(0.0f, m_IconRotY, 0.0f));

    }

    void ChestOpen()
    {
        //상자 열림 상태로 전환.
        m_IsChestOpen = true;

        m_ItemPanelBG.gameObject.SetActive(true);

        //안내 허드는 끄기
        m_ChestHud.gameObject.SetActive(false);

        //열린 상자 이미지로 변경
        if (m_ChestRender != null || m_OpenChestSpr != null)
        {
            m_ChestRender.sprite = m_OpenChestSpr;
        }


        //랜덤 아이템 결정, UI연출 함수 호출
        DropRandomItem();
    }

    void DropRandomItem()
    {

        if (m_IsChestOpen == false)
        {
            return;
        }

        if (m_SkillItemImg.sprite != null)
        {
            //이미 아이템 드롭 완료
            return;
        }
        //----- 랜덤 총기 아이템 추출

        //3에서 5사이의 난수 추출(스페셜 건타입이 차지하는 범위)
        int a_Idx = Random.Range(3, 6);

        //건 인포 클라스 객체를 하나 생성
        Gun_Info a_NewGun = new Gun_Info();

        //추출된 난수 번호에 해당하는 총기 설정값을 객체 변수에 대입하는 init함스 호출
        a_NewGun.SetGunInfo((GunType)a_Idx);

        //글로벌 벨류에 플레이어 보유 총기 리스트에 추가
        GlobalValue.g_MyGunList.Insert(1, a_NewGun);
        m_HeroMgr.m_SpecialGunSpr = a_NewGun.m_GunIconSprite;

        if (2 <= GlobalValue.g_MyGunList.Count)
        {
            m_GunHelpTxt.gameObject.SetActive(true);
        }
        //----- 랜덤 스킬 아이템 추출

        //0에서 5까지의 랜덤 난수 발생(SkillType enum 에서 액티브 스킬이 차지하는 범위)

        a_Idx = Random.Range(0, 3);//debug_ 0,1,2 3개중 한개만
        //a_Idx = 0;

        //해당 난수에 해당하는 보유 스킬 리스트의 갯수 증가
        GlobalValue.m_SkillInfoList[a_Idx].m_MyCount++;

        //아이템의 고유 키값을 발행
        string a_KeyBuff = string.Format("Skill_{0}_Count", (int)a_Idx);

        //차감된 스킬 갯수 로컬에 저장
        PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)a_Idx].m_MyCount);

        //저장된 정도 글로벌벨루에 불러와두기
        GlobalValue.LoadGlobalValueData();

        //UI갱신
        UI_Manager a_UIMgr = GameObject.FindObjectOfType<UI_Manager>();
        a_UIMgr.LoadSkillItem();

        //----- ui에 추출된 두 아이템 정보 입력 
        if (m_GunItemImg == null || m_SkillItemImg == null)
        {
            return;
        }

        //m_GunItemImg에는 a_Idx번 총기에 해당하는 스프라이트 대입
        m_SkillItemImg.sprite = GlobalValue.m_SkillInfoList[a_Idx].m_IconImg;
        m_GunItemImg.sprite = a_NewGun.m_GunIconSprite;


    }

    //드롭 아이템의 정보를 띄워주는 UI 판넬 화면 중앙으로 이동하는 연출 업데이트 코드
    void ItemInfoTabUpdate()
    {
        if (m_IsChestOpen == false)
        {
            return;
        }

        if (m_ItemInfoPanelTr == null)
        {
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            m_TargetPosition = m_Position_1;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            m_TargetPosition = m_Position_2;
        }

        //인포 판넬 끄집어내기 
        if (m_ItemInfoPanelTr.localPosition.x < m_TargetPosition.x)
        {
            m_ItemInfoPanelTr.localPosition = Vector3.MoveTowards(m_ItemInfoPanelTr.localPosition,
                m_TargetPosition, 5.0f * Time.unscaledTime);

            //목표 지점 근사치에 도달했다면
            if ((m_TargetPosition.x - 0.5f ) <= m_ItemInfoPanelTr.localPosition.x)
            {
                //도착한 것으로 처리
                m_ItemInfoPanelTr.localPosition = m_TargetPosition;
                m_IsPanelArrive = true;
                //Time.timeScale = 0.0f;
                //Debug.Log("인포 판넬 도착 완료" + m_IsPanelArrive);
            }
        }
    }
}
