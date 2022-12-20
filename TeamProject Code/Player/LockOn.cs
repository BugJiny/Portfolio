using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    public bool m_lockOn=false;
    public GameObject target;
    public GameObject m_DamPrefab;
    public float moveSpeed;

    private float m_dist;
    private Vector3 m_targetPos;

    void Start()
    {
        if(m_lockOn)
        {
            this.transform.LookAt(target.transform.position);      // 목표의 방향 설정.
            m_targetPos = target.transform.position;
        }
    }

    void Update()
    {

        if (m_lockOn)
        {
            m_dist = Vector3.Distance(transform.position, target.transform.position);
            if (target == null )
            {
                Destroy(this.gameObject);
            }
            else if(m_dist < 1f)
            {
                if (target.tag == "Boss")
                {

                    CharacterClass EnemyClass = target.GetComponent<CharacterClass>();
                    CharacterClass PlayerClass = GameObject.Find("Player").transform.GetChild(0).gameObject.GetComponent<CharacterClass>();
                    float minDamage = PlayerClass.m_CharacterStat.Atk - EnemyClass.m_BossStatData.Def;

                    //최소 데미지 보정.
                    if (minDamage <= 0)
                        minDamage = 1;

                    float power = (int)GameObject.Find("Player").transform.GetChild(0).GetComponent<PlayerManager>().m_Spirit.m_SpiritClass.m_SpiritSkillData.Power;

                    float resultDamage = (minDamage * power) * EnemyClass.Invincibility;

                    EnemyClass.m_BossStatData.HP -= resultDamage;
                    //해당 플레이어의 UI에 접근해서 데미지 표시 외적으로 띄어주기.
                    target.GetComponent<BossFSM>().Damage((int)resultDamage);

                    Transform[] allChildren = target.GetComponentsInChildren<Transform>();
                    Transform PosBody = null;
                    foreach (Transform child in allChildren)
                    {
                        if (child.name == "PosBody")
                        {
                            PosBody = child;
                        }
                    }

                    Instantiate(m_DamPrefab, PosBody);

                }
                else if(target.tag == "Mob")
                {
                    CharacterClass EnemyClass = target.GetComponent<CharacterClass>();
                    CharacterClass PlayerClass = GameObject.Find("Player").transform.GetChild(0).gameObject.GetComponent<CharacterClass>();

                    float minDamage = (int)PlayerClass.m_CharacterStat.Atk;

                    //최소 데미지 보정.
                    if (minDamage <= 0)
                        minDamage = 1;

                    float power = (int)GameObject.Find("Player").transform.GetChild(0).GetComponent<PlayerManager>().m_Spirit.m_SpiritClass.m_SpiritSkillData.Power;

                    float resultDamage = minDamage * power;

                    target.transform.GetComponent<Enemy>().TakeDamage((int)resultDamage);

                    Transform[] allChildren = target.GetComponentsInChildren<Transform>();
                    Transform PosBody = null;
                    foreach (Transform child in allChildren)
                    {
                        if (child.name == "PosBody")
                        {
                            PosBody = child;
                        }
                    }
                    Instantiate(m_DamPrefab, PosBody);
                }

                Destroy(this.gameObject);
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed);
            }
        }
    }
}
