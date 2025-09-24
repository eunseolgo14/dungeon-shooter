using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    public GameObject m_WormBulletPrefab = null;

    BulletManager m_BulletMgr = null;
    GameObject m_BulletObj = null;
    Vector3 m_DirVec = Vector3.zero;
    float a_radius = 1.0f;

    float m_WaitTime = 1.0f;

    Animator m_Anim = null;
    // Start is called before the first frame update
    void Start()
    {
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_WaitTime)
        {
            m_WaitTime -= Time.deltaTime;

            if (m_WaitTime <= 0.0f)
            {
                ShootFire();
                m_Anim.SetTrigger("IsDisappear");
            }
        }
    }

    void ShootFire()
    {
        float a_CacAngle = 0.0f;
        int i = 0;
        for (float angle = 0.0f; angle < 360.0f; angle += 15.0f) // => 이 15도씩 틀어서 발사한다를 한 프레임에 이루어진다? => 단발 확산 발사의 효과
        {
            m_DirVec.x = a_radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            m_DirVec.y = a_radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            m_DirVec.Normalize();

            m_BulletObj = Instantiate(m_WormBulletPrefab) as GameObject;
            m_BulletMgr = m_BulletObj.GetComponent<BulletManager>();

            m_BulletObj.transform.position = this.transform.position;


            a_CacAngle = Mathf.Atan2(m_DirVec.y, m_DirVec.x) * Mathf.Rad2Deg;
            a_CacAngle += 180.0f;

            m_BulletObj.transform.eulerAngles = new Vector3(0.0f, 0.0f, a_CacAngle);



            m_BulletMgr.m_FlyDir = Vector3.right; //(this.transform.right하니까 이상한 방향임)
            m_BulletMgr.m_BulletType = BulletType.EnemyBullet;
            m_BulletMgr.m_StartPos = m_BulletObj.transform.position;
            m_BulletMgr.m_Reach = 4.5f;
            m_BulletMgr.m_Speed = 5.0f;

        }
    }

    public void DisappearEvent()
    {

        Destroy(this.gameObject);
    }
   
}
