using UnityEngine;
using System.Collections;

namespace Week02
{

	public class PlayerVisualiser : MonoBehaviour
	{
		public Transform playerControllerTransform;
		public PlayerController playerController;

		public Transform capsuleVisualTransform;

		private void LateUpdate()
		{
			transform.localPosition = playerControllerTransform.localPosition - Vector3.up*0.5f;

			switch (playerController.currentStance)
			{
				case PlayerStance.Prone:
					capsuleVisualTransform.localScale = new Vector3(1f, 0.25f, 1f);
					capsuleVisualTransform.localPosition = new Vector3(0f, 0.25f, 0f);

					break;
				case PlayerStance.Crouched:
					capsuleVisualTransform.localScale = new Vector3(1f, 0.5f, 1f);
					capsuleVisualTransform.localPosition = new Vector3(0f, 0.5f, 0f);


					break;
				case PlayerStance.Standing:
					capsuleVisualTransform.localScale = new Vector3(1f, 1f, 1f);
					capsuleVisualTransform.localPosition = new Vector3(0f, 1f, 0f);

					break;
				default:
					throw new System.ArgumentOutOfRangeException();
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