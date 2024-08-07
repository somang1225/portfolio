using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int wpID;
    public int prefabID;
    public int count;
    public float Wp_damage; //무기의 기본 강화 상수
    public float speed;

    float timer;
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if(!GameManager.Instance.isLive)
        {
            return;
        }

        switch (wpID)
        {
            case 1:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;

            default:

                timer += Time.deltaTime;

                if(timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Weapon_LevelUP(2);
            //Debug.Log("레벨업");
        }
        
    }


    public void Init()
    {
        //count++;
        switch (wpID)
        {
            case 1:
                count++;
                speed = 150;
                Batch();
                break; 
            default:
                speed = 0.3f;
                break;
        }
    }

    public void Weapon_LevelUP(float damage)
    {
        this.Wp_damage += damage;
        //this.count += count;


    }


    //근접용 무기
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.pool.Get_weapon(prefabID).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 roVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(roVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(Wp_damage, -100, Vector3.zero); //-1은 무한 관통
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.Instance.pool.Get_weapon(prefabID).transform;
        bullet.parent = transform;

        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(Wp_damage, count, dir);

    }

}
