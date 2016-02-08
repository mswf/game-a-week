using UnityEngine;
using System.Collections;

namespace Week01
{
	[RequireComponent(typeof(MeshRenderer))]
	public class TextureCreator : MonoBehaviour
	{
		public int resolution = 256;

		private Texture2D texture;

		private void OnEnable()
		{
			texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			GetComponent<MeshRenderer>().material.mainTexture = texture;
			FillTexture();
		}

		private void FillTexture()
		{
			for (int y = 0; y < resolution; y++)
			{
				for (int x = 0; x < resolution; x++)
				{
					texture.SetPixel(x, y, Color.red);
				}
			}
			texture.Apply();
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