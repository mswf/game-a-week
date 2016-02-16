using System;
using UnityEngine;
using System.Collections;

namespace Week02
{

	public class PlayerVisualiser : MonoBehaviour
	{
		public Transform playerControllerTransform;
		public PlayerController playerController;

		private void LateUpdate()
		{
			transform.localPosition = playerControllerTransform.localPosition - Vector3.up*0.5f;

			switch (playerController.currentStance)
			{
				case PlayerStance.Prone:
					transform.localScale = new Vector3(1f, 0.25f, 1f);
					break;
				case PlayerStance.Crouched:
					transform.localScale = new Vector3(1f, 0.5f, 1f);

					break;
				case PlayerStance.Standing:
					transform.localScale = new Vector3(1f, 1f, 1f);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		// Use this for initialization
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{
		}
	}
}