using UnityEngine;
using System.Collections;
using JetBrains.Annotations;


namespace NoiseLib
{
	public class NoiseLib
	{

	}


	public interface IFunction
	{
		float Get2D(Vector2 position);
	}

	public interface IGeneratorFunction : IFunction
	{
		
	}

	public interface ITransformFunction : IFunction
	{
		void SetSources(params IFunction[] sources);
	}


	public class Constant : IGeneratorFunction
	{
		public float value = 0f;

		public float Get2D(Vector2 position)
		{
			return value;
		}
	}

	public class Gradient : IGeneratorFunction
	{
		public float x0 = 0f;
		public float x1 = 0f;

		public float y0 = 0f;
		public float y1 = 0f;

		public float Get2D(Vector2 position)
		{
			MathS.Lerp(x0, x1, position.x);

			MathS.Lerp(y0, y1, position.y);
			return 0f;
		}
	}

}	
