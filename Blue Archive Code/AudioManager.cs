using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance = null;
    public AudioClip[] Character_BGM = new AudioClip[2]; //2���� ����� �ҽ��� ����.
    public bool soundCheck = true;
    private int character;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        

    }


    public static AudioManager Instance()
    {
        return instance;
    }

    public void MusicSet(int Character)
    {
        character = Character;

        if (Character == 0)
        {
            this.GetComponent<AudioSource>().clip = Character_BGM[0];
            this.GetComponent<AudioSource>().volume = 0.6f; //�÷��� bgm�� ����.

        }
        else
        {
            this.GetComponent<AudioSource>().clip = Character_BGM[1];
            this.GetComponent<AudioSource>().volume = 1f; //�̿���, �Ͻ��̴� ���󺼷�����.
        }

        this.GetComponent<AudioSource>().Play();  //���� ĳ���� �뷡 Ʋ����

    }

    public void MusicReStart()
    {
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().Play();  
        //�����

    }



    private void Start()
    {
        this.GetComponent<AudioSource>().clip = Character_BGM[0];
        this.GetComponent<AudioSource>().Play();  //���� ĳ���� �뷡 Ʋ����
    }

    private void Update()
    {

        if(soundCheck)
        {
            if (character == 0)
                this.GetComponent<AudioSource>().volume = 0.6f; //�÷��� bgm�� ����.
            else
                this.GetComponent<AudioSource>().volume = 1f; //�̿���, �Ͻ��̴� ���󺼷�����.
        }
        else
        {
            this.GetComponent<AudioSource>().volume = 0f; 
        }
    }


}
