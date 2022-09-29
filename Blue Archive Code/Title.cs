using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //�� �̵��� ���� �ʿ�
using UnityEngine.Video; //�����÷��̸� ����ϱ� ���� ����
using UnityEngine.UI; //UIƲ



public class Title : MonoBehaviour
{
    /*PUBLIC*/
    public VideoPlayer video; //���� ����
    public InputField ID;
    public InputField PW;
    public GameObject BlurPanel;
    public GameObject LoginPanel;
    public Button ExitBtn;   //�α���â ����� ��ư
    public Button LoginBtn; //�α��� �����ϴ� ��ư.

    public Text TTS;  //Touch To Start  ����.
  


    /*PRIVATE*/
    private string SceneToLoad = "MainScene";  //�������� �̵� �� ��
    private string TestID = "Test00"; //�׽�Ʈ ���̵�
    private string TestPW = "1234"; //�׽�Ʈ ��й�ȣ


    private void Awake()
    {
        ID.enabled = false; // ��� ����.
        ID.placeholder.GetComponent<Text>().text = ""; //�̸��� �����ش�.
        ID.image.color = Color.clear; //������ �����ش�.

        PW.enabled = false; //��� ����
        PW.placeholder.GetComponent<Text>().text = ""; //�̸��� �����ش�.
        PW.image.color = Color.clear; //������ �����ش�.

        BlurPanel.GetComponent<Image>().color = Color.clear; //���ǳ��� ������ �ʰ�.

        LoginPanel.GetComponent<Image>().color = Color.clear; //�α��� ���� �ǳ��� ������ �ʰ�.
        LoginPanel.transform.Find("Logo").GetComponent<Image>().color = Color.clear; //�α��� �гο� �ΰ� ���̰�.

        ExitBtn.GetComponent<Image>().color = Color.clear;  //�ڷΰ��� ��ư �Ⱥ��̰�
        ExitBtn.interactable = false; //��ư ��ȣ�ۿ� ���ϰ�.

        LoginBtn.GetComponent<Image>().color = Color.clear; //�α��� ���� ��ư�� ������ �ʰ�
        LoginBtn.interactable = false;  //��ư ��ȣ�ۿ� ���ϰ�.
        


        TTS.color = Color.white; //Touch To Start������ ���̰� ������� 
        
    }


    public void LoginInfo()
    {
        //ȭ����ġ�� �α��������� �����鼭 ������ ���߰��Ѵ�.


        /*�α��� UIâ�� ���´�.*/
        ID.enabled = true; // ��� ����.
        ID.placeholder.GetComponent<Text>().text = "User ID"; 
        ID.image.color = Color.white; //������ ���

        PW.enabled = true; //��� ����
        PW.placeholder.GetComponent<Text>().text = "User PW"; 
        PW.image.color = Color.white; //������ ���


        BlurPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 150/256f); //���г� ���̵��� ->����� ��Ӱ� ���ֱ�(��ȿ�� ��������)

        LoginPanel.GetComponent<Image>().color = new Color(0f,0f,0f, 120/256f);  //�α��� �г� ����.
        LoginPanel.transform.Find("Logo").GetComponent<Image>().color = Color.white; //�α��� �гο� �ΰ� ���̰�.

        ExitBtn.GetComponent<Image>().color = Color.white;  //�ڷΰ��� ��ư ���̵���
        ExitBtn.interactable = true; //��ư ��ȣ�ۿ� �� �� �ְ�.


        LoginBtn.GetComponent<Image>().color = Color.white; //�α��� ���� ��ư�� ������ �ʰ�
        LoginBtn.interactable = true;  //��ư ��ȣ�ۿ� ���ϰ�.
        

        TTS.color = Color.clear; //Touch To Start ���� �Ⱥ��̰�

        video.Pause(); //���� �Ͻ�����.

    }

    public void ExitLoginInfo()
    {
        ID.enabled = false; // ��� ����.
        ID.placeholder.GetComponent<Text>().text = "";
        ID.text = "";
        ID.image.color = Color.clear; //������ ���

        PW.enabled = false; //��� ����
        PW.placeholder.GetComponent<Text>().text = "";
        PW.text = "";
        PW.image.color = Color.clear; //������ ���


        BlurPanel.GetComponent<Image>().color = Color.clear;

        LoginPanel.GetComponent<Image>().color = Color.clear;
        LoginPanel.transform.Find("Logo").GetComponent<Image>().color = Color.clear; 

        ExitBtn.GetComponent<Image>().color = Color.clear;  //�ڷΰ��� ��ư ���̵���
        ExitBtn.interactable = false; //��ư ��ȣ�ۿ� �� �� �ְ�.


        LoginBtn.GetComponent<Image>().color = Color.clear; //�α��� ���� ��ư�� ������ �ʰ�
        LoginBtn.interactable = false;  //��ư ��ȣ�ۿ� ���ϰ�.
        

        TTS.color = Color.white; //Touch To Start ���� �Ⱥ��̰�


        video.Play(); //���� �Ͻ�����.


    }


    public void Login()
    {


        if(string.Compare(ID.text,TestID,false)==0 && string.Compare(PW.text,TestPW,false)==0)
        {

            Debug.Log("�α��� ����!");
            LoadGame();


        }
        else
        {
            Debug.Log("���̵� ��й�ȣ�� Ʋ�Ƚ��ϴ�.");

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
