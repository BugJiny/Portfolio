using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEditor;

public class SpiritSkill : MonoBehaviour
{
    enum SkillNumber { ICE=0, POISON ,INVINCIBILITY ,SANCTITY, HEAL ,SPEED }
    enum SkillType { ATTACK=1, WIDE_MOVE, TARGET, WIDE_FIX, TILE  }
	public GameObject[] Lunch_Prefabs;  //정령의 PosAtk에서 나오는 것.
    public GameObject[] Fire_Prefabs;
    public GameObject[] Damage_Prefabs;
    public Transform[] PosAtk;
    public GameObject[] LunchObjects;
    private int effectNumber = 0;
	private StageManager m_StageMgr;
    private int Row;
    private int Column;

    CharacterClass player_characterclass = null;

    //부채꼴 스킬에 범위안에 들어왔는지 확인할 변수
    bool isCollision;

    Color _red = new Color(1f, 0, 0);
    Color _blue = new Color(0, 0, 1f);
    public float angleRange;  // Cshape1;
    // public float radius;// Cshape2;

    private float minDamage;
    private float resultDamage;
    private float power;
    private CharacterClass PlayerClass;
    public CharacterClass EnemyClass;

    public float recognition = 10;

    public void SkillUse(SpiritSkill_TableExcel skillInfo, GameObject Spirit)  //비타일형
	{

        //현재 플레이어의 좌표
        Row = MainManager.instance.GetStageManager().m_PlayerRow;
        Column = MainManager.instance.GetStageManager().m_PlayerCoulmn;
        
        GameObject tempSpirit = Spirit;

        // 0 -> q / 1 -> w / 2 -> e / 3 -> a / 4 -> s / 5 -> d
        effectNumber = skillInfo.SpritSkillIndex - 170001;
       
        if(PosAtk[effectNumber] == null)
        {
            Transform[] allChildren = Spirit.GetComponentsInChildren<Transform>();

            foreach (Transform child in allChildren)
            {
                if (child.name == "PosAtk")
                {
                    PosAtk[effectNumber] = child;
                }
            }
        }

        Lunch_Prefabs[effectNumber] = PrefabLoader.Instance.PrefabDic[skillInfo.LunchPrefb];
        Fire_Prefabs[effectNumber] = PrefabLoader.Instance.PrefabDic[skillInfo.FirePrefb];
        Damage_Prefabs[effectNumber] = PrefabLoader.Instance.PrefabDic[skillInfo.DamPrefb];

        LockOn LockCheck;
        LockCheck = Fire_Prefabs[effectNumber].GetComponent<LockOn>();  
        if(LockCheck ==null)
        {
            Fire_Prefabs[effectNumber].AddComponent<LockOn>();
            //LockCheck = Fire_Prefabs[effectNumber].GetComponent<LockOn>();
        }


        // 데미지 체크 가능하게
        DamageCheck check;
        check = Fire_Prefabs[effectNumber].GetComponent<DamageCheck>();
        if(check == null)
        {
            Fire_Prefabs[effectNumber].AddComponent<DamageCheck>();
            check = Fire_Prefabs[effectNumber].GetComponent<DamageCheck>();
        }

        check.power = skillInfo.Power;

        // 데미지 프리팹 셋팅 및 데미지 체크 가능하게
        check.dmg_check = true;
        check.m_DamageEffect = Damage_Prefabs[effectNumber];

        // 데미지 프리팹 설정 셋
        DamageEffect dmgCheck;
        dmgCheck = Damage_Prefabs[effectNumber].GetComponent<DamageEffect>();
        if(dmgCheck == null)
        {
            Damage_Prefabs[effectNumber].AddComponent<DamageEffect>();
        }

        // 이건 스킬 고정형태..
        switch (skillInfo.SpritSkillIndex)
        {
            case 170001:  //얼음장판
                StartCoroutine(IceField(skillInfo, Row, Column, effectNumber));
                break;
            case 170002:  //독구름(투사체 고정)
                //StartCoroutine(PoisonCloud(skillInfo, tempSpirit, effectNumber));
                 StartCoroutine(Spirit_Target(skillInfo, tempSpirit, effectNumber));
                break;
            case 170003:  //무적
                StartCoroutine(Invincibility(skillInfo, tempSpirit, effectNumber));
                break;
            case 170004:  //신성지대
                StartCoroutine(Sanctity(skillInfo, tempSpirit, effectNumber));
                break;
            case 170005: //힐
                StartCoroutine(Heal(skillInfo, tempSpirit, effectNumber));
                break;
            case 170006: //이속증가
                StartCoroutine(SpeedField(skillInfo, Row, Column, effectNumber));
                break;
        }

        // 엑셀데이터에서 불러와 만들어지는 가변행태로 만들기.
        // skillInfo.SkillType  // 1-> 근접공격(파이어 프리팹 안씀,대미지 프리팹만 대상에게 출력)
        // 2-> 광역(cshape1 = 부채꼴 각도, cshape2 = 부채꼴의 넓이)  -> 가장 가까운 적을 찾아서 그 적을 추격하며 브레스? 형태의 스킬. 
        // 3-> 원거리 공격(range안에 있는 가장 가까운 적을 TargetNum값만큼 찾아서 fireFrepab실행)
        // 4-> 광역(2번과 똑같지만 고정 브레스 형태의 스킬)
        // 5-> 타일형(보스의 1번과 동일하게) 추후의 보스의 스킬 4가지형태로 추가하는 방향

        // 나중에 스킬셋형태로 보스가 1번 스킬을 사용할때(ex. 4개의 스킬을 들고있는 엑셀 데이터를 불러와서) 거기서 랜덤으로 스킬을 사용하는 ... 작업이 끝나면 추가 예정..
        // 모든 위치 추적 스킬은 보스를 가장 최우선으로 한다..

        //switch ((SkillType)skillInfo.SkillType)
        //{
        //    case SkillType.ATTACK:
        //        StartCoroutine(Spirit_Attack(skillInfo, tempSpirit, effectNumber));
        //        StartCoroutine(Spirit_Attack(skillInfo, tempSpirit, effectNumber));
        //        break;
        //    case SkillType.WIDE_MOVE:
        //        StartCoroutine(Spirit_Wide_Move(skillInfo));
        //        break;
        //    case SkillType.TARGET:
        //        StartCoroutine(Spirit_Target(skillInfo, tempSpirit, effectNumber));
        //        break;
        //    case SkillType.WIDE_FIX:
        //        if (skillInfo.Shapeform == 1)
        //        {
        //            StartCoroutine(SectorFormSkill(skillInfo, tempSpirit, effectNumber));
        //        }
        //        if (skillInfo.Shapeform == 2)
        //        {
        //            StartCoroutine(RectangleSkill(skillInfo, tempSpirit, effectNumber));
        //        }
        //        break;
        //    //StartCoroutine(Spirit_Wide_Fix(skillInfo));
        //    case SkillType.TILE:
        //        StartCoroutine(Spirit_Tile(skillInfo, Row, Column, effectNumber));
        //        break;
        //}
    }

