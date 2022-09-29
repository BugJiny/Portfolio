using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //씬 이동을 위해 필요
using UnityEngine.Video; //비디오플레이를 사용하기 위해 선언
using UnityEngine.UI; //UI틀



public class Title : MonoBehaviour
{
    /*PUBLIC*/
    public VideoPlayer video; //비디오 제어
    public InputField ID;
    public InputField PW;
    public GameObject BlurPanel;
    public GameObject LoginPanel;
    public Button ExitBtn;   //로그인창 지우는 버튼
    public Button LoginBtn; //로그인 실행하는 버튼.

    public Text TTS;  //Touch To Start  약자.
  


    /*PRIVATE*/
    private string SceneToLoad = "MainScene";  //다음으로 이동 될 씬
    private string TestID = "Test00"; //테스트 아이디
    private string TestPW = "1234"; //테스트 비밀번호


    private void Awake()
    {
        ID.enabled = false; // 기능 정지.
        ID.placeholder.GetComponent<Text>().text = ""; //이름을 지워준다.
        ID.image.color = Color.clear; //배경색을 지워준다.

        PW.enabled = false; //기능 정지
        PW.placeholder.GetComponent<Text>().text = ""; //이름을 지워준다.
        PW.image.color = Color.clear; //배경색을 지워준다.

        BlurPanel.GetComponent<Image>().color = Color.clear; //블러판넬이 보이지 않게.

        LoginPanel.GetComponent<Image>().color = Color.clear; //로그인 정보 판넬이 보이지 않게.
        LoginPanel.transform.Find("Logo").GetComponent<Image>().color = Color.clear; //로그인 패널에 로고가 보이게.

        ExitBtn.GetComponent<Image>().color = Color.clear;  //뒤로가기 버튼 안보이게
        ExitBtn.interactable = false; //버튼 상호작용 못하게.

        LoginBtn.GetComponent<Image>().color = Color.clear; //로그인 실행 버튼이 보이지 않게
        LoginBtn.interactable = false;  //버튼 상호작용 못하게.
        


        TTS.color = Color.white; //Touch To Start문구가 보이게 흰색으로 
        
    }


    public void LoginInfo()
    {
        //화면터치시 로그인정보가 나오면서 영상이 멈추게한다.


        /*로그인 UI창이 나온다.*/
        ID.enabled = true; // 기능 가동.
        ID.placeholder.GetComponent<Text>().text = "User ID"; 
        ID.image.color = Color.white; //배경색을 흰색

        PW.enabled = true; //기능 정지
        PW.placeholder.GetComponent<Text>().text = "User PW"; 
        PW.image.color = Color.white; //배경색을 흰색


        BlurPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 150/256f); //블러패널 보이도록 ->배경을 어둡게 해주기(블러효과 느낌나게)

        LoginPanel.GetComponent<Image>().color = new Color(0f,0f,0f, 120/256f);  //로그인 패널 색상값.
        LoginPanel.transform.Find("Logo").GetComponent<Image>().color = Color.white; //로그인 패널에 로고가 보이게.

        ExitBtn.GetComponent<Image>().color = Color.white;  //뒤로가기 버튼 보이도록
        ExitBtn.interactable = true; //버튼 상호작용 할 수 있게.


        LoginBtn.GetComponent<Image>().color = Color.white; //로그인 실행 버튼이 보이지 않게
        LoginBtn.interactable = true;  //버튼 상호작용 못하게.
        

        TTS.color = Color.clear; //Touch To Start 문구 안보이게

        video.Pause(); //비디오 일시정지.

    }

    public void ExitLoginInfo()
    {
        ID.enabled = false; // 기능 가동.
        ID.placeholder.GetComponent<Text>().text = "";
        ID.text = "";
        ID.image.color = Color.clear; //배경색을 흰색

        PW.enabled = false; //기능 정지
        PW.placeholder.GetComponent<Text>().text = "";
        PW.text = "";
        PW.image.color = Color.clear; //배경색을 흰색


        BlurPanel.GetComponent<Image>().color = Color.clear;

        LoginPanel.GetComponent<Image>().color = Color.clear;
        LoginPanel.transform.Find("Logo").GetComponent<Image>().color = Color.clear; 

        ExitBtn.GetComponent<Image>().color = Color.clear;  //뒤로가기 버튼 보이도록
        ExitBtn.interactable = false; //버튼 상호작용 할 수 있게.


        LoginBtn.GetComponent<Image>().color = Color.clear; //로그인 실행 버튼이 보이지 않게
        LoginBtn.interactable = false;  //버튼 상호작용 못하게.
        

        TTS.color = Color.white; //Touch To Start 문구 안보이게


        video.Play(); //비디오 일시정지.


    }


    public void Login()
    {


        if(string.Compare(ID.text,TestID,false)==0 && string.Compare(PW.text,TestPW,false)==0)
        {

            Debug.Log("로그인 성공!");
            LoadGame();


        }
        else
        {
            Debug.Log("아이디나 비밀번호가 틀렸습니다.");

            ID.placeholder.GetComponent<Text>().text = "";
            ID.text = "";
            PW.placeholder.GetComponent<Text>().text = "";
            PW.text = "";

        }


    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
    public void LoadGame()
    {
        SceneManager.LoadScene(SceneToLoad); 
    }
    // Start is called before the first frame update

    // Update is called once per frame
   

}
