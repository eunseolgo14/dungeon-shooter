using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region ����� ���� Ŭ��
public enum HeroType
{
    FirstHero,
    SecondHero,
    ThirdHero,
    HeroCount
}
public class Hero_Info
{
    public static HeroType m_HeroType;
    public static string m_HeroName;
    //public int m_HeroLevel = 0;
    public static string m_HeroWeapon;
    //public string m_HeroWeapon_2;
    //public string m_HeroPassiveSkill;
    public static string m_HeroPassiveSkill_1;
    public static string m_HeroPassiveSkill_2;
    public static string m_Purpose;
    public static Sprite m_HeroImg = null;
    public static Sprite m_HeroWeapImg = null;
    public static Sprite m_HeroPassImg_1 = null;
    public static Sprite m_HeroPassImg_2 = null;

    public static List<Hero_Info> m_HeroInfoList = new List<Hero_Info>();

    //������ �Լ��� �����ε�
    public static void SetHero(HeroType a_Herotype)
    {
        m_HeroType = a_Herotype;

        if (a_Herotype == HeroType.FirstHero)
        {
            m_HeroName = "�� ���� ���";
            m_HeroWeapon = "[H&K] USP 45";
            m_HeroPassiveSkill_1 = "0% ����";
            m_HeroPassiveSkill_2 = "���� ü�� 20%���� źȯ ������ �پ��";
            m_Purpose = "<���������� ������ ���� �������� ��� ���>\n" +
                "\n�ΰ��� �Ǿ� ���� �ڽ��� ã�ƿ��ִ� �� ����\n" +
                "�մ԰� ģ���� �Ǿ� ������� �翡 ���� �ʹٴ�\n" +
                "�ҿ��� ������ �ִ�."; 

            m_HeroImg = Resources.Load("Hero/penguin_0", typeof(Sprite)) as Sprite;
            m_HeroWeapImg = Resources.Load("Hero/Weapon1", typeof(Sprite)) as Sprite;
            m_HeroPassImg_1 = Resources.Load("IconImg/7", typeof(Sprite)) as Sprite;
            m_HeroPassImg_2 = Resources.Load("IconImg/8", typeof(Sprite)) as Sprite;
        }
        else if (a_Herotype == HeroType.SecondHero)
        {
            m_HeroName = "���ָ� �鰳";
            m_HeroWeapon = "[S&M] M19";
            m_HeroPassiveSkill_1 = "źȯ 1.5�� ���� ���� ���� ";
            m_HeroPassiveSkill_2 = "15% ���� �ִ�ü������ ����";
            m_Purpose = "<ö������ �ѷ��� ���� �������� ��� �鰳>\n" +
                "\n�ΰ��� �Ǿ� ���� �� �ڽ��� �Ƶ鿡�� ������ ����\n" +
                "�� �ΰ����� �Ӱ����� ���ڴ� ��ǥ�� �̷�� ����\n" +
                "�� ������ ���Դ�.";

            m_HeroImg = Resources.Load("Hero/dog_0", typeof(Sprite)) as Sprite;
            m_HeroWeapImg = Resources.Load("Hero/Weapon2", typeof(Sprite)) as Sprite;
            m_HeroPassImg_1 = Resources.Load("IconImg/9", typeof(Sprite)) as Sprite;
            m_HeroPassImg_2 = Resources.Load("IconImg/10", typeof(Sprite)) as Sprite;
        }
        else if (a_Herotype == HeroType.ThirdHero)
        {
            m_HeroName = "�߸��� �����";
            m_HeroWeapon = "Luger P08";
            m_HeroPassiveSkill_1 = "20% ���� ����ӵ�";
            m_HeroPassiveSkill_2 = "���������� �Ѿ �� ü�� 50% ����";
            m_Purpose = "<���׿� ���� ���� ���� ����ϴ� ���� �����>\n" +
                "\n�ڽ��� ��ġ���� �Ǿ ������ �̰ͺ��� ���ڴٴ�\n" +
                "������ ������� �̱��� ���ϰ� ���� ���迡\n" +
                "�����ϱ� ���� �ΰ��� �ǰ��� �Ѵ�.";

            m_HeroImg = Resources.Load("Hero/cat_0", typeof(Sprite)) as Sprite;
            m_HeroWeapImg = Resources.Load("Hero/Weapon3", typeof(Sprite)) as Sprite;
            m_HeroPassImg_1 = Resources.Load("IconImg/11", typeof(Sprite)) as Sprite;
            m_HeroPassImg_2 = Resources.Load("IconImg/12", typeof(Sprite)) as Sprite;
        }

       
    }//public void SetHero(HeroType a_Herotype)

