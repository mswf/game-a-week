using UnityEngine;
using System.Collections;
using System.Net;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Week04
{
	[RequireComponent(typeof(Image))]
	public class CameraScrollButton : Button
	{
		public ScrollDirection scrollDirection = ScrollDirection.ToLeft;

		private bool _isHeldDown = false;

		// Use this for early referencing
		protected override void Awake()
		{
			base.Awake();
		}

		// Update is called once per frame
		private void Update () 
		{
			if (_isHeldDown)
			{
				if (Globals.inputManager.IsCameraMovable() == false)
				{
					_isHeldDown = false;
					return;
				}

				var scrollSpeed = CameraController.DefaultScrollSpeed * Time.deltaTime;
				if (scrollDirection == ScrollDirection.ToLeft)
				{
					scrollSpeed *= -1f;
				}

				Globals.cameraController.Scroll(scrollSpeed);
			}
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (Globals.inputManager.IsCameraMovable())
			{
				_isHeldDown = true;

				base.OnPointerDown(eventData);
			}
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			_isHeldDown = false;
			base.OnPointerUp(eventData);

		}
	}
}
