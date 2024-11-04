using UnityEngine;
using UnityEngine.InputSystem;

public class Projectile : MonoBehaviour
{
    private Actor _source = null;
    private Actor _target = null;
    private RocketGrab _rocketGrab = null;

    private Vector3 _startPosition = Vector3.zero;
    private Vector3 _targetDirection = Vector3.zero;
    private float _speed = 30f;
    private int _range = 0;

    private bool isCatchTarget = false;
    private float _elapsedTime = 0f;
    private float maxForwardTime = 0f;

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        SetDirection();
    }

    private void OnDisable()
    {
        ResetPosition();
    }

    private void Update()
    {
        Work();
    }

    #region Functions
    public void Settings(Actor source, RocketGrab rocketGrab, int range)
    {
        _source = source;
        _rocketGrab = rocketGrab;
        _range = range;
    }

    private void Init()
    {
        _startPosition = transform.localPosition;

        maxForwardTime = _range / _speed;
    }

    private void SetDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 mouseDirection = new Vector3(hitInfo.point.x, 1f, hitInfo.point.z);
            _targetDirection = (mouseDirection - new Vector3(_source.transform.position.x, 1f, _source.transform.position.z)).normalized;
            transform.rotation = Quaternion.LookRotation(_targetDirection);
        }
        else
        {
            _targetDirection = _source.transform.forward;
            transform.rotation = Quaternion.LookRotation(_targetDirection);
        }
    }

    private void ResetPosition()
    {
        transform.localPosition = _startPosition;
    }

    private void Work()
    {
        if (!isCatchTarget)
        {
            _elapsedTime += Time.deltaTime;
            transform.position += _targetDirection * _speed * Time.deltaTime;

            if (_elapsedTime >= maxForwardTime)
            {
                _elapsedTime = 0f;
                gameObject.SetActive(false);
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _startPosition, _speed * Time.deltaTime);

            // 추후 다른 그랩류 등의 스킬이 겹칠 경우 처리 필요
            if (_target != null)
                _target.transform.position = Vector3.MoveTowards(_target.transform.position, _source.transform.position, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.localPosition, _startPosition) < 0.1f)
            {
                isCatchTarget = false;
                _elapsedTime = 0f;
                gameObject.SetActive(false);
            }
        }
    }

    private void SetCondition()
    {
        isCatchTarget = true;
        _elapsedTime = 0f;
    }
    #endregion

    private void OnTriggerEnter(Collider col)
    {
        if (isCatchTarget)
            return;

        if (col.CompareTag("Player") && col.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            SetCondition();
            _target = col.gameObject.GetComponent<Actor>();
            _rocketGrab.OnRocketGrabSuccess(_source, _target);
        }
        else if (col.CompareTag("Monster")) // 추후 actor를 제외한 다른 object 처리
        {
            SetCondition();
            _target = null;
        }
    }
}