    IEnumerator Spirit_Attack(SpiritSkill_TableExcel skill, GameObject spirit, int effect_num)
    {
        SpiritSkillSoundPlay(skill);
        // 정령이 소환된 후.
        GameObject findEnemy = null;
        // 근처에 가장 가까운 적을 찾는다.
        findEnemy = FindNearbyEnemy(spirit.transform.position, skill.SkillRange);

        if (findEnemy == null) // 널이라면
        {
            StopCoroutine("Spirit_Attack");
        }
        else  // 널이 아니라면.
        {
            spirit.transform.LookAt(findEnemy.transform);

            // SpiritMove에 근접공격을 해야할 대상을 저장.
            spirit.GetComponent<SpiritMove>().isMove = true;
            spirit.GetComponent<SpiritMove>().TargetEnemy = findEnemy; // 근처에 가장 가까운적 넣어주기.
            spirit.GetComponent<SpiritMove>().DamPrefab = Damage_Prefabs[effect_num]; // 데미지 프리팹 넣어주기.
            spirit.GetComponent<SpiritMove>().moveSpeed = skill.BulletSpeed;
        }
        yield return null;
    }

    IEnumerator Spirit_Wide_Move(SpiritSkill_TableExcel skill)
    {

        yield return null;
    }

    IEnumerator Spirit_Target(SpiritSkill_TableExcel skill, GameObject spirit, int effect_num)
    {
        SpiritSkillSoundPlay(skill);
        // 정령이 소환된 후.

        int number = (int)skill.TargetNum;

        List<GameObject> findEnemys = null;

        // 근처에 가장 가까운 적을 찾는다.
        findEnemys = FindNearbyEnemys(spirit.transform.position, skill.SkillRange, number);

        if (findEnemys == null) // 널이라면
        {
            StopCoroutine("Spirit_Target");
        }
        else  // 널이 아니라면.
        {
            if(findEnemys.Count <= 0) //적을 발견하지 못했을 경우..
            {
                StopCoroutine("Spirit_Target");
            }
            else
            {
                spirit.transform.LookAt(findEnemys[0].transform);

                GameObject effect = null;
                foreach (GameObject enemy in findEnemys)
                {

                    effect = Instantiate(Fire_Prefabs[effect_num]);
                    effect.GetComponent<LockOn>().m_DamPrefab = Damage_Prefabs[effect_num];

                    effect.transform.position = spirit.transform.position;
                    effect.GetComponent<LockOn>().m_lockOn = true;
                    effect.GetComponent<LockOn>().target = enemy;
                    effect.GetComponent<LockOn>().moveSpeed = skill.BulletSpeed;


                    // 타겟이 2(적군)일 경우에 데미지 셋팅을 해줌.
                    if (skill.Target == 2)
                        effect.GetComponent<DamageCheck>().dmg_check = false;
                }

                // 딜레이 및 LunchPrefab셋팅
                DelayAndLunchPrefabSet(effect_num);

                float time = 0f;
                while (true)
                {

                    time += Time.deltaTime;

                    if (time > skill.LifeTime)
                    {
                        Destroy(effect);
                        Destroy(LunchObjects[effect_num]);
                        isLunch[effect_num] = false;
                        yield break;
                    }
                   
                    yield return null;
                }
            }
        }

        yield return null;
    }

    IEnumerator Spirit_Wide_Fix(SpiritSkill_TableExcel skill)
    {
        yield return null;
    }

