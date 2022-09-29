using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class Attack : MonoBehaviour
{
    // Start is called before the first frame update

    RaycastHit hit;

    public AudioClip character_Attacksound;
    


    public Slider Character_HP;  //캐릭터 위의 보여줄 HP ...슬라이더 형식
    public GameObject explosion;  //공격 이펙트.
  

    public float hp = 100f;      //캐릭터 hp
    public float attack_distance;  //어택 거리.
    public float addDemage;  //추가 데미지(이오리만 사용)  Stage에서 스킬 발동시 -> addDemage의 값을올려줌.
    public int addDemage_count;  //추가 데미지 적용시킬 탄환 갯수(이오리만 사용) 
    public float addAttackSpeed; //추가 공격속도(이오리만 사용) 

    private int attackTime;  //기본공격 속도
    private Stopwatch Attackwatch;  //시간을 재기위한 변수
    private bool attackCheck;  //공격체크
    private float demage;    //캐릭터 데미지.

    private Vector3 pos;    //위치 관련 변수.

   
    

    void Start()
    {

        this.gameObject.AddComponent<AudioSource>().clip = character_Attacksound;  // AudioSource 컴포넌트 추가.


        if (this.gameObject.name == "Shiroko")  //시로코 공격셋팅
        {
            addAttackSpeed = 0f;
            addDemage = 0f;
            addDemage_count = 0;
            attack_distance = 20;
            attackTime = 1500;
            demage = 9f;

        }
            
        else if (this.gameObject.name == "Iori") //이오리 공격셋팅
        {
            addAttackSpeed = 0f;
            addDemage = 0f;
            addDemage_count = 0;
            attack_distance = 20;
            attackTime = 2500;
            demage = 12f;
        }
           
        else if (this.gameObject.name == "Hasumi")  //하스미 공격셋팅
        {
            addAttackSpeed = 0f;
            addDemage = 0f;
            addDemage_count = 0;
            attack_distance = 20f;
            attackTime = 3500;
            demage = 21f;
        }


        Attackwatch = new Stopwatch();  
        attackCheck = false;  //공격중이 아님.
       
        

    }


    private void SetHPbar()
    {
        if (this.gameObject.name == "Shiroko")
        {
            if (Character_HP.value <= 0)
                Character_HP.transform.Find("Fill Area").gameObject.SetActive(false);
            else
                Character_HP.transform.Find("Fill Area").gameObject.SetActive(true);

            pos = this.gameObject.transform.position;
            pos.z += 2f;
            Character_HP.transform.position = Camera.main.WorldToScreenPoint(pos);
            Character_HP.value = hp / 100;

        }
        else if (this.gameObject.name == "Iori")
        {
            if (Character_HP.value <= 0)
                Character_HP.transform.Find("Fill Area").gameObject.SetActive(false);
            else
                Character_HP.transform.Find("Fill Area").gameObject.SetActive(true);

            pos = this.gameObject.transform.position;
            pos.z += 2f;
            Character_HP.transform.position = Camera.main.WorldToScreenPoint(pos);
            Character_HP.value = hp / 100;
        }
        else if (this.gameObject.name == "Hasumi")
        {
            if (Character_HP.value <= 0)
                Character_HP.transform.Find("Fill Area").gameObject.SetActive(false);
            else
                Character_HP.transform.Find("Fill Area").gameObject.SetActive(true);

            pos = this.gameObject.transform.position;
            pos.z += 2f;
            Character_HP.transform.position = Camera.main.WorldToScreenPoint(pos);
            Character_HP.value = hp / 100;
        }
    }

    // Update is called once per frame
    void Update()
    {

        

        SetHPbar();  //캐릭터HP UI(Slider)설정

        if (CharacterMove.fight)   //전투 중이라면
        {
            aiming();  //레이캐스트 확인용.

            if(!attackCheck)  //공격을 안하고 있다면.
            {
                Attackwatch.Start(); //공격시간 체크
                attackCheck = true;  //공격중
            }
            
            if( attackCheck &&Attackwatch.ElapsedMilliseconds >= attackTime - addAttackSpeed)  //공격시간만큼 시간이 경과했다면.
            {
                fire();  //공격
                Attackwatch.Restart();  //재공격을 위해 시간을 다시 측정.
            }
        }
        else  //전투중이 아니라면
        {
            attackCheck = false;  //공격을 안하도록.
        }
            



    }


    private void aiming()
    {
        var heading = CharacterMove.TargetEnemy.transform.position - transform.position;

        var distance = heading.magnitude;

        var direction = heading / distance;

        //캐릭터와 상대의 거리와 방향을 구해 레이캐스트로 씬뷰에서 확인.

        if (Physics.Raycast(transform.position, direction, out hit, attack_distance))
        {
            UnityEngine.Debug.DrawRay(transform.position, direction * hit.distance, Color.green);

        }
      
    }

    private void fire()
    {

        this.gameObject.GetComponent<AudioSource>().Play();  //공격음 재생.

        if (addDemage_count > 0)  //강화된 총알이 있다면
        {
            addDemage_count--;  //갯수감소
            UnityEngine.Debug.Log("강화된총알을쏩니다...남은갯수" + addDemage_count); //확인용.
        }
        else  //강화된 총알이 없다면
        {
            addDemage = 0f;  // 추가데미지를 0으로.
            addAttackSpeed = 0f; //추가 공격속도를 0으로.
        }
            


        if (CharacterMove.TargetEnemy.name =="Boss")  //보스일 경우.
        {

            CharacterMove.TargetEnemy.GetComponent<Enemy>().hp -= (demage+ addDemage) /3;  //데미지 1/3로 반감.
        }
        else  //보스가 아닐경우
        {
            CharacterMove.TargetEnemy.GetComponent<Enemy>().hp -= demage+ addDemage;  //통상 데미지.
        }

     
        Instantiate(explosion, CharacterMove.TargetEnemy.transform.position, Quaternion.identity);  //공격 이펙트 발생.

        //에너미의 접근해서 hp에 직접적으로 타격을 줌.
        UnityEngine.Debug.Log(this.gameObject.name +"가 "+ hit.collider.name +"에게 공격!...남은 HP:"+ CharacterMove.TargetEnemy.GetComponent<Enemy>().hp);
        


    }
}
