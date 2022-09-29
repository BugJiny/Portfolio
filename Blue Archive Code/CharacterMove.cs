using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    // Start is called before the first frame update


    public static bool fight = false;  //ĳ���� ���� ����

    public static GameObject TargetEnemy=null;  //Ÿ���� �����ΰ�... ����Ÿ���� 3���̼� ����
                                                
    public static bool Shiroko = true;          //���� ���� �� �̵� ����(�÷���)
    public static bool Hasumi = true;           //���� ���� �� �̵� ����(�Ͻ���)
    public static bool Iori = true;             //���� ���� �� �̵� ����(�̿���)


    public  float find_distance;       //�� ã�� �Ÿ�
    public float speed = 5f;           //�̵��ӵ�



    void Start()
    {

        /*ĳ���� �� ã�� �Ÿ� ����*/
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




        if (!Shiroko && !Iori && !Hasumi)  //���� �̵��� �Ұ��� -> ������
        {
            fight = true;
        }



        if (TargetEnemy == null)         //Ÿ���� ���ٸ� �������̾ƴ� ->�̵�
        {
            fight = false;
            Shiroko = true;
            Iori = true;
            Hasumi = true;
        }


        if (this.gameObject.name == "Shiroko")
        {
            if(!fight)       //�������� �ƴ϶�� ���� ã��
                FindEnemy();


            if (Shiroko)   //�̵�
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
        int mask = 1 << (LayerMask.NameToLayer("Enemy"));  //Enemy���̾ ���� ����.

        Collider[] cols = Physics.OverlapSphere(this.transform.position, find_distance,mask);  //������ ���� �̿��� ã�´�.


        if(cols.Length > 0) // "��븦 �߰��ϸ�"
        {

            for(int i=0;i<cols.Length;i++)  //Ÿ�ϼ���.
            {

                if (this.gameObject.name == "Shiroko" && Shiroko)
                {
                    Debug.Log("�÷��� �����¼�");
                    Shiroko = false;
                    TargetEnemy = cols[i].gameObject;

                }
                else if (this.gameObject.name == "Iori" && Iori)
                {
                    Debug.Log("�̿��� �����¼�");
                    Iori = false;
                    TargetEnemy = cols[i].gameObject;

                }
                else if (this.gameObject.name == "Hasumi" && Hasumi)
                {
                    Debug.Log("�Ͻ��� �����¼�");
                    Hasumi = false;
                    TargetEnemy = cols[i].gameObject;

                }

            }

        }
        else  //���� ã�� ������ ��� �̵�.
        {
            if (this.gameObject.name == "Shiroko" && Shiroko)
                Shiroko = true;
            else if (this.gameObject.name == "Iori" && Iori)
                Iori = true;
            else if (this.gameObject.name == "Hasumi" && Hasumi)
                Hasumi = true;
        }
        

    }


    private void Move()  //�̵� �ӵ�.
    {
        var direction = this.transform.position;
        direction = transform.right * Time.deltaTime * speed;

        this.transform.Translate(direction);
    }
}
