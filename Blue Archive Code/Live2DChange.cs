using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Diagnostics;

public class Live2DChange : MonoBehaviour
{
    /*STATIC*/
   


    /*PUBLIC*/
    public VideoPlayer Live2DController;
    public VideoPlayer Live2DEffect;
    public Image BackGround_img;

    public RawImage MemorialEffect_rawimage;
    public GameObject speechBubble;
    public GameObject speechBubble_Big;
    public Button DialogBtn;



    [Header("보이스")]
    public GameObject Voice;

    [Header("UI")]
    public GameObject UpUI;
    public GameObject DownUI;
    public GameObject OptionPanel;
    public GameObject StagePanel;
    public Button ChangeBtn;
    public Button UiShowAndHideBtn;
    public Button StageBtn;


    /*PRIVATE*/
    private int Character;
    private bool live2D = false;      //라이브2D 제어변수
    private bool effect = true;       //라이브2D 연출효과 제어변수1
    private float effectTime;         //연출효과 시간 변수.  
    private Stopwatch Effectwatch;    //연출효과 시간 측정.
    private bool illust = true;       //일러스트 제어변수

 

    private Sprite[] Shiroko_dialogSource = new Sprite[5];  //시로코 대사 모음
    private AudioClip[] Shiroko_Voice = new AudioClip[5];   //시로코 음성 모음.

    private Sprite[] Hasumi_dialogSource = new Sprite[6];   //하스미 대사 모음
    private AudioClip[] Hasumi_Voice = new AudioClip[5];    //하스미 음성 모음

    private Sprite[] Iori_dialogSource = new Sprite[7];    //이오리 대사 모음
    private AudioClip[] Iori_Voice = new AudioClip[5];      //이오리 음성 모음


    private Sprite[] Character_BackGround = new Sprite[3];   //캐릭터 배경화면 모음
    private VideoClip[] Character_Live2D = new VideoClip[3];  //캐릭터 라이브 2D 모음
    private VideoClip[] Character_Effect = new VideoClip[3];  //캐릭터 라이브 2D 연출효과 모음


    
    private Sprite EndPanel_img;  //종료




    private int dialog;               //대사 변수
    private float dialogEffectTime;         //대사 시간 변수.
    private bool dialogEffect = false;        //대사 연출 변수
    private Stopwatch dialogWatch;        //대사 연출 시간측정.
    private float voicelen;            //음성길이.
     
   



    private bool uiHide = false;   //UI 페이드 인 페이드 아웃 제어 변수.


    private bool SoundCheck = true;  //사운드 버튼 보이는 용도.
    private bool SoundOptionShow = false;  //사운드 창 보여주는용도.


    enum CharacterNumber
    {
        SHIROKO = 0,
        HASUMI,
        IORI
    }