    public void LoadHeroData()
    { 
    
    }
}
#endregion

#region ��ų ���� Ŭ��
public enum SkillType
{
    Skill_0,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5, //<--- ������� Active Skill

    Skill_6,
    Skill_7,
    Skill_8,
    Skill_9,
    Skill_10,
    Skill_11, //<--- ������� Passive Skill
    SkillCount
}
public class Skill_Info
{
    //--- �нú� ��Ƽ�� ��� ��ų �⺻ ������
    public SkillType m_SkillType;
    
    //��ų ������
    public string m_SkillName = "";
    public string m_SkillExp = "";    //��ų ȿ�� ����
    public Sprite m_IconImg = null;   //ĳ���� �����ۿ� ���� �̹���
    public int m_MyCount = 0; //<---������ �ش� ��ų�� ��� �����ϰ� �ִ���

    //--- ��Ƽ�� ��ų ���� ��
    public int m_ActiveSk_Price = 100;   //������ �⺻ ���� 
    public float m_CoolTime = 0.0f;
    public float m_BackUpTime = 0.0f;
    //public int m_ActiveSk_Count = 0;

    //--- �нú� ��ų ������(���׷��̵� �ʿ� ���� �����Ǿ� ���� ������ �ʿ��� ����)
    public int m_PassiveSk_Up_Price = 100; //���׷��̵��� ����, Ÿ�Կ� ���� //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�
    public int m_PassiveSk_Grade = 7;
    //public int m_GemCount = 0;//���׷��̵忡 �ʿ��� ���� ����
    //public string m_GemColor = ""; //���׷��̵忡 �ʿ��� ������ ����



