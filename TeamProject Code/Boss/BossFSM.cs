using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFSM : MonoBehaviour
{
    private int currentBossNumber;
    private Boss_TableExcel m_BossStat;
    private float time;
    private int actionPoint;
    private bool spCheck = false;
   
    [SerializeField]
    private float bossStaminaSave;
    private float currentBossStamina;
    [SerializeField]
    private float maxStamina;

    [SerializeField]
    private float maxHp;

    [SerializeField]
    public float bossSpeed;
    private int BossRandomSkill = 0;
    public bool behavior = false;
    public CharacterClass m_BossClass;
    private BossSkill m_CurrentBossSkill;

    [SerializeField]
    private BossSkill_TableExcel m_BossSkill;

    public GameObject hudDamageText;
    public Transform hudPos;
    private Animator anim;
    public Board GameBoard;
    public GameObject player;
    private AnimatorStateInfo info;
    private float animation_length =0f;
    private bool setIdle = false;
    private bool skill = false;
    private float animation_time = 0f;


    public bool[] CanSkill; // 스킬이 사용이 가능한지 판단하기 위한 bool값

    //각각 스킬의 쿨타임 넣어놓는 배열
    public float[] SkillCoolTime;
    public float[] currenCoolTime;
    private int checkDuplicateCoolTime = 0;
    public GameObject m_MenuObj;
    private float deathTime = 0f;
    private bool player_Invin = false;
    private bool stIsFull = false;  //스태미너가 다차면.
    float h, v; //BossDirection
    Color _origin = new Color(0f, 0.541f, 0.603f, 0.784f); //원래대로 돌릴 색깔

    private void Awake()
    {
        player = GameObject.Find("obj_10001");
        hudDamageText = Resources.Load<GameObject>("DamageText");
        h = transform.position.x;
        v = transform.position.z;
        bossSpeed = 1.0f;
    }

    void Start()
    {
        m_BossClass = this.gameObject.GetComponent<CharacterClass>();
        m_CurrentBossSkill = this.gameObject.GetComponent<BossSkill>();
        currentBossNumber = DataManager.Instance.m_userSelectStage;   //스테이지에서 해당 스테이지에 맞는 보스 정보를 가져와야됨 지금은 임시로 1로 설정

        maxStamina = DataTableManager.Instance.GetDataTable<Boss_TableExcelLoader>().DataList[currentBossNumber].MaxStamina;
        maxHp = DataTableManager.Instance.GetDataTable<Boss_TableExcelLoader>().DataList[currentBossNumber].HP;
        m_BossClass.m_BossStatData = DataTableManager.Instance.GetDataTable<Boss_TableExcelLoader>().DataList[currentBossNumber];
        m_BossClass.m_SkillMgr.m_BossSkill = m_CurrentBossSkill;

        anim = this.transform.GetChild(0).GetComponent<Animator>();

        if (anim == null)
        {
            anim = this.transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        }

        CanSkill = new bool[4];
        SkillCoolTime = new float[4];
        currenCoolTime = new float[4];

        for (int i = 0; i < CanSkill.Length; i++)
        {
            CanSkill[i] = true;
            SkillCoolTime[i] = 0.0f;
            currenCoolTime[i] = SkillCoolTime[i];
        }

        GameBoard = new Board(6, 5);
        m_MenuObj = GameObject.Find("MenuManager");

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (!MainManager.Instance.GetStageManager().m_MapInfo[i, j].IsUnWalkable)
                {
                    GameBoard.SetBlock(j, i, true);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        MonsterDirection();
    }

    //보스가 이동할때.
    public void Move()
    {
        //보스 위치 업데이트 해줘야함(Row,Column)
        if(!bossMove) //보스가 움직이지 않고 있다면. 이동을 시작하는 단계니깐
        {

            // 보스이동 코드.
            if (originBlock.next == null || currentBossStamina < m_BossClass.m_BossStatData.MoveStUsed)   //  
            {
                MainManager.Instance.GetStageManager().m_BossRow = originBlock.x;
                MainManager.Instance.GetStageManager().m_BossColumn = originBlock.y;
                behavior = false;
                moving = false;

                while (true)
                {
                    if (startBlock != null)
                    {
                        MainManager.Instance.GetStageManager().m_MapInfo[startBlock.x, startBlock.y].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
                        tempBlock = startBlock.prev;
                    }
                    else
                    {
                        AnimationManager.GetInstance().PlayAnimation(anim, "Idle01");
                     
                        break;
                    }
                    startBlock = tempBlock;
                }
                return;
            }

            tempBlock = originBlock.next;
            currentBossStamina -= m_BossClass.m_BossStatData.MoveStUsed; //스태미너 감소
            stIsFull = false;
            targetPos = MainManager.Instance.GetStageManager().m_MapInfo[tempBlock.x, tempBlock.y].MapPos;    //x=row y=column
            bossMove = true;  
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,targetPos, bossSpeed);
        }



        if(transform.position == targetPos)
        {
            int bossRow = MainManager.Instance.GetStageManager().m_BossRow;
            int bossColumn = MainManager.Instance.GetStageManager().m_BossColumn;
            int playerRow = MainManager.Instance.GetStageManager().m_PlayerRow;
            int playerColumn = MainManager.Instance.GetStageManager().m_PlayerCoulmn;

            if (bossRow == playerRow && bossColumn == playerColumn)
            {
                while (true)
                {
                    if (startBlock != null)
                    { 
                        MainManager.Instance.GetStageManager().m_MapInfo[startBlock.x, startBlock.y].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.white;
                        tempBlock = startBlock.prev;
                    }
                    else
                    {
                        AnimationManager.GetInstance().PlayAnimation(anim, "Idle01");
                        break;
                    }
                    startBlock = tempBlock;
                }

                behavior = false;
                moving = false;
                bossMove = false;
                stIsFull = false;
            }
            else
            {
                originBlock = tempBlock;
                bossMove = false;
            }
        }
        StartCoroutine(AllTileOriginColor());
    }
    public float rotateSpeed = 5.0f;

    void MonsterDirection()
    {
        float _h, _v;
        _h = 0;
        _v = 0;

        if (h!=transform.position.x || v!=transform.position.z)
        {
            if (h > transform.position.x)
                _h = -1;
            else if (h < transform.position.x)
                _h = 1;

            if (v > transform.position.z)
                _v = -1;
            else if (v < transform.position.z)
                _v = 1;

            Vector3 dir = new Vector3(_h, 0, _v);
            transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * rotateSpeed);

            h = transform.position.x;
            v = transform.position.z;
        }
    }

    public void Damage(int _damage)
    {
        TakeDamagePrint(_damage);
    }

    public void TakeDamagePrint(int damage)
    {
        if (damage >= 1)
        {
            GameObject hudText = Instantiate(hudDamageText);

            hudText.transform.position = hudPos.position + (Vector3.up * 20);
            hudText.GetComponent<DamageTxt>().damage = damage;
        }
        
    }
    public float GetPerStamina()
    {
        return currentBossStamina / maxStamina;
    }

    public float GetPerHp()
    {
        return m_BossClass.m_BossStatData.HP / maxHp;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }
    public float GetCurrentHp()
    {
        if (m_BossClass.m_BossStatData.HP < 0)
            m_BossClass.m_BossStatData.HP = 0;

        return m_BossClass.m_BossStatData.HP;
    }
    public string GetBossName()
    {
        return m_BossClass.m_BossStatData.Name_EN;
    }

    bool moving = false;
    bool bossMove = false;
    float moveTime;

    Vector3 targetPos;
    Block bossBlock;
    Block PlayerBlock;
    Block startBlock;
    Block tempBlock;
    Block originBlock;

    bool BossUseSkill()
    {

        BossRandomSkill = Random.Range(1, 5); // 1~4까지

        BossRandomSkill = 3; // 몹소환 고정

        int CoolTimeindex = BossRandomSkill - 1;

        int skillIndex = 0;
        // 사용가능한지 체크.(쿨타임)

        if(!CanSkill[CoolTimeindex])  //스킬이 쿨타임 중이라면.
        {
            int count = 0;

            // 남아있는 스킬중에 쿨타임이 있는지 확인.
            for(int i=0;i<4;i++)
            {
                if(CanSkill[i])
                {
                    BossRandomSkill = i + 1;
                    CoolTimeindex = i;
                }
                else
                {
                    count++;
                }
            }

            if(count == 4)  //모든 스킬이 쿨타임 중이기 때문에...
            {
                bossMove = false;
                behavior = false;
                moving = true;
                stIsFull = true;

                return false;
            }

        }

        switch (BossRandomSkill)
        {
            case 1:
                skillIndex = m_BossClass.m_BossStatData.Skill1;
                break;
            case 2:
                skillIndex = m_BossClass.m_BossStatData.Skill2;
                break;
            case 3:
                skillIndex = m_BossClass.m_BossStatData.Skill3;
                break;
            case 4:
                skillIndex = m_BossClass.m_BossStatData.Skill4;
                break;
        }


       

        foreach (var item in DataTableManager.Instance.GetDataTable<BossSkill_TableExcelLoader>().DataList)
        {
            if (item.BossSkillIndex == skillIndex)
            {
                m_BossSkill = item; //현재 스킬 정보를 찾아낸다.
                break;
            }
        }

        if(currentBossStamina - m_BossSkill.UseStat < 0)
            return false;
        else
            currentBossStamina -= m_BossSkill.UseStat;


        SkillCoolTime[CoolTimeindex] = m_BossSkill.CoolTime;
        currenCoolTime[CoolTimeindex] = SkillCoolTime[CoolTimeindex];
        StartCoroutine(Activation(CoolTimeindex));
        switch (BossRandomSkill)
        {
            case 1:
                m_BossClass.Skill(CharacterClass.Character.BOSS, m_BossClass.m_BossStatData.Skill1);
                CanSkill[CoolTimeindex] = false;
                AnimationManager.GetInstance().PlayAnimation(anim, "Skill01");
                anim.SetBool("isSkill01 0", true);
                break;
            case 2:
                m_BossClass.Skill(CharacterClass.Character.BOSS, m_BossClass.m_BossStatData.Skill2);
                CanSkill[CoolTimeindex] = false;
                AnimationManager.GetInstance().PlayAnimation(anim, "Skill02");
                anim.SetBool("isSkill02", true);
                break;
            case 3:
                m_BossClass.Skill(CharacterClass.Character.BOSS, m_BossClass.m_BossStatData.Skill3);
                CanSkill[CoolTimeindex] = false;
                AnimationManager.GetInstance().PlayAnimation(anim, "Skill03");
                anim.SetBool("isSkill03", true);
                break;
            case 4:
                m_BossClass.Skill(CharacterClass.Character.BOSS, m_BossClass.m_BossStatData.Skill4);
                CanSkill[CoolTimeindex] = false;
                AnimationManager.GetInstance().PlayAnimation(anim, "Skill04");
                anim.SetBool("isSkill04", true);
                break;
        }

        anim.SetBool("isSkill01 0", false);
        anim.SetBool("isSkill02", false);
        anim.SetBool("isSkill03", false);
        anim.SetBool("isSkill04", false);

        return true;
    }

    bool isAnimationEnd(string _name, float _exitTime)//애니메이션 끝났는지 확인
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(_name) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > _exitTime;
    }
    
    void Update()
    {
        time += Time.deltaTime;
        hudPos = this.gameObject.transform;

        if (m_BossClass.m_BossStatData.HP <= 0)
        {
            //보스 죽음 애니메이션을 출력(1~초 정도 뒤) -> 보스의 행동을 정지 시켜야됨
            if(!player_Invin)
            {
                GameObject.Find("Player").transform.GetChild(0).gameObject.GetComponent<CharacterClass>().Invincibility = 0.0f;  //플레이어 무적으로 만들고.
                anim.SetBool("isDie", true);
                player_Invin = true;
            }

            deathTime += Time.deltaTime;
            // 플레이어 죽는 모션을 취한후 게임 메뉴 보여주기.
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if (deathTime >= info.length)
            {
                GameObject.Destroy(this.gameObject);
                m_MenuObj.GetComponent<MenuManager>().ShowWinMenu();
            }

            if (isAnimationEnd("Die", 0.95f))
            {
                GameObject.Destroy(this.gameObject);
                m_MenuObj.GetComponent<MenuManager>().ShowWinMenu();
            }
        }

        if (skill)
        {
            animation_time += Time.deltaTime;

            if(animation_time >= animation_length)
            {
                skill = false;
                animation_time = 0f;
                AnimationManager.GetInstance().PlayAnimation(anim, "Idle01");
                anim.SetBool("isSkill", false);
            }
        }

        if (behavior && moving)
        {
            Move();
        }

        if (time > 1.0f && !behavior)
        {
            if (currentBossStamina >= maxStamina )  // && !stIsFull
            {
                // 랜덤값 추출후 행동(이동 or 스킬)을 정함.
                int moveAndSkill = Random.Range(1, 11); 
                if (moveAndSkill > 4)
                {
                    moveAndSkill = 2;
                }
                else
                {
                    moveAndSkill = 1;
                }
                // 만약 상태가 
                if(stIsFull)
                {
                    moveAndSkill = 1;
                }

                if (moveAndSkill ==1)
                {
                    int bossRow = MainManager.Instance.GetStageManager().m_BossRow;
                    int bossColumn = MainManager.Instance.GetStageManager().m_BossColumn;
                    int playerRow = MainManager.Instance.GetStageManager().m_PlayerRow;
                    int playerColumn = MainManager.Instance.GetStageManager().m_PlayerCoulmn;

                    if( (bossRow == playerRow) && ( bossColumn == playerColumn ))
                    {

                    }
                    else
                    {
                        //a*알고리즘 발동.
                        bossBlock = new Block(bossRow, bossColumn); //보스 위치좌표 할당
                        PlayerBlock = new Block(playerRow, playerColumn); //플레이어 위치좌표 할당

                        //현재 패스얻어오는곳에서 가중치 값이 똑같은 타일에 있을경우 보스가 안움직이는 버그 발생 + 일정이상의 거리를 벌려야 추격.(한칸 주위에서는 플레이어 자리까지 추격안됨)
                        startBlock = AStarNJ.PathFinding(GameBoard, bossBlock, PlayerBlock);  // 두 좌표로 a* 사용후 패스 얻어오기
                        tempBlock = null;  //임시 블럭 다음블럭을 저장후 바꾸기위한 값. 

                        if (startBlock != null) //원본값 저장.
                        {
                            originBlock = startBlock;
                        }

                        // 플레이어까지의 타일 루트  노란색으로 색칠해주기
                        while (true)
                        {
                            if (startBlock.next != null)
                            {
                                MainManager.Instance.GetStageManager().m_MapInfo[startBlock.next.x, startBlock.next.y].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = Color.yellow;
                                tempBlock = startBlock.next;
                            }
                            else
                            {
                                break;
                            }
                            startBlock = tempBlock;
                        }
                        bossMove = false;
                        behavior = true;
                        moving = true;
                        AnimationManager.GetInstance().PlayAnimation(anim, "Run");
                    }

                }
                else if(moveAndSkill == 2)
                {
                    skill = true;
                    // 스킬 사용
                    if(BossUseSkill())
                    {
                          info = anim.GetCurrentAnimatorStateInfo(0);
                          animation_length = info.length;
                    }
                }
            }
            else
            {
                currentBossStamina += DataTableManager.Instance.GetDataTable<Boss_TableExcelLoader>().DataList[0].Speed;
            }
            time = 0;
        }
    }
    WaitForSeconds seconds = new WaitForSeconds(0.1f);

    IEnumerator Activation(int index)
    {
        
        while (currenCoolTime[index] > 0)
        {
            currenCoolTime[index] -= 0.1f;
            yield return seconds;
        }
        currenCoolTime[index] = SkillCoolTime[index];
        CanSkill[index] = true;
        StopCoroutine("Activation");
    }

    IEnumerator AllTileOriginColor()
    {
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < MainManager.Instance.GetStageManager().mapZ; i++)
        {
            for (int j = 0; j < MainManager.Instance.GetStageManager().mapX; j++)
            {
                MainManager.Instance.GetStageManager().m_MapInfo[i, j].MapObject.transform.Find("indicator hexa").GetComponent<MeshRenderer>().material.color = _origin;
            }
        }
    }
}