    private void Awake()
    {

        //로드시킨후 메모리에 적재된후 게임이 종료되면 Resources.UnloadAsset 으로 초기화를 시켜줘야 한다.

        //시로코 대화창 소스 초기화
        Shiroko_dialogSource[0] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg1");
        Shiroko_dialogSource[1] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg2");
        Shiroko_dialogSource[2] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg3");
        Shiroko_dialogSource[3] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg4");
        Shiroko_dialogSource[4] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg5");

       
        //시로코 음성 소스 초기화
        Shiroko_Voice[0] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice1");
        Shiroko_Voice[1] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice2");
        Shiroko_Voice[2] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice3");
        Shiroko_Voice[3] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice4");
        Shiroko_Voice[4] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice5");

        //하스미 대화창 소스 초기화
        Hasumi_dialogSource[0] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg1");
        Hasumi_dialogSource[1] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg2");
        Hasumi_dialogSource[2] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg3");
        Hasumi_dialogSource[3] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg4");
        Hasumi_dialogSource[4] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg5-1");
        Hasumi_dialogSource[5] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg5-2");

        //하스미 음성 소스 초기화
        Hasumi_Voice[0] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice1");
        Hasumi_Voice[1] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice2");
        Hasumi_Voice[2] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice3");
        Hasumi_Voice[3] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice4");
        Hasumi_Voice[4] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice5");

        //이오리 대화창 소스 초기화
        Iori_dialogSource[0] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg1");
        Iori_dialogSource[1] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg2");
        Iori_dialogSource[2] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg3-1");
        Iori_dialogSource[3] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg3-2");
        Iori_dialogSource[4] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg4");
        Iori_dialogSource[5] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg5-1");
        Iori_dialogSource[6] = Resources.Load<Sprite>("Main_Source/Iori/Iori_dlg5-2");

        Iori_Voice[0] = Resources.Load<AudioClip>("Main_Source/Iori/Voice/IoriVoice1");
        Iori_Voice[1] = Resources.Load<AudioClip>("Main_Source/Iori/Voice/IoriVoice2");
        Iori_Voice[2] = Resources.Load<AudioClip>("Main_Source/Iori/Voice/IoriVoice3");
        Iori_Voice[3] = Resources.Load<AudioClip>("Main_Source/Iori/Voice/IoriVoice4");
        Iori_Voice[4] = Resources.Load<AudioClip>("Main_Source/Iori/Voice/IoriVoice5");


        Character_BackGround[0] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_MemorialLobby_illust");
        Character_BackGround[1] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_MemorialLobby_illust");
        Character_BackGround[2] = Resources.Load<Sprite>("Main_Source/Iori/Iori_MemorialLobby_illust");

        Character_Live2D[0] = Resources.Load<VideoClip>("Main_Source/Shiroko/Shiroko_MemorialLobby_Live2D");
        Character_Live2D[1] = Resources.Load<VideoClip>("Main_Source/Hasumi/Hasumi_MemorialLobby_Live2D");
        Character_Live2D[2] = Resources.Load<VideoClip>("Main_Source/Iori/Iori_MemorialLobby_Live2D");

        Character_Effect[0] = Resources.Load<VideoClip>("Main_Source/Shiroko/Shiroko_Memorial_Effect");
        Character_Effect[1] = Resources.Load<VideoClip>("Main_Source/Hasumi/Hasumi_Memorial_Effect");
        Character_Effect[2] = Resources.Load<VideoClip>("Main_Source/Iori/Iori_Memorial_Effect");


        EndPanel_img = Resources.Load<Sprite>("Main_Source/EndStage");  



    }

    // Start is called before the first frame update
    void Start()
    {

        OptionPanel.GetComponent<Image>().color = Color.clear;
        OptionPanel.SetActive(false);

        StagePanel.GetComponent<Image>().color = Color.clear;
        StagePanel.SetActive(false);

        Effectwatch = new Stopwatch();
        dialogWatch = new Stopwatch();

        speechBubble.GetComponent<Image>().color = Color.clear; // 대화창 지워주기

        Character = Status.MainCharacter; //캐릭터 번호를 가져옴.  -> 스테이터스 창에서 메인캐릭터로 설정한 캐릭터 번호를  가져옴.

        //해당 번호에 맞는 배경,Live2D, 연출효과를 셋팅.
        BackGround_img.sprite = Character_BackGround[Character];            
        Live2DController.clip = Character_Live2D[Character];
        Live2DEffect.clip = Character_Effect[Character];

        Live2DEffect.time = 0f;
        Live2DEffect.Pause();

        if(Status.ChangeBGM)
            AudioManager.Instance().MusicSet(Character);
           


        if (Character == (int)CharacterNumber.HASUMI)
        {
            DialogBtn.transform.localPosition = new Vector3(-148, 0, 0);
          
        }
        else if(Character == (int)CharacterNumber.IORI)
        {
            DialogBtn.transform.localPosition = new Vector3(116, 0, 0);
            speechBubble.transform.localPosition = new Vector3(486, -6, 0);

        }

       
    }


    public void ChangeBackGround()
    {
        if (illust)
        {
            illust = false;
            live2D = true;
        }
        else
        {
            illust = true;
            live2D = false;
        }

        //배경이 전환될 때 UI,다른 상호작용 멈추기.
        speechBubble.GetComponent<Image>().color = Color.clear; //화면전환시 대사 안보이게 컬러값 초기화.
        speechBubble_Big.GetComponent<Image>().color = Color.clear; //화면전환시 대사 안보이게 컬러값 초기화.

        Voice.GetComponent<AudioSource>().Stop();    //캐릭터 목소리 정지.
        dialogWatch.Reset(); //대사 시간측정 리셋
        dialogWatch.Stop();  //대사 시간측정 스탑 
        dialogEffect = false;  //대사 상호작용 못하게
    }

