using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance = null;
    public AudioClip[] Character_BGM = new AudioClip[2]; //2개의 오디오 소스를 받음.
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
            this.GetComponent<AudioSource>().volume = 0.6f; //시로코 bgm만 적용.

        }
        else
        {
            this.GetComponent<AudioSource>().clip = Character_BGM[1];
            this.GetComponent<AudioSource>().volume = 1f; //이오리, 하스미는 정상볼륨적용.
        }

        this.GetComponent<AudioSource>().Play();  //현재 캐릭터 노래 틀어줌

    }

    public void MusicReStart()
    {
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().Play();  
        //재시작

    }



    private void Start()
    {
        this.GetComponent<AudioSource>().clip = Character_BGM[0];
        this.GetComponent<AudioSource>().Play();  //현재 캐릭터 노래 틀어줌
    }

    private void Update()
    {

        if(soundCheck)
        {
            if (character == 0)
                this.GetComponent<AudioSource>().volume = 0.6f; //시로코 bgm만 적용.
            else
                this.GetComponent<AudioSource>().volume = 1f; //이오리, 하스미는 정상볼륨적용.
        }
        else
        {
            this.GetComponent<AudioSource>().volume = 0f; 
        }
    }


}
