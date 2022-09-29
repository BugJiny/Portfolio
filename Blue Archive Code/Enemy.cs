using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{


    public AudioClip[] E_AttackSound = new AudioClip[2];


    public GameObject explosion;  //에너미 공격 이펙트
    RaycastHit hit;
    public float hp;     //에너미 hp
    public Slider BossHPbar=null;  //스테이지 보스와 에너미가 스크립트를 공유해서 쓰고있음.


    private GameObject character;   //공격 대상 캐릭터.
    private int attackTime;         //공격시간
    private float demage;           //데미지
    private Stopwatch Attackwatch;  //시간체크변수
    private bool attackCheck;       //공격여부
    private bool readyFight=false;  //전투중
    private Vector3 pos;            //공격이펙트 위치


    // Start is called before the first frame update
    void Start()
    {
        
        hp = 100f;
        attackTime = Random.Range(2,6);
        demage = 5f + attackTime/2;
        attackTime *= 1000;

        this.gameObject.AddComponent<AudioSource>().clip = E_AttackSound[0];


        if (this.gameObject.name == "Boss")  //만약 보스라면.
        {
            hp = 100f;
            demage = 15f;
            attackTime = 2500;
            this.gameObject.GetComponent<AudioSource>().clip = E_AttackSound[1];
        }

        attackCheck = false;
        Attackwatch = new Stopwatch();

        

    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.name == "Boss" )  //스테이지 보스일경우.
        {
            /*스테이지 보스는 HP바가 있기때문에 HP바 설정.*/
            pos = this.gameObject.transform.position;
            pos.z += 5f;
            BossHPbar.transform.position = Camera.main.WorldToScreenPoint(pos);
            BossHPbar.value = hp / 100;

            if (hp <= 0)
            {
                BossHPbar.transform.Find("Fill Area").gameObject.SetActive(false);
                Destroy(BossHPbar.gameObject);
            }
            else
                BossHPbar.transform.Find("Fill Area").gameObject.SetActive(true);

        }





        if (hp <= 0)    //체력이 0이하가 되면
        {
            if (this.gameObject.name == "Boss")  //스테이지 보스일경우 
            {
                UnityEngine.Debug.Log("보스가 쓰러졌습니다.");  //디버그 확인용
                Destroy(this.gameObject);   //오브젝트 파괴.
                Stage.exitStage = true;           //스테이지 종료
            }
            else                      //스테이지 보스가 아닐경우.
            {
                int i = GameObject.Find("StageManager").GetComponent<Stage>().GetEnemyID();  //hp가 0이된 에너미의 정보를 찾아서

                if (i != -1)       //반환된 값이 -1이 아니라면
                {
                    GameObject.Find("StageManager").GetComponent<Stage>().Enemy_fac.RemoveAt(i);  //Stage->List에 접근해서 삭제
                    Destroy(this.gameObject);      //리스트에서 삭제후 게임오브젝트 파괴.
                    CharacterMove.TargetEnemy = null;  //캐릭터들의 타켓을 null로설정
                }

                int a = GameObject.Find("StageManager").GetComponent<Stage>().Enemy_fac.Count;  //현재 리스트 카운트를 찍어봄.
                UnityEngine.Debug.Log("현재 리스트 카운트:" + a);  //디버그 확인용.
            }


        }


        if (CharacterMove.fight)  //캐릭터가 전투 중이라면.
        {
            Search();  //캐릭터를 찾는다.

            if (readyFight)  //전투준비가 완료되었다면.
            {
               
                if (!attackCheck)          //공격준비가 안되었다면
                {
                    Attackwatch.Start();  //시간체크
                    attackCheck = true;  //공격준비
                    aiming();  //적 조준(디버그 확인용)

                }

                if (attackCheck && Attackwatch.ElapsedMilliseconds >= attackTime)  //공격준비가 되었고 공격시간이 되었다면.
                {
                    Fire();  //공격
                    Attackwatch.Reset(); //재공격을 위해 시간리셋.
                    attackCheck = false;  //공격준비 해제.
                }

               
            }
                


        }
           



    }



    private void aiming()
    {
        var heading = character.transform.position - transform.position;

        var distance = heading.magnitude;

        var direction = heading / distance;

        if (Physics.Raycast(transform.position, direction, out hit, 20f))
        {
            UnityEngine.Debug.DrawRay(transform.position, direction * hit.distance, Color.red,0.5f);
        }

    }


    private void Fire()
    {

        this.gameObject.GetComponent<AudioSource>().Play();
        character.GetComponent<Attack>().hp -= demage;
        Instantiate(explosion, character.transform.position, Quaternion.identity);
        UnityEngine.Debug.Log(this.gameObject.name +"가 "+ hit.collider.name+"에게 공격!...남은HP:" + character.GetComponent<Attack>().hp);
    }

    private void Search()  
    {
        int mask = 1 << (LayerMask.NameToLayer("Character"));

        Collider[] cols = Physics.OverlapSphere(this.transform.position, 20f, mask);  //20f 주변에 캐릭터를 발견했다면.

        if(cols.Length > 0)  //캐릭터를 발견했다면
        {
            character = cols[0].gameObject;  //첫번째로 발견한 캐릭터를 공격해라.
            readyFight = true;              //전투준비
        }
        else
            readyFight = false;
       
    }
}
