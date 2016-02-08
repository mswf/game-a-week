using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


namespace Week00
{
	public class MenuController : MonoBehaviour
	{
		public Scene sceneToLoad;

		public void LoadScene()
		{
			SceneManager.LoadScene(sceneToLoad.name);
		}
	}
}
