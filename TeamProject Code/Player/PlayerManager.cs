using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yggdrasil.PlayerSkillSet;
using Model;

public class PlayerManager : MonoBehaviour
{
    //플레이어 오브젝트
    public static GameObject p_Object;

    public GameObject m_MenuManager;

    FollowCam cam;

    //스킬 
    public Spirit m_Spirit;
    //public SpiritSkill m_SpiritSkill;

    private CharStat_TableExcel m_CurrentCharStat;
    private const int m_CurrentIndex = 10002;  //임시로 사용하는 값(플레이어가 메뉴 모드에서 선택한 index값을 참고해서 솔로모드인지 멀티모드인지 판별)

    public CharacterClass PlayerClass;
    private float m_MaxHp;
    private float m_PerHp;

    public KeyCode[] m_SpiritSkillKey;


    public GameObject hudDamageText;
    public Transform hudPos;

    [SerializeField]
    private Animator anim;
    private PlayerDirection dir;

    float h, v;
    public float rotateSpeed = 5f;

    private CharacterController characterController;

    int x;
    int y;

    // 0 -> q / 1 -> w / 2 -> e / 3 -> a / 4 -> s / 5 -> d
    public bool[] CanSkill; // 스킬이 사용이 가능한지 판단하기 위한 bool값

    //각각 스킬의 쿨타임 넣어놓는 배열
    public float[] SkillCollTime;
    public float[] currenCollTime;

    public float m_BuffCoolDown = 0.0f; //아이템 혹은 버프타일에 의하여 쿨타임이 줄어드는 버프에 사용을 하기 위해 넣어놓은 변수 

    //벽인지 체크 하기 위한 불값
    public bool isWall;

    float addSpeed = 0f;

    bool boss_Invin = false;

    [SerializeField]
    private Image imageBloodScreen; //플레이어가 공격받았을 때 화면에 표시되는 image
    [SerializeField]
    private AnimationCurve cureveBloodScreen;

    private float deathTime =0f;

    private void InputCheck()
    {
        // 0 -> q / 1 -> w / 2 -> e / 3 -> a / 4 -> s / 5 -> d
        if (Input.GetKeyDown(m_SpiritSkillKey[3]) && CanSkill[3]) //플레이어 A스킬
        {
            CanSkill[3] = false;
            m_Spirit.SpiritSummon(PlayerClass.m_CharacterStat.Skill1);
            // 쿨타임 코루틴 작동
            StartCoroutine(CoolTime(SkillCollTime[3], 3, m_BuffCoolDown));
        }

        if (Input.GetKeyDown(m_SpiritSkillKey[4]) && CanSkill[4]) //플레이어 S스킬
        {
            CanSkill[4] = false;
            m_Spirit.SpiritSummon(PlayerClass.m_CharacterStat.Skill2);
            // 쿨타임 코루틴 작동
            StartCoroutine(CoolTime(SkillCollTime[4], 4, m_BuffCoolDown));
        }

        if (Input.GetKeyDown(m_SpiritSkillKey[5]) && CanSkill[5]) //플레이어 D스킬
        {
            CanSkill[5] = false;
            m_Spirit.SpiritSummon(PlayerClass.m_CharacterStat.Skill3);
            // 쿨타임 코루틴 작동
            StartCoroutine(CoolTime(SkillCollTime[5], 5, m_BuffCoolDown));
        }

        if (Input.GetKeyDown(m_SpiritSkillKey[0]) && CanSkill[0]) //플레이어 Q스킬
        {
            CanSkill[0] = false;
            m_Spirit.SpiritSummon(PlayerClass.m_CharacterStat.Skill4);
            // 쿨타임 코루틴 작동
            StartCoroutine(CoolTime(SkillCollTime[0], 0, m_BuffCoolDown));
        }

        if (Input.GetKeyDown(m_SpiritSkillKey[1]) && CanSkill[1]) //플레이어 W스킬
        {
            CanSkill[1] = false;
            m_Spirit.SpiritSummon(PlayerClass.m_CharacterStat.Skill5);
            // 쿨타임 코루틴 작동
            StartCoroutine(CoolTime(SkillCollTime[1], 1, m_BuffCoolDown));
        }

        if (Input.GetKeyDown(m_SpiritSkillKey[2]) && CanSkill[2]) //플레이어 E스킬
        {
            CanSkill[2] = false;
            m_Spirit.SpiritSummon(PlayerClass.m_CharacterStat.Skill6);
            // 쿨타임 코루틴 작동
            StartCoroutine(CoolTime(SkillCollTime[2], 2, m_BuffCoolDown));
        }

    }

    public void Move()
    {
        h = Input.GetAxis("Horizontal"); //프로젝트 셋팅으로 wasd 값 날려놓음 방향키만 적용이 될겁니다.
        v = Input.GetAxis("Vertical");

        Vector3 MoveForce = new Vector3(h * (PlayerClass.m_CharacterStat.MoveSpeed + addSpeed), 0, v * PlayerClass.m_CharacterStat.MoveSpeed);

        if(h != 0 || v != 0) //이동중일때 런 애니메이션 실행
        {
            AnimationManager.GetInstance().PlayAnimation(anim, "Run");

            anim.SetBool("isRunning", true);
        }
        else if(h == 0 && v == 0) //이동이 아닐때 아이들 애니메이션으로 넘겨준다.
        {
            anim.SetBool("isRunning", false);
        }

        characterController.Move(MoveForce * Time.deltaTime);
    }

