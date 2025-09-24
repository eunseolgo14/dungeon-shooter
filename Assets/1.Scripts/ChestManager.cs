using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    //�÷��̾�� ���� ���� ���� ���� ����
    bool m_IsTouch = false;

    //--- ���� ���� ������ UI ����� ����
    public GameObject m_ChestHud = null;
    public Transform m_ChestIcon = null;
    public GameObject m_HelpTxtObj = null;

    //�ý�Ʈ �����ӿ���� �ð� ����
    float m_TestTimer = 0.5f;
    //�ؽ�Ʈ ������Ʈ ���� ���� ����
    bool m_TextIsOnOff = false;
    //�̹��� ȸ�� ���⿵ ����
    float m_IconRotY = 0.0f;

    //--- ���� ���� �� ����� ����
    //���� ���� ���� Ȯ�ο� ����
    bool m_IsChestOpen = false;
    //���� �̹��� ���ٿ� ����
    public SpriteRenderer m_ChestRender = null;
    //���� ���� �� ������ ���� ���� �̹���
    public Sprite m_OpenChestSpr = null;

    //--- ȹ�� ������ ���� ǥ�ÿ� UI�� ����� ����
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


    //����� ��Ʈ�ѷ� ��������
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

            //�ð� ��� ����
            print("�ð� �Ͻ� ����");

            ChestOpen();
        }

        if (m_IsChestOpen == true)
        {
            //m_IsChestOpen�� true��� ���� deltaTime��� �Ұ���
            m_FlyTime += Time.unscaledTime;
        }

        if (m_IsPanelArrive == true && Input.GetKeyDown(KeyCode.Return) == true)
        {
            //���� ���ݱ� ���� �Ұ��� ó��
            m_IsChestOpen = false;
            //�ѿ� ��� �����
            m_ItemPanelBG.gameObject.SetActive(false);
            //���� �ݸ��� ��Ȱ ó�� => �浹 ����
            GetComponent<BoxCollider2D>().enabled = false;
            //Debug.Log("�о� �����Կ�...!");
            //�ð� ���� Ǯ��
            Time.timeScale = 1.0f;
            //������ ���� �ǳ� �ٽ� �о�ֱ�
            m_ItemInfoPanelTr.localPosition = m_StartPosition;
            //���� ���� ����
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

    //�÷��̾ ü��Ʈ�� ������ �ִ� ���� ������ �� ������Ʈ �Լ�
    void HudUpdate()
    {

        //���� ������ ���� ���� ó��
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
            //Ÿ�̸� ����
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
        //���� ���� ���·� ��ȯ.
        m_IsChestOpen = true;

        m_ItemPanelBG.gameObject.SetActive(true);

        //�ȳ� ���� ����
        m_ChestHud.gameObject.SetActive(false);

        //���� ���� �̹����� ����
        if (m_ChestRender != null || m_OpenChestSpr != null)
        {
            m_ChestRender.sprite = m_OpenChestSpr;
        }


        //���� ������ ����, UI���� �Լ� ȣ��
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
            //�̹� ������ ��� �Ϸ�
            return;
        }
        //----- ���� �ѱ� ������ ����

        //3���� 5������ ���� ����(����� ��Ÿ���� �����ϴ� ����)
        int a_Idx = Random.Range(3, 6);

        //�� ���� Ŭ�� ��ü�� �ϳ� ����
        Gun_Info a_NewGun = new Gun_Info();

        //����� ���� ��ȣ�� �ش��ϴ� �ѱ� �������� ��ü ������ �����ϴ� init�Խ� ȣ��
        a_NewGun.SetGunInfo((GunType)a_Idx);

        //�۷ι� ������ �÷��̾� ���� �ѱ� ����Ʈ�� �߰�
        GlobalValue.g_MyGunList.Insert(1, a_NewGun);
        m_HeroMgr.m_SpecialGunSpr = a_NewGun.m_GunIconSprite;

        if (2 <= GlobalValue.g_MyGunList.Count)
        {
            m_GunHelpTxt.gameObject.SetActive(true);
        }
        //----- ���� ��ų ������ ����

        //0���� 5������ ���� ���� �߻�(SkillType enum ���� ��Ƽ�� ��ų�� �����ϴ� ����)

        a_Idx = Random.Range(0, 3);//debug_ 0,1,2 3���� �Ѱ���
        //a_Idx = 0;

        //�ش� ������ �ش��ϴ� ���� ��ų ����Ʈ�� ���� ����
        GlobalValue.m_SkillInfoList[a_Idx].m_MyCount++;

        //�������� ���� Ű���� ����
        string a_KeyBuff = string.Format("Skill_{0}_Count", (int)a_Idx);

        //������ ��ų ���� ���ÿ� ����
        PlayerPrefs.SetInt(a_KeyBuff, GlobalValue.m_SkillInfoList[(int)a_Idx].m_MyCount);

        //����� ���� �۷ι����翡 �ҷ��͵α�
        GlobalValue.LoadGlobalValueData();

        //UI����
        UI_Manager a_UIMgr = GameObject.FindObjectOfType<UI_Manager>();
        a_UIMgr.LoadSkillItem();

        //----- ui�� ����� �� ������ ���� �Է� 
        if (m_GunItemImg == null || m_SkillItemImg == null)
        {
            return;
        }

        //m_GunItemImg���� a_Idx�� �ѱ⿡ �ش��ϴ� ��������Ʈ ����
        m_SkillItemImg.sprite = GlobalValue.m_SkillInfoList[a_Idx].m_IconImg;
        m_GunItemImg.sprite = a_NewGun.m_GunIconSprite;


    }

    //��� �������� ������ ����ִ� UI �ǳ� ȭ�� �߾����� �̵��ϴ� ���� ������Ʈ �ڵ�
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

        //���� �ǳ� ������� 
        if (m_ItemInfoPanelTr.localPosition.x < m_TargetPosition.x)
        {
            m_ItemInfoPanelTr.localPosition = Vector3.MoveTowards(m_ItemInfoPanelTr.localPosition,
                m_TargetPosition, 5.0f * Time.unscaledTime);

            //��ǥ ���� �ٻ�ġ�� �����ߴٸ�
            if ((m_TargetPosition.x - 0.5f ) <= m_ItemInfoPanelTr.localPosition.x)
            {
                //������ ������ ó��
                m_ItemInfoPanelTr.localPosition = m_TargetPosition;
                m_IsPanelArrive = true;
                //Time.timeScale = 0.0f;
                //Debug.Log("���� �ǳ� ���� �Ϸ�" + m_IsPanelArrive);
            }
        }
    }
}
