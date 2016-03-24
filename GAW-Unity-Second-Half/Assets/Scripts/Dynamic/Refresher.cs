using UnityEngine;
using System.Collections;

using BehaviorTree;
using UnityEngine.SceneManagement;


public class TestObject
{
	public TestObject()
	{
		Debug.Log("Constructed the test object");
	}

	public void CallTest(float callValue)
	{
		Debug.Log("Called into testMethod");
		Debug.Log(callValue);
	}
}

public class Refresher : MonoBehaviour {
	void Update () {
		if (Input.GetKeyDown(KeyCode.F6))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	public INode Test()
	{
		Debug.Log("Called into C# from JS!");
		return new StubNode("Ghello");
	}
}