    public void UIShowAndHide()
    {

        if(uiHide)        //UI가 숨겨져있으면 페이드인으로 보여주기
        {
            ChangeBtn.GetComponent<Image>().color = Color.white;
            ChangeBtn.interactable = true;

            UiShowAndHideBtn.GetComponent<Image>().color = Color.white;
            UiShowAndHideBtn.interactable = true;

            StageBtn.GetComponent<Image>().color = Color.white;
            StageBtn.interactable = true;

            StartCoroutine("UIFadeIn");
            uiHide = false;

        }
        else             //UI가 보이고 있으면 페이드 아웃으로 감추기
        {

            ChangeBtn.GetComponent<Image>().color = Color.clear;
            ChangeBtn.interactable = false;
            UiShowAndHideBtn.GetComponent<Image>().color = Color.clear;
            UiShowAndHideBtn.interactable = false;
            StageBtn.GetComponent<Image>().color = Color.clear;
            StageBtn.interactable = false;

            StartCoroutine("UIFadeOut");
            uiHide = true;
        }

    }


    private void DialogSet(Sprite image, AudioClip audio)  //대사 셋팅함수
    {
        speechBubble.GetComponent<Image>().sprite = image;
        Voice.GetComponent<AudioSource>().clip = audio;
    }

    private void DialogSet_Big(Sprite image1,Sprite image2 ,AudioClip audio)  //이오리 특정대사(3) 셋팅함수.
    {
        speechBubble_Big.GetComponent<Image>().sprite = image1;
        speechBubble.GetComponent<Image>().sprite = image2;

        Voice.GetComponent<AudioSource>().clip = audio;
    }

