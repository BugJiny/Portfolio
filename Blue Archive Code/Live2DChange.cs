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



    [Header("���̽�")]
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
    private bool live2D = false;      //���̺�2D �����
    private bool effect = true;       //���̺�2D ����ȿ�� �����1
    private float effectTime;         //����ȿ�� �ð� ����.  
    private Stopwatch Effectwatch;    //����ȿ�� �ð� ����.
    private bool illust = true;       //�Ϸ���Ʈ �����

 

    private Sprite[] Shiroko_dialogSource = new Sprite[5];  //�÷��� ��� ����
    private AudioClip[] Shiroko_Voice = new AudioClip[5];   //�÷��� ���� ����.

    private Sprite[] Hasumi_dialogSource = new Sprite[6];   //�Ͻ��� ��� ����
    private AudioClip[] Hasumi_Voice = new AudioClip[5];    //�Ͻ��� ���� ����

    private Sprite[] Iori_dialogSource = new Sprite[7];    //�̿��� ��� ����
    private AudioClip[] Iori_Voice = new AudioClip[5];      //�̿��� ���� ����


    private Sprite[] Character_BackGround = new Sprite[3];   //ĳ���� ���ȭ�� ����
    private VideoClip[] Character_Live2D = new VideoClip[3];  //ĳ���� ���̺� 2D ����
    private VideoClip[] Character_Effect = new VideoClip[3];  //ĳ���� ���̺� 2D ����ȿ�� ����


    
    private Sprite EndPanel_img;  //����




    private int dialog;               //��� ����
    private float dialogEffectTime;         //��� �ð� ����.
    private bool dialogEffect = false;        //��� ���� ����
    private Stopwatch dialogWatch;        //��� ���� �ð�����.
    private float voicelen;            //��������.
     
   



    private bool uiHide = false;   //UI ���̵� �� ���̵� �ƿ� ���� ����.


    private bool SoundCheck = true;  //���� ��ư ���̴� �뵵.
    private bool SoundOptionShow = false;  //���� â �����ִ¿뵵.


    enum CharacterNumber
    {
        SHIROKO = 0,
        HASUMI,
        IORI
    }



    private void Awake()
    {

        //�ε��Ų�� �޸𸮿� ������� ������ ����Ǹ� Resources.UnloadAsset ���� �ʱ�ȭ�� ������� �Ѵ�.

        //�÷��� ��ȭâ �ҽ� �ʱ�ȭ
        Shiroko_dialogSource[0] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg1");
        Shiroko_dialogSource[1] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg2");
        Shiroko_dialogSource[2] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg3");
        Shiroko_dialogSource[3] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg4");
        Shiroko_dialogSource[4] = Resources.Load<Sprite>("Main_Source/Shiroko/Shiroko_dlg5");

       
        //�÷��� ���� �ҽ� �ʱ�ȭ
        Shiroko_Voice[0] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice1");
        Shiroko_Voice[1] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice2");
        Shiroko_Voice[2] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice3");
        Shiroko_Voice[3] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice4");
        Shiroko_Voice[4] = Resources.Load<AudioClip>("Main_Source/Shiroko/Voice/ShirokoVoice5");

        //�Ͻ��� ��ȭâ �ҽ� �ʱ�ȭ
        Hasumi_dialogSource[0] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg1");
        Hasumi_dialogSource[1] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg2");
        Hasumi_dialogSource[2] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg3");
        Hasumi_dialogSource[3] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg4");
        Hasumi_dialogSource[4] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg5-1");
        Hasumi_dialogSource[5] = Resources.Load<Sprite>("Main_Source/Hasumi/Hasumi_dlg5-2");

        //�Ͻ��� ���� �ҽ� �ʱ�ȭ
        Hasumi_Voice[0] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice1");
        Hasumi_Voice[1] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice2");
        Hasumi_Voice[2] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice3");
        Hasumi_Voice[3] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice4");
        Hasumi_Voice[4] = Resources.Load<AudioClip>("Main_Source/Hasumi/Voice/HasumiVoice5");

        //�̿��� ��ȭâ �ҽ� �ʱ�ȭ
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

        speechBubble.GetComponent<Image>().color = Color.clear; // ��ȭâ �����ֱ�

        Character = Status.MainCharacter; //ĳ���� ��ȣ�� ������.  -> �������ͽ� â���� ����ĳ���ͷ� ������ ĳ���� ��ȣ��  ������.

        //�ش� ��ȣ�� �´� ���,Live2D, ����ȿ���� ����.
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

        //����� ��ȯ�� �� UI,�ٸ� ��ȣ�ۿ� ���߱�.
        speechBubble.GetComponent<Image>().color = Color.clear; //ȭ����ȯ�� ��� �Ⱥ��̰� �÷��� �ʱ�ȭ.
        speechBubble_Big.GetComponent<Image>().color = Color.clear; //ȭ����ȯ�� ��� �Ⱥ��̰� �÷��� �ʱ�ȭ.

        Voice.GetComponent<AudioSource>().Stop();    //ĳ���� ��Ҹ� ����.
        dialogWatch.Reset(); //��� �ð����� ����
        dialogWatch.Stop();  //��� �ð����� ��ž 
        dialogEffect = false;  //��� ��ȣ�ۿ� ���ϰ�
    }

    public void UIShowAndHide()
    {

        if(uiHide)        //UI�� ������������ ���̵������� �����ֱ�
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
        else             //UI�� ���̰� ������ ���̵� �ƿ����� ���߱�
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


    private void DialogSet(Sprite image, AudioClip audio)  //��� �����Լ�
    {
        speechBubble.GetComponent<Image>().sprite = image;
        Voice.GetComponent<AudioSource>().clip = audio;
    }

    private void DialogSet_Big(Sprite image1,Sprite image2 ,AudioClip audio)  //�̿��� Ư�����(3) �����Լ�.
    {
        speechBubble_Big.GetComponent<Image>().sprite = image1;
        speechBubble.GetComponent<Image>().sprite = image2;

        Voice.GetComponent<AudioSource>().clip = audio;
    }

    public void Change_Dialog()        //��� �ٲٱ� -> ��ư �̺�Ʈ �Լ�.
    {

        dialog = Random.Range(1, 6);  //��縦 �������� �̴´�.

        speechBubble_Big.GetComponent<Image>().color = Color.clear; //��ư�� ������ ���� ��� �÷����� �ʱ�ȭ.
        speechBubble.GetComponent<Image>().color = Color.clear; //��ư�� ������ ���� ��� �÷����� �ʱ�ȭ.



        if (!dialogEffect)       //��簡 ������ ���� ������ Ŭ����
        {
            dialogEffect = true;
            dialogWatch.Start();
        }
        else                   //��簡 ������ �ִ��� Ŭ���� �ٸ���簡 ���Ë�
        {
            dialogEffectTime = 0;
            dialogWatch.Restart();    //�ٽ� ������ �ض�.
        }
       

        switch (dialog)       //ĳ���Ϳ� ���� ��� ����.
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
                    DialogSet_Big(Iori_dialogSource[2], Iori_dialogSource[3], Iori_Voice[2]);  //��簡 �� 2,3 �迭�� ��縦 ���̾�  
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
                    DialogSet(Hasumi_dialogSource[4], Hasumi_Voice[4]);              //��簡 �� 4,5 �迭�� ��縦 ���̾�
                else if (Character == (int)CharacterNumber.IORI)   
                    DialogSet(Iori_dialogSource[5], Iori_Voice[4]);                 // ��簡 �� 5,6 �迭�� ��縦 ���̾�
                break;
        }

        voicelen = Voice.GetComponent<AudioSource>().clip.length * 1000;   //���̽�����
        Voice.GetComponent<AudioSource>().Play();    //ĳ���� ��Ҹ� ���.
     
    }

  


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (dialogEffect)  //��簡 Ʈ����.
        {
            dialogEffectTime = dialogWatch.ElapsedMilliseconds;


            if (dialogEffectTime > voicelen ) //���� ���̸�ŭ ��ȭâ ����.
            {
                dialogEffectTime = 0;
                dialogWatch.Reset();
                dialogWatch.Stop();
                dialogEffect = false;
            }
            else                            //�������̸�ŭ�� �ð��� ��������ʾҴٸ�
            {


                if (dialogEffectTime <= 3000)            //UI���ߴ� ��ư�� 3�ʵ��Ⱥ������
                {
                    UiShowAndHideBtn.GetComponent<Image>().color = Color.white;
                    UiShowAndHideBtn.interactable = true;
                }


                if (Character == (int)CharacterNumber.IORI && dialog ==3)         //��簡 Ư���ϰų� �� ��� �̿���(3)
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
                else if(Character==(int)CharacterNumber.IORI && dialog ==5)       //��簡 Ư���ϰų� �� ��� �̿���(5)
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
                else if(Character ==(int)CharacterNumber.HASUMI && dialog==5)        //��簡 Ư���ϰų� �� ��� �Ͻ���(5)
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
                else          //�� ���� ����� ���
                {

                    if (dialogEffectTime < voicelen / 2)       //���������� ������ �ð���ŭ ��縦 �����������ش�
                    {
                        UiShowAndHideBtn.GetComponent<Image>().color = Color.white;
                        UiShowAndHideBtn.interactable = true;
                        StartCoroutine("DialogFadeIn",false);
                    }
                    else if (dialogEffectTime >= voicelen / 2 && dialogEffectTime < voicelen)  //���������� ���ݺ��� ������ ��縦 ������ �������.
                    {
                        StartCoroutine("DialogFadeOut", false);
                      
                    }
                }


                if (dialogEffectTime >= 3000 && dialogEffectTime < 6000)       //UI���ߴ� ��ư�� 3�ʵ��� ������ ������� �����.
                {
                    if (uiHide)
                    {
                        StartCoroutine("UIShowAndHideButtonFadeOut");
                        UiShowAndHideBtn.interactable = false;
                    }
                }

            }


        }


        if (live2D)   //��� ���̺�2D Ȥ�� �Ϸ���Ʈ
        {

            BackGround_img.color = Color.clear; //�Ϸ���Ʈ ��� �Ⱥ��̰�

            if (effect)
            {
                Live2DEffect.Play();      //����ȿ�� ���.
                Effectwatch.Start();      //�ð�����.


                NotInteractable();      //������ UI�� ��ȣ�ۿ��� �����.(��ư���� interactable ->false)

                effectTime = Effectwatch.ElapsedMilliseconds;

                if (effectTime >= Live2DEffect.length*1000)          //�ش� ����ȿ����ŭ�� �ð��� ������
                {
                    Effectwatch.Stop();         
                    Live2DEffect.Pause();  //���� ����
                    MemorialEffect_rawimage.color = Color.clear; //�����̹��� �Ⱥ��̰�

                    Interactable();         //UI�� ��ȣ�ۿ밡��.  (��ư���� interactable ->true)

                    effect = false;    //���� ��� �����ִٸ� ������ �ѹ��� �ǵ���.  
                }
            }
          
        }
        else
        {
            BackGround_img.color = Color.white;  //�Ϸ���Ʈ ��� ���̰�.

        }

        SoundCheck = AudioManager.Instance().soundCheck;  //������ on/off�� �ܺ������� ǥ�� ����

        if (SoundOptionShow)        //���� �ɼ�â�� true��� ���� �ɼ�â�� ��� �����Ѵ�.
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


    public void ShowStage()       //��ư�̺�Ʈ
    {
        NotInteractable();
        UiShowAndHideBtn.interactable = false;


        if(Stage.exitStage)
        {
            StagePanel.GetComponent<Image>().sprite = EndPanel_img;  //���������� ���� �Դٸ� �г� �̹����� ���� �������� �̹�����
            StagePanel.GetComponent<Image>().color = Color.white;   
            StagePanel.SetActive(true);

            StagePanel.transform.Find("Stage2").gameObject.SetActive(false);  //2��° ��ư�� ��Ȱ��ȭ
            StagePanel.transform.Find("Stage3").gameObject.SetActive(false);  //3��° ��ư�� ��Ȱ��ȭ

        }
        else
        {
            StagePanel.GetComponent<Image>().color = Color.white;
            StagePanel.SetActive(true);
        }
    
    }

    public void ExitStage()      //ShowStage�� �Բ��ϴ� ��ư�̺�Ʈ
    {
        Interactable();
        UiShowAndHideBtn.interactable = true;

        StagePanel.GetComponent<Image>().color = Color.clear;
        StagePanel.SetActive(false);
    }


    
    public void ShowSoundOption()     //��ư�̺�Ʈ
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

    public void ExitSoundOption()  //ShowSoundOption�� �Բ��ϴ� ��ư�̺�Ʈ
    {
        Interactable();
        UiShowAndHideBtn.interactable = true;
        OptionPanel.GetComponent<Image>().color = Color.clear;
        OptionPanel.SetActive(false);
        SoundOptionShow = false;
    }


    public void SoundOnClick()        //ShowSoundOptionâ���� on/off�� �����ϴ� �̺�Ʈ
    {
        AudioManager.Instance().soundCheck = true;
    }

    public void SoundOffClick()      //ShowSoundOptionâ���� on/off�� �����ϴ� �̺�Ʈ
    {
        AudioManager.Instance().soundCheck = false;
    }



    /*////////////////////////�ڷ�ƾ//////////////////////////////////*/

    IEnumerator UIFadeIn()
    {

        //���� UI ������ ��ġ�� ���� ��ġ
        Vector2 UpUI_startPos = UpUI.transform.position;
        Vector2 UpUI_TargetPos = new Vector2(UpUI_startPos.x - 250, UpUI_startPos.y);

        //�Ʒ��� UI ������ ��ġ�� ���� ��ġ
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
      

        //���� UI ������ ��ġ�� ���� ��ġ
        Vector2 UpUI_startPos = UpUI.transform.position;
        Vector2 UpUI_TargetPos = new Vector2(UpUI_startPos.x + 250, UpUI_startPos.y);

        //�Ʒ��� UI ������ ��ġ�� ���� ��ġ
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

        Color dialogColor = speechBubble.GetComponent<Image>().color;         //��� ���� ����.

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



        yield return null; //�ڷ�ƾ ����
    }


    IEnumerator DialogFadeOut(bool dialog_big)
    {
        Color dialogColor = speechBubble.GetComponent<Image>().color; ;        //��� ���� ����.

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

        yield return null; //�ڷ�ƾ ����
    }

    IEnumerator UIShowAndHideButtonFadeOut()
    {
        Color dialogColor = UiShowAndHideBtn.GetComponent<Image>().color; ;        //��� ���� ����.


        for (int i = 100; i >= 0; i--)
        {
            dialogColor.r -= Time.deltaTime * 0.01f;
            dialogColor.g -= Time.deltaTime * 0.01f;
            dialogColor.b -= Time.deltaTime * 0.01f;
            dialogColor.a -= Time.deltaTime * 0.01f;
        }


        UiShowAndHideBtn.GetComponent<Image>().color = dialogColor;

        yield return null; //�ڷ�ƾ ����
    }


}
