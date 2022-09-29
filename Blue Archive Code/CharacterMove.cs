using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    // Start is called before the first frame update


    public static bool fight = false;  //캐릭터 전투 여부

    public static GameObject TargetEnemy=null;  //타켓이 누구인가... 지정타겟을 3명이서 공격
                                                
    public static bool Shiroko = true;          //전투 여부 및 이동 변수(시로코)
    public static bool Hasumi = true;           //전투 여부 및 이동 변수(하스미)
    public static bool Iori = true;             //전투 여부 및 이동 변수(이오리)


    public  float find_distance;       //적 찾기 거리
    public float speed = 5f;           //이동속도



    void Start()
    {

        /*캐릭터 적 찾기 거리 설정*/
        if (this.gameObject.name == "Shiroko")
            find_distance = 12f;
        else if (this.gameObject.name == "Iori")
            find_distance = 13f;
        else if (this.gameObject.name == "Hasumi")
            find_distance = 15f;


    }

    // Update is called once per frame
    void Update()
    {




        if (!Shiroko && !Iori && !Hasumi)  //전부 이동이 불가능 -> 전투중
        {
            fight = true;
        }



        if (TargetEnemy == null)         //타켓이 없다면 전투중이아님 ->이동
        {
            fight = false;
            Shiroko = true;
            Iori = true;
            Hasumi = true;
        }


        if (this.gameObject.name == "Shiroko")
        {
            if(!fight)       //전투중이 아니라면 적을 찾기
                FindEnemy();


            if (Shiroko)   //이동
                Move();

        }
        else if (this.gameObject.name == "Iori" )
        {
            if(!fight)
                FindEnemy();

            if(Iori)
                Move();
           
        }
        else if (this.gameObject.name == "Hasumi" )
        {
            if(!fight)
                FindEnemy();

            if(Hasumi)
                Move();

        }
            


    }


    private void FindEnemy()  
    {
        int mask = 1 << (LayerMask.NameToLayer("Enemy"));  //Enemy레이어를 가진 적만.

        Collider[] cols = Physics.OverlapSphere(this.transform.position, find_distance,mask);  //오버랩 스페어를 이용해 찾는다.


        if(cols.Length > 0) // "상대를 발견하면"
        {

            for(int i=0;i<cols.Length;i++)  //타켓설정.
            {

                if (this.gameObject.name == "Shiroko" && Shiroko)
                {
                    Debug.Log("시로코 전투태세");
                    Shiroko = false;
                    TargetEnemy = cols[i].gameObject;

                }
                else if (this.gameObject.name == "Iori" && Iori)
                {
                    Debug.Log("이오리 전투태세");
                    Iori = false;
                    TargetEnemy = cols[i].gameObject;

                }
                else if (this.gameObject.name == "Hasumi" && Hasumi)
                {
                    Debug.Log("하스미 전투태세");
                    Hasumi = false;
                    TargetEnemy = cols[i].gameObject;

                }

            }

        }
        else  //적을 찾지 못했을 경우 이동.
        {
            if (this.gameObject.name == "Shiroko" && Shiroko)
                Shiroko = true;
            else if (this.gameObject.name == "Iori" && Iori)
                Iori = true;
            else if (this.gameObject.name == "Hasumi" && Hasumi)
                Hasumi = true;
        }
        

    }


    private void Move()  //이동 속도.
    {
        var direction = this.transform.position;
        direction = transform.right * Time.deltaTime * speed;

        this.transform.Translate(direction);
    }
}
