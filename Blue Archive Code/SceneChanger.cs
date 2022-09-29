using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
   


    /*PUBLIC*/
    public Button OptionBtn;
    public Button MessageBtn;
    public Button FriendBtn;
    public Button StatusBtn;
    public Button ShopBtn;
    public Button FavoravilityBtn;
    public GameObject ErrorPanel;
    

    /*PRIVATE*/
    private string SceneToLoad = "StatusScene";  //다음으로(스탯) 이동 될 씬
    private string SceneToLoad2 = "StageScene";  //다음으로(스테이지) 이동 될 씬
    private string SceneToLoad3 = "EndScene";  //다음으로(보스) 이동 될 씬




    // Start is called before the first frame update
    void Start()
    {
        ErrorPanel.GetComponent<Image>().color = Color.clear;
        ErrorPanel.SetActive(false);
        ErrorPanel.transform.Find("ExitButton").GetComponent<Image>().color = Color.clear;
        ErrorPanel.transform.Find("ExitButton").GetComponent<Button>().interactable = false;

    }

  


    public void ShowErrorScreen()
    {
        ErrorPanel.GetComponent<Image>().color = Color.white;
        ErrorPanel.SetActive(true);
        ErrorPanel.transform.Find("ExitButton").GetComponent<Image>().color = Color.white;
        ErrorPanel.transform.Find("ExitButton").GetComponent<Button>().interactable = true;

        GameObject.Find("MainManager").GetComponent<Live2DChange>().UiShowAndHideBtn.interactable = false;
        GameObject.Find("MainManager").GetComponent<Live2DChange>().ChangeBtn.interactable = false;
        GameObject.Find("MainManager").GetComponent<Live2DChange>().StageBtn.interactable = false;
        OptionBtn.interactable = false;

    }

    public void ExitErrorScreen()
    {
        ErrorPanel.GetComponent<Image>().color = Color.clear;
        ErrorPanel.SetActive(false);
        ErrorPanel.transform.Find("ExitButton").GetComponent<Image>().color = Color.clear;
        ErrorPanel.transform.Find("ExitButton").GetComponent<Button>().interactable = false;

        GameObject.Find("MainManager").GetComponent<Live2DChange>().UiShowAndHideBtn.interactable = true;
        GameObject.Find("MainManager").GetComponent<Live2DChange>().ChangeBtn.interactable = true;
        GameObject.Find("MainManager").GetComponent<Live2DChange>().StageBtn.interactable = true;
        OptionBtn.interactable = true;
    }


    public void NextStageScene()
    {

        //스테이지에서 if(Stage.exitStage) 라면 보스 스테이지로 이동.

        if(Stage.exitStage)
        {
            SceneManager.LoadScene(SceneToLoad3);
        }
        else
        {
            SceneManager.LoadScene(SceneToLoad2);
        }
      
    }
    public void NextStatusScene()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

}
