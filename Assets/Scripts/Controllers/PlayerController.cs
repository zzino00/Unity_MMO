using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);// 매번 Layer를 스트링으로 받는게 번거로워서 비트연산자를 이용해서 해당 레이어를 저장?

    PlayerStat _stat;
    Vector3 _destPos; // 마우스를 클릭한 지점을 저장하는 변수

    [SerializeField]
    PlayerState _state = PlayerState.Idle;

  
    GameObject _lockTarget;

    void Start()
    {

        _stat = GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);// UI_HPBar를 생성한다음 플레이어한테 붙임
    }

    public PlayerState State// 자동으로 플레이어 상태에 맞는 애니메이션을 실행할수 있도록
    {
        get { return _state; }
        set {
            _state = value;
        Animator anim = GetComponent<Animator>();
            switch(_state)
            {
                case PlayerState.Die:
                    break;
                case PlayerState.Idle:
                    anim.CrossFade("WAIT", 0.1f);// 다른애니메이션으로 넘어갈때 0.1초의 블랜딩을 섞음
                    break;
                case PlayerState.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case PlayerState.Skill:
                    anim.CrossFade("ATTACK", 0.1f,-1,0);// 마지막 매개변수 normalizedTimeOffset을 0으로 설정해주면 처음으로 돌아가 다시실행됨
                                                        // 루프를 설정안해도 반복된다는 뜻
                    break;
            }
        }
    }
    void UpdateDie()
    {

    }

    void UpdateMoving()
    {
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude; // 플레이어와 몬스터까지의 거리
            if (distance <= 1)
            {
                State = PlayerState.Skill;
                Debug.Log("Attack");
                return;
            }
        }

        Vector3 dir = _destPos - transform.position;// 플레이어 위치에서 목적지까지의 방향벡터
        if (dir.magnitude < 0.1f)// 도착했을 때: float간의 계산에서는 0이 나오기 힘들기 때문에 그냥 엄청 작은숫자로 설정
        {
            State = PlayerState.Idle;
        }
        else
        {
            float MoveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);// 움직이는 거리가 목적지까지의 거리를 넘어가면 안된다. clamp를 통해 움직이는 거리에 범위를 정해줌

            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();// Util에 선언한 GetOrAddComponent함수로 NavMeshAgent컴포넌트를 저장

            nma.Move(dir.normalized * MoveDist);// 기존에는 아래에 주석처리한거 처럼 현재위치에 방향벡터를 더하는 방식을 이용했지만
                                                // NavMeshAgent에서 제공하는 Move함수는 위의 방식만큼 정밀하지 않다는 것을 알수 있음, 그래서 오차범위를 좀더 크게 잡아야함


            //  transform.position += dir.normalized * MoveDist;// 현재 위치에다가 방향벡터를 노멀라이즈한거(길이가 1이됨)* 속도);

            Debug.DrawRay(transform.position, dir.normalized, Color.green, 1.0f);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))// Laycast를 해서 앞에 장애물이 있으면 멈추게 설정
            {
                if (Input.GetMouseButton(0) == false) // 마우스를 클릭하고 있으면 Idle로 안변함
                    State = PlayerState.Idle;

                return;
            }



            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// 이동하는 방향으로 캐릭터가 몸을 돌리게 할때 단순히 LookAt을 사용하면
                                                                                                                         // 뚝뚝 끊기게 된다. 이를 해결하기위해 Slerp를 사용
                                                                                                                         //애니메이션

        }

    }

    void UpdateIdle()
    {

    }

    void UpdateSkill()
    {
       if(_lockTarget !=null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;// 공격해야하는 벡터=> 타겟위치에서 내위치 빼기
            Quaternion quat = Quaternion.LookRotation(dir);//LookRotation함수를 통해 quat으로 저장
            transform.rotation =Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);// Lerp함수를 통해 부드럽게 움직이게
        }
    }

    void OnHitEvent()
    {
        if(_lockTarget!=null)
        {
          Stat targetStat = _lockTarget.GetComponent<Stat>();
          Stat myStat = gameObject.GetComponent<PlayerStat>();
          int damage =Mathf.Max(0, myStat.Attack - targetStat.Defense); // 내 공격력 - 타겟의 방어력 => 데미지, 근데 음수로 안떨어지게 최소값 0으로설정
          Debug.Log(damage);
            targetStat.HP -= damage;
            
        }


        Debug.Log("OnHitEvent");
        if(_stopSkill)// 한번누르면 1번 공격 
        {
            State = PlayerState.Idle;
        }
        else// 꾹누르면 연속공격
        {
            State = PlayerState.Skill;
          //  _stopSkill = false;
        }
    }
    void Update()
    {
        switch (State)
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
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }
    }

    bool _stopSkill = false;

    void OnMouseEvent(Define.MouseEvent evt)
    {
       switch(State)
       {
            case PlayerState.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill://이건 공격한 생태에서 다시 공격했을때 1번 공격을 설정하기 위해서
                {
                    if (evt == Define.MouseEvent.PointerUp)// 공격상태에서 마우스가 올라갔다면
                    {
                        _stopSkill = true;
                    }
                }
                break;
            
       }
       
        
        
    }// 마우스로 조작

  
  
  
    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        if (State == PlayerState.Die)
            return;

        RaycastHit hit; // 부딪친 물체의 정보를 저장하는 변수
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// 카메라에서 Mouse커서 위치로 빛을 쏨
        bool raycasthit = Physics.Raycast(ray, out hit, 100.0f, _mask);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// 눈으로 볼수 있게 디버깅용 빛을 쏨
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycasthit)
                    {
                        _destPos = hit.point;// 부딪힌 지점의 백터값 저장
                        State = PlayerState.Moving;
                        _stopSkill = false;// 클릭을 한 시점에는 일단 공격을 계속할지 멈출지 모르니까 false

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;

            case Define.MouseEvent.Press:
                {
                 if (_lockTarget==null &&raycasthit == true)
                        _destPos = hit.point;
                }
                break;

            case Define.MouseEvent PointerUp: // 이건 Idle이나 Run상태에서 공격을 실행했을때 1번공격하는 설정을 위해서
                {
                    _stopSkill = true;// 마우스가 올라갔으면(클릭이 풀렸으면) 일단 한번만 공격하는 거니까 true
                }
                break;
        }

    }


}