    public void Change_Dialog()        //대사 바꾸기 -> 버튼 이벤트 함수.
    {

        dialog = Random.Range(1, 6);  //대사를 랜덤으로 뽑는다.

        speechBubble_Big.GetComponent<Image>().color = Color.clear; //버튼을 누를떄 마다 대사 컬러값을 초기화.
        speechBubble.GetComponent<Image>().color = Color.clear; //버튼을 누를떄 마다 대사 컬러값을 초기화.



        if (!dialogEffect)       //대사가 나오고 있지 않을때 클릭시
        {
            dialogEffect = true;
            dialogWatch.Start();
        }
        else                   //대사가 나오고 있는중 클릭해 다른대사가 나올떄
        {
            dialogEffectTime = 0;
            dialogWatch.Restart();    //다시 측정을 해라.
        }
       

        switch (dialog)       //캐릭터에 따른 대사 셋팅.
        {
            case 1:
                if(Character == (int)CharacterNumber.SHIROKO)
                    DialogSet(Shiroko_dialogSource[0], Shiroko_Voice[0]);
                else if(Character == (int)CharacterNumber.HASUMI)
                    DialogSet(Hasumi_dialogSource[0], Hasumi_Voice[0]);
                else if(Character == (int)CharacterNumber.IORI)
                    DialogSet(Iori_dialogSource[0], Iori_Voice[0]);
                break;

            case 2:
                if (Character == (int)CharacterNumber.SHIROKO)
                    DialogSet(Shiroko_dialogSource[1], Shiroko_Voice[1]);
                else if (Character == (int)CharacterNumber.HASUMI)
                    DialogSet(Hasumi_dialogSource[1], Hasumi_Voice[1]);
                else if (Character == (int)CharacterNumber.IORI)
                    DialogSet(Iori_dialogSource[1], Iori_Voice[1]);
                break;
            case 3:
                if (Character == (int)CharacterNumber.SHIROKO)
                    DialogSet(Shiroko_dialogSource[2], Shiroko_Voice[2]);
                else if (Character == (int)CharacterNumber.HASUMI)
                    DialogSet(Hasumi_dialogSource[2], Hasumi_Voice[2]);
                else if (Character == (int)CharacterNumber.IORI)
                    DialogSet_Big(Iori_dialogSource[2], Iori_dialogSource[3], Iori_Voice[2]);  //대사가 길어서 2,3 배열의 대사를 같이씀  
                break;
            case 4:
                if (Character == (int)CharacterNumber.SHIROKO)
                    DialogSet(Shiroko_dialogSource[3], Shiroko_Voice[3]);
                else if (Character == (int)CharacterNumber.HASUMI)
                    DialogSet(Hasumi_dialogSource[3], Hasumi_Voice[3]);
                else if (Character == (int)CharacterNumber.IORI)
                    DialogSet(Iori_dialogSource[4], Iori_Voice[3]);
                break;
            case 5:
                if (Character == (int)CharacterNumber.SHIROKO)
                    DialogSet(Shiroko_dialogSource[4], Shiroko_Voice[4]);
                else if (Character == (int)CharacterNumber.HASUMI)
                    DialogSet(Hasumi_dialogSource[4], Hasumi_Voice[4]);              //대사가 길어서 4,5 배열의 대사를 같이씀
                else if (Character == (int)CharacterNumber.IORI)   
                    DialogSet(Iori_dialogSource[5], Iori_Voice[4]);                 // 대사가 길어서 5,6 배열의 대사를 같이씀
                break;
        }

        voicelen = Voice.GetComponent<AudioSource>().clip.length * 1000;   //보이스길이
        Voice.GetComponent<AudioSource>().Play();    //캐릭터 목소리 재생.
     
    }

  


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (dialogEffect)  //대사가 트루라면.
        {
            dialogEffectTime = dialogWatch.ElapsedMilliseconds;


            if (dialogEffectTime > voicelen ) //음성 길이만큼 대화창 진행.
            {
                dialogEffectTime = 0;
                dialogWatch.Reset();
                dialogWatch.Stop();
                dialogEffect = false;
            }
            else                            //음성길이만큼의 시간이 경과하지않았다면
            {


                if (dialogEffectTime <= 3000)            //UI감추는 버튼을 3초동안보여줘라
                {
                    UiShowAndHideBtn.GetComponent<Image>().color = Color.white;
                    UiShowAndHideBtn.interactable = true;
                }


                if (Character == (int)CharacterNumber.IORI && dialog ==3)         //대사가 특수하거나 긴 대사 이오리(3)
                {
                   
                    if (dialogEffectTime < 6500)
                        StartCoroutine("DialogFadeIn",true);
                    else if (dialogEffectTime >=6500 && dialogEffectTime < 13000)
                        StartCoroutine("DialogFadeOut",true);
                    else if(dialogEffectTime>=13000 && dialogEffectTime < 16000)
                    {
                        speechBubble.GetComponent<Image>().sprite = Iori_dialogSource[3];
                        StartCoroutine("DialogFadeIn", false);
                    }
                    else if(dialogEffectTime >= 16000 && dialogEffectTime < voicelen)
                        StartCoroutine("DialogFadeOut", false);

                }
                else if(Character==(int)CharacterNumber.IORI && dialog ==5)       //대사가 특수하거나 긴 대사 이오리(5)
                {

                    if (dialogEffectTime < 2500)
                        StartCoroutine("DialogFadeIn", false);
                    else if (dialogEffectTime >= 2500 && dialogEffectTime < 5000)
                        StartCoroutine("DialogFadeOut", false);
                    else if (dialogEffectTime >= 5000 && dialogEffectTime < 7500)
                    {
                        speechBubble.GetComponent<Image>().sprite = Iori_dialogSource[6];
                        StartCoroutine("DialogFadeIn", false);
                    }
                    else if (dialogEffectTime >= 7500 && dialogEffectTime < voicelen)
                    {
                        speechBubble.GetComponent<Image>().sprite = Iori_dialogSource[6];
                        StartCoroutine("DialogFadeOut", false);
                    }

                }
                else if(Character ==(int)CharacterNumber.HASUMI && dialog==5)        //대사가 특수하거나 긴 대사 하스미(5)
                {

                    if (dialogEffectTime < 4000)
                        StartCoroutine("DialogFadeIn", false);
                    else if (dialogEffectTime >= 4000 && dialogEffectTime < 8000)
                        StartCoroutine("DialogFadeOut", false);
                    else if (dialogEffectTime >= 8000 && dialogEffectTime < 11500)
                    {
                        speechBubble.GetComponent<Image>().sprite = Hasumi_dialogSource[5];
                        StartCoroutine("DialogFadeIn", false);
                    }
                    else if (dialogEffectTime >= 11500 && dialogEffectTime < voicelen)
                    {
                        speechBubble.GetComponent<Image>().sprite = Hasumi_dialogSource[5];
                        StartCoroutine("DialogFadeOut", false);
                    }

                }
                else          //그 외의 평범한 대사
                {

                    if (dialogEffectTime < voicelen / 2)       //음성길이의 절반의 시간만큼 대사를 서서히보여준다
                    {
                        UiShowAndHideBtn.GetComponent<Image>().color = Color.white;
                        UiShowAndHideBtn.interactable = true;
                        StartCoroutine("DialogFadeIn",false);
                    }
                    else if (dialogEffectTime >= voicelen / 2 && dialogEffectTime < voicelen)  //음성길이의 절반부터 끝까지 대사를 서서히 감줘춘다.
                    {
                        StartCoroutine("DialogFadeOut", false);
                      
                    }
                }


                if (dialogEffectTime >= 3000 && dialogEffectTime < 6000)       //UI감추는 버튼을 3초동안 서서히 사라지게 만든다.
                {
                    if (uiHide)
                    {
                        StartCoroutine("UIShowAndHideButtonFadeOut");
                        UiShowAndHideBtn.interactable = false;
                    }
                }

            }


        }


