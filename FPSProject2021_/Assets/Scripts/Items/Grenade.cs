using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    // 이동관련
    public Vector3 _velocity;
    public Rigidbody _rigid;

    // 최대 시간은 조정할 수 있게 해줌
    private float _currentTime;
    [SerializeField] private float _maxTime;

    // 폭발 범위를 설정
    [SerializeField] private float _boomRadius = 2.0f;
    // 폭발의 힘
    [SerializeField] private float _force = 15.0f;

    [SerializeField] private GameObject _particle = null;

    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
        if(_rigid == null) {
            _rigid = gameObject.AddComponent<Rigidbody>();
		}

        _currentTime = 0.0f;
    }

	private void FixedUpdate()
	{
		
	}

	void Update()
    {
        _currentTime += Time.deltaTime;
        // 일정시간이 지나면 터진다.
        if(_currentTime >= _maxTime) {
            // 일단 귀찮아서 응급조치
            _currentTime = 0.0f;
            _maxTime = float.MaxValue;
            Boom();
            StartCoroutine(BoomParticle());
        }
    }

    IEnumerator BoomParticle()
	{
        GameObject particle = Managers.Resource.NewPrefab("Particle/BoomParticle");
        particle.transform.position = transform.position;
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        particle.SetActive(false);
        Managers.Resource.DelPrefab(particle);
        Managers.Resource.DelPrefab(gameObject);
    }

    void Boom()
	{
        Collider[] colliders = Physics.OverlapSphere(transform.position, _boomRadius);

        // 플레이어에게 대미지와 폭발 위력까지 게산을 해준다.
        foreach(Collider collider in colliders) {
            // 대미지를 넣을 것인지 판별해준다.

            PlayerController player = collider.GetComponent<PlayerController>();
            if (player == null) {
                continue;
			}

            player._hp -= 1;

            // 현재 폭탄과의 거리를 구하고 폭탄과의 방향벡터를 구한다.
            Vector3 velocity = transform.position - player.transform.position;
            float power = velocity.magnitude;
            velocity.Normalize();
            velocity = new Vector3(velocity.x, 0.0f, velocity.z);

            velocity = velocity * power * 3.0f;
            player.transform.position -= velocity;
		}
	}

	#region 이동
	public void Velocity(Transform shut)
    {
        _rigid = GetComponent<Rigidbody>();
        _rigid.velocity = shut.up * 10.0f + shut.forward * 3.0f;
    }
	#endregion

	private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _boomRadius);
	}

}
