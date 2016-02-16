using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;


namespace Week02
{
	public class EnemyTurret : MonoBehaviour
	{

		public Transform target;
		public PlayerController playerController;
		public Light muzzleLight;

		private Ray _rayToHitTarget;
		private RaycastHit _rayToHitTargetHitInfo;

		public CapsuleCollider playerCollider;

		public GameObject impactParticleEffect;

		private float _originalMuzzleIntensiy;

		// Use this for initialization
		void Start()
		{
			_originalMuzzleIntensiy = muzzleLight.intensity;
			muzzleLight.intensity = 1f;
		
			InvokeRepeating("FireGun", 0f, 2f);	
		}

		// Update is called once per frame
		void LateUpdate()
		{


			transform.rotation = Quaternion.FromToRotation(Vector3.forward, playerController.getHeadPosition() - transform.position);


		}

		private void FireGun()
		{


			_rayToHitTarget.direction = playerController.getHeadPosition() - transform.position;
			_rayToHitTarget.origin = transform.position;

			

			var layerMaskShell = 1 << LayerMask.NameToLayer("PlayerShell");
			var layerMaskDefault = 1 << LayerMask.NameToLayer("Default");
			int layerMask = layerMaskShell | layerMaskDefault;

			string[] layers = new string[2];
			layers[0] = "PlayerShell";
			layers[1] = "Default";

			//		


			playerCollider.enabled = true;
			if (Physics.Raycast(_rayToHitTarget, out _rayToHitTargetHitInfo, 100f, 1))
			{
				if (_rayToHitTargetHitInfo.collider.tag == "Player")
				{
					Log.Steb("Hit the player!");
					Debug.DrawRay(_rayToHitTarget.origin, _rayToHitTarget.direction*_rayToHitTargetHitInfo.distance, Color.red, 0.8f);

				}
				else
				{
				Debug.DrawRay(_rayToHitTarget.origin, _rayToHitTarget.direction * _rayToHitTargetHitInfo.distance, Color.black, 0.8f);

				}

				var paricle = (GameObject) Instantiate(impactParticleEffect, _rayToHitTargetHitInfo.point - _rayToHitTarget.direction*0.1f, Quaternion.identity);
				paricle.transform.SetParent(_rayToHitTargetHitInfo.collider.gameObject.transform); 
				//Debug.DrawRay(transform.position, playerController.getHeadPosition() - transform.position, Color.red, 0.5f);

			}
			else
			{
				Debug.DrawRay(_rayToHitTarget.origin, _rayToHitTarget.direction * 100f, Color.white, 5f);

			}
			playerCollider.enabled = false;

			muzzleLight.intensity = _originalMuzzleIntensiy;
			muzzleLight.DOIntensity(1f, 0.5f).SetEase(Ease.OutExpo);

		}
	}
}
