using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "HeroBullet")
        {
            //히어로 불렛은 충돌 무효처리
            return;
        }
        else if (collision.gameObject.tag == "EnemyBullet" || collision.gameObject.tag == "BossBullet")
        {
            //print("적의 총알 충돌");
            Destroy(collision.gameObject);
        }

    }


}