    public void SetSkillType(SkillType a_SkType)
    {
        m_SkillType = a_SkType;

        if (a_SkType == SkillType.Skill_0)
        {
            m_SkillName = "ü�� ȸ��";

            m_ActiveSk_Price = 100; //�⺻����
            m_CoolTime = 10.0f;

            m_SkillExp = "ü�� +10 ȸ��";
            m_IconImg = Resources.Load("IconImg/1", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_1)
        {
            m_SkillName = "����ź";
            m_CoolTime = 30.0f;

            m_ActiveSk_Price = 150; //�⺻����

            m_SkillExp = "��Ÿ� �� ���� ����";
            m_IconImg = Resources.Load("IconImg/2", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            m_SkillName = "����";

            m_ActiveSk_Price = 200; //�⺻����
            m_CoolTime = 30.0f;

            m_SkillExp = "30�ʰ� ������ ��ȣ��";
            m_IconImg = Resources.Load("IconImg/3", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            m_SkillName = "ü�� ȸ��";

            m_ActiveSk_Price = 250; //�⺻����
            m_CoolTime = 30.0f;

            m_SkillExp = "Hp 50% ȸ��";
            m_IconImg = Resources.Load("IconImg/4", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_4)
        {
            m_SkillName = "���� ����";

            m_ActiveSk_Price = 300; //�⺻����
            m_CoolTime = 30.0f;

            m_SkillExp = "1�а� 20%���� ����";
            m_IconImg = Resources.Load("IconImg/5", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_5)
        {
            m_SkillName = "��ȯ��";

            m_ActiveSk_Price = 350; //�⺻����
            m_CoolTime = 120.0f;

            m_SkillExp = "���� �����ϴ� �Ʊ� ��ȯ";
            m_IconImg = Resources.Load("IconImg/6", typeof(Sprite)) as Sprite;
        }

        //--- �ܽ��� Passive Skill Info
        else if (a_SkType == SkillType.Skill_6)
        {
            m_SkillName = "ö����";


            m_SkillExp = "���߷� ��� ������ 20% ����";
            m_IconImg = Resources.Load("IconImg/7", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_7)
        {
            m_SkillName = "�����ϻ�";


            m_SkillExp = "ü���� 20% �̸��϶� źȯ ������ �پ��";
            m_IconImg = Resources.Load("IconImg/8", typeof(Sprite)) as Sprite;
        }

        //--- ���Ƹ� Passive Skill Info
        else if (a_SkType == SkillType.Skill_8)
        {
            m_SkillName = "ū źâ";


            m_SkillExp = "1.05�� ���� źȯ ���� ����";
            m_IconImg = Resources.Load("IconImg/9", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_9)
        {
            m_SkillName = "������ ��";


            m_SkillExp = "�ִ�ü���� 15% ���� ���¿��� ����";
            m_IconImg = Resources.Load("IconImg/10", typeof(Sprite)) as Sprite;
        }

        //--- �Ǵ� Passive Skill Info
        else if (a_SkType == SkillType.Skill_10)
        {
            m_SkillName = "���� �Ѿ�";


            m_SkillExp = "����ӵ� 20%������";
            m_IconImg = Resources.Load("IconImg/11", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_11)
        {
            m_SkillName = "������";


            m_SkillExp = "���� ���������� �Ѿ �� 50�ۼ�Ʈ ü�� ����";
            m_IconImg = Resources.Load("IconImg/12", typeof(Sprite)) as Sprite;
        }


    }//public void SetType(SkillType a_SkType)

    
}
#endregion

#region �۷ι� ��� ���� Ŭ��
public class GlobalValue
{
    public static HeroType g_HeroType;
    public static int g_StageNum = 0;
    public static int g_UserGold;
    public static int g_UserBlueGem;
    public static int g_UserGreenGem;
    public static int g_UserRedGem;
    public static int g_SkillCount = 0;
    public static float g_CurrHP = 100;
    public static float g_MaxHP = 100;

    //�÷��̾� ���� �нú� ��ų ������ ���� ����

    //m_HeroName = "���� �ܽ���";
    //m_HeroWeapon = "�ܽ���_�ѱ�_1";
    //m_HeroPassiveSkill_1 = "���߷� ��� ������ 20% ����";
    //m_HeroPassiveSkill_2 = "ü���� 20% �̸��϶����� źȯ ������ �پ��";

    //

    //public static float act_BulletCushion = 0.0f;
    //public static float act_BombCushion = 0.0f;
    //public static int act_ExtraMagazine = 0;
    //public static float act_ExtraHP = 0.0f;
    //public static float act_FasterShhot = 0.0f;
    //public static float act_SaveHP = 0.0f;

    public static int g_CurScene = -1;

    //��� ��ų�� ������ ������ ���� ����Ʈ
    public static List<Skill_Info> m_SkillInfoList = new List<Skill_Info>();

    //�÷��̾ �����ϰ� �ִ� �ѱ� ���� ����� ����Ʈ ����
    public static List<Gun_Info> g_MyGunList = new List<Gun_Info>(2);

    //�÷��̾ ���� �������� �ѱ� ����
    public static Gun_Info g_CurGun;

    public static void LoadGlobalValueData()
    {
        string a_KeyBuff = "";

        
        //(1) ���� �� ��� ��ų �������� ������ŭ for���� ����
        if (m_SkillInfoList.Count <= 0 ) 
        {
            Skill_Info a_SkillInfo;
            for (int ii = 0; ii < (int)SkillType.SkillCount; ii++)
            {
                a_SkillInfo = new Skill_Info();
                a_SkillInfo.SetSkillType((SkillType)ii);

                //�� �������� �⺻������ ����Ʈ ���鿡 ����
                m_SkillInfoList.Add(a_SkillInfo);
               
            }
        }

        //(2) �÷��̾ �����ϰ� �ִ� ��� ��Ƽ�� ��ų �������� ������ ���÷κ��� �����ͼ� ��´�
        for (int i = 0; i < m_SkillInfoList.Count; i++)//6�� ����
        {
            //�������� ���� Ű���� ����
            a_KeyBuff = string.Format("Skill_{0}_Count", i);

            //����� Ű���� ��� ���� ���� ���� �ִ��� Ȯ��
            m_SkillInfoList[i].m_MyCount = PlayerPrefs.GetInt(a_KeyBuff, 0); //<---ó���� ���� ���� 0�̴�

            ////����� ���(�ش� ������ ���� ������ 0�̶�� 5���� �ְ� ����)
            //if ((int)(m_SkillInfoList[i].m_SkillType) < 6 && m_SkillInfoList[i].m_MyCount == 0)
            //{
            //    m_SkillInfoList[i].m_MyCount = 3;
            //    Debug.Log("������ �߰�");
            //}
        }

        //(3) �÷��̾��� �нú� ��ų �������� ����� �����ͼ� ��´�
        for (int i = 0; i < m_SkillInfoList.Count; i++)
        {
            a_KeyBuff = string.Format("Skill_{0}_Grade", i);
            //����� Ű���� ��� ���� ���� ���� �ִ��� Ȯ��
            m_SkillInfoList[i].m_PassiveSk_Grade = PlayerPrefs.GetInt(a_KeyBuff, 7); //���ٸ� 7�� �ʱ�ȭ
        }
      

        g_HeroType = (HeroType)PlayerPrefs.GetInt("HeroType", 0);
        g_StageNum = PlayerPrefs.GetInt("StageNumber", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
        g_UserBlueGem = PlayerPrefs.GetInt("BlueGem", 0);
        g_UserGreenGem = PlayerPrefs.GetInt("GreenGem", 0);
        g_UserRedGem = PlayerPrefs.GetInt("RedGem", 0);

        //����� ���
        //if (g_UserGold <= 500)
        //{
        //    g_UserGold = 500;
        //}
    }

    public static void SaveGlovalValueData()
    { 
    
        //�켱�� ��ų ����Ʈ��
    }
}
#endregion

//�۷ι������� ���ӵ����� �ҷ����� => �κ�
//��
public enum GunType
{
    Hero1_Defalut,
    Hero2_Defalut,
    Hero3_Defalut,
    Special_1,
    Special_2,
    Special_3
}

public class Gun_Info
{
   
    public bool m_IsAuto = false; //�ڵ� ���� ����
    public string m_GunName = ""; //�̸�
    public string m_Tag = ""; //�̸�
    public float m_Reach = 0.0f; //��Ÿ�
    public float m_BulletSpeed = 0.0f; //ź��
    public int m_MagazineSize = 0; //��ź��
    public float m_LoatTime = 0.0f; //�����ð�
    public int m_Damage = 0; //��������
    public GameObject m_BulletPrefab = null;
    public GameObject m_GunArmPrefab = null;
    public Sprite m_GunIconSprite = null;//ui�� ǥ�õ� �ѱ��� ����
    public bool m_IsInfinite = false;

    public float m_PaneltySpeed = 0.0f; //����� ���� ������ �ڵ�ĸ �̼�
    public float m_Panelty_Hp = 0.0f; //����� ���� ������ �ڵ�ĸ ü��

    public GunType m_GunType;


    public void SetGunInfo(GunType a_gunType)
    {
        m_GunType = a_gunType;

        switch (m_GunType)
        {
            //------ ����Ʈ ����

            case GunType.Hero1_Defalut:

                m_IsAuto = true; 
                m_GunName = "[H&K] USP 45";
                m_Tag = "Default";
                m_Reach = 30.0f;//3.0f;
                m_BulletSpeed = 20.0f;
                m_MagazineSize = 30; 
                m_LoatTime = 2.0f;
                m_Damage = 10; 
                m_BulletPrefab = Resources.Load("Bullet/HeroBullet") as GameObject;

                m_GunIconSprite = Resources.Load("Gun/[H&K] USP 45", typeof(Sprite)) as Sprite; 

                m_PaneltySpeed = 0.0f; 
                m_Panelty_Hp = 0.0f; 

                break;
            case GunType.Hero2_Defalut:

                m_IsAuto = false;
                m_GunName = "[S&M] M19";
                m_Tag = "Default";
                m_Reach = 25.0f;
                m_BulletSpeed = 20.0f;
                m_MagazineSize = 25;
                //m_LoatTime = 1.0f;
                m_Damage = 10;
                m_BulletPrefab = Resources.Load("Bullet/HeroBullet") as GameObject;
                m_GunIconSprite = Resources.Load("Gun/[S&M] M19", typeof(Sprite)) as Sprite; 

                m_PaneltySpeed = 0.0f;
                m_Panelty_Hp = 0.0f;

                break;
            case GunType.Hero3_Defalut:

                m_IsAuto = true;
                m_GunName = "Luger P08";
                m_Tag = "Default";
                m_Reach = 30.0f;
                m_BulletSpeed = 20.0f;
                m_MagazineSize = 35;
                m_LoatTime = 1.0f;
                m_Damage = 10;
                m_BulletPrefab = Resources.Load("Bullet/HeroBullet") as GameObject;
                m_GunIconSprite = Resources.Load("Gun/Luger P08", typeof(Sprite)) as Sprite;

                m_PaneltySpeed = 0.0f;
                m_Panelty_Hp = 0.0f;
                break;

                //------- ����� ����

            case GunType.Special_1:

                m_IsAuto = true;
                m_GunName = "M134 Mini Gun";
                m_Tag = "Special";
                m_Reach = 20.0f;
                m_BulletSpeed = 40.0f;
                m_MagazineSize = 15; //����
                m_IsInfinite = true;
                m_LoatTime = 3.0f;
                m_Damage = 30;
                m_BulletPrefab = Resources.Load("Bullet/SpecialBulletRoot") as GameObject;
                m_GunArmPrefab = Resources.Load("Arm/MiniGun") as GameObject;
                m_GunIconSprite = Resources.Load("Gun/M134-Mini-Gun", typeof(Sprite)) as Sprite;

                m_PaneltySpeed = 5.0f;
                m_Panelty_Hp = 0.05f;
                
                break;

            case GunType.Special_2:

                m_IsAuto = true;
                m_GunName = "Stg 44";
                m_Tag = "Special";
                m_Reach = 20.0f;
                m_BulletSpeed = 35.0f;
                m_MagazineSize = 10;
                m_LoatTime = 3.0f;
                m_Damage = 35;
                m_BulletPrefab = Resources.Load("Bullet/SpecialBulletRoot") as GameObject; 
                m_GunArmPrefab = Resources.Load("Arm/Stg44") as GameObject;
                m_GunIconSprite = Resources.Load("Gun/Stg 44", typeof(Sprite)) as Sprite;

                m_PaneltySpeed = 3.0f;
                m_Panelty_Hp = 0.0f;

                break;
            case GunType.Special_3:

                m_IsAuto = true;
                m_GunName = "Thompson M1A1";
                m_Tag = "Special";
                m_Reach = 20.0f;
                m_BulletSpeed = 25.0f;
                m_MagazineSize = 20;
                m_LoatTime = 2.5f;
                m_Damage = 20;
                m_BulletPrefab = Resources.Load("Bullet/SpecialBulletRoot") as GameObject; 
                m_GunArmPrefab = Resources.Load("Arm/M1A1") as GameObject;
                m_GunIconSprite = Resources.Load("Gun/Thompson M1A1", typeof(Sprite)) as Sprite;

                m_PaneltySpeed = 2.0f;
                m_Panelty_Hp = 0.0f;

                break;
            default:
                break;
        }
    }
}