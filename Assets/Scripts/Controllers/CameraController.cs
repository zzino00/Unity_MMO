using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField]
     Define.CameraMode _mode = Define.CameraMode.QuaterView;

    [SerializeField]
     Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;
    
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }
    void Start()
    {
        
    }

   
    void LateUpdate()// 플레이어 update가 끝나고 update가 실행되어야 한다.
    {
        if(_mode == Define.CameraMode.QuaterView)
        {
            if (!_player.IsValid())
            {
                return;
            }
            RaycastHit hit;
            if(Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Block")))// 플레이어 위치에서 카메라를 향해 빛을 쏘는데 중간에 벽이 있을때만 반환
            {
                float dist = (hit.point - _player.transform.position+ new Vector3(0,2.5f,0)).magnitude*0.8f;// 플레이어부터 벽에 빛이 닿은 부분까지의 방향과 크기, 0.8을 곱함으로써 벽에서 살짝 떨어지게 된다.
                transform.position = _player.transform.position + _delta.normalized*dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;// _delta는 플레이어위치를 그대로 받으면 플레이어랑 겹치니까 플레이어가 잘보이는 위치
                transform.LookAt(_player.transform);// 바라보는 방향 설정
            }
        }
           
      
    }


    public void SetQuaterView(Vector3 delta) // 나중에 코드상에서 쿼터뷰를 설정하고 싶을때 이용가능
    {
        _mode = Define.CameraMode.QuaterView;
        _delta = delta;
    }
}
