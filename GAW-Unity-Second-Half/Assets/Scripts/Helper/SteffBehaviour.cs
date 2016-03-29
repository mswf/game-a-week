using UnityEngine;
using System.Collections;


public class SteffBehaviour : MonoBehaviour 
{
	[HideInInspector]
	public Transform _transform;

	// Use this for early referencing
	protected virtual void Awake()
	{
		_transform = GetComponent<Transform>();
	}

}