        if (live2D)   //배경 라이브2D 혹은 일러스트
        {

            BackGround_img.color = Color.clear; //일러스트 배경 안보이게

            if (effect)
            {
                Live2DEffect.Play();      //연출효과 재생.
                Effectwatch.Start();      //시간측정.


                NotInteractable();      //연출중 UI의 상호작용을 멈춘다.(버튼들의 interactable ->false)

                effectTime = Effectwatch.ElapsedMilliseconds;

                if (effectTime >= Live2DEffect.length*1000)          //해당 연출효과만큼의 시간이 지나면
                {
                    Effectwatch.Stop();         
                    Live2DEffect.Pause();  //연출 종료
                    MemorialEffect_rawimage.color = Color.clear; //연출이미지 안보이게

                    Interactable();         //UI들 상호작용가능.  (버튼들의 interactable ->true)

                    effect = false;    //씬에 계속 남아있다면 연출이 한번만 되도록.  
                }
            }
          
        }
        else
        {
            BackGround_img.color = Color.white;  //일러스트 배경 보이게.

        }

        SoundCheck = AudioManager.Instance().soundCheck;  //볼륨을 on/off를 외부적으로 표현 여부

        if (SoundOptionShow)        //사운드 옵션창이 true라면 사운드 옵션창을 계속 갱신한다.
            ShowSoundOption();
    }


     public void NotInteractable()
    {
        DialogBtn.interactable = false;  
        ChangeBtn.interactable = false;  
        StageBtn.interactable = false;   

        UpUI.transform.Find("OptionButton").GetComponent<Button>().interactable = false;
        UpUI.transform.Find("MessageButton").GetComponent<Button>().interactable = false;
        UpUI.transform.Find("FriendButton").GetComponent<Button>().interactable = false;

        DownUI.transform.Find("Status").GetComponent<Button>().interactable = false;
        DownUI.transform.Find("Shop").GetComponent<Button>().interactable = false;
        DownUI.transform.Find("Favoravility").GetComponent<Button>().interactable = false;

    }


    private void Interactable()
    {
        DialogBtn.interactable = true;
        ChangeBtn.interactable = true;
        StageBtn.interactable = true;

        UpUI.transform.Find("OptionButton").GetComponent<Button>().interactable = true;
        UpUI.transform.Find("MessageButton").GetComponent<Button>().interactable = true;
        UpUI.transform.Find("FriendButton").GetComponent<Button>().interactable = true;

        DownUI.transform.Find("Status").GetComponent<Button>().interactable = true;
        DownUI.transform.Find("Shop").GetComponent<Button>().interactable = true;
        DownUI.transform.Find("Favoravility").GetComponent<Button>().interactable = true;

    }


    public void ShowStage()       //버튼이벤트
    {
        NotInteractable();
        UiShowAndHideBtn.interactable = false;


        if(Stage.exitStage)
        {
            StagePanel.GetComponent<Image>().sprite = EndPanel_img;  //스테이지를 깨고 왔다면 패널 이미지를 보스 스테이지 이미지로
            StagePanel.GetComponent<Image>().color = Color.white;   
            StagePanel.SetActive(true);

            StagePanel.transform.Find("Stage2").gameObject.SetActive(false);  //2번째 버튼을 비활성화
            StagePanel.transform.Find("Stage3").gameObject.SetActive(false);  //3번째 버튼을 비활성화

        }
        else
        {
            StagePanel.GetComponent<Image>().color = Color.white;
            StagePanel.SetActive(true);
        }
    
    }

    public void ExitStage()      //ShowStage와 함께하는 버튼이벤트
    {
        Interactable();
        UiShowAndHideBtn.interactable = true;

        StagePanel.GetComponent<Image>().color = Color.clear;
        StagePanel.SetActive(false);
    }


    
    public void ShowSoundOption()     //버튼이벤트
    {
        NotInteractable();
        UiShowAndHideBtn.interactable = false;



        OptionPanel.GetComponent<Image>().color = Color.white;
        OptionPanel.SetActive(true);
        SoundOptionShow = true;

        if (SoundCheck)
        {
            OptionPanel.transform.Find("OnButton").GetComponent<Image>().color = Color.white;
            OptionPanel.transform.Find("OffButton").GetComponent<Image>().color = Color.clear;
        }
        else
        {
            OptionPanel.transform.Find("OnButton").GetComponent<Image>().color = Color.clear;
            OptionPanel.transform.Find("OffButton").GetComponent<Image>().color = Color.white;
        }
           

    }

    public void ExitSoundOption()  //ShowSoundOption과 함께하는 버튼이벤트
    {
        Interactable();
        UiShowAndHideBtn.interactable = true;
        OptionPanel.GetComponent<Image>().color = Color.clear;
        OptionPanel.SetActive(false);
        SoundOptionShow = false;
    }


    public void SoundOnClick()        //ShowSoundOption창에서 on/off를 결정하는 이벤트
    {
        AudioManager.Instance().soundCheck = true;
    }

    public void SoundOffClick()      //ShowSoundOption창에서 on/off를 결정하는 이벤트
    {
        AudioManager.Instance().soundCheck = false;
    }



    /*////////////////////////코루틴//////////////////////////////////*/

    IEnumerator UIFadeIn()
    {

        //위쪽 UI 시작점 위치랑 끝점 위치
        Vector2 UpUI_startPos = UpUI.transform.position;
        Vector2 UpUI_TargetPos = new Vector2(UpUI_startPos.x - 250, UpUI_startPos.y);

        //아래쪽 UI 시작점 위치랑 끝점 위치
        Vector2 DownUI_starPos = DownUI.transform.position;
        Vector2 DownUI_TargetPos = new Vector2(DownUI_starPos.x + 480, DownUI_starPos.y);

        Vector2 UpUIPosition = UpUI.transform.position;

        Vector2 DownUIPosition = DownUI.transform.position;

        for (float i = UpUI_startPos.x; i > UpUI_TargetPos.x; i -= 1.0f)
        {
            UpUIPosition.x = i;
            UpUI.transform.position = UpUIPosition;

        }

        for (float i = DownUI_starPos.x; i <= DownUI_TargetPos.x; i += 1.0f)
        {
            DownUIPosition.x = i;
            DownUI.transform.position = DownUIPosition;
        }


        UpUI.GetComponent<Image>().color = Color.white;
        UpUI.transform.Find("Option").GetComponent<Image>().color = Color.white;


        DownUI.GetComponent<Image>().color = Color.white;
        DownUI.transform.Find("Status").GetComponent<Image>().color = Color.white;
        DownUI.transform.Find("Shop").GetComponent<Image>().color = Color.white;
        DownUI.transform.Find("Favoravility").GetComponent<Image>().color = Color.white;

        yield return null;
    }





 

    IEnumerator UIFadeOut()
    {
      

        //위쪽 UI 시작점 위치랑 끝점 위치
        Vector2 UpUI_startPos = UpUI.transform.position;
        Vector2 UpUI_TargetPos = new Vector2(UpUI_startPos.x + 250, UpUI_startPos.y);

        //아래쪽 UI 시작점 위치랑 끝점 위치
        Vector2 DownUI_starPos = DownUI.transform.position;
        Vector2 DownUI_TargetPos = new Vector2(DownUI_starPos.x - 480, DownUI_starPos.y);

        Vector2 UpUIPosition = UpUI.transform.position;

        Vector2 DownUIPosition = DownUI.transform.position;

        for(float i= UpUI_startPos.x;  i<UpUI_TargetPos.x; i+=0.1f)
        {
            UpUIPosition.x = i;
            UpUI.transform.position = UpUIPosition;

        }

        for(float i = DownUI_starPos.x; i >= DownUI_TargetPos.x; i -= 0.1f)
        {
            DownUIPosition.x = i;
            DownUI.transform.position = DownUIPosition;
        }

        UpUI.GetComponent<Image>().color = Color.clear;
        UpUI.transform.Find("Option").GetComponent<Image>().color = Color.clear;


        DownUI.GetComponent<Image>().color = Color.clear;
        DownUI.transform.Find("Status").GetComponent<Image>().color = Color.clear;
        DownUI.transform.Find("Shop").GetComponent<Image>().color = Color.clear;
        DownUI.transform.Find("Favoravility").GetComponent<Image>().color = Color.clear;


        yield return null;

    }


    IEnumerator DialogFadeIn(bool dialog_big)
    {

        Color dialogColor = speechBubble.GetComponent<Image>().color;         //대사 색깔 변수.

        if(Character == (int)CharacterNumber.IORI && dialog == 3 && dialog_big)
            dialogColor = speechBubble_Big.GetComponent<Image>().color;

        for (int i=0; i<= 100; i++)
        {
            dialogColor.r += Time.deltaTime * 0.01f;
            dialogColor.g += Time.deltaTime * 0.01f;
            dialogColor.b += Time.deltaTime * 0.01f;
            dialogColor.a += Time.deltaTime * 0.01f;
        }

       

        if(Character == (int)CharacterNumber.IORI && dialog == 3 && dialog_big)
            speechBubble_Big.GetComponent<Image>().color = dialogColor;
        else
            speechBubble.GetComponent<Image>().color = dialogColor;



        yield return null; //코루틴 종료
    }


    IEnumerator DialogFadeOut(bool dialog_big)
    {
        Color dialogColor = speechBubble.GetComponent<Image>().color; ;        //대사 색깔 변수.

        if (Character == (int)CharacterNumber.IORI && dialog == 3 && dialog_big)
            dialogColor = speechBubble_Big.GetComponent<Image>().color;

        for (int i=100; i>=0; i--)
        {
            dialogColor.r -= Time.deltaTime * 0.01f;
            dialogColor.g -= Time.deltaTime * 0.01f;
            dialogColor.b -= Time.deltaTime * 0.01f;
            dialogColor.a -= Time.deltaTime * 0.01f;
        }

        if (Character == (int)CharacterNumber.IORI && dialog == 3 && dialog_big)
            speechBubble_Big.GetComponent<Image>().color = dialogColor;
        else
            speechBubble.GetComponent<Image>().color = dialogColor;

        yield return null; //코루틴 종료
    }

    IEnumerator UIShowAndHideButtonFadeOut()
    {
        Color dialogColor = UiShowAndHideBtn.GetComponent<Image>().color; ;        //대사 색깔 변수.


        for (int i = 100; i >= 0; i--)
        {
            dialogColor.r -= Time.deltaTime * 0.01f;
            dialogColor.g -= Time.deltaTime * 0.01f;
            dialogColor.b -= Time.deltaTime * 0.01f;
            dialogColor.a -= Time.deltaTime * 0.01f;
        }


        UiShowAndHideBtn.GetComponent<Image>().color = dialogColor;

        yield return null; //코루틴 종료
    }


}