    IEnumerator Spirit_Tile(SpiritSkill_TableExcel skill, int row, int column, int effect_number)
    {
        SpiritSkillSoundPlay(skill);
        int Row = row;
        int Column = column;

        float spirit_time = 0f;

        float range = skill.SkillRange - 1.0f;
        float xRange = skill.SkillRange + range;

        //정령의 스킬범위를 표시해 주는 부분.
        if (range > 0)  //range는 -1을한값 범위가 2부터 여기 들어온다. 범위가 1일경우는 해당 타일에 계산하면 된다.
        {
            int saveRow = Row;
            int saveColumn = 0;

            int checkRow_P;   //정령 기준 아래쪽에 있는 Column값
            int checkRow_M;   //정령 기준 위쪽에 있는 Column값
            int checkColumn;  //현재 색을 바꿀 타일의 Column값

            for (float i = 0; i < skill.SkillRange; i += 1.0f)
            {
                checkRow_P = Row + (int)i;
                checkRow_M = Row - (int)i;

                if (checkRow_P % 2 == 1) //024(135라인)
                    saveColumn++;

                for (float j = 0; j < xRange; j += 1.0f)
                {
                    if (i != 0) //정령이 있는 라인의 +-1라인씩 그림.
                    {
                        checkColumn = saveColumn + (int)j;

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        if (checkRow_P < 5)
                        {
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].SpiritEffect1 = true;
                        }

                        if (checkRow_M >= 0)
                        {
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].SpiritEffect1 = true;
                        }
                    }
                    else  //정령이 있는 라인을 쭉그림.
                    {
                        checkColumn = Column - (int)range + (int)j;

                        if (j == 0)
                            saveColumn = checkColumn;   //정령라인에서 첫번째 타일 색변환위치 저장.

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        m_StageMgr.m_MapInfo[Row, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                        m_StageMgr.m_MapInfo[Row, checkColumn].SpiritEffect1 = true;
                    }
                }
                xRange -= 1.0f;
            }
        }
        else  //range가 0이하면 사거리가 1 자기자신의 타일만 해당
        {
            m_StageMgr.m_MapInfo[Row, Column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
            m_StageMgr.m_MapInfo[Row, Column].SpiritEffect1 = true;
        }

        DelayAndLunchPrefabSet(effect_number);
        yield return new WaitForSeconds(2f);
        
        Destroy(LunchObjects[effect_number]);
        

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                if (m_StageMgr.m_MapInfo[i, j].SpiritEffect1)
                {
                    m_StageMgr.m_MapInfo[i, j].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
                    GameObject effect = Instantiate(Fire_Prefabs[effect_number]);

                    effect.GetComponent<DamageCheck>().Dot = skill.DoT;
                    effect.GetComponent<DamageCheck>().who = 1;
                    ////버프 스킬이 있는지 확인후 스킬실행.
                    //if (skill.BuffAdded != 0)
                    //{

                    //    effect.GetComponent<DamageCheck>().buffIndex = skill.BuffAdded;
                    //}
                    //버프 스킬이 있는지 확인후 스킬실행.
                    if (skill.BuffAdded != 0)
                    {
                        effect.GetComponent<DamageCheck>().buffIndex = skill.BuffAdded;
                    }

                    effect.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
                    m_StageMgr.m_MapInfo[i, j].SpiritEffectObject1 = effect;
                }
            }
        }

        while (true)
        {
            //지속시간 체크
            spirit_time += Time.deltaTime;

            //정령 지속시간이 경과시 
            if (spirit_time >= skill.LifeTime)
            {
                //이펙트 파괴
                for (int i = 0; i < m_StageMgr.mapZ; i++)
                {
                    for (int j = 0; j < m_StageMgr.mapX; j++)
                    {
                        if (m_StageMgr.m_MapInfo[i, j].SpiritEffect1)
                        {
                            m_StageMgr.m_MapInfo[i, j].SpiritEffect1 = false;
                            Object.Destroy(m_StageMgr.m_MapInfo[i, j].SpiritEffectObject1);
                        }
                    }
                }
                //연계스킬있는지 확인후 다시 스킬실행.
                if (skill.SkillAdded != 0)
                {

                }

                isLunch[effect_number] = false;
                yield break;
            }
            yield return null;
        }
    }
    private bool[] isLunch;
    private void DelayAndLunchPrefabSet(int number)
    {
        if (isLunch[number])
            return;

        isLunch[number] = true;

        LunchObjects[number] = Instantiate(Lunch_Prefabs[number]);
        LunchObjects[number].transform.SetParent(PosAtk[number]);
        LunchObjects[number].transform.position = PosAtk[number].position;
    }

    IEnumerator SpeedField(SpiritSkill_TableExcel skill, int row, int column,int n)
    {
        SpiritSkillSoundPlay(skill);
        int Row = row;
        int Column = column;
        int number = n;
        float spirit_time = 0f;

        float range = skill.SkillRange - 1.0f;
        float xRange = skill.SkillRange + range;

        //정령의 스킬범위를 표시해 주는 부분.
        if (range > 0)  //range는 -1을한값 범위가 2부터 여기 들어온다. 범위가 1일경우는 해당 타일에 계산하면 된다.
        {
            int saveRow = Row;
            int saveColumn = 0;

            int checkRow_P;   //정령 기준 아래쪽에 있는 Column값
            int checkRow_M;   //정령 기준 위쪽에 있는 Column값
            int checkColumn;  //현재 색을 바꿀 타일의 Column값

            for (float i = 0; i < skill.SkillRange; i += 1.0f)
            {

                checkRow_P = Row + (int)i;
                checkRow_M = Row - (int)i;


                if (checkRow_P % 2 == 1) //024(135라인)
                    saveColumn++;

                for (float j = 0; j < xRange; j += 1.0f)
                {

                    if (i != 0) //정령이 있는 라인의 +-1라인씩 그림.
                    {

                        checkColumn = saveColumn + (int)j;

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        if (checkRow_P < 5)
                        {
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].SpiritEffect2 = true;

                        }

                        if (checkRow_M >= 0)
                        {
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].SpiritEffect2 = true;

                        }

                    }
                    else  //정령이 있는 라인을 쭉그림.
                    {

                        checkColumn = Column - (int)range + (int)j;

                        if (j == 0)
                            saveColumn = checkColumn;   //정령라인에서 첫번째 타일 색변환위치 저장.

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        m_StageMgr.m_MapInfo[Row, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                        m_StageMgr.m_MapInfo[Row, checkColumn].SpiritEffect2 = true;
                    }


                }

                xRange -= 1.0f;
            }
        }
        else  //range가 0이하면 사거리가 1 자기자신의 타일만 해당
        {
            m_StageMgr.m_MapInfo[Row, Column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
            m_StageMgr.m_MapInfo[Row, Column].SpiritEffect2 = true;

        }

        //경고시간(모든스킬 0.5f초로 고정)이  경과하면 파란색을 다시 원래색깔로 돌린후 그 범위에 이펙트 출현하고 데미지 로직 처리.

        DelayAndLunchPrefabSet(number);

        yield return new WaitForSeconds(2f);

        isLunch[number] = false;
        Destroy(LunchObjects[number]);


        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                if (m_StageMgr.m_MapInfo[i, j].SpiritEffect2)
                {
                    m_StageMgr.m_MapInfo[i, j].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
                    GameObject effect = Instantiate(Fire_Prefabs[(int)SkillNumber.SPEED]);

                    effect.GetComponent<DamageCheck>().Dot = skill.DoT;
                    effect.GetComponent<DamageCheck>().who = 1;

                    //버프 스킬이 있는지 확인후 스킬실행.
                    if (skill.BuffAdded != 0)
                    {
                        effect.GetComponent<DamageCheck>().buffIndex = skill.BuffAdded;
                    }

                    effect.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
                    m_StageMgr.m_MapInfo[i, j].SpiritEffectObject2 = effect;
                    
                }
            }
        }

        float speed;
        float originspeed = this.gameObject.GetComponent<CharacterClass>().m_CharacterStat.MoveSpeed;

        while (true)
        {
            //지속시간 체크
            spirit_time += Time.deltaTime;

            //정령 지속시간이 경과시 
            if (spirit_time >= skill.LifeTime)
            {
                //이펙트 파괴
                for (int i = 0; i < m_StageMgr.mapZ; i++)
                {
                    for (int j = 0; j < m_StageMgr.mapX; j++)
                    {

                        if (m_StageMgr.m_MapInfo[i, j].SpiritEffect2)
                        {
                            m_StageMgr.m_MapInfo[i, j].SpiritEffect2 = false;
                            Object.Destroy(m_StageMgr.m_MapInfo[i, j].SpiritEffectObject2);
                           
                        }
                    }
                }

                this.gameObject.GetComponent<CharacterClass>().m_CharacterStat.MoveSpeed = originspeed;


                //연계스킬있는지 확인후 다시 스킬실행.
                if (skill.SkillAdded != 0)
                {

                }

                yield break;
            }

           if(MainManager.Instance.GetStageManager().GetPlayerMapInfo().SpiritEffect2)
           {
                //transform.Find("Player").transform.GetChild(0).GetComponent<CharacterClass>().m_CharacterStat.MoveSpeed = 20f;

                speed = originspeed;
                speed *= 2f;


                this.gameObject.GetComponent<CharacterClass>().m_CharacterStat.MoveSpeed = speed;

           }
           else
           {
                this.gameObject.GetComponent<CharacterClass>().m_CharacterStat.MoveSpeed = originspeed;
           }

           
            
           yield return null;
        }


       
    }


    IEnumerator IceField(SpiritSkill_TableExcel skill, int row, int column, int n)
    {
        SpiritSkillSoundPlay(skill);
        int Row = row;
        int Column = column;
        int number = n;

        float spirit_time = 0f;

        float range = skill.SkillRange - 1.0f;
        float xRange = skill.SkillRange + range;

        //정령의 스킬범위를 표시해 주는 부분.
        if (range > 0)  //range는 -1을한값 범위가 2부터 여기 들어온다. 범위가 1일경우는 해당 타일에 계산하면 된다.
        {
            int saveRow = Row;
            int saveColumn = 0;

            int checkRow_P;   //정령 기준 아래쪽에 있는 Column값
            int checkRow_M;   //정령 기준 위쪽에 있는 Column값
            int checkColumn;  //현재 색을 바꿀 타일의 Column값

            for (float i = 0; i < skill.SkillRange; i += 1.0f)
            {

                checkRow_P = Row + (int)i;
                checkRow_M = Row - (int)i;


                if (checkRow_P % 2 == 1) //024(135라인)
                    saveColumn++;

                for (float j = 0; j < xRange; j += 1.0f)
                {

                    if (i != 0) //정령이 있는 라인의 +-1라인씩 그림.
                    {

                        checkColumn = saveColumn + (int)j;

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        if (checkRow_P < 5)
                        {
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].SpiritEffect1 = true;

                        }

                        if (checkRow_M >= 0)
                        {
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].SpiritEffect1 = true;

                        }

                    }
                    else  //정령이 있는 라인을 쭉그림.
                    {

                        checkColumn = Column - (int)range + (int)j;

                        if (j == 0)
                            saveColumn = checkColumn;   //정령라인에서 첫번째 타일 색변환위치 저장.

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        m_StageMgr.m_MapInfo[Row, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.blue;
                        m_StageMgr.m_MapInfo[Row, checkColumn].SpiritEffect1 = true;
                    }


                }

                xRange -= 1.0f;
            }
        }
        else  //range가 0이하면 사거리가 1 자기자신의 타일만 해당
        {
            m_StageMgr.m_MapInfo[Row, Column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
            m_StageMgr.m_MapInfo[Row, Column].SpiritEffect1 = true;

        }

        DelayAndLunchPrefabSet(number);
        yield return new WaitForSeconds(2f);
        
        Destroy(LunchObjects[number]);

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {

                if (m_StageMgr.m_MapInfo[i, j].SpiritEffect1)
                {
                    m_StageMgr.m_MapInfo[i, j].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
                    GameObject effect = Instantiate(Fire_Prefabs[(int)SkillNumber.ICE]);

                    effect.GetComponent<DamageCheck>().Dot = skill.DoT;
                    effect.GetComponent<DamageCheck>().who = 1;
                    //버프 스킬이 있는지 확인후 스킬실행.
                    if (skill.BuffAdded != 0)
                    {

                        effect.GetComponent<DamageCheck>().buffIndex = skill.BuffAdded;
                    }

                    effect.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
                    m_StageMgr.m_MapInfo[i, j].SpiritEffectObject1 = effect;
                   
                }
            }
        }

        while(true)
        {
            //지속시간 체크
            spirit_time += Time.deltaTime;

            //정령 지속시간이 경과시 
            if (spirit_time >= skill.LifeTime)
            {
                //이펙트 파괴
                for (int i = 0; i < m_StageMgr.mapZ; i++)
                {
                    for (int j = 0; j < m_StageMgr.mapX; j++)
                    {

                        if (m_StageMgr.m_MapInfo[i, j].SpiritEffect1)
                        {
                            m_StageMgr.m_MapInfo[i, j].SpiritEffect1 = false;
                            Object.Destroy(m_StageMgr.m_MapInfo[i, j].SpiritEffectObject1);
                           
                        }
                    }
                }
                //연계스킬있는지 확인후 다시 스킬실행.
                if (skill.SkillAdded != 0)
                {

                }

                isLunch[number] = false;
                yield break;
            }


            yield return null;
        }

    }

	IEnumerator Heal(SpiritSkill_TableExcel skill, GameObject spirit, int n)
	{
        SpiritSkillSoundPlay(skill);
        GameObject tempEffect = Instantiate(Fire_Prefabs[(int)SkillNumber.HEAL]);
        tempEffect.GetComponent<DamageCheck>().Dot = skill.DoT;
        tempEffect.GetComponent<DamageCheck>().who = 1;

        if(skill.SpritSkillIndex == 1)
        {
            SEManager.instance.PlaySE("Heal1");
        }

        int number = n;

        //버프 스킬이 있는지 확인후 스킬실행.
        if (skill.BuffAdded != 0)
        {

            tempEffect.GetComponent<DamageCheck>().buffIndex = skill.BuffAdded;
        }

        DelayAndLunchPrefabSet(number);

        tempEffect.transform.position = spirit.transform.position;
		Collider[] colls = null;

		float spirit_time = 0f;
		float buff_Time = 0f;

		while (true)
		{
			//지속시간 체크
			spirit_time += Time.deltaTime;
			buff_Time += Time.deltaTime;

			//정령 지속시간이 경과시 
			if (spirit_time >= skill.LifeTime)
			{
				//정령 파괴후 코루틴 종료
				Object.Destroy(tempEffect);
                Destroy(LunchObjects[number]);
                isLunch[number] = false;
                yield break;
			}

			if(buff_Time >= skill.DoT)
			{
				buff_Time = 0f;

                colls = Physics.OverlapSphere(spirit.transform.position, skill.SkillRange, 1 << 14);  //11번째 레이어 = Player    ,14번째 레이어 메인카메라.

                //colls = Physics.OverlapSphere(spirit.transform.position, skill.SkillRange);

				foreach (var rangeCollider in colls)
				{
                    CharacterClass cc = rangeCollider.GetComponent<CharacterClass>();
                    PlayerManager pm = rangeCollider.GetComponent<PlayerManager>();


                    //플레이어 회복시키는 코드.
                    if(pm.GetMaxHp() > cc.m_CharacterStat.HP)
                    {
                        int heal = (int)(pm.GetMaxHp() - cc.m_CharacterStat.HP);

                        if(heal >= 200)
                        {
                            rangeCollider.GetComponent<CharacterClass>().m_CharacterStat.HP += 200f;
                        }
                        else
                        {
                            rangeCollider.GetComponent<CharacterClass>().m_CharacterStat.HP += heal;
                        }

                        Transform PosBody = null;
                        Transform[] allChildren = rangeCollider.GetComponentsInChildren<Transform>();
                        foreach (Transform child in allChildren)
                        {
                            if (child.name == "PosBody")
                            {
                                PosBody = child;
                            }
                        }
                        Instantiate(Damage_Prefabs[effectNumber], PosBody);
                    }
                    else
                    {

                    }

                }

			}
			yield return null;
		}

	}

    //밀려나는 코드
	IEnumerator Sanctity(SpiritSkill_TableExcel skill, GameObject spirit, int n)
	{
        SpiritSkillSoundPlay(skill);
		GameObject tempEffect = Instantiate(Fire_Prefabs[(int)SkillNumber.SANCTITY]);

        tempEffect.GetComponent<DamageCheck>().dmg_check = false;

        tempEffect.transform.position = spirit.transform.position;
		Collider[] colls = null;

        int number = n;
        float spirit_time = 0f;

        DelayAndLunchPrefabSet(number);

        while (true)
		{
			//지속시간 체크
			spirit_time += Time.deltaTime;

			//정령 지속시간이 경과시 
			if (spirit_time >= skill.LifeTime)
			{
				//정령 파괴후 코루틴 종료
				Object.Destroy(tempEffect);
                Destroy(LunchObjects[number]);
                isLunch[number] = false;
                yield break;
			}

            colls = Physics.OverlapSphere(spirit.transform.position, skill.SkillRange, 1 << 9);  //9번째 레이어 = Enemy  //콜라이더가 없어서 OverlapSphere가 안됨. 잡몹한테 콜라이더 부착.
            
            if (colls != null)
			{
               
				foreach (var rangeCollider in colls)
				{
                    
                    //보스가 아니라면
                    if (!rangeCollider.CompareTag("Boss"))  //태그나 레이어로 바꿔야함   => 931001(Clone)는 보스
					{

                        Transform PosBody = null;
                        Transform[] allChildren = rangeCollider.GetComponentsInChildren<Transform>();
                        foreach (Transform child in allChildren)
                        {
                            if (child.name == "PosBody")
                            {
                                PosBody = child;
                            }
                        }
                        Instantiate(Damage_Prefabs[effectNumber], PosBody);

                        //밀리는 물체와 밀리는 마지막 점과의 거리(힘)을 구하고

                        //정령과 밀리는 물체사이의 방향을 구해서
                        var heading = rangeCollider.transform.position - spirit.transform.position;
                        heading.y = 0f;
                        heading *= skill.SkillRange;

                        rangeCollider.gameObject.transform.position = Vector3.MoveTowards(rangeCollider.gameObject.transform.position, heading, 15f);
					}
				}
			}
			yield return null;
		}
	}

	IEnumerator Invincibility(SpiritSkill_TableExcel skill, GameObject spirit, int n)
	{
        SpiritSkillSoundPlay(skill);
        GameObject tempEffect = Instantiate(Fire_Prefabs[(int)SkillNumber.INVINCIBILITY]);
		tempEffect.transform.position = spirit.transform.position;

        int number = n;
        tempEffect.GetComponent<DamageCheck>().dmg_check = false;

        //All 
        Collider[] colls = null;

        //8번째 레이어 = Player
        //무적버프 
        float spirit_time = 0f;
        float skill_time = 0f;

        DelayAndLunchPrefabSet(number);

        while (true)
        {
            //지속시간 체크
            spirit_time += Time.deltaTime;
            skill_time += Time.deltaTime;


            //정령 지속시간이 경과시 
            if (spirit_time >= skill.LifeTime)
            {
                //정령 파괴후 코루틴 종료
                Object.Destroy(tempEffect);
                Destroy(LunchObjects[number]);
                isLunch[number] = false;

                if (player_characterclass != null)
                {
                    player_characterclass.Invincibility = 1.0f;
                }
                    
                yield break;
            }


            if(skill_time >= skill.DoT)
            {
                colls = Physics.OverlapSphere(spirit.transform.position, skill.SkillRange, 1 << 14);

                if (colls.Length > 0)
                {

                    foreach (var p in colls)
                    {

                        if (p.name != "911001(Clone)")
                            continue;
                        else
                        {
                            Transform PosBody = null;
                            Transform[] allChildren = p.GetComponentsInChildren<Transform>();
                            foreach (Transform child in allChildren)
                            {
                                if (child.name == "PosBody")
                                {
                                    PosBody = child;
                                }
                            }


                            Instantiate(Damage_Prefabs[number], PosBody);

                            player_characterclass = p.GetComponent<CharacterClass>();
                            player_characterclass.Invincibility = 0.0f;
                        }
                    }
                }
            }

            yield return null;
        }
	}

	
	IEnumerator PoisonCloud(SpiritSkill_TableExcel skill,GameObject spirit, int n)
	{
        SpiritSkillSoundPlay(skill);
        GameObject nearEnemy = FindNearbyEnemy(spirit.transform.position, skill.SkillRange);
		GameObject tempEffect = null;

        int number = n;

        DelayAndLunchPrefabSet(number);

        if (nearEnemy  != null)
		{
			tempEffect = Instantiate(Fire_Prefabs[(int)SkillNumber.POISON]);
            tempEffect.GetComponent<DamageCheck>().Dot = skill.DoT;
            tempEffect.GetComponent<DamageCheck>().who = 1;

            //버프 스킬이 있는지 확인후 스킬실행.
            if (skill.BuffAdded != 0)
            {

                tempEffect.GetComponent<DamageCheck>().buffIndex = skill.BuffAdded;
            }
            tempEffect.transform.position = nearEnemy.transform.position + new Vector3(0, 5f, 0);
		}

		float spirit_time = 0f;
		float attack_time = 0f;

		while (true)
		{
			//지속시간 체크
			spirit_time += Time.deltaTime;
			attack_time += Time.deltaTime;

			//정령 지속시간이 경과시 
			if (spirit_time >= skill.LifeTime)
			{
				//정령 파괴후 코루틴 종료

				if(nearEnemy != null)
				{
					nearEnemy = null;
                    Object.Destroy(tempEffect);
                }

                Destroy(LunchObjects[number]);

                yield break;
			}

			if (attack_time > skill.DoT)
			{
				//도트 시간이 지나면 데미지 판정.
			}

			yield return null;
		}

	}

	GameObject FindNearbyPlayer(GameObject findStartObject, float distance)
	{
		float Dist = 0f;
		float near = 0f;
		GameObject nearEnemy = null;

		//범위 내의 적을 찾는다.
		Collider[] colls = Physics.OverlapSphere(findStartObject.transform.position, distance, 1 << 8);  //8번째 레이어 = Player
		if (colls.Length == 0)
		{
			return null;
		}
		else
		{

			//적이 있다면 그 적들 중에
			for (int i = 0; i < colls.Length; i++)
			{

				//정령과의 거리를 계산후
				Dist = Vector3.Distance(findStartObject.transform.position, colls[i].transform.position);

				if (i == 0)
				{
					near = Dist;
					nearEnemy = colls[i].gameObject;
				}

				//그 거리가 작다면 거리를 저장하고 해당 오브젝트를 저장
				if (Dist < near)
				{
					near = Dist;
					nearEnemy = colls[i].gameObject;
				}
			}
			return nearEnemy;
		}

	}


	GameObject FindNearbyEnemy(Vector3 StartPos, float distance)
	{
		float Dist = 0f;
		float near = 0f;
		GameObject nearEnemy = null;

		//범위 내의 적을 찾는다.
		Collider[] colls = Physics.OverlapSphere(StartPos, distance, 1 << 9);  //9번째 레이어 = Enemy

		if (colls.Length == 0)
		{
			return null;
		}
		else
		{
			//적이 있다면 그 적들 중에
			for (int i = 0; i < colls.Length; i++)
			{
				//보스가 있을경우 보스로 고정
				if(colls[i].gameObject.name == "Boss")
				{
					nearEnemy = colls[i].gameObject;
					break;
				}

				//정령과의 거리를 계산후
				Dist = Vector3.Distance(StartPos, colls[i].transform.position);

				if (i == 0)
				{
					near = Dist;
					nearEnemy = colls[i].gameObject;
				}

				//그 거리가 작다면 거리를 저장하고 해당 오브젝트를 저장
				if (Dist < near)
				{
					near = Dist;
					nearEnemy = colls[i].gameObject;
				}
			}

			return nearEnemy;
		}
	}


    List<GameObject> FindNearbyEnemys(Vector3 StartPos, float range , int targetNum)
    {
        float Dist = 0f;
        GameObject findBoss = null;

        List<GameObject> findEnemyList = new List<GameObject>();
        
        

        // 범위 내의 적을 찾는다.
        Collider[] colls = Physics.OverlapSphere(StartPos, range, 1 << 9);  //9번째 레이어 = Enemy

        
        // 범위에 적이없을경우
        if (colls.Length <= 0)
        {
            return null;
        }
        else  // 적이 있다면 그 적들 중에
        {
            List<float> EnemyNearList = new List<float>();

            for (int i = 0; i < colls.Length; i++)
            {
                // 보스가 있을경우 보스로 고정
                if (colls[i].gameObject.name == "931001(Clone)")
                {
                    findBoss = colls[i].gameObject;
                    continue;
                }

                // 정령과의 거리를 계산후
                Dist = Vector3.Distance(StartPos, colls[i].gameObject.transform.position);

                if (i < targetNum)  // 범위내의 찾은 적이 설정한 목표치 보다 적을때 전부 리스트에 추가.
                {
                    
                    findEnemyList.Add(colls[i].gameObject);
                    EnemyNearList.Add(Dist);
                }
                else // i >= targetNum    // 범위내의 찾은 적이 설정한 목표보다 초과했을 때 
                {
                   
                    if(findEnemyList.Count < targetNum) // 보스때문에 i가 증가할 수 도 있음...
                    {
                        findEnemyList.Add(colls[i].gameObject);
                        EnemyNearList.Add(Dist);
                    }
                    else // 그게 아니라면
                    {
                        for (int j= findEnemyList.Count -1; j>0; j--)   // 현재 저장된 리스트 순회.(역for문으로 해야 오류가 안남) 정방향으로 하면 인덱스가 삭제될시 당겨지기 때문에.
                        {
                            // 그 거리가 작다면 거리를 저장하고 해당 오브젝트를 저장
                            if (Dist < EnemyNearList[j])
                            {
                                findEnemyList.RemoveAt(j);
                                EnemyNearList.RemoveAt(j);

                                //findEnemyList[j] = colls[i].gameObject;
                                //EnemyNearList[j] = Dist;
                                findEnemyList.Insert(j, colls[i].gameObject);
                                EnemyNearList.Insert(j,Dist);
                                break;
                            }
                        }

                    }

                }

            }

            if(findBoss != null)
            {

                if(findEnemyList.Count < targetNum)
                {
                    findEnemyList.Add(findBoss);
                }
                else
                {
                    int count = findEnemyList.Count-1;
                    findEnemyList.RemoveAt(count);
                    findEnemyList.Insert(count,findBoss);
                }
            }
            return findEnemyList;
        }
    }


    private void Start()
	{
        m_StageMgr = MainManager.Instance.GetStageManager();
        Lunch_Prefabs = new GameObject[6];
        Fire_Prefabs = new GameObject[6];
        Damage_Prefabs = new GameObject[6];

        PosAtk = new Transform[6];
        LunchObjects = new GameObject[6];

        isLunch = new bool[6];
        isLunch[0] = false;
        isLunch[1] = false;
        isLunch[2] = false;
        isLunch[3] = false;
        isLunch[4] = false;
        isLunch[5] = false;
    }


    // 부채꼴 스킬
    IEnumerator SectorFormSkill(SpiritSkill_TableExcel skill, GameObject spirit, int prefabNum)
    {
        SpiritSkillSoundPlay(skill);
        spirit.transform.rotation = Quaternion.Euler(0, -180f, 0);
        GameObject FindEnemys = FindNearbyEnemy(spirit.transform.position, skill.SkillRange);

        if (FindEnemys == null)
            yield break;

        GameObject tempEffect = null;
        Collider[] colls = null;

        angleRange = skill.Cshape1;

        float time = 0f;
        float damageTime = 0f;

        DelayAndLunchPrefabSet(prefabNum);

        if (FindEnemys != null)
        {
            spirit.transform.LookAt(FindEnemys.transform);
            tempEffect = Instantiate(Fire_Prefabs[prefabNum]);
            tempEffect.GetComponent<DamageCheck>().Dot = skill.DoT;
            tempEffect.GetComponent<DamageCheck>().who = 1;
            tempEffect.GetComponent<DamageCheck>().dmg_check = false;
            tempEffect.transform.position = FindEnemys.transform.position + new Vector3(0, 5f, 0);
        }

        while (true)
        {
            time += Time.deltaTime;
            damageTime += Time.deltaTime;

            if (time > skill.LifeTime)
            {
                Destroy(LunchObjects[prefabNum]);
                Destroy(tempEffect);
                yield break;
            }

            if(damageTime> skill.DoT)
            {
                damageTime = 0f;
                colls = Physics.OverlapSphere(spirit.transform.position, skill.SkillRange, 1 << 9);

                foreach (var rangeCollider in colls)
                {
                    Vector3 interV = rangeCollider.transform.position - spirit.transform.position;
                    // '타겟-나 벡터'와 '내 정면 벡터'를 내적
                    float dot = Vector3.Dot(interV.normalized, spirit.transform.forward);
                    // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
                    float theta = Mathf.Acos(dot);
                    // angleRange와 비교하기 위해 degree로 변환
                    float degree = Mathf.Rad2Deg * theta;

                    if (degree <= angleRange / 2f)
                    {
                        if (rangeCollider.CompareTag("Boss"))
                        {
                            EnemyClass = rangeCollider.GetComponent<CharacterClass>();
                            minDamage = PlayerClass.m_CharacterStat.Atk - EnemyClass.m_BossStatData.Def;

                            //최소 데미지 보정.
                            if (minDamage <= 0)
                                minDamage = 1;

                            power = (int)transform.GetChild(0).GetComponent<PlayerManager>().m_Spirit.m_SpiritClass.m_SpiritSkillData.Power;

                            resultDamage = (minDamage * power) * EnemyClass.Invincibility;

                            rangeCollider.GetComponent<BossFSM>().m_BossClass.m_BossStatData.HP -= resultDamage;
                        }
                        if(rangeCollider.CompareTag("Mob"))
                        {
                            EnemyClass = rangeCollider.GetComponent<CharacterClass>();
                            minDamage = (int)PlayerClass.m_CharacterStat.Atk - EnemyClass.m_SubMonsterData.Defense;

                            //최소 데미지 보정.
                            if (minDamage <= 0)
                                minDamage = 1;

                            power = (int)transform.GetChild(0).GetComponent<PlayerManager>().m_Spirit.m_SpiritClass.m_SpiritSkillData.Power;

                            resultDamage = minDamage * power;

                            rangeCollider.GetComponent<Enemy>().TakeDamage((int)resultDamage);
                        }
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator RectangleSkill(SpiritSkill_TableExcel skill, GameObject spirit, int prefabNum)
    {
        SpiritSkillSoundPlay(skill);
        spirit.transform.rotation = Quaternion.Euler(0, -180f, 0);
        GameObject FindEnemys = FindNearbyEnemy(spirit.transform.position, skill.SkillRange);

        if (FindEnemys == null)
            yield break;

        GameObject tempEffect = null;
        Collider[] colls = null;

        angleRange = skill.Cshape1;

        float time = 0f;
        float damageTime = 0f;

        DelayAndLunchPrefabSet(prefabNum);

        if (FindEnemys != null)
        {
            spirit.transform.LookAt(FindEnemys.transform);
            tempEffect = Instantiate(Fire_Prefabs[prefabNum]);
            tempEffect.GetComponent<DamageCheck>().Dot = skill.DoT;
            tempEffect.GetComponent<DamageCheck>().who = 1;
            tempEffect.GetComponent<DamageCheck>().dmg_check = false;
            tempEffect.transform.position = FindEnemys.transform.position + new Vector3(0, 5f, 0);
        }

        while (true)
        {
            time += Time.deltaTime;
            damageTime += Time.deltaTime;

            if (time > skill.LifeTime)
            {
                Destroy(LunchObjects[prefabNum]);
                Destroy(tempEffect);
                yield break;
            }

            if (damageTime > skill.DoT)
            {
                damageTime = 0f;
                colls = Physics.OverlapBox(spirit.transform.position, new Vector3(2, 1, skill.SkillRange) * 0.5f, spirit.transform.rotation , 1 << 9);

                foreach (var rangeCollider in colls)
                {
                    if (rangeCollider.CompareTag("Boss"))
                    {
                        rangeCollider.GetComponent<BossFSM>().m_BossClass.m_BossStatData.HP -= 100;
                    }
                    if (rangeCollider.CompareTag("Mob"))
                    {
                        rangeCollider.GetComponent<Enemy>().TakeDamage(100);
                    }
                }
            }
            yield return null;
        }
    }

    void SpiritSkillSoundPlay(SpiritSkill_TableExcel skill)
    {
        switch(skill.SpritSkillIndex)
        {
            case 170012:
            case 170001:
                SEManager.instance.PlaySE("Slasher");
                break;
            case 170011:
            case 170002:
                SEManager.instance.PlaySE("Throw");
                break;
            case 170003:
                SEManager.instance.PlaySE("Holy");
                break;
            case 170004:
                SEManager.instance.PlaySE("KnockBack");
                break;
            case 170005:
                SEManager.instance.PlaySE("Heal1");
                break;
            case 170006:
                SEManager.instance.PlaySE("SpeedUp");
                break;
            case 170007:
                SEManager.instance.PlaySE("CutDown");
                break;
            case 170008:
                SEManager.instance.PlaySE("FirePilar");
                break;
            case 170009:
                SEManager.instance.PlaySE("Boom");
                break;
            case 170010:
                SEManager.instance.PlaySE("Wind");
                break;
            case 170014:
                SEManager.instance.PlaySE("Stun");
                break;
            case 170015:
                SEManager.instance.PlaySE("Heal2");
                break;
            case 170016:
                SEManager.instance.PlaySE("AttackField");
                break;
        }
    }
}
