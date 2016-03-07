using UnityEngine;
using System.Collections;

namespace Week04
{
	public class GameController : MonoBehaviour
	{
		private void Awake()
		{
			#region SetupAndroidScreen
			Screen.orientation = ScreenOrientation.LandscapeRight;
			Screen.orientation = ScreenOrientation.AutoRotation;

			Screen.autorotateToLandscapeLeft	= true;
			Screen.autorotateToLandscapeRight	= true;

			Screen.autorotateToPortrait				= false;
			Screen.autorotateToPortraitUpsideDown	= false;

			Screen.orientation = ScreenOrientation.AutoRotation;
			#endregion


		}

		// Use this for initialization
		private void Start()
		{

		}

		// Update is called once per frame
		private void Update()
		{

		}
	}
}