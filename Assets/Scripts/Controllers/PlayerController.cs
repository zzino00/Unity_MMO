using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    PlayerStat _stat;
    Vector3 _destPos; // 마우스를 클릭한 지점을 저장하는 변수
    Texture2D _attackIcon;
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;
    void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon =Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        _stat = GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

    
    }


    void UpdateMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// 카메라에서 Mouse커서 위치로 빛을 쏨

        RaycastHit hit; // 부딪친 물체의 정보를 저장하는 변수
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))// 빛을 쏴서 부딪힌 물체를 반환함 근데 물체의 레이어가 Wall일때만 반환
        {

            if (hit.collider.gameObject.layer != (int)Define.Layer.Monster)
            {
              if(_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);// 1번째 인자는 사용할 이미지, 2번째는 이미지 안에서 클릭이 되는 좌표설정, 마지막은 일단 Auto로 설정
                    _cursorType = CursorType.Attack;
                }
               
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }


    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);// 매번 Layer를 스트링으로 받는게 번거로워서 비트연산자를 이용해서 해당 레이어를 저장?
    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// 카메라에서 Mouse커서 위치로 빛을 쏨
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// 눈으로 볼수 있게 디버깅용 빛을 쏨

        RaycastHit hit; // 부딪친 물체의 정보를 저장하는 변수
        if(Physics.Raycast(ray, out hit, 100.0f, _mask))// 빛을 쏴서 부딪힌 물체를 반환함 근데 물체의 레이어가 Wall일때만 반환
        {
            _destPos =hit.point;// 부딪힌 지점의 백터값 저장
            _state =PlayerState.Moving;

            if(hit.collider.gameObject.layer==(int)Define.Layer.Monster)
            {
                Debug.Log("Monster Clicked");
            }
            else
            {
                Debug.Log("Ground Clicked");
            }
        }
        
        
    }// 마우스로 조작

  
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    PlayerState _state = PlayerState.Idle;

    void UpdateDie()
    {

    }

    void UpdateMoving()
    {
     
        
            Vector3 dir = _destPos - transform.position;// 플레이어 위치에서 목적지까지의 방향벡터
            if (dir.magnitude < 0.1f)// 도착했을 때: float간의 계산에서는 0이 나오기 힘들기 때문에 그냥 엄청 작은숫자로 설정
            {
            _state = PlayerState.Idle;
            }
            else
            {
            float MoveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);// 움직이는 거리가 목적지까지의 거리를 넘어가면 안된다. clamp를 통해 움직이는 거리에 범위를 정해줌

            NavMeshAgent nma =  gameObject.GetOrAddComponent<NavMeshAgent>();// Util에 선언한 GetOrAddComponent함수로 NavMeshAgent컴포넌트를 저장

               nma.Move(dir.normalized * MoveDist);// 기존에는 아래에 주석처리한거 처럼 현재위치에 방향벡터를 더하는 방식을 이용했지만
            // NavMeshAgent에서 제공하는 Move함수는 위의 방식만큼 정밀하지 않다는 것을 알수 있음, 그래서 오차범위를 좀더 크게 잡아야함


            //  transform.position += dir.normalized * MoveDist;// 현재 위치에다가 방향벡터를 노멀라이즈한거(길이가 1이됨)* 속도);

            Debug.DrawRay(transform.position, dir.normalized, Color.green, 1.0f);
            if (Physics.Raycast(transform.position+Vector3.up*0.5f, dir, 1.0f, LayerMask.GetMask("Block")))// Laycast를 해서 앞에 장애물이 있으면 멈추게 설정
            {
                _state = PlayerState.Idle;
                return;
            }
                


            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// 이동하는 방향으로 캐릭터가 몸을 돌리게 할때 단순히 LookAt을 사용하면
                                                                                                                             // 뚝뚝 끊기게 된다. 이를 해결하기위해 Slerp를 사용
                                                                                                                             //애니메이션
           
            }


        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _stat.MoveSpeed);


    }

    void UpdateIdle()
    {

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0.0f);

    }
    void Update()
    {
        UpdateMouseCursor();

        switch(_state)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
        }
        
        

        
    }
}