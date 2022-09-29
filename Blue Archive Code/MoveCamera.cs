using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{

    public GameObject target;
    



    private float CameraNowX;
    private float CameraY;
    private float CameraZ;
    
    // Start is called before the first frame update

    void Start()
    {
        CameraNowX = this.transform.position.x;
        CameraY = this.transform.position.y;
        CameraZ = this.transform.position.z;
    }

    // Update is called once per frame



    private void Update()
    {
        if(!CharacterMove.fight)  //캐릭터가 전투중이 아닐때
            Move();

      
    }
  
    private void Move()  //카메라 이동
    {
        Vector3 TargetPos = new Vector3(target.transform.position.x+6.0f, CameraY, CameraZ);
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * 5f);
    }
}
