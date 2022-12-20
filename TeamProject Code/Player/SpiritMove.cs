using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritMove : MonoBehaviour
{

    public bool isMove = false;
    public GameObject TargetEnemy;
    public GameObject DamPrefab;
    public float moveSpeed;
    public Animator anim;

    private float m_dist;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isMove)
        {

            m_dist = Vector3.Distance(transform.position, TargetEnemy.transform.position);


            // 대상에게 이동 

            // 거리가 가까워지면 근접공격 모션 밑 이펙트 출력.
            if (TargetEnemy == null)
            {
                Destroy(this.gameObject);
            }
            else if (m_dist < 0.6f)
            {

                if(anim != null)
                {
                    Debug.Log("애니메이션 실행");
                    AnimationManager.GetInstance().PlayAnimation(anim, "Attack01");
                }
                else
                {
                    Debug.Log("애니메이션 null");
                }

                if (TargetEnemy.tag == "Boss")
                {

                    CharacterClass EnemyClass = TargetEnemy.GetComponent<CharacterClass>();
                    CharacterClass PlayerClass = GameObject.Find("Player").transform.GetChild(0).gameObject.GetComponent<CharacterClass>();


                    float minDamage = PlayerClass.m_CharacterStat.Atk - EnemyClass.m_BossStatData.Def;

                    //최소 데미지 보정.
                    if (minDamage <= 0)
                        minDamage = 1;

                    float power = (int)GameObject.Find("Player").transform.GetChild(0).GetComponent<PlayerManager>().m_Spirit.m_SpiritClass.m_SpiritSkillData.Power;

                    float resultDamage = (minDamage * power) * EnemyClass.Invincibility;

                    EnemyClass.m_BossStatData.HP -= resultDamage;
                    //해당 플레이어의 UI에 접근해서 데미지 표시 외적으로 띄어주기.
                    TargetEnemy.GetComponent<BossFSM>().Damage((int)resultDamage);

                    

                }
                else if (TargetEnemy.tag == "Mob")
                {
                    CharacterClass EnemyClass = TargetEnemy.GetComponent<CharacterClass>();
                    CharacterClass PlayerClass = GameObject.Find("Player").transform.GetChild(0).gameObject.GetComponent<CharacterClass>();

                    float minDamage = (int)PlayerClass.m_CharacterStat.Atk;

                    //최소 데미지 보정.
                    if (minDamage <= 0)
                        minDamage = 1;

                    float power = (int)GameObject.Find("Player").transform.GetChild(0).GetComponent<PlayerManager>().m_Spirit.m_SpiritClass.m_SpiritSkillData.Power;

                    float resultDamage = minDamage * power;

                    TargetEnemy.transform.GetComponent<Enemy>().TakeDamage((int)resultDamage);
                }

                Transform[] allChildren = TargetEnemy.GetComponentsInChildren<Transform>();
                Transform PosBody = null;
                foreach (Transform child in allChildren)
                {
                    if (child.name == "PosBody")
                    {
                        PosBody = child;
                    }
                }

                isMove = false;
                Instantiate(DamPrefab, PosBody);
               
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(transform.position, TargetEnemy.transform.position, moveSpeed);
            }

        }


    }
}
