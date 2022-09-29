using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Status : MonoBehaviour
{
    /*STATIC*/
    public static int MainCharacter = 0;
    public static bool ChangeBGM = false;

    /*PUBLIC*/
    public Button ExitButton;
    public Image Character_img;
    public GameObject SelectPanel;
    public GameObject Notice;
    public Canvas StatusCanvas;
    



    /*PRIVATE*/
    private Sprite[] Character_BackGround = new Sprite[3];  
    private string SceneToLoad = "MainScene";  //다음으로 이동 될 씬    
    private int count;
    private Vector3 vector;

    enum CharacterNumber
    {
        SHIROKO = 0,
        HASUMI,
        IORI
    }






    // Start is called before the first frame update
    void Start()
    {
        Character_BackGround[0] = Resources.Load<Sprite>("Status_Source/Shiroko_Status");
        Character_BackGround[1] = Resources.Load<Sprite>("Status_Source/Hasumi_Status");
        Character_BackGround[2] = Resources.Load<Sprite>("Status_Source/Iori_Status");

        Notice.SetActive(false);
        Notice.transform.Find("ExitButton").GetComponent<Image>().color = Color.clear;
        Notice.transform.Find("ExitButton").GetComponent<Button>().interactable = false;


        ChangeBGM = false;
    }

    // Update is called once per frame
    void Update()
    {
        StatusBackGroundSet(count);  //백그라운드를 셋팅을 보여줌.

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void StatusRightButtonDown()
    {
        count++;
        if (count >= 3)
            count = 0;
    }

    public void StatusLeftButtonDown()
    {
        count--;
        if (count <= -1)
            count = 2;
    }


    private void StatusBackGroundSet(int count)
    {

        vector = ExitButton.transform.localPosition;

        switch ((CharacterNumber)count)
        {
            case CharacterNumber.SHIROKO:
                Character_img.sprite = Character_BackGround[count];
                break;
            case CharacterNumber.HASUMI:
                Character_img.sprite = Character_BackGround[count];
                break;
            case CharacterNumber.IORI:
                Character_img.sprite = Character_BackGround[count];
                break;

        
        }

        if(count ==1 || count==2 )
        {
            vector.x = -523;
            vector.y = 317;
            ExitButton.transform.localPosition = vector;
        }
        else
        {
            vector.x = -585;
            vector.y = 328;
            ExitButton.transform.localPosition = vector;
        }

    }

    public void SelectShiroko()
    {
        count = 0;
        SelectPanel.GetComponent<Image>().color = Color.clear;
      
        SelectPanel.SetActive(false);
    }

    public void SelectHasumi()
    {
        count = 1;
        SelectPanel.GetComponent<Image>().color = Color.clear;
  
        SelectPanel.SetActive(false);
    }

    public void SelectIori()
    {
        count = 2;
        SelectPanel.GetComponent<Image>().color = Color.clear;
    
        SelectPanel.SetActive(false);
    }



    public void MainCharacterSet()
    {
        Notice.SetActive(true);
        Notice.GetComponent<Image>().color = Color.white;
        Notice.transform.Find("ExitButton").GetComponent<Image>().color = Color.white;
        Notice.transform.Find("ExitButton").GetComponent<Button>().interactable = true;

        StatusCanvas.transform.Find("StatusPanel").transform.Find("ExitButton").GetComponent<Button>().interactable = false;
        StatusCanvas.transform.Find("RightButton").GetComponent<Button>().interactable = false;
        StatusCanvas.transform.Find("LeftButton").GetComponent<Button>().interactable = false;

        MainCharacter = count;
        ChangeBGM = true;

    }

    public void ExitNotice()
    {
        Notice.SetActive(false);
        Notice.GetComponent<Image>().color = Color.clear;
        Notice.transform.Find("ExitButton").GetComponent<Image>().color = Color.clear;
        Notice.transform.Find("ExitButton").GetComponent<Button>().interactable = false;

        StatusCanvas.transform.Find("StatusPanel").transform.Find("ExitButton").GetComponent<Button>().interactable = true;
        StatusCanvas.transform.Find("RightButton").GetComponent<Button>().interactable = true;
        StatusCanvas.transform.Find("LeftButton").GetComponent<Button>().interactable = true;
    }

    public void NextMainScene()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
