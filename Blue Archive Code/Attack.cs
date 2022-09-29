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
    


    public Slider Character_HP;  //ĳ���� ���� ������ HP ...�����̴� ����
    public GameObject explosion;  //���� ����Ʈ.
  

    public float hp = 100f;      //ĳ���� hp
    public float attack_distance;  //���� �Ÿ�.
    public float addDemage;  //�߰� ������(�̿����� ���)  Stage���� ��ų �ߵ��� -> addDemage�� �����÷���.
    public int addDemage_count;  //�߰� ������ �����ų źȯ ����(�̿����� ���) 
    public float addAttackSpeed; //�߰� ���ݼӵ�(�̿����� ���) 

    private int attackTime;  //�⺻���� �ӵ�
    private Stopwatch Attackwatch;  //�ð��� ������� ����
    private bool attackCheck;  //����üũ
    private float demage;    //ĳ���� ������.

    private Vector3 pos;    //��ġ ���� ����.

   
    

    void Start()
    {

        this.gameObject.AddComponent<AudioSource>().clip = character_Attacksound;  // AudioSource ������Ʈ �߰�.


        if (this.gameObject.name == "Shiroko")  //�÷��� ���ݼ���
        {
            addAttackSpeed = 0f;
            addDemage = 0f;
            addDemage_count = 0;
            attack_distance = 20;
            attackTime = 1500;
            demage = 9f;

        }
            
        else if (this.gameObject.name == "Iori") //�̿��� ���ݼ���
        {
            addAttackSpeed = 0f;
            addDemage = 0f;
            addDemage_count = 0;
            attack_distance = 20;
            attackTime = 2500;
            demage = 12f;
        }
           
        else if (this.gameObject.name == "Hasumi")  //�Ͻ��� ���ݼ���
        {
            addAttackSpeed = 0f;
            addDemage = 0f;
            addDemage_count = 0;
            attack_distance = 20f;
            attackTime = 3500;
            demage = 21f;
        }


        Attackwatch = new Stopwatch();  
        attackCheck = false;  //�������� �ƴ�.
       
        

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

        

        SetHPbar();  //ĳ����HP UI(Slider)����

        if (CharacterMove.fight)   //���� ���̶��
        {
            aiming();  //����ĳ��Ʈ Ȯ�ο�.

            if(!attackCheck)  //������ ���ϰ� �ִٸ�.
            {
                Attackwatch.Start(); //���ݽð� üũ
                attackCheck = true;  //������
            }
            
            if( attackCheck &&Attackwatch.ElapsedMilliseconds >= attackTime - addAttackSpeed)  //���ݽð���ŭ �ð��� ����ߴٸ�.
            {
                fire();  //����
                Attackwatch.Restart();  //������� ���� �ð��� �ٽ� ����.
            }
        }
        else  //�������� �ƴ϶��
        {
            attackCheck = false;  //������ ���ϵ���.
        }
            



    }


    private void aiming()
    {
        var heading = CharacterMove.TargetEnemy.transform.position - transform.position;

        var distance = heading.magnitude;

        var direction = heading / distance;

        //ĳ���Ϳ� ����� �Ÿ��� ������ ���� ����ĳ��Ʈ�� ���信�� Ȯ��.

        if (Physics.Raycast(transform.position, direction, out hit, attack_distance))
        {
            UnityEngine.Debug.DrawRay(transform.position, direction * hit.distance, Color.green);

        }
      
    }

    private void fire()
    {

        this.gameObject.GetComponent<AudioSource>().Play();  //������ ���.

        if (addDemage_count > 0)  //��ȭ�� �Ѿ��� �ִٸ�
        {
            addDemage_count--;  //��������
            UnityEngine.Debug.Log("��ȭ���Ѿ������ϴ�...��������" + addDemage_count); //Ȯ�ο�.
        }
        else  //��ȭ�� �Ѿ��� ���ٸ�
        {
            addDemage = 0f;  // �߰��������� 0����.
            addAttackSpeed = 0f; //�߰� ���ݼӵ��� 0����.
        }
            


        if (CharacterMove.TargetEnemy.name =="Boss")  //������ ���.
        {

            CharacterMove.TargetEnemy.GetComponent<Enemy>().hp -= (demage+ addDemage) /3;  //������ 1/3�� �ݰ�.
        }
        else  //������ �ƴҰ��
        {
            CharacterMove.TargetEnemy.GetComponent<Enemy>().hp -= demage+ addDemage;  //��� ������.
        }

     
        Instantiate(explosion, CharacterMove.TargetEnemy.transform.position, Quaternion.identity);  //���� ����Ʈ �߻�.

        //���ʹ��� �����ؼ� hp�� ���������� Ÿ���� ��.
        UnityEngine.Debug.Log(this.gameObject.name +"�� "+ hit.collider.name +"���� ����!...���� HP:"+ CharacterMove.TargetEnemy.GetComponent<Enemy>().hp);
        


    }
}