    // Manager -> 관리자 Player 전부 3인용
    // Movement -> Rotation, Move - PlayerController
    public void ChangePlayerDirection()
    {
        float h, v;
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        if (!(h == 0 && v == 0))
        {
           transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * rotateSpeed);
        }
    }

    private void Awake()
    {
        isWall = false;

        m_SpiritSkillKey = new KeyCode[6];
        #region 정령스킬 키코드 값 설정
        m_SpiritSkillKey[0] = KeyCode.Q;
        m_SpiritSkillKey[1] = KeyCode.W;
        m_SpiritSkillKey[2] = KeyCode.E;
        m_SpiritSkillKey[3] = KeyCode.A;
        m_SpiritSkillKey[4] = KeyCode.S;
        m_SpiritSkillKey[5] = KeyCode.D;
        #endregion
        hudDamageText = Resources.Load<GameObject>("DamageText");
    }

    void Start()
    {
        imageBloodScreen = GameObject.Find("PlayerDamageImage").GetComponent<Image>();
        SkillCollTime = new float[6];
        //스피릿에서 넣은 쿨타임 배열 받아오기
        SkillCollTime = (float[])GetComponent<Spirit>().CoolTimeArr.Clone();
        characterController = GetComponent<CharacterController>();
        cam = GetComponent<FollowCam>();
        p_Object = this.gameObject;
        m_Spirit = this.GetComponent<Spirit>();

        PlayerClass = this.gameObject.GetComponent<CharacterClass>();

        CanSkill = new bool[6]; // 0 -> q / 1 -> w / 2 -> e / 3 -> a / 4 -> s / 5 -> d 
        currenCollTime = new float[6];

        //Start 되는 부분에서 스킬사용 가능 불값과 스킬 쿨타임값을 설정
        for (int i = 0; i < CanSkill.Length; i++)
        {
            CanSkill[i] = true;
            currenCollTime[i] = SkillCollTime[i];
        }

        anim = this.transform.GetChild(0).GetComponent<Animator>();

        if (anim == null)
        {
            anim = this.transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        }
        //AnimationManager.GetInstance().PlayAnimation(anim, "Idle01");
        anim.SetBool("isRunning", false);


        foreach (var item in DataTableManager.Instance.GetDataTable<CharStat_TableExcelLoader>().DataList)
        {
            if (item.CharStatIndex == m_CurrentIndex)
            {
                PlayerClass.m_CharacterStat = item; //현재 캐릭터 정보를 찾아낸다(나중가서는 로비창에서 선택한 데이터를 비교해서 캐릭터 선택해주기)
                break;
            }
        }


        //Damage();
        m_MaxHp = PlayerClass.m_CharacterStat.HP;

        m_MenuManager = GameObject.Find("MenuManager");

    }

    void Update()
    {
        x = MainManager.Instance.GetStageManager().m_PlayerRow;
        y = MainManager.Instance.GetStageManager().m_PlayerCoulmn;

        //이동
        Move();

        //키보드 입력 체크 함수.
        InputCheck();

        //플레이어 체력 백분률
        RearTimePerHP();

        //hudPos = GameObject.Find("Player(Clone)").transform;
        hudPos = GameObject.Find("Player").transform.GetChild(0).gameObject.transform;

        if(PlayerClass.m_CharacterStat.HP <=0)
        {
            if(!boss_Invin)
            {
                //GameObject.Find("931001(Clone)").transform.GetChild(0).gameObject.GetComponent<CharacterClass>().Invincibility = 0.0f;  //보스를 무적으로 만들고.
                AnimationManager.GetInstance().PlayAnimation(anim, "Die");
                boss_Invin = true;
            }

            deathTime += Time.deltaTime;
            // 플레이어 죽는 모션을 취한후 게임 메뉴 보여주기.

            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
            if(deathTime >= info.length)
            {
                //deathTime = 0f;
                m_MenuManager.GetComponent<MenuManager>().ShowLoseMenu();
            }
        }

    }

    private void FixedUpdate()
    {
        ChangePlayerDirection();

        //PlayerDirection.GetInstance().ChangePlayerDirection(gameObject);
    }


    public void Damage(int _damage)
    {
        TakeDamagePrint(_damage);
    }

    public void BloodImage()
    {
        StartCoroutine(OnBloodScreen());
    }

    public void TakeDamage(float _damage)
    {
        PlayerClass.m_CharacterStat.HP -= _damage;
        if(PlayerClass.m_CharacterStat.HP <= 0)
        {
            PlayerClass.m_CharacterStat.HP = 0;
        }
        StartCoroutine(OnBloodScreen());
        TakeDamagePrint((int)_damage);
    }

    public void TakeDamagePrint(int damage)
    {
        if (damage >= 1)
        {
            GameObject hudText = Instantiate(hudDamageText);

            hudText.transform.position = hudPos.position + (Vector3.up * 15);
            hudText.GetComponent<DamageTxt>().damage = damage;
        }
        
    }

    private void RearTimePerHP()
    {
        m_PerHp = PlayerClass.m_CharacterStat.HP / m_MaxHp;
    }
    public float GetPlayerPerHp()
    {
        return m_PerHp;
    }
    public float GetMaxHp()
    {
        return m_MaxHp;
    }
    public float GetRealHp()
    {
        return PlayerClass.m_CharacterStat.HP;
    }

    IEnumerator CoolTime(float cool, int index, float Buff)
    {
        StartCoroutine(Activation(index));
        yield return new WaitForSeconds(cool - Buff);
        CanSkill[index] = true;
    }

    WaitForSeconds seconds = new WaitForSeconds(0.1f);

    IEnumerator Activation(int index)
    {
        while (currenCollTime[index] > 0)
        {
            currenCollTime[index] -= 0.1f;
            yield return seconds;
        }
        currenCollTime[index] = SkillCollTime[index];
        StopCoroutine("Activation");
    }

    public IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, cureveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
