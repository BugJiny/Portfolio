using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public partial class EMath
{
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        float Func(float x) => 4 * (-height * x * x + height * x);

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        float Func(float x) => 4 * (-height * x * x + height * x);

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t));
    }
}

public class BossSkill : MonoBehaviour
{
    public enum BossSkillType { WIDE = 1, TARGET, LINE, DIFFUSION, SUMMONS }
    public GameObject LineSkillTarget;
    private BossFSM m_BossFSM;
    public GameObject MobPrefabs;
    public GameObject DamagePrefab;  // 맞았을때 이펙트 
    public GameObject firePrefab;    // 실제로 보이거나 날아가는 이펙트
    public GameObject LunchPrefab;   // PosAtk의 위치에서 발생되는 이펙트
    public GameObject DelayPrefab;   // 딜레이 시 나올 프리팹.
    public GameObject EmptyPrefab;   // SingleTile일때 데미지 판정을 위한 임시 이펙트
    public GameObject DebuffTilePrefab; //디버프 타일 생성에 사용할 프리팹
    public Transform PosAtk;
    private Animator anim;

    public float angleRange = 60f;
    public float distance = 5f;
    public bool isCollision = false;
    //디버프 타일 지속시간 임시 변수
    public float DelayTime = 10f;
    Color _origin = new Color(0f, 0.541f, 0.603f, 0.784f); //원래대로 돌릴 색깔

    private int m_PlayerRow;
    private int m_PlayerColumn;

    private int m_BossRow;
    private int m_BossColumn;

    private StageManager m_StageMgr;
    private bool TargetLockOn = false;
    private int m_TargetRow;
    private int m_TargetColumn;

    //bool DebuffTile;

    public Transform PlayerPosition; //잡몹의 플레이어 위치를 잡기 위한 변수
    public BossSkill_TableExcel m_CurrentBossSkill;
    private GameObject TempLunch = null;

    struct ClockDirectionList
    {
        public int row;
        public int column;
        public int check;
        public bool effect;  //스킬(이펙트)을 발동할 타일.
    }

    ClockDirectionList[] OriginArr;

    public void BossSkillAction(int skillIndex)
    {
        foreach (var item in DataTableManager.Instance.GetDataTable<BossSkill_TableExcelLoader>().DataList)
        {
            if (item.BossSkillIndex == skillIndex)
            {
                m_CurrentBossSkill = item; //현재 스킬 정보를 찾아낸다.
                break;
            }
        }

        //플레이어의 위치와 보스의 배열 위치를 알아낸다.
        m_PlayerRow = MainManager.Instance.GetStageManager().m_PlayerRow;
        m_PlayerColumn = MainManager.Instance.GetStageManager().m_PlayerCoulmn;
        m_BossRow = MainManager.Instance.GetStageManager().m_BossRow;
        m_BossColumn = MainManager.Instance.GetStageManager().m_BossColumn;

        LunchPrefab = PrefabLoader.Instance.PrefabDic[m_CurrentBossSkill.LunchPrefb];
        DelayPrefab = PrefabLoader.Instance.PrefabDic[m_CurrentBossSkill.DelayPrefb];
        EmptyPrefab = PrefabLoader.Instance.PrefabDic[000000];
        firePrefab = PrefabLoader.Instance.PrefabDic[m_CurrentBossSkill.FirePrefb];
        DamagePrefab = PrefabLoader.Instance.PrefabDic[m_CurrentBossSkill.DamPrefb];

        DamageCheck check1,check2;
        check1 = firePrefab.GetComponent<DamageCheck>();
        if (check1 == null)
        {
            firePrefab.AddComponent<DamageCheck>();
            check1 = firePrefab.GetComponent<DamageCheck>();
        }

        check1.dmg_check = true;
        DamageEffect dmgCheck;
        dmgCheck = DamagePrefab.GetComponent<DamageEffect>();
        if (dmgCheck == null)
        {
            DamagePrefab.AddComponent<DamageEffect>();
        }

        check1.m_DamageEffect = DamagePrefab;  // 데미지 프리팹 넣어주기.

        check2 = EmptyPrefab.GetComponent<DamageCheck>();
        if (check2 == null)
        {
            EmptyPrefab.AddComponent<DamageCheck>();
        }
        check2.dmg_check = true;
        check2.m_DamageEffect = PrefabLoader.Instance.PrefabDic[m_CurrentBossSkill.DamPrefb];    // 공백프리팹에 데미지 프리팹 넣어주기.

        DebuffTileChk debuffCheck;
        debuffCheck = DebuffTilePrefab.GetComponent<DebuffTileChk>();
        if(debuffCheck == null)
        {
            DebuffTilePrefab.AddComponent<DebuffTileChk>();
        }

        SkillAction();
    }

