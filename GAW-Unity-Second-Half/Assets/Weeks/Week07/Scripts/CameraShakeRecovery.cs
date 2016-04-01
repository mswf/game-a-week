using UnityEngine;
using System.Collections;

namespace Week07
{
	public class CameraShakeRecovery : SteffBehaviour
	{
		private float idleTime = 0f;

		private Vector3 prevPosition;

		private void Update()
		{
			var curPosition = _transform.localPosition;

			if (curPosition != prevPosition)
			{
				idleTime = 0;
				_transform.hasChanged = false;

				prevPosition = curPosition;
			}
			else
			{
				idleTime += Time.deltaTime;
			}

			if (idleTime > 0.1f)
			{
				_transform.localPosition = _transform.localPosition * 0.95f;
				_transform.hasChanged = false;

				prevPosition = _transform.localPosition;
			}
		}
	}
}
