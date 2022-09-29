using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{


    public AudioClip[] E_AttackSound = new AudioClip[2];


    public GameObject explosion;  //���ʹ� ���� ����Ʈ
    RaycastHit hit;
    public float hp;     //���ʹ� hp
    public Slider BossHPbar=null;  //�������� ������ ���ʹ̰� ��ũ��Ʈ�� �����ؼ� ��������.


    private GameObject character;   //���� ��� ĳ����.
    private int attackTime;         //���ݽð�
    private float demage;           //������
    private Stopwatch Attackwatch;  //�ð�üũ����
    private bool attackCheck;       //���ݿ���
    private bool readyFight=false;  //������
    private Vector3 pos;            //��������Ʈ ��ġ


    // Start is called before the first frame update
    void Start()
    {
        
        hp = 100f;
        attackTime = Random.Range(2,6);
        demage = 5f + attackTime/2;
        attackTime *= 1000;

        this.gameObject.AddComponent<AudioSource>().clip = E_AttackSound[0];


        if (this.gameObject.name == "Boss")  //���� �������.
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
        if (this.gameObject.name == "Boss" )  //�������� �����ϰ��.
        {
            /*�������� ������ HP�ٰ� �ֱ⶧���� HP�� ����.*/
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





        if (hp <= 0)    //ü���� 0���ϰ� �Ǹ�
        {
            if (this.gameObject.name == "Boss")  //�������� �����ϰ�� 
            {
                UnityEngine.Debug.Log("������ ���������ϴ�.");  //����� Ȯ�ο�
                Destroy(this.gameObject);   //������Ʈ �ı�.
                Stage.exitStage = true;           //�������� ����
            }
            else                      //�������� ������ �ƴҰ��.
            {
                int i = GameObject.Find("StageManager").GetComponent<Stage>().GetEnemyID();  //hp�� 0�̵� ���ʹ��� ������ ã�Ƽ�

                if (i != -1)       //��ȯ�� ���� -1�� �ƴ϶��
                {
                    GameObject.Find("StageManager").GetComponent<Stage>().Enemy_fac.RemoveAt(i);  //Stage->List�� �����ؼ� ����
                    Destroy(this.gameObject);      //����Ʈ���� ������ ���ӿ�����Ʈ �ı�.
                    CharacterMove.TargetEnemy = null;  //ĳ���͵��� Ÿ���� null�μ���
                }

                int a = GameObject.Find("StageManager").GetComponent<Stage>().Enemy_fac.Count;  //���� ����Ʈ ī��Ʈ�� ��.
                UnityEngine.Debug.Log("���� ����Ʈ ī��Ʈ:" + a);  //����� Ȯ�ο�.
            }


        }


        if (CharacterMove.fight)  //ĳ���Ͱ� ���� ���̶��.
        {
            Search();  //ĳ���͸� ã�´�.

            if (readyFight)  //�����غ� �Ϸ�Ǿ��ٸ�.
            {
               
                if (!attackCheck)          //�����غ� �ȵǾ��ٸ�
                {
                    Attackwatch.Start();  //�ð�üũ
                    attackCheck = true;  //�����غ�
                    aiming();  //�� ����(����� Ȯ�ο�)

                }

                if (attackCheck && Attackwatch.ElapsedMilliseconds >= attackTime)  //�����غ� �Ǿ��� ���ݽð��� �Ǿ��ٸ�.
                {
                    Fire();  //����
                    Attackwatch.Reset(); //������� ���� �ð�����.
                    attackCheck = false;  //�����غ� ����.
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
        UnityEngine.Debug.Log(this.gameObject.name +"�� "+ hit.collider.name+"���� ����!...����HP:" + character.GetComponent<Attack>().hp);
    }

    private void Search()  
    {
        int mask = 1 << (LayerMask.NameToLayer("Character"));

        Collider[] cols = Physics.OverlapSphere(this.transform.position, 20f, mask);  //20f �ֺ��� ĳ���͸� �߰��ߴٸ�.

        if(cols.Length > 0)  //ĳ���͸� �߰��ߴٸ�
        {
            character = cols[0].gameObject;  //ù��°�� �߰��� ĳ���͸� �����ض�.
            readyFight = true;              //�����غ�
        }
        else
            readyFight = false;
       
    }
}