    void SkillAction()
    {
        this.gameObject.GetComponent<BossFSM>().behavior = true;

        switch ((BossSkillType)m_CurrentBossSkill.SkillType)
        {
            case BossSkillType.WIDE:
                StartCoroutine(SkillWideAction());
                break;
            case BossSkillType.TARGET:
                StartCoroutine(SkillTargetAction());
                TargetLockOn = true;
                break;
            case BossSkillType.LINE:
                StartCoroutine(SkillLineAction());
                break;
            case BossSkillType.DIFFUSION:
                StartCoroutine(SkillDiffusionAction());
                break;
            case BossSkillType.SUMMONS:
                StartCoroutine(MobSummons());
                break;
        }
    }

    //쫄몹 소환 코루틴
    IEnumerator MobSummons()
    {
        int Row = MainManager.Instance.GetStageManager().m_BossRow; //현재 보스 Row
        int column = MainManager.Instance.GetStageManager().m_BossColumn; //현재 보스 Column

        m_StageMgr.m_MapInfo[Row, column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red; //현재 보스가 서있는 타일의 색깔을 red로 변경
        m_StageMgr.m_MapInfo[Row, column].BossEffect = true; //현재 타일의 보스 이펙트를 true로 설정
        PlayerPosition = GameObject.Find("Player").transform;

        yield return new WaitForSeconds(m_CurrentBossSkill.SkillDelay);

        m_StageMgr.m_MapInfo[Row, column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
        GameObject effect = Instantiate(firePrefab);
        effect.transform.position = m_StageMgr.m_MapInfo[Row, column].MapPos + new Vector3(0, 5f, 0);
        m_StageMgr.m_MapInfo[Row, column].BossEffectObject = effect;

        //몬스터 소환.
        int m_Count = 3;
        GameObject[] monsters = new GameObject[m_Count];

        for (int i = 0; i < m_Count; i++)
        {
            float randX = Random.Range(this.gameObject.transform.position.x - 15f, this.gameObject.transform.position.x + 15f);
            float randZ = Random.Range(this.gameObject.transform.position.z - 15f, this.gameObject.transform.position.z + 15f);

            monsters[i] = Instantiate(MobPrefabs);

            monsters[i].transform.position = new Vector3(randX, 0, randZ);
        }

        yield return new WaitForSeconds(m_CurrentBossSkill.LifeTime);

        m_StageMgr.m_MapInfo[Row, column].BossEffect = false;
        Destroy(m_StageMgr.m_MapInfo[Row, column].BossEffectObject);

        StartCoroutine(AllTileOriginColor());
        //AnimationManager.GetInstance().PlayAnimation(anim, "Idle01");
        this.gameObject.GetComponent<BossFSM>().behavior = false;

        yield return null;
    }

    private void DelayAndLunchPrefabSet()
    {
        TempLunch = Instantiate(LunchPrefab);
        TempLunch.transform.SetParent(PosAtk);
        TempLunch.transform.position = PosAtk.position;

        //lunch.transform.position = PosAtk.position;

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                if (m_StageMgr.m_MapInfo[i, j].BossEffect || m_StageMgr.m_MapInfo[i, j].EmptyEffect)
                {
                    GameObject effect = Instantiate(DelayPrefab);
                    effect.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);

                    m_StageMgr.m_MapInfo[i, j].BossDelayObject = effect;
                }
            }
        }
    }

    IEnumerator SkillWideAction()
    {
        int Row = 0;
        int Column = 0;

        //DebuffTile = true;

        if (TargetLockOn)
        {
            Row = m_TargetRow;
            Column = m_TargetColumn;
        }
        else
        {
            Row = m_BossRow;
            Column = m_BossColumn;
        }

        float range = m_CurrentBossSkill.SkillRange - 1.0f;
        float xRange = m_CurrentBossSkill.SkillRange + range;

        //보스 스킬범위를 표시해 주는 부분.
        if (range > 0)  //range는 -1을한값 범위가 2부터 여기 들어온다. 범위가 1일경우는 해당 타일에 계산하면 된다.
        {
            // 규칙 1
            // 가운데 줄은 해당 거리+ -1 칸만큼   ex) 범위가 2인스킬의 경우 2+(2-1) = 3칸이 보스 중심의 가장 먼 거리가 되고 대칭으로 -1씩줄어들며 3칸...2칸이됨
            // ex) 범위가 3인스킬의 경우 3+(3-1) = 5칸이 보스위치의 가장 먼거리가 되고 대칭으로 -1씩 줄어들며 5칸...4칸...3칸으로 칸수가 된다.
            // 따라서 가운데 범위는 보스가 위치한 좌표를 기준으로 [z,x-(범위-1)]~ [z,x+(범위-1)]한 범위가 된다 ex) [2,2]가 보스위치고 범위가 3이라면 [2,0] ~ [2,4]의 위치로.

            // 규칙 2 
            // 보스라인이 끝나면 나머지 줄은 대칭해서  ex)  ([z-1,x] / [z+1,x]) | ([z-2,x] / [z+2,x])의     페어형식으로 증가한다.

            // 규칙 3
            // 보스의 위치가 배열의 0,2,4번째요소(1,3,5번째 줄) 일 경우 제일 마지막배열에서 row값+-1,  column값은 변경이없다. 
            // 배열의 1,3번째요소(2,4번째 줄) 일 경우 제일 마지막배열에서 row값+-1, column값 +1 값을 해줘야한다.

            int saveRow = Row;
            int saveColumn = 0;

            int checkRow_P;   //보스 기준 아래쪽에 있는 Column값
            int checkRow_M;   //보스 기준 위쪽에 있는 Column값
            int checkColumn;  //현재 색을 바꿀 타일의 Column값

            for (float i = 0; i < m_CurrentBossSkill.SkillRange; i += 1.0f)
            {
                checkRow_P = Row + (int)i;
                checkRow_M = Row - (int)i;

                if (checkRow_P % 2 == 1) //024(135라인)
                    saveColumn++;

                for (float j = 0; j < xRange; j += 1.0f)
                {
                    if (i != 0) //보스가 있는 라인의 +-1라인씩 그림.
                    {
                        checkColumn = saveColumn + (int)j;
                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        if (checkRow_P < 5)
                        {
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                            
                            if(!m_CurrentBossSkill.SingleTile)
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].BossEffect = true;
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].BossEffect = false;
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].EmptyEffect = true;
                            } 
                        }

                        if (checkRow_M >= 0)
                        {
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                            if (!m_CurrentBossSkill.SingleTile)
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].BossEffect = true;
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].BossEffect = false;
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].EmptyEffect = true;
                            }
                        }
                    }
                    else  //보스가 있는 라인을 쭉그림.
                    {
                        checkColumn = Column - (int)range + (int)j;
                        if (j == 0)
                            saveColumn = checkColumn;   //보스라인에서 첫번째 타일 색변환위치 저장.

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;
                       
                        m_StageMgr.m_MapInfo[Row, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                        
                        if (!m_CurrentBossSkill.SingleTile)
                        {
                            m_StageMgr.m_MapInfo[Row, checkColumn].BossEffect = true;
                            m_StageMgr.m_MapInfo[Row, checkColumn].EmptyEffect = false;
                        }
                        else
                        {
                            if(checkColumn == Column)
                            {
                                m_StageMgr.m_MapInfo[Row, checkColumn].BossEffect = true;
                                m_StageMgr.m_MapInfo[Row, checkColumn].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[Row, checkColumn].BossEffect = false;
                                m_StageMgr.m_MapInfo[Row, checkColumn].EmptyEffect = true;
                            }
                        }        
                    }
                }
                xRange -= 1.0f;
            }
        }
        else  //range가 0이하면 사거리가 1 자기자신의 타일만 해당
        {
            m_StageMgr.m_MapInfo[Row, Column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
            m_StageMgr.m_MapInfo[Row, Column].BossEffect = true;
            m_StageMgr.m_MapInfo[Row, Column].EmptyEffect = false;
        }

        //경고시간동안 딜레이 프리팹 생성한 것을 지우고 타일 빨간색을 다시 원래색깔로 돌린후 그 범위에 이펙트 출현하고 데미지 로직 처리.
        DelayAndLunchPrefabSet();
        yield return new WaitForSeconds(m_CurrentBossSkill.SkillDelay);
        Destroy(TempLunch);

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                checkSettingEffect(i, j);
            }
        }

        yield return new WaitForSeconds(m_CurrentBossSkill.LifeTime);  //생존시간이 지나면 이펙트 지우기

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {

                if (m_StageMgr.m_MapInfo[i, j].BossEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].BossEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEffectObject);
                }
                else if(m_StageMgr.m_MapInfo[i, j].EmptyEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].EmptyEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEmptyObject);
                }
            }
        }

        StartCoroutine(AllTileOriginColor());

        //연계스킬있는지 확인후 다시 스킬실행.
        if (m_CurrentBossSkill.SkillAdded != 0)
            BossSkillAction(m_CurrentBossSkill.SkillAdded);
        else
        {
            this.gameObject.GetComponent<BossFSM>().behavior = false;
            if (TargetLockOn)
            {
                TargetLockOn = false;
                m_TargetRow = 0;
                m_TargetColumn = 0;
            }
        }
        yield break;
    }

    IEnumerator SkillTargetAction()
    {
        //타켓에서 연계스킬이 있는경우 광역,확산,도넛의 시작범위가 보스가 아니라 타겟된 대상을 기준으로 적용됨.
        float range = m_CurrentBossSkill.SkillRange - 1.0f;
        float xRange = m_CurrentBossSkill.SkillRange + range;

        m_TargetRow = m_PlayerRow;
        m_TargetColumn = m_PlayerColumn;

        //보스 스킬범위를 표시해 주는 부분.
        if (range > 0)  //range는 -1을한값 범위가 2부터 여기 들어온다. 범위가 1일경우는 해당 타일에 계산하면 된다.
        {
            //타겟의 경우 와이드 스킬의 범위기준을 보스가 아닌 플레이어의 위치좌표로 잡으면 된다. 규칙은 와이드 스킬가 동일.

            int saveRow = m_PlayerRow;
            int saveColumn = 0;

            int checkRow_P;   //보스 기준 아래쪽에 있는 Column값
            int checkRow_M;   //보스 기준 위쪽에 있는 Column값
            int checkColumn;  //현재 색을 바꿀 타일의 Column값

            for (float i = 0; i < m_CurrentBossSkill.SkillRange; i += 1.0f)
            {
                checkRow_P = m_PlayerRow + (int)i;
                checkRow_M = m_PlayerRow - (int)i;

                if (checkRow_P % 2 == 1) //024(135라인)
                    saveColumn++;

                for (float j = 0; j < xRange; j += 1.0f)
                {
                    if (i != 0) //보스가 있는 라인의 +-1라인씩 그림.
                    {
                        checkColumn = saveColumn + (int)j;

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        if (checkRow_P < 5)
                        {
                            m_StageMgr.m_MapInfo[checkRow_P, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                            
                            if(!m_CurrentBossSkill.SingleTile)
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].BossEffect = true;
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].BossEffect = false;
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].EmptyEffect = true;
                            }   
                        }

                        if (checkRow_M >= 0)
                        {
                            m_StageMgr.m_MapInfo[checkRow_M, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                            if (!m_CurrentBossSkill.SingleTile)
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].BossEffect = true;
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].BossEffect = false;
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].EmptyEffect = true;
                            }
                        }
                    }
                    else  //플레이어가 있는 라인을 쭉그림.
                    {
                        checkColumn = m_PlayerColumn - (int)range + (int)j;

                        if (j == 0)
                            saveColumn = checkColumn;   //보스라인에서 첫번째 타일 색변환위치 저장.

                        if (checkColumn < 0 || checkColumn > 5)
                            continue;

                        m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                        if (!m_CurrentBossSkill.SingleTile)
                        {
                            m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].BossEffect = true;
                            m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].EmptyEffect = false;
                        }
                        else
                        {
                            if (checkColumn == m_PlayerColumn)
                            {
                                m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].BossEffect = true;
                                m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].BossEffect = false;
                                m_StageMgr.m_MapInfo[m_PlayerRow, checkColumn].EmptyEffect = true;
                            }
                        }
                    }
                }
                xRange -= 1.0f;
            }
        }
        else  //range가 0이하면 사거리가 1 자기자신의 타일만 해당
        {
            m_StageMgr.m_MapInfo[m_PlayerRow, m_PlayerColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
            m_StageMgr.m_MapInfo[m_PlayerRow, m_PlayerColumn].BossEffect = true;
            m_StageMgr.m_MapInfo[m_PlayerRow, m_PlayerColumn].EmptyEffect = false;
        }

        DelayAndLunchPrefabSet();
        yield return new WaitForSeconds(m_CurrentBossSkill.SkillDelay);
        Destroy(TempLunch);

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                checkSettingEffect(i, j);
            }
        }

        yield return new WaitForSeconds(m_CurrentBossSkill.LifeTime);  //생존시간이 지나면 이펙트 지우기

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {

                if (m_StageMgr.m_MapInfo[i, j].BossEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].BossEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEffectObject);
                }
                else if (m_StageMgr.m_MapInfo[i, j].EmptyEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].EmptyEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEmptyObject);
                }
            }
        }

        this.gameObject.GetComponent<BossFSM>().behavior = false;
        StartCoroutine(AllTileOriginColor());

        //연계스킬 처리
        if (m_CurrentBossSkill.SkillAdded != 0)
        {
            BossSkillAction(m_CurrentBossSkill.SkillAdded);
            TargetLockOn = true;
        }
        else
        {
            this.gameObject.GetComponent<BossFSM>().behavior = false;

            if (TargetLockOn)
                TargetLockOn = false;
        }
        yield break;
    }

    //각도얻기 함수
    public float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.forward, to - from).eulerAngles.y;
    }

    IEnumerator SkillLineAction()
    {
        LineSkillTarget = GameObject.Find("Player").transform.GetChild(0).gameObject;

        //DebuffTile = true;

        int Row = 0;
        int Column = 0;

        if (TargetLockOn)
        {
            Row = m_TargetRow;
            Column = m_TargetColumn;
        }
        else
        {
            Row = m_BossRow;
            Column = m_BossColumn;
        }

        float range = m_CurrentBossSkill.SkillRange - 1.0f;

        //플레이어와의 각도를 구함. 
        float tempAngle = CalculateAngle(this.gameObject.transform.position, LineSkillTarget.transform.position);
        int checkIndex = 0;

        //각도의 범위를 고정으로
        if (tempAngle > 0 && tempAngle <= 45)
        {
            tempAngle = 30;
            checkIndex = 0;
        }
        else if (tempAngle > 45 && tempAngle <= 90)
        {
            tempAngle = 90;
            checkIndex = 1;
        }
        else if (tempAngle > 90 && tempAngle <= 180)
        {
            tempAngle = 150;
            checkIndex = 2;
        }
        else if (tempAngle > 180 && tempAngle <= 225)
        {
            tempAngle = 210;
            checkIndex = 3;
        }
        else if (tempAngle > 225 && tempAngle <= 270)
        {
            tempAngle = 270;
            checkIndex = 4;
        }
        else if (tempAngle > 270 && tempAngle <= 360)
        {
            tempAngle = 330;
            checkIndex = 5;
        }

        // 보스가 스킬을 사용할때 해당 각도로 돌려준다음
        this.gameObject.transform.rotation = Quaternion.Euler(0, tempAngle, 0);

        // 해당 각도를 시작으로 시계방향으로 타일 방향체크후 증감값 셋팅. 
        for (int i = 0; i < OriginArr.Length; i++)
        {
            //Row
            if (i == 0 || i == 5)
            {
                OriginArr[i].row = Row - 1;
            }
            else if (i == 2 || i == 3)
            {
                OriginArr[i].row = Row + 1;
            }

            if (i == 1 || i == 4)
                OriginArr[i].row = Row;

            //Column
            if (Row % 2 == 0)
            {
                if (i == 0 || i == 2)
                {
                    OriginArr[i].column = Column +1;
                }
                else if (i == 3 || i == 5)
                {
                    OriginArr[i].column = Column;
                }

                if (i == 1)
                {
                    OriginArr[i].column = Column + 1;
                }
                else if (i == 4)
                {
                    OriginArr[i].column = Column - 1;
                }
            }
            else
            {
                if (i == 0 || i == 2)
                {
                    OriginArr[i].column = Column;
                }
                else if (i == 3 || i == 5)
                {
                    OriginArr[i].column = Column -1;
                }

                if (i == 1)
                {
                    OriginArr[i].column = Column + 1;
                }
                else if (i == 4)
                {
                    OriginArr[i].column = Column - 1;
                }
            }
        }

        List<ClockDirectionList> tempList = new List<ClockDirectionList>();
        tempList = OriginArr.ToList<ClockDirectionList>();

        //리스트에서 정렬을 끝내준뒤.
        ClockDirectionList tempCDL = new ClockDirectionList();
        for (int i = 0; i < tempList.Count; i++)
        {
            if (checkIndex == tempList[i].check)
                break;
            else   //보고있는방향부터 시계방향으로 돌아가도록 정렬.
            {
                tempCDL = tempList[i];
                tempList.RemoveAt(i);
                tempList.Add(tempCDL);
                i--;
            }
        }

        ClockDirectionList[] tempCDLARR = tempList.ToArray();

        //이펙트를 내보낼 타일 설정.
        if (m_CurrentBossSkill.Direction1)
        {
            tempCDLARR[0].effect = true;
        }
        if (m_CurrentBossSkill.Direction2)
        {
            tempCDLARR[1].effect = true;
        }
        if (m_CurrentBossSkill.Direction3)
        {
            tempCDLARR[2].effect = true;
        }
        if (m_CurrentBossSkill.Direction4)
        {
            tempCDLARR[3].effect = true;
        }
        if (m_CurrentBossSkill.Direction5)
        {
            tempCDLARR[4].effect = true;
        }
        if (m_CurrentBossSkill.Direction6)
        {
            tempCDLARR[5].effect = true;
        }

        //확산스킬은 무조건 거리가 2부터 시작.
        int checkRow = 0;
        int checkColumn = 0;

        for (float i = 0; i < range; i += 1.0f) 
        {
            if (i == 0)
            {
                m_StageMgr.m_MapInfo[Row, Column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                m_StageMgr.m_MapInfo[Row, Column].BossEffect = true;
                m_StageMgr.m_MapInfo[Row, Column].EmptyEffect = false;

                for (int j = 0; j < tempCDLARR.Length; j++)
                {
                    if (tempCDLARR[j].effect)
                    {
                        if ((tempCDLARR[j].column >= 0 && tempCDLARR[j].column < 6) && (tempCDLARR[j].row >= 0 && tempCDLARR[j].row < 5))
                        {
                            m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                            if (!m_CurrentBossSkill.SingleTile)
                            {
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].BossEffect = true;
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].BossEffect = false;
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].EmptyEffect = true;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < tempCDLARR.Length; j++)
                {
                    if (tempCDLARR[j].effect)
                    {
                        //Column
                        if (tempCDLARR[j].row % 2 == 0) //135라인.
                        {
                            if (tempCDLARR[j].check == 0 || tempCDLARR[j].check == 2)
                            {
                                checkColumn = tempCDLARR[j].column + 1;
                                tempCDLARR[j].column = checkColumn;
                            }

                            if (tempCDLARR[j].check == 1)
                            {
                                checkColumn = tempCDLARR[j].column + 1;
                                tempCDLARR[j].column = checkColumn;
                            }

                            if (tempCDLARR[j].check == 4)
                            {
                                checkColumn = tempCDLARR[j].column - 1;
                                tempCDLARR[j].column = checkColumn;
                            }
                        }
                        else
                        {
                            if (tempCDLARR[j].check == 3 || tempCDLARR[j].check == 5)
                            {
                                checkColumn = tempCDLARR[j].column -1;
                                tempCDLARR[j].column = checkColumn;
                            }

                            if (tempCDLARR[j].check == 1)
                            {
                                checkColumn = tempCDLARR[j].column + 1;
                                tempCDLARR[j].column = checkColumn;
                            }

                            if (tempCDLARR[j].check == 4)
                            {
                                checkColumn = tempCDLARR[j].column - 1;
                                tempCDLARR[j].column = checkColumn;
                            }
                        }

                        //Row
                        checkRow = tempCDLARR[j].row - Row;
                        if (checkRow > 0)
                        {
                            tempCDLARR[j].row++;
                        }
                        else if (checkRow < 0)
                        {
                            tempCDLARR[j].row--;
                        }

                        if ((tempCDLARR[j].column >= 0 && tempCDLARR[j].column < 6) && (tempCDLARR[j].row >= 0 && tempCDLARR[j].row < 5))
                        {
                            m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                           
                            if (!m_CurrentBossSkill.SingleTile)
                            {
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].BossEffect = true;
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].EmptyEffect = false;
                            }
                            else
                            {
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].BossEffect = false;
                                m_StageMgr.m_MapInfo[tempCDLARR[j].row, tempCDLARR[j].column].EmptyEffect = true;
                            }
                        }
                    }
                }
            }
        }
       
        DelayAndLunchPrefabSet();
        yield return new WaitForSeconds(m_CurrentBossSkill.SkillDelay);
        Destroy(TempLunch);

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                checkSettingEffect(i, j,tempAngle);
            }
        }
        //이쪽 에서 데미지 처리.
        yield return new WaitForSeconds(m_CurrentBossSkill.LifeTime);  //생존시간이 지나면 이펙트 지우기

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {

                if (m_StageMgr.m_MapInfo[i, j].BossEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].BossEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEffectObject);
                }
                else if (m_StageMgr.m_MapInfo[i, j].EmptyEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].EmptyEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEmptyObject);
                }
            }
        }

        StartCoroutine(AllTileOriginColor());
        //AnimationManager.GetInstance().PlayAnimation(anim, "Idle01");

        //연계스킬있는지 확인후 다시 스킬실행.
        if (m_CurrentBossSkill.SkillAdded != 0)
            BossSkillAction(m_CurrentBossSkill.SkillAdded);
        else
        {
            if (TargetLockOn)
            {
                TargetLockOn = false;
                m_TargetRow = 0;
                m_TargetColumn = 0;
            }
            this.gameObject.GetComponent<BossFSM>().behavior = false;
        }

        yield break;
    }

    private void checkSettingEffect(int i,int j, float angle =0)
    {
        

        if (m_StageMgr.m_MapInfo[i, j].BossEffect)
        {
            Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossDelayObject);
            //m_StageMgr.m_MapInfo[i, j].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
            GameObject effect = Instantiate(firePrefab);

            effect.transform.rotation = Quaternion.Euler(effect.transform.rotation.x, angle, effect.transform.rotation.z);
            effect.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
            effect.GetComponent<DamageCheck>().who = 2;
            effect.GetComponent<DamageCheck>().Dot = m_CurrentBossSkill.DoT;

            //버프가 있다면 버프를 발동.
            if (m_CurrentBossSkill.BuffAdded != 0)
            {
                effect.GetComponent<DamageCheck>().buffIndex = m_CurrentBossSkill.BuffAdded;
            }
            m_StageMgr.m_MapInfo[i, j].BossEffectObject = effect;

            //if (DebuffTile)
            //{
            //    GameObject DebuffTile = Instantiate(DebuffTilePrefab);
            //    DebuffTile.GetComponent<DebuffTileChk>().debuffTile_chk = true;
            //    DebuffTile.GetComponent<DebuffTileChk>().who = 1;  // 플레이어(1) -> 디버프 / 에너미(2) -> 버프
            //    DebuffTile.GetComponent<DebuffTileChk>().Dot = 1f;  // 우선 1초로 고정 -> 가변형이면 데이터에 따라 변경.
            //    DebuffTile.GetComponent<DebuffTileChk>().lifeTime = 7f;  // 우선 7초로 고정 -> 가변형이면 데이터에 따라 변경.
            //    DebuffTile.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
            //    m_StageMgr.m_MapInfo[i, j].BossDebuffTileObjcet = DebuffTile;
            //}

        }
        else if (m_StageMgr.m_MapInfo[i, j].EmptyEffect)
        {
            Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossDelayObject);
            GameObject effect = Instantiate(EmptyPrefab);
            effect.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
            effect.GetComponent<DamageCheck>().who = 2;
            effect.GetComponent<DamageCheck>().Dot = m_CurrentBossSkill.DoT;

            //버프가 있다면 버프를 발동.
            if (m_CurrentBossSkill.BuffAdded != 0)
            {
                effect.GetComponent<DamageCheck>().buffIndex = m_CurrentBossSkill.BuffAdded;
            }
            m_StageMgr.m_MapInfo[i, j].BossEmptyObject = effect;

            //if (DebuffTile)
            //{
            //    GameObject DebuffTile = Instantiate(DebuffTilePrefab);
            //    DebuffTile.GetComponent<DebuffTileChk>().debuffTile_chk = true;
            //    DebuffTile.GetComponent<DebuffTileChk>().who = 1;  // 플레이어(1) -> 디버프 / 에너미(2) -> 버프
            //    DebuffTile.GetComponent<DebuffTileChk>().Dot = 1f;  // 우선 1초로 고정 -> 가변형이면 데이터에 따라 변경.
            //    DebuffTile.GetComponent<DebuffTileChk>().lifeTime = 7f;  // 우선 7초로 고정 -> 가변형이면 데이터에 따라 변경.
            //    DebuffTile.transform.position = m_StageMgr.m_MapInfo[i, j].MapPos + new Vector3(0, 5f, 0);
            //    m_StageMgr.m_MapInfo[i, j].BossDebuffTileObjcet = DebuffTile;
            //}


        }

       
    }


    IEnumerator SkillDiffusionAction()
    {
        int Row = 0;
        int Column = 0;

        //DebuffTile = true;

        if (TargetLockOn)
        {
            Row = m_TargetRow;
            Column = m_TargetColumn;
        }
        else
        {
            Row = m_BossRow;
            Column = m_BossColumn;
        }
        float range = m_CurrentBossSkill.SkillRange - 1.0f;
        float xRange = m_CurrentBossSkill.SkillRange + range;

        //보스 스킬범위를 표시해 주는 부분.
        if (range > 0)  //range는 -1을한값 범위가 2부터 여기 들어온다. 범위가 1일경우는 해당 타일에 계산하면 된다.
        {
            //방출(도넛)의 경우 거리가 1일때는 자기자신은 동일하지만 거리가 2부터 자기자신을  제외한영역 
            //거리가 3일때는 거리가 2인 영역을 제외한 영역만 포함해야한다.

            //스킬범위의 시작 기준점을 중심으로 제일처음과 끝에만 색칠해주기 그리고 스킬범위의 마지막 부분은 전부 다 칠해주기.

            int saveRow = Row;
            int saveColumn_P = 0;
            int saveColumn_M = 0;

            int checkRow_P;   //보스 기준 아래쪽에 있는 Column값
            int checkRow_M;   //보스 기준 위쪽에 있는 Column값
            int checkColumn;  //현재 색을 바꿀 타일의 Column값

            if (m_CurrentBossSkill.SingleTile)
            {
                m_StageMgr.m_MapInfo[Row, Column].BossEffect = true;
                m_StageMgr.m_MapInfo[Row, Column].EmptyEffect = false;
            }

            for (float i = 0; i < m_CurrentBossSkill.SkillRange; i += 1.0f)
            {
                checkRow_P = Row + (int)i;
                checkRow_M = Row - (int)i;

                if (checkRow_P % 2 == 1) //024(135라인)
                    saveColumn_M++;
                else
                    saveColumn_P--;

                if (i != 0)
                {

                    if (i != range)
                    {
                        if (saveColumn_M >= 0)
                        {
                            if (checkRow_M >= 0)
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, saveColumn_M].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                                
                                if(!m_CurrentBossSkill.SingleTile)
                                {
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_M].BossEffect = true;
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_M].EmptyEffect = false;
                                }
                                else
                                {
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_M].BossEffect = false;
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_M].EmptyEffect = true;
                                }
                            }

                            if(checkRow_P < 5)
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, saveColumn_M].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                                if (!m_CurrentBossSkill.SingleTile)
                                {
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_M].BossEffect = true;
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_M].EmptyEffect = false;
                                }
                                else
                                {
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_M].BossEffect = false;
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_M].EmptyEffect = true;
                                }
                            }
                        }

                        if (saveColumn_P < 6)
                        {
                            if(checkRow_M >= 0)
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, saveColumn_P].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                                if (!m_CurrentBossSkill.SingleTile)
                                {
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_P].BossEffect = true;
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_P].EmptyEffect = false;
                                }
                                else
                                {
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_P].BossEffect = false;
                                    m_StageMgr.m_MapInfo[checkRow_M, saveColumn_P].EmptyEffect = true;
                                }
                                
                            }

                            if(checkRow_P < 5)
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, saveColumn_P].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                                if (!m_CurrentBossSkill.SingleTile)
                                {
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_P].BossEffect = true;
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_P].EmptyEffect = false;
                                }
                                else
                                {
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_P].BossEffect = false;
                                    m_StageMgr.m_MapInfo[checkRow_P, saveColumn_P].EmptyEffect = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (float j = 0; j < xRange; j += 1.0f)
                        {
                            checkColumn = saveColumn_M + (int)j;

                            if (checkColumn < 0 || checkColumn > 5)
                                continue;

                            if (checkRow_P < 5)
                            {
                                m_StageMgr.m_MapInfo[checkRow_P, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                                
                                if(!m_CurrentBossSkill.SingleTile)
                                {
                                    m_StageMgr.m_MapInfo[checkRow_P, checkColumn].BossEffect = true;
                                    m_StageMgr.m_MapInfo[checkRow_P, checkColumn].EmptyEffect = false;
                                }
                                else
                                {
                                    m_StageMgr.m_MapInfo[checkRow_P, checkColumn].BossEffect = false;
                                    m_StageMgr.m_MapInfo[checkRow_P, checkColumn].EmptyEffect = true;
                                }
                            }

                            if (checkRow_M >= 0)
                            {
                                m_StageMgr.m_MapInfo[checkRow_M, checkColumn].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;

                                if (!m_CurrentBossSkill.SingleTile)
                                {
                                    m_StageMgr.m_MapInfo[checkRow_M, checkColumn].BossEffect = true;
                                    m_StageMgr.m_MapInfo[checkRow_M, checkColumn].EmptyEffect = false;
                                }
                                else
                                {
                                    m_StageMgr.m_MapInfo[checkRow_M, checkColumn].BossEffect = false;
                                    m_StageMgr.m_MapInfo[checkRow_M, checkColumn].EmptyEffect = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    int Column_M = Column - (int)range;
                    int Column_P = Column + (int)range;
                    saveColumn_M = Column_M;
                    saveColumn_P = Column_P;

                    if (Column_M >= 0)
                    {
                        m_StageMgr.m_MapInfo[Row, Column_M].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                        
                        if(!m_CurrentBossSkill.SingleTile)
                        {
                            m_StageMgr.m_MapInfo[Row, Column_M].BossEffect = true;
                            m_StageMgr.m_MapInfo[Row, Column_M].EmptyEffect = false;
                        }
                        else
                        {
                            m_StageMgr.m_MapInfo[Row, Column_M].BossEffect = false;
                            m_StageMgr.m_MapInfo[Row, Column_M].EmptyEffect = true;
                        }
                    }

                    if (Column_P < 6)
                    {
                        m_StageMgr.m_MapInfo[Row, Column_P].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
                        
                        if(!m_CurrentBossSkill.SingleTile)
                        {
                            m_StageMgr.m_MapInfo[Row, Column_P].BossEffect = true;
                            m_StageMgr.m_MapInfo[Row, Column_P].EmptyEffect = false;
                        }
                        else
                        {
                            m_StageMgr.m_MapInfo[Row, Column_P].BossEffect = false;
                            m_StageMgr.m_MapInfo[Row, Column_P].EmptyEffect = true;
                        }
                    }
                }
                xRange -= 1.0f;
            }
        }
        else  //range가 0이하면 사거리가 1 자기자신의 타일만 해당
        {
            m_StageMgr.m_MapInfo[Row, Column].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.red;
            m_StageMgr.m_MapInfo[Row, Column].BossEffect = true;
            m_StageMgr.m_MapInfo[Row, Column].EmptyEffect = false;
        }

        DelayAndLunchPrefabSet();
        yield return new WaitForSeconds(m_CurrentBossSkill.SkillDelay);
        Destroy(TempLunch);

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {
                checkSettingEffect(i, j);
            }
        }
        //이쪽 에서 데미지 처리.
        yield return new WaitForSeconds(m_CurrentBossSkill.LifeTime);  //생존시간이 지나면 이펙트 지우기

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for (int j = 0; j < m_StageMgr.mapX; j++)
            {

                if (m_StageMgr.m_MapInfo[i, j].BossEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].BossEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEffectObject);
                }
                else if (m_StageMgr.m_MapInfo[i, j].EmptyEffect)
                {
                    m_StageMgr.m_MapInfo[i, j].EmptyEffect = false;
                    Object.Destroy(m_StageMgr.m_MapInfo[i, j].BossEmptyObject);
                }
            }
        }
        StartCoroutine(AllTileOriginColor());

        //연계스킬있는지 확인후 다시 스킬실행.
        if (m_CurrentBossSkill.SkillAdded != 0)
            BossSkillAction(m_CurrentBossSkill.SkillAdded);
        else
        {
            this.gameObject.GetComponent<BossFSM>().behavior = false;

            if (TargetLockOn)
            {
                TargetLockOn = false;
                m_TargetRow = 0;
                m_TargetColumn = 0;
            }
        }
        yield break;
    }


    IEnumerator AllTileOriginColor()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < m_StageMgr.mapZ; i++)
        {
            for(int j = 0; j < m_StageMgr.mapX; j++)
            {
                //rgba값이 이상하게 들어감
                m_StageMgr.m_MapInfo[i, j].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = _origin;
            }
        }
    }

    void Start()
    {
        m_StageMgr = MainManager.Instance.GetStageManager();

        OriginArr = new ClockDirectionList[6];
        //DebuffTile = false;

        for (int i = 0; i < 6; i++)
        {
            OriginArr[i].check = i;
            OriginArr[i].effect = false;
        }

        anim = this.transform.GetChild(0).GetComponent<Animator>();

        if (anim == null)
        {
            anim = this.transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        }

        PlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach(Transform child in allChildren)
        {
            if(child.name =="PosAtk")
            {
                PosAtk = child;
            }
        }
        m_BossFSM = GetComponent<BossFSM>();
    }
}
