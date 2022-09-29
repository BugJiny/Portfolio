using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{


    public static bool exitStage = false;  //���� Ŭ���� ����...exitStage�� TRUE�� ������ ���� ���� ���������� �����ٴ� �ǹ�.

    [Header("UI")]
    public GameObject ClearUI = null;    //���̶�Ű(UI)���� Panel�Ҵ�  <StageŬ����� ������ UI> 
    public Slider Gauge = null;        //���̶�Ű(UI)���� �����̴� �Ҵ�
    public Button Shiroko_Skill_Btn;   //���̶�Ű���� �Ҵ�
    public Button Hasumi_Skill_Btn;    //���̶�Ű���� �Ҵ�
    public Button Iori_Skill_Btn;      //���̶�Ű���� �Ҵ�


    [Header("Prefabs")]
    public GameObject enemyObj = null; //�����տ��� ���ʹ� �Ҵ� 
    public GameObject HasumiSkillEffect;    //�����տ� ����� ��ų ����Ʈ 
    public GameObject ShirokoSkillEffect;   //�����տ� ����� ��ų ����Ʈ
    public GameObject IoriSkillEffect;      //�����տ� ����� ��ų ����Ʈ


    [Header("Sound")]
    public AudioClip ShirokoSkillSound;      //�÷��� ��ų ����
    public AudioClip IoriSkillSound;         //�̿��� ��ų ����
    public AudioClip HasumiSkillSound;       //�Ͻ��� ��ų ����.





    public List<GameObject> Enemy_fac = new List<GameObject>();  //���ʹ� ���� ����Ʈ
  
    private Sprite[] Cost_img = new Sprite[11];    //���������� �ش��ϴ� �ڽ�Ʈ �̹��� Resources���� �Ҵ�
    private Sprite[] Shiroko_Skill_img = new Sprite[2];  //��ų on/off �̹��� Resources���� �Ҵ�(�÷���)
    private Sprite[] Hasumi_Skill_img = new Sprite[2];   //��ų on/off �̹��� Resources���� �Ҵ�(�Ͻ���)
    private Sprite[] Iori_Skill_img = new Sprite[2];     //��ų on/off �̹��� Resources���� �Ҵ�(�̿���)


    // Start is called before the first frame update

    enum CharacterSkillCost    //ĳ���� ��ų �ڽ�Ʈ ��.
    {
        SHIROKO=2,
        IORI,
        HASUMI=5

    }

    void Start()
    {

        AudioManager.Instance().GetComponent<AudioSource>().Stop();  //Main���� �ı��������� ������� ������Ŵ
        //���⿡ �������� ��� ����.


        if(!exitStage)  //���������� Ŭ���� �ȵǾ��ٸ�.
        {
            for (int i = 0; i < 10; i++)  //���������� ��Ÿ�� ��
            {
                GameObject _obj = Instantiate(enemyObj) as GameObject;
                Enemy_fac.Add(_obj);
                Enemy_fac[i].SetActive(true);
            }

            //�������� �� ��ġ ���� �Ҵ�.
            Enemy_fac[0].transform.position = new Vector3(-15f, 1f, -15f);
            Enemy_fac[1].transform.position = new Vector3(-13f, 2f, -11f);
            Enemy_fac[2].transform.position = new Vector3(-10f, 2f, -13f);
            Enemy_fac[3].transform.position = new Vector3(3.8f, 1f, -8.5f);
            Enemy_fac[4].transform.position = new Vector3(5f, 2f, -11.9f);
            Enemy_fac[5].transform.position = new Vector3(3.9f, 1f, -16f);
            Enemy_fac[6].transform.position = new Vector3(24.5f, 2f, -8.1f);
            Enemy_fac[7].transform.position = new Vector3(24.5f, 2f, -13.2f);
            Enemy_fac[8].transform.position = new Vector3(24.5f, 2f, -18.3f);
            Enemy_fac[9].transform.position = new Vector3(21.5f, 1f, -13.2f);
        }

       


        Gauge.value = 0f;  //��ų ������ �� �ʱ�ȭ.


        /*�ڽ�Ʈ ���õ� �̹��� �ε�*/
        Cost_img[0] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_0");
        Cost_img[1] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_1");
        Cost_img[2] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_2");
        Cost_img[3] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_3");
        Cost_img[4] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_4");
        Cost_img[5] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_5");
        Cost_img[6] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_6");
        Cost_img[7] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_7");
        Cost_img[8] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_8");
        Cost_img[9] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_9");
        Cost_img[10] = Resources.Load<Sprite>("Stage_Source/Cost_img/cost_10");


        /*ĳ���� ��ų on/off ���� �̹��� �ε�*/
        Shiroko_Skill_img[0] = Resources.Load<Sprite>("Stage_Source/Shiroko_no_Skill");
        Shiroko_Skill_img[1] = Resources.Load<Sprite>("Stage_Source/Shiroko_Skill");

        Hasumi_Skill_img[0] = Resources.Load<Sprite>("Stage_Source/Hasumi_no_Skill");
        Hasumi_Skill_img[1] = Resources.Load<Sprite>("Stage_Source/Hasumi_Skill");

        Iori_Skill_img[0] = Resources.Load<Sprite>("Stage_Source/Iori_no_Skill");
        Iori_Skill_img[1] = Resources.Load<Sprite>("Stage_Source/Iori_Skill");


        /*Ŭ���� UI ȭ�鿡�� �Ⱥ��̰� + ��ȣ�ۿ� ���ϰ�*/
        ClearUI.GetComponent<Image>().color = Color.clear;
        ClearUI.SetActive(false);
        ClearUI.transform.Find("ExitButton").GetComponent<Image>().color = Color.clear;
        ClearUI.transform.Find("ExitButton").gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {

       

        if (exitStage)  //���������� Ŭ���� �ߴٸ�
        {

            /*�������� Ŭ���� ���� UI�� �����ֱ� + ��ȣ�ۿ� �����ϰ�*/
            ClearUI.GetComponent<Image>().color = Color.white;
            ClearUI.SetActive(true);
            ClearUI.transform.Find("ExitButton").GetComponent<Image>().color = Color.white;
            ClearUI.transform.Find("ExitButton").gameObject.SetActive(true);

            /*ĳ���͵��� �̵��� �����.*/
            GameObject.Find("Shiroko").GetComponent<CharacterMove>().speed = 0f;
            GameObject.Find("Iori").GetComponent<CharacterMove>().speed = 0f;
            GameObject.Find("Hasumi").GetComponent<CharacterMove>().speed = 0f;

        }
        else  //���������� �ȳ����ٸ�.
        {
            if (Gauge.value < 1)  //�������� 1���϶�� 
            {
                GaugeCharge();   //�������� ������Ų��.
            }
        }




    }

    private void GaugeCharge()  //�������� ����
    {
        Gauge.value += Time.deltaTime * 0.05f;  //�ش� �ӵ� ��ŭ �������� ������Ŵ.

        int cost = (int)(Gauge.value * 10);  
        Gauge.transform.Find("SkillCost").GetComponent<Image>().sprite = Cost_img[cost];  //�������� �����κ� ���������� �̹����� �ٲ��ش�.

        Skill_Set(cost);     //�������� �����κ� ������ ���� ��ų��� ������ ĳ������ �̹����� �ٲ��ش�.

    }

    public int GetEnemyID()   //Enemy.cs���� hp�� 0�̵� ���ӿ�����Ʈ�� ����Ʈ�� �����ؼ� RemoveAt()+Destroy��ų�� 
    {                         //�ش� ���ʹ��� ��ȣ(id)�� �Ѱ���.

        if (Enemy_fac.Count == 0)  //����ó��
            return -1;

        for(int i=0; i<Enemy_fac.Count;i++)  //���� ����Ʈ�� �ִ� ���ʹ���
        {
            if(Enemy_fac[i].gameObject.GetComponent<Enemy>().hp <=0)  //hp�� 0�� ���ʹ̸� ã�Ƽ� �� ��ȣ�� ��ȯ.
            {
                return i;
            }

        }

        return -1;
    }



    public void ExitStage()  //stage�� Ŭ�����ϰ� ����
    {
        Application.Quit();
        //SceneManager.LoadScene("MainScene");
        //AudioManager.Instance().GetComponent<AudioSource>().Play(); //���� ����� ���.

    }


    private void Skill_Set(int nowCost)  //��ų��� ���ɿ��ο� ���� �̹��� ����
    {
        
        if(nowCost>=(int)CharacterSkillCost.SHIROKO)
        {

            //�÷���
            Shiroko_Skill_Btn.GetComponent<Image>().sprite = Shiroko_Skill_img[1];

            //�̿���
            if(nowCost>=(int)CharacterSkillCost.IORI)
                Iori_Skill_Btn.GetComponent<Image>().sprite = Iori_Skill_img[1];
            else
                Iori_Skill_Btn.GetComponent<Image>().sprite = Iori_Skill_img[0];

            //�Ͻ���
            if (nowCost>=(int)CharacterSkillCost.HASUMI)
                Hasumi_Skill_Btn.GetComponent<Image>().sprite = Hasumi_Skill_img[1];
            else
                Hasumi_Skill_Btn.GetComponent<Image>().sprite = Hasumi_Skill_img[0];

        }
        else
        {
            Shiroko_Skill_Btn.GetComponent<Image>().sprite = Shiroko_Skill_img[0];
            Hasumi_Skill_Btn.GetComponent<Image>().sprite = Hasumi_Skill_img[0];
            Iori_Skill_Btn.GetComponent<Image>().sprite = Iori_Skill_img[0];
        }

        


    }



    public void Shiroko_Skill()   //�÷��� ��ư Ŭ���� �ߵ��� ��ų.
    {
        float cost = (float)CharacterSkillCost.SHIROKO / 10;

        if (Gauge.value >= cost)  //�������� �ڽ�Ʈ���� ���ų� ������ ��ų �ߵ�.
        {

            this.gameObject.GetComponent<AudioSource>().clip = ShirokoSkillSound;
            this.gameObject.GetComponent<AudioSource>().Play();

            Gauge.value -= cost;

            GameObject shiroko = GameObject.Find("Shiroko");

            Vector3 pos = shiroko.transform.position;
            pos.y += 2f;



            float shirokoHP, ioriHP, hasumiHP;

            shirokoHP = shiroko.GetComponent<Attack>().hp;
            ioriHP = GameObject.Find("Iori").GetComponent<Attack>().hp;
            hasumiHP = GameObject.Find("Hasumi").GetComponent<Attack>().hp;


            if(shirokoHP <= 90)
                shiroko.GetComponent<Attack>().hp += 10f;
            else if( 90<shirokoHP && 100>= shirokoHP) //91~100
            {
                float heal = 100 - shirokoHP;
                shiroko.GetComponent<Attack>().hp += heal;
            }


            if(ioriHP <= 90)
                GameObject.Find("Iori").GetComponent<Attack>().hp += 10f;
            else if(90 < ioriHP && 100 >= ioriHP)
            {
                float heal = 100-ioriHP;
                GameObject.Find("Iori").GetComponent<Attack>().hp += heal;
            }


            if(hasumiHP<=90)
                GameObject.Find("Hasumi").GetComponent<Attack>().hp += 10f;
            else if(90 < hasumiHP && 100 >= hasumiHP)
            {
                float heal = 100-hasumiHP;
                GameObject.Find("Hasumi").GetComponent<Attack>().hp += heal;
            }


            Instantiate(ShirokoSkillEffect, pos, Quaternion.identity);   //�÷��� ��ų ����Ʈ �߻�.
            UnityEngine.Debug.Log("�÷����� ��ų �ߵ�!...��� ĳ���Ͱ� HP�� ȸ���մϴ�.");

        }
        else
            UnityEngine.Debug.Log("�÷����� ��ų �ڽ�Ʈ�� �����մϴ�.");

    }

    public void Iori_Skill()    //�̿��� ��ư Ŭ���� �ߵ��� ��ų.
    {
        float cost = (float)CharacterSkillCost.IORI / 10;

        if (Gauge.value >= cost)
        {

            this.gameObject.GetComponent<AudioSource>().clip = IoriSkillSound;
            this.gameObject.GetComponent<AudioSource>().Play();

            Gauge.value -= cost;

            GameObject iori = GameObject.Find("Iori");
            iori.GetComponent<Attack>().addDemage = 6f;     //�߰�������. 
            iori.GetComponent<Attack>().addDemage_count = 5;  //�߰��������� ����� �Ѿ�.
            iori.GetComponent<Attack>().addAttackSpeed = 800;  //�߰� ���ݼӵ�.
            Instantiate(IoriSkillEffect, iori.transform.position, Quaternion.identity);
            UnityEngine.Debug.Log("�̿����� ��ų �ߵ�!...���ݷ°� �ӵ��� �����մϴ�.");
            UnityEngine.Debug.Log("��ȭ�� �Ѿ��� �弱�߽��ϴ�...������:" + iori.GetComponent<Attack>().addDemage_count);


        }
           
        else
            UnityEngine.Debug.Log("�̿����� ��ų �ڽ�Ʈ�� �����մϴ�.");
    }

    public void Hasumi_Skill()  //�Ͻ��� ��ư Ŭ���� �ߵ��� ��ų.
    {
        float demage = 60f;
        
        float cost = (float)CharacterSkillCost.HASUMI / 10;

        if (Gauge.value >= cost)
        {

            this.gameObject.GetComponent<AudioSource>().clip = HasumiSkillSound;
            this.gameObject.GetComponent<AudioSource>().Play();

            Gauge.value -= cost;
            Instantiate(HasumiSkillEffect, CharacterMove.TargetEnemy.transform.position, Quaternion.identity);
            UnityEngine.Debug.Log("�Ͻ����� ��ų �ߵ�!...���� ������ ����մϴ�.");

            if (CharacterMove.TargetEnemy.name == "Boss")
            {
                CharacterMove.TargetEnemy.GetComponent<Enemy>().hp -= demage / 3;
            }
            else
            {

                CharacterMove.TargetEnemy.GetComponent<Enemy>().hp -= demage;
            }

        }
        else
            UnityEngine.Debug.Log("�Ͻ����� ��ų �ڽ�Ʈ�� �����մϴ�.");
    }
   


}
