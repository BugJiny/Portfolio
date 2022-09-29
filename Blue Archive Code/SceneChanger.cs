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
    private string SceneToLoad = "StatusScene";  //��������(����) �̵� �� ��
    private string SceneToLoad2 = "StageScene";  //��������(��������) �̵� �� ��
    private string SceneToLoad3 = "EndScene";  //��������(����) �̵� �� ��




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

        //������������ if(Stage.exitStage) ��� ���� ���������� �̵�.

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
