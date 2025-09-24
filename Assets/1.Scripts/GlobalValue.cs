using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 히어로 관리 클라스
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

    //생성자 함수의 오버로딩
    public static void SetHero(HeroType a_Herotype)
    {
        m_HeroType = a_Herotype;

        if (a_Herotype == HeroType.FirstHero)
        {
            m_HeroName = "말 없는 펭귄";
            m_HeroWeapon = "[H&K] USP 45";
            m_HeroPassiveSkill_1 = "0% 적음";
            m_HeroPassiveSkill_2 = "잔존 체력 20%부터 탄환 데미지 줄어듦";
            m_Purpose = "<잊혀져가는 동네의 작은 수족관에 사는 펭귄>\n" +
                "\n인간이 되어 매일 자신을 찾아와주는 한 여자\n" +
                "손님과 친구가 되어 오래토록 곁에 남고 싶다는\n" +
                "소원을 가지고 있다."; 

            m_HeroImg = Resources.Load("Hero/penguin_0", typeof(Sprite)) as Sprite;
            m_HeroWeapImg = Resources.Load("Hero/Weapon1", typeof(Sprite)) as Sprite;
            m_HeroPassImg_1 = Resources.Load("IconImg/7", typeof(Sprite)) as Sprite;
            m_HeroPassImg_2 = Resources.Load("IconImg/8", typeof(Sprite)) as Sprite;
        }
        else if (a_Herotype == HeroType.SecondHero)
        {
            m_HeroName = "굶주린 들개";
            m_HeroWeapon = "[S&M] M19";
            m_HeroPassiveSkill_1 = "탄환 1.5배 많이 장전 가능 ";
            m_HeroPassiveSkill_2 = "15% 높은 최대체력으로 시작";
            m_Purpose = "<철조망이 둘러진 공터 구석에서 사는 들개>\n" +
                "\n인간이 되어 수년 전 자신의 아들에게 끔찍한 짓을\n" +
                "한 인간에게 앙갚음을 하자는 목표를 이루기 위해\n" +
                "이 던전에 들어왔다.";

            m_HeroImg = Resources.Load("Hero/dog_0", typeof(Sprite)) as Sprite;
            m_HeroWeapImg = Resources.Load("Hero/Weapon2", typeof(Sprite)) as Sprite;
            m_HeroPassImg_1 = Resources.Load("IconImg/9", typeof(Sprite)) as Sprite;
            m_HeroPassImg_2 = Resources.Load("IconImg/10", typeof(Sprite)) as Sprite;
        }
        else if (a_Herotype == HeroType.ThirdHero)
        {
            m_HeroName = "야망가 고양이";
            m_HeroWeapon = "Luger P08";
            m_HeroPassiveSkill_1 = "20% 빠른 연사속도";
            m_HeroPassiveSkill_2 = "스테이지가 넘어갈 때 체력 50% 충전";
            m_Purpose = "<동네에 장이 서는 날에 출몰하는 묘한 고양이>\n" +
                "\n자신이 정치가가 되어도 세상이 이것보단 낫겠다는\n" +
                "생각에 답답함을 이기지 못하고 직접 정계에\n" +
                "진출하기 위해 인간이 되고자 한다.";

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

#region 스킬 관리 클라스
public enum SkillType
{
    Skill_0,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
    Skill_5, //<--- 여기까지 Active Skill

    Skill_6,
    Skill_7,
    Skill_8,
    Skill_9,
    Skill_10,
    Skill_11, //<--- 여기까지 Passive Skill
    SkillCount
}
public class Skill_Info
{
    //--- 패시브 액티브 모든 스킬 기본 설정값
    public SkillType m_SkillType;
    
    //스킬 정보값
    public string m_SkillName = "";
    public string m_SkillExp = "";    //스킬 효과 설명
    public Sprite m_IconImg = null;   //캐릭터 아이템에 사용될 이미지
    public int m_MyCount = 0; //<---유저가 해당 스킬을 몇개나 보유하고 있는지

    //--- 액티브 스킬 설정 값
    public int m_ActiveSk_Price = 100;   //아이템 기본 가격 
    public float m_CoolTime = 0.0f;
    public float m_BackUpTime = 0.0f;
    //public int m_ActiveSk_Count = 0;

    //--- 패시브 스킬 설정값(업그레이드 됨에 따라 변동되어 추후 저장이 필요할 예정)
    public int m_PassiveSk_Up_Price = 100; //업그레이드할 가격, 타입에 따라서 //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) 가격 필요
    public int m_PassiveSk_Grade = 7;
    //public int m_GemCount = 0;//업그레이드에 필요한 보석 갯수
    //public string m_GemColor = ""; //업그레이드에 필요한 보석의 색깔



    public void SetSkillType(SkillType a_SkType)
    {
        m_SkillType = a_SkType;

        if (a_SkType == SkillType.Skill_0)
        {
            m_SkillName = "체력 회복";

            m_ActiveSk_Price = 100; //기본가격
            m_CoolTime = 10.0f;

            m_SkillExp = "체력 +10 회복";
            m_IconImg = Resources.Load("IconImg/1", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_1)
        {
            m_SkillName = "유도탄";
            m_CoolTime = 30.0f;

            m_ActiveSk_Price = 150; //기본가격

            m_SkillExp = "사거리 밖 적을 조준";
            m_IconImg = Resources.Load("IconImg/2", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            m_SkillName = "방패";

            m_ActiveSk_Price = 200; //기본가격
            m_CoolTime = 30.0f;

            m_SkillExp = "30초간 유지되 보호막";
            m_IconImg = Resources.Load("IconImg/3", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            m_SkillName = "체력 회복";

            m_ActiveSk_Price = 250; //기본가격
            m_CoolTime = 30.0f;

            m_SkillExp = "Hp 50% 회복";
            m_IconImg = Resources.Load("IconImg/4", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_4)
        {
            m_SkillName = "빠른 연사";

            m_ActiveSk_Price = 300; //기본가격
            m_CoolTime = 30.0f;

            m_SkillExp = "1분간 20%빠른 연사";
            m_IconImg = Resources.Load("IconImg/5", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_5)
        {
            m_SkillName = "소환수";

            m_ActiveSk_Price = 350; //기본가격
            m_CoolTime = 120.0f;

            m_SkillExp = "적을 공격하는 아군 소환";
            m_IconImg = Resources.Load("IconImg/6", typeof(Sprite)) as Sprite;
        }

        //--- 햄스터 Passive Skill Info
        else if (a_SkType == SkillType.Skill_6)
        {
            m_SkillName = "철갑옷";


            m_SkillExp = "폭발로 얻는 데미지 20% 적음";
            m_IconImg = Resources.Load("IconImg/7", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_7)
        {
            m_SkillName = "구사일생";


            m_SkillExp = "체력이 20% 미만일때 탄환 데미지 줄어듦";
            m_IconImg = Resources.Load("IconImg/8", typeof(Sprite)) as Sprite;
        }

        //--- 병아리 Passive Skill Info
        else if (a_SkType == SkillType.Skill_8)
        {
            m_SkillName = "큰 탄창";


            m_SkillExp = "1.05배 많은 탄환 장전 가능";
            m_IconImg = Resources.Load("IconImg/9", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_9)
        {
            m_SkillName = "초인적 힘";


            m_SkillExp = "최대체력이 15% 높은 상태에서 시작";
            m_IconImg = Resources.Load("IconImg/10", typeof(Sprite)) as Sprite;
        }

        //--- 판다 Passive Skill Info
        else if (a_SkType == SkillType.Skill_10)
        {
            m_SkillName = "빠른 총알";


            m_SkillExp = "연사속도 20%빠르다";
            m_IconImg = Resources.Load("IconImg/11", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_11)
        {
            m_SkillName = "재충전";


            m_SkillExp = "다음 스테이지로 넘어갈 때 50퍼센트 체력 충전";
            m_IconImg = Resources.Load("IconImg/12", typeof(Sprite)) as Sprite;
        }


    }//public void SetType(SkillType a_SkType)

    
}
#endregion

#region 글로벌 밸류 관리 클라스
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

    //플레이어 보유 패시브 스킬 적용을 위한 변수

    //m_HeroName = "성난 햄스터";
    //m_HeroWeapon = "햄스터_총기_1";
    //m_HeroPassiveSkill_1 = "폭발로 얻는 데미지 20% 적음";
    //m_HeroPassiveSkill_2 = "체력이 20% 미만일때부터 탄환 데미지 줄어듦";

    //

    //public static float act_BulletCushion = 0.0f;
    //public static float act_BombCushion = 0.0f;
    //public static int act_ExtraMagazine = 0;
    //public static float act_ExtraHP = 0.0f;
    //public static float act_FasterShhot = 0.0f;
    //public static float act_SaveHP = 0.0f;

    public static int g_CurScene = -1;

    //모든 스킬의 갯수와 정보를 담은 리스트
    public static List<Skill_Info> m_SkillInfoList = new List<Skill_Info>();

    //플레이어가 보유하고 있는 총기 종류 저장용 리스트 변수
    public static List<Gun_Info> g_MyGunList = new List<Gun_Info>(2);

    //플레이어가 지금 장착중인 총기 정보
    public static Gun_Info g_CurGun;

    public static void LoadGlobalValueData()
    {
        string a_KeyBuff = "";

        
        //(1) 게임 내 모든 스킬 아이템의 갯수만큼 for문을 돌며
        if (m_SkillInfoList.Count <= 0 ) 
        {
            Skill_Info a_SkillInfo;
            for (int ii = 0; ii < (int)SkillType.SkillCount; ii++)
            {
                a_SkillInfo = new Skill_Info();
                a_SkillInfo.SetSkillType((SkillType)ii);

                //각 아이템의 기본정보를 리스트 노드들에 저장
                m_SkillInfoList.Add(a_SkillInfo);
               
            }
        }

        //(2) 플레이어가 보유하고 있는 모든 액티브 스킬 아이템의 갯수를 로컬로부터 가져와서 담는다
        for (int i = 0; i < m_SkillInfoList.Count; i++)//6번 돈다
        {
            //아이템의 고유 키값을 발행
            a_KeyBuff = string.Format("Skill_{0}_Count", i);

            //발행된 키값에 담긴 보유 갯수 값이 있는지 확인
            m_SkillInfoList[i].m_MyCount = PlayerPrefs.GetInt(a_KeyBuff, 0); //<---처음엔 갯수 전부 0이다

            ////디버그 모드(해당 아이템 보유 갯수가 0이라면 5개씩 주고 시작)
            //if ((int)(m_SkillInfoList[i].m_SkillType) < 6 && m_SkillInfoList[i].m_MyCount == 0)
            //{
            //    m_SkillInfoList[i].m_MyCount = 3;
            //    Debug.Log("아이템 추가");
            //}
        }

        //(3) 플레이어의 패시브 스킬 아이템의 등급을 가져와서 담는다
        for (int i = 0; i < m_SkillInfoList.Count; i++)
        {
            a_KeyBuff = string.Format("Skill_{0}_Grade", i);
            //발행된 키값에 담긴 보유 갯수 값이 있는지 확인
            m_SkillInfoList[i].m_PassiveSk_Grade = PlayerPrefs.GetInt(a_KeyBuff, 7); //없다면 7로 초기화
        }
      

        g_HeroType = (HeroType)PlayerPrefs.GetInt("HeroType", 0);
        g_StageNum = PlayerPrefs.GetInt("StageNumber", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
        g_UserBlueGem = PlayerPrefs.GetInt("BlueGem", 0);
        g_UserGreenGem = PlayerPrefs.GetInt("GreenGem", 0);
        g_UserRedGem = PlayerPrefs.GetInt("RedGem", 0);

        //디버그 모드
        //if (g_UserGold <= 500)
        //{
        //    g_UserGold = 500;
        //}
    }

    public static void SaveGlovalValueData()
    { 
    
        //우선은 스킬 리스트의
    }
}
#endregion

//글로벌벨류의 게임데이터 불러오기 => 로비
//ㅇ
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
   
    public bool m_IsAuto = false; //자동 장전 여부
    public string m_GunName = ""; //이름
    public string m_Tag = ""; //이름
    public float m_Reach = 0.0f; //사거리
    public float m_BulletSpeed = 0.0f; //탄속
    public int m_MagazineSize = 0; //장탄수
    public float m_LoatTime = 0.0f; //장전시간
    public int m_Damage = 0; //피해정도
    public GameObject m_BulletPrefab = null;
    public GameObject m_GunArmPrefab = null;
    public Sprite m_GunIconSprite = null;//ui로 표시될 총기의 사진
    public bool m_IsInfinite = false;

    public float m_PaneltySpeed = 0.0f; //스페셜 무기 장착시 핸디캡 이속
    public float m_Panelty_Hp = 0.0f; //스페셜 무기 장착시 핸디캡 체력

    public GunType m_GunType;


    public void SetGunInfo(GunType a_gunType)
    {
        m_GunType = a_gunType;

        switch (m_GunType)
        {
            //------ 디폴트 무기

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

                //------- 스페셜 무기

            case GunType.Special_1:

                m_IsAuto = true;
                m_GunName = "M134 Mini Gun";
                m_Tag = "Special";
                m_Reach = 20.0f;
                m_BulletSpeed = 40.0f;
                m_MagazineSize = 15; //무한
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