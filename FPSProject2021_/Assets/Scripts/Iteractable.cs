using UnityEngine;

public class Iteractable : MonoBehaviour
{
    public float _radius = 1.0f;

    public bool _isFocus = false;
	public bool _isHasInteract = false;

	public Transform _player = null;

	private void Update()
	{
		// 상호작용이 한번이라도 작동이 안됬으면
		if (_isFocus == true && _isHasInteract == false) {
			float distance = Vector3.Distance(_player.position, transform.position);

			if(distance <= _radius) {
				OnInteract();
				_isHasInteract = true;
			}
		}
	}

	public virtual void OnInteract() 
	{
		// This method is meant to be overwritten

	}

	public void SetFoused(Transform player)
	{
		_player = player;
		_isFocus = true;
		_isHasInteract = false;
	}

	public void DeFocused()
	{
		_player = null;
		_isFocus = false;
		_isHasInteract = false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _radius);
	}

}
