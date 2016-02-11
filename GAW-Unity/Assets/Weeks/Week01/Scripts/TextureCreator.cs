using UnityEngine;

namespace Week01
{
	[RequireComponent(typeof(MeshRenderer))]
	public class TextureCreator : MonoBehaviour
	{
		[Range(2,512)]
		public int resolution = 256;

		public float frequency = 10f;

		[Range(1, 8)]
		public int octaves = 1;

		[Range(1f, 4f)]
		public float lacunarity = 2f;

		[Range(0f, 1f)]
		public float persistence = 0.5f;

		public NoiseMethodType noiseType;

		[Range(1, 3)]
		public int dimensions = 3;


		private Texture2D texture;

		private Color[] samples;

		private void OnEnable()
		{
			if (texture == null)
			{
				texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true)
				{
					name = "Procedural Texture",
					wrapMode = TextureWrapMode.Clamp,
					filterMode = FilterMode.Trilinear,
					anisoLevel = 9
				};
				GetComponent<MeshRenderer>().material.mainTexture = texture;

				samples = new Color[resolution * resolution];
			}
			FillTexture();
		}

		public void FillTexture()
		{
			if (texture.width != resolution)
			{
				texture.Resize(resolution, resolution);
				samples = new Color[resolution * resolution];
			}

			Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
			Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f, -0.5f));
			Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f,  0.5f));
			Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f,  0.5f));

			NoiseMethod method = Noise.NoiseMethods[(int)noiseType][dimensions - 1];

			float stepSize = 1f/resolution;

			//float startTime = Time.realtimeSinceStartup;

			Color whiteColor = Color.white;

			if (noiseType != NoiseMethodType.Value)
			//  sample = sample * 0.5f + 0.5f
			{
				for (int y = 0, i = 0; y < resolution; y++)
				{
					float t = (y + 0.5f)*stepSize;
					Vector3 point0 = MathS.Vector3LerpUnclamped(point00, point01, t);
					Vector3 point1 = MathS.Vector3LerpUnclamped(point10, point11, t);

					for (int x = 0; x < resolution; x++, i++)
					{
						Vector3 point = MathS.Vector3LerpUnclamped(point0, point1, (x + 0.5f)*stepSize);
						float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence) * 0.5f + 0.5f;
						samples[i] = whiteColor*sample;
					}
				}
			}
			else
			// sample = sample
			{
				for (int y = 0, i = 0; y < resolution; y++)
				{
					float t = (y + 0.5f) * stepSize;
					Vector3 point0 = MathS.Vector3LerpUnclamped(point00, point01, t);
					Vector3 point1 = MathS.Vector3LerpUnclamped(point10, point11, t);

					for (int x = 0; x < resolution; x++, i++)
					{
						Vector3 point = MathS.Vector3LerpUnclamped(point0, point1, (x + 0.5f) * stepSize);
						float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
						samples[i] = whiteColor * sample;
					}
				}
			}

			texture.SetPixels(samples);

			//Log.Steb(Time.realtimeSinceStartup - startTime);

			texture.Apply();
		}

		// Update is called once per frame
		private void Update()
		{
			if (transform.hasChanged)
			{
				transform.hasChanged = false;
				FillTexture();
			}
		}
	}
}