using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{


    public static bool exitStage = false;  //게임 클리어 여부...exitStage가 TRUE면 보스를 전부 깨고 스테이지를 나갔다는 의미.

    [Header("UI")]
    public GameObject ClearUI = null;    //하이라키(UI)에서 Panel할당  <Stage클리어시 보여줄 UI> 
    public Slider Gauge = null;        //하이라키(UI)에서 슬라이더 할당
    public Button Shiroko_Skill_Btn;   //하이라키에서 할당
    public Button Hasumi_Skill_Btn;    //하이라키에서 할당
    public Button Iori_Skill_Btn;      //하이라키에서 할당


    [Header("Prefabs")]
    public GameObject enemyObj = null; //프리팹에서 에너미 할당 
    public GameObject HasumiSkillEffect;    //프리팹에 저장된 스킬 이펙트 
    public GameObject ShirokoSkillEffect;   //프리팹에 저장된 스킬 이펙트
    public GameObject IoriSkillEffect;      //프리팹에 저장된 스킬 이펙트


    [Header("Sound")]
    public AudioClip ShirokoSkillSound;      //시로코 스킬 사운드
    public AudioClip IoriSkillSound;         //이오리 스킬 사운드
    public AudioClip HasumiSkillSound;       //하스미 스킬 사운드.





    public List<GameObject> Enemy_fac = new List<GameObject>();  //에너미 관리 리스트
  
    private Sprite[] Cost_img = new Sprite[11];    //게이지값에 해당하는 코스트 이미지 Resources에서 할당
    private Sprite[] Shiroko_Skill_img = new Sprite[2];  //스킬 on/off 이미지 Resources에서 할당(시로코)
    private Sprite[] Hasumi_Skill_img = new Sprite[2];   //스킬 on/off 이미지 Resources에서 할당(하스미)
    private Sprite[] Iori_Skill_img = new Sprite[2];     //스킬 on/off 이미지 Resources에서 할당(이오리)


    // Start is called before the first frame update

    enum CharacterSkillCost    //캐릭터 스킬 코스트 값.
    {
        SHIROKO=2,
        IORI,
        HASUMI=5

    }

    void Start()
    {

        AudioManager.Instance().GetComponent<AudioSource>().Stop();  //Main에서 파괴되지않은 배경음을 정지시킴
        //여기에 스테이지 브금 설정.


        if(!exitStage)  //스테이지가 클리어 안되었다면.
        {
            for (int i = 0; i < 10; i++)  //스테이지에 나타날 적
            {
                GameObject _obj = Instantiate(enemyObj) as GameObject;
                Enemy_fac.Add(_obj);
                Enemy_fac[i].SetActive(true);
            }

            //스테이지 적 위치 직접 할당.
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

       


        Gauge.value = 0f;  //스킬 게이지 값 초기화.


        /*코스트 관련된 이미지 로드*/
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


        /*캐릭터 스킬 on/off 관련 이미지 로드*/
        Shiroko_Skill_img[0] = Resources.Load<Sprite>("Stage_Source/Shiroko_no_Skill");
        Shiroko_Skill_img[1] = Resources.Load<Sprite>("Stage_Source/Shiroko_Skill");

        Hasumi_Skill_img[0] = Resources.Load<Sprite>("Stage_Source/Hasumi_no_Skill");
        Hasumi_Skill_img[1] = Resources.Load<Sprite>("Stage_Source/Hasumi_Skill");

        Iori_Skill_img[0] = Resources.Load<Sprite>("Stage_Source/Iori_no_Skill");
        Iori_Skill_img[1] = Resources.Load<Sprite>("Stage_Source/Iori_Skill");


        /*클리어 UI 화면에서 안보이게 + 상호작용 못하게*/
        ClearUI.GetComponent<Image>().color = Color.clear;
        ClearUI.SetActive(false);
        ClearUI.transform.Find("ExitButton").GetComponent<Image>().color = Color.clear;
        ClearUI.transform.Find("ExitButton").gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {

       

        if (exitStage)  //스테이지를 클리어 했다면
        {

            /*스테이지 클리어 관련 UI를 보여주기 + 상호작용 가능하게*/
            ClearUI.GetComponent<Image>().color = Color.white;
            ClearUI.SetActive(true);
            ClearUI.transform.Find("ExitButton").GetComponent<Image>().color = Color.white;
            ClearUI.transform.Find("ExitButton").gameObject.SetActive(true);

            /*캐릭터들의 이동을 멈춘다.*/
            GameObject.Find("Shiroko").GetComponent<CharacterMove>().speed = 0f;
            GameObject.Find("Iori").GetComponent<CharacterMove>().speed = 0f;
            GameObject.Find("Hasumi").GetComponent<CharacterMove>().speed = 0f;

        }
        else  //스테이지가 안끝났다면.
        {
            if (Gauge.value < 1)  //게이지가 1이하라면 
            {
                GaugeCharge();   //게이지를 충전시킨다.
            }
        }




    }

    private void GaugeCharge()  //게이지를 충전
    {
        Gauge.value += Time.deltaTime * 0.05f;  //해당 속도 만큼 게이지를 충전시킴.

        int cost = (int)(Gauge.value * 10);  
        Gauge.transform.Find("SkillCost").GetComponent<Image>().sprite = Cost_img[cost];  //게이지가 일정부분 오를떄마다 이미지를 바꿔준다.

        Skill_Set(cost);     //게이지가 일정부분 오를때 마다 스킬사용 가능한 캐릭터의 이미지를 바꿔준다.

    }

    public int GetEnemyID()   //Enemy.cs에서 hp가 0이된 게임오브젝트를 리스트를 참조해서 RemoveAt()+Destroy시킬때 
    {                         //해당 에너미의 번호(id)를 넘겨줌.

        if (Enemy_fac.Count == 0)  //예외처리
            return -1;

        for(int i=0; i<Enemy_fac.Count;i++)  //현재 리스트에 있는 에너미중
        {
            if(Enemy_fac[i].gameObject.GetComponent<Enemy>().hp <=0)  //hp가 0된 에너미를 찾아서 그 번호를 반환.
            {
                return i;
            }

        }

        return -1;
    }



    public void ExitStage()  //stage를 클리어하고 종료
    {
        Application.Quit();
        //SceneManager.LoadScene("MainScene");
        //AudioManager.Instance().GetComponent<AudioSource>().Play(); //메인 배경음 재생.

    }


    private void Skill_Set(int nowCost)  //스킬사용 가능여부에 따라 이미지 셋팅
    {
        
        if(nowCost>=(int)CharacterSkillCost.SHIROKO)
        {

            //시로코
            Shiroko_Skill_Btn.GetComponent<Image>().sprite = Shiroko_Skill_img[1];

            //이오리
            if(nowCost>=(int)CharacterSkillCost.IORI)
                Iori_Skill_Btn.GetComponent<Image>().sprite = Iori_Skill_img[1];
            else
                Iori_Skill_Btn.GetComponent<Image>().sprite = Iori_Skill_img[0];

            //하스미
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



    public void Shiroko_Skill()   //시로코 버튼 클릭시 발동될 스킬.
    {
        float cost = (float)CharacterSkillCost.SHIROKO / 10;

        if (Gauge.value >= cost)  //게이지가 코스트보다 같거나 높으면 스킬 발동.
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


            Instantiate(ShirokoSkillEffect, pos, Quaternion.identity);   //시로코 스킬 이펙트 발생.
            UnityEngine.Debug.Log("시로코의 스킬 발동!...모든 캐릭터가 HP를 회복합니다.");

        }
        else
            UnityEngine.Debug.Log("시로코의 스킬 코스트가 부족합니다.");

    }

    public void Iori_Skill()    //이오리 버튼 클릭시 발동될 스킬.
    {
        float cost = (float)CharacterSkillCost.IORI / 10;

        if (Gauge.value >= cost)
        {

            this.gameObject.GetComponent<AudioSource>().clip = IoriSkillSound;
            this.gameObject.GetComponent<AudioSource>().Play();

            Gauge.value -= cost;

            GameObject iori = GameObject.Find("Iori");
            iori.GetComponent<Attack>().addDemage = 6f;     //추가데미지. 
            iori.GetComponent<Attack>().addDemage_count = 5;  //추가데미지가 적용될 총알.
            iori.GetComponent<Attack>().addAttackSpeed = 800;  //추가 공격속도.
            Instantiate(IoriSkillEffect, iori.transform.position, Quaternion.identity);
            UnityEngine.Debug.Log("이오리의 스킬 발동!...공격력과 속도가 증가합니다.");
            UnityEngine.Debug.Log("강화된 총알을 장선했습니다...남은수:" + iori.GetComponent<Attack>().addDemage_count);


        }
           
        else
            UnityEngine.Debug.Log("이오리의 스킬 코스트가 부족합니다.");
    }

    public void Hasumi_Skill()  //하스미 버튼 클릭시 발동될 스킬.
    {
        float demage = 60f;
        
        float cost = (float)CharacterSkillCost.HASUMI / 10;

        if (Gauge.value >= cost)
        {

            this.gameObject.GetComponent<AudioSource>().clip = HasumiSkillSound;
            this.gameObject.GetComponent<AudioSource>().Play();

            Gauge.value -= cost;
            Instantiate(HasumiSkillEffect, CharacterMove.TargetEnemy.transform.position, Quaternion.identity);
            UnityEngine.Debug.Log("하스미의 스킬 발동!...강한 공격을 사용합니다.");

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
            UnityEngine.Debug.Log("하스미의 스킬 코스트가 부족합니다.");
    }
   


}
