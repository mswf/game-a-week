using UnityEngine;

public class Log : MonoBehaviour {

	public static void Weikie(object msg){
		Debug.Log(string.Format("<color='yellow'>Weikie: {0}</color>", msg));
	}

	public static void Steb(object msg)
	{
		Debug.Log(string.Format("<color='black'>Steb: {0}</color>", msg));
	}

	public static void Arny(object msg)
	{
		Debug.Log(string.Format("<color='orange'>Arni: {0}</color>", msg));
	}

	public static void Bobn(object msg)
	{
		Debug.Log(string.Format("<color='aqua'>Bobn: {0}</color>", msg));
	}

	public static void Tinas(object msg)
	{
		Debug.Log(string.Format("<color='green'>Tinas: {0}</color>", msg));
	}
}
