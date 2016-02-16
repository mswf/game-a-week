
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

		private Vector3 _originalCameraPosition;

		// Use this for initialization
		void Start()
		{
			_originalCameraPosition = Camera.main.transform.position;
			_originalMuzzleIntensiy = muzzleLight.intensity;
			muzzleLight.intensity = 1f;
		
			InvokeRepeating("FireGun", 1f + Random.value, 2f);	
		}

		// Update is called once per frame
		void LateUpdate()
		{


			transform.rotation = Quaternion.FromToRotation(Vector3.forward, playerController.getHeadPosition() - transform.position);


		}

		private void FireGun()
		{
			Vector3 shootingDirection = (playerController.getHeadPosition() - transform.position).normalized+ new Vector3(Random.value, Random.value, Random.value)*0.02f;
			_rayToHitTarget.direction = shootingDirection;
			_rayToHitTarget.origin = transform.position;

			
			


			playerCollider.enabled = true;
			if (Physics.Raycast(_rayToHitTarget, out _rayToHitTargetHitInfo, 100f, 1))
			{
				if (_rayToHitTargetHitInfo.collider.tag == "Player")
				{
					//Log.Steb("Hit the player!");
					Debug.DrawRay(_rayToHitTarget.origin, shootingDirection * _rayToHitTargetHitInfo.distance, Color.red, 0.8f);
					Camera.main.DOShakePosition(0.4f, 1.6f).OnComplete( () =>
						{

							if (!DOTween.IsTweening(Camera.main))
							{
								Camera.main.transform.DOMove(_originalCameraPosition, 0.1f);

							}
						});
				}
				else
				{
					Debug.DrawRay(_rayToHitTarget.origin, shootingDirection * _rayToHitTargetHitInfo.distance, Color.black, 0.8f);


					var impactDistance = Vector3.Distance(target.position, _rayToHitTargetHitInfo.point);

					float minDistance = 5f;

					if (impactDistance < minDistance)
					{
						Camera.main.DOShakePosition(0.6f, MathS.Lerp(.6f, 0f, (impactDistance)/ minDistance), 20)
							.OnComplete(() =>
							{
								if (!DOTween.IsTweening(Camera.main))
								{
									Camera.main.transform.DOMove(_originalCameraPosition, 0.1f);
									
								}
							});


					}
				}

				var paricle = (GameObject) Instantiate(impactParticleEffect, _rayToHitTargetHitInfo.point - shootingDirection * 0.1f, Quaternion.identity);
				paricle.transform.SetParent(_rayToHitTargetHitInfo.collider.gameObject.transform); 
				//Debug.DrawRay(transform.position, playerController.getHeadPosition() - transform.position, Color.red, 0.5f);

			}
			else
			{
				Debug.DrawRay(_rayToHitTarget.origin, shootingDirection * 100f, Color.white, 5f);

			}
			playerCollider.enabled = false;

			muzzleLight.intensity = _originalMuzzleIntensiy;
			muzzleLight.DOIntensity(1f, 0.5f).SetEase(Ease.OutExpo);


		}
	}
}
