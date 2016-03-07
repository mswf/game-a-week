using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Week04
{
	[RequireComponent(typeof(Image), typeof(Button))]
	public class CameraScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{

		private Image _image;
		private Button _button;

		public ScrollDirection scrollDirection = ScrollDirection.ToLeft;

		private bool _isHeldDown = false;

		// Use this for early referencing
		private void Awake()
		{
			_image = GetComponent<Image>();
			_button = GetComponent<Button>();

		}

		// Update is called once per frame
		private void Update () 
		{
			if (_isHeldDown)
			{
				var scrollSpeed = CameraController.DefaultScrollSpeed * Time.deltaTime;
				if (scrollDirection == ScrollDirection.ToLeft)
				{
					scrollSpeed *= -1f;
				}

				Globals.cameraController.Scroll(scrollSpeed);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_isHeldDown = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isHeldDown = false;
		}
	}
}
