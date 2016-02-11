using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;



namespace Week01
{
	[RequireComponent(typeof(ParticleSystem))]
	public class PoissonDiskCreator : MonoBehaviour
	{
		[Range(1f, 10f)]
		public float rejectionDistance = 1f;

		[Range(0.01f,1f)]
		public float minDistance = 1f;

		[Range(10, 50)]
		public int samplingCount = 30;

		public Vector3 dimensions;

		[Range(0, 10)]
		public int seed = 0;

		[Range(0.01f, 0.3f)]
		public float frequency = 5f;

		public Gradient gizmoGradient;

		private Vector3[] points;

		private new ParticleSystem particleSystem;
		private ParticleSystem.EmitParams[] emitParams;

		[Range(0.1f, 100f)]
		public float spawnTimeFrame = 1f;

		[Range(0.1f, 100f)]
		public float particleLifeTime = 1f;

		private float particleStartTime = 0f;

		private void OnEnable()
		{
			Refresh();
		}

		void Update()
		{

			if (transform.hasChanged)
			{
				transform.hasChanged = false;
				Refresh();
			}
		}

		private void LateUpdate()
		{
			PositionParticles();

		}

		public void Refresh()
		{

			//float cellSize = minDistance/Mathf.Sqrt(2f);

			RandomHelper.Random = new System.Random(seed);
			
			points = SamplePoints(transform.TransformPoint(Vector3.zero), transform.TransformPoint(dimensions), null, minDistance, frequency, samplingCount).ToArray();

			if (particleSystem == null)
			{
				particleSystem = GetComponent<ParticleSystem>();
			}
			
			
			InitializeParticles();

		}

		private void InitializeParticles()
		{
			particleSystem.Stop();
			particleSystem.Clear();

			particleStartTime = Time.time;

			particleSystem.maxParticles = points.Length;

			emitParams = new ParticleSystem.EmitParams[points.Length];

			for (int i = 0; i < emitParams.Length; i++)
			{
				float sample = (Noise.Perlin3D(points[i], frequency) + 0.5f);

				//if (sample < 0.5f)
				{
					//sample *= 2f;

					emitParams[i].position = points[i];
					emitParams[i].velocity = Vector3.zero;
					emitParams[i].startLifetime = particleLifeTime;
					emitParams[i].startSize = (sample * -1f + 1f) * 1.5f;
					emitParams[i].rotation = sample * 10f;
					emitParams[i].velocity = Vector3.up*Mathf.Sin(sample)*0.06f;
					emitParams[i].startColor = gizmoGradient.Evaluate(sample);
					//particleSystem.Emit(emit, 1);
				}

			}

			Helper.RandomizeArray<ParticleSystem.EmitParams>(ref emitParams);

			particleSystem.Play();
		}

		private void PositionParticles()
		{
			if (points == null)
				return;
			

			float deltaTime = (Time.time - particleStartTime) % spawnTimeFrame;
			float rateStart = deltaTime/spawnTimeFrame;
			float rateEnd = (deltaTime+Time.deltaTime) / spawnTimeFrame;
			
			int length = emitParams.Length;
			int startRange = MathS.FloorToInt(length * rateStart);
			int endRange = MathS.FloorToInt(length * rateEnd);
			
			if (endRange < startRange)
			{
				for (int i = startRange; i < length; i++)
				{
					particleSystem.Emit(
						emitParams[i], 1
					);
				}
				for (int i = 0; i < endRange; i++)
				{
					particleSystem.Emit(
						emitParams[i], 1
					);
				}
			}
			else if (endRange > length)
			{
				for (int i = startRange; i < length; i++)
				{
					particleSystem.Emit(
						emitParams[i], 1
					);
				}
				for (int i = 0; i < length - endRange; i++)
				{
					particleSystem.Emit(
						emitParams[i], 1
					);
				}
			}
			else
			{
				for (int i = startRange; i < endRange; i++)
				{
					particleSystem.Emit(
						emitParams[i], 1
					);
				}
			}

			


		}

		/*
		private void OnDrawGizmosSelected()
		{
			if (points != null)
			{
				for (int v = 0; v < points.Length; v++)
				{
					float sample = (Noise.Perlin3D(points[v], frequency) + 0.5f);
					Gizmos.color = gizmoGradient.Evaluate(sample);
					Gizmos.DrawSphere(points[v], .1f);
				}
			}
		}
		*/

		// Poisson Stuff
		public const int DefaultPointsPerIteration = 30;

		static readonly float SquareRootTwo = (float)System.Math.Sqrt(2);

		private struct State
		{
			public Vector3?[,,] grid;
			public List<Vector3> activePoints, points;
		}

		struct Settings
		{
			public Vector3 TopLeft, LowerRight, Center;
			public Vector3 Dimensions;
			public float? RejectionSqDistance;
			public float MinimumDistance;
			public float CellSize;
			public int GridWidth;
			public int GridHeight;
			public int GridDepth;

			public float Frequency;

		}


		static List<Vector3> SamplePoints(Vector3 topLeft, Vector3 lowerRight, float? rejectionDistance, float minimumDistance, float frequency, int pointsPerIteration)
		{
			var settings = new Settings()
			{
				TopLeft = topLeft,
				LowerRight = lowerRight,
				Dimensions = lowerRight - topLeft,
				Center = (topLeft + lowerRight) / 2f,
				CellSize = minimumDistance / SquareRootTwo,
				MinimumDistance = minimumDistance,
				RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance,
				Frequency = frequency
			};
			
			settings.GridWidth = (int)(settings.Dimensions.x / settings.CellSize) + 1;
			settings.GridHeight = (int)(settings.Dimensions.y / settings.CellSize) + 1;
			settings.GridDepth = (int)(settings.Dimensions.z / settings.CellSize) + 1;

			//int guesstimateSize = (settings.GridWidth* settings.GridHeight * settings.GridDepth * 2);
			//Log.Steb(guesstimateSize);
			//TODO: proper guesstimate, prolly research this
			var state = new State()
			{
				grid = new Vector3?[settings.GridWidth, settings.GridHeight, settings.GridDepth],
				activePoints = new List<Vector3>(),
				points = new List<Vector3>()
			};

			AddFirstPoint(ref settings, ref state);


			int iterations = 0;

			while (state.activePoints.Count != 0 && iterations < 20000)
			{
				var listIndex = RandomHelper.Random.Next(state.activePoints.Count);

				var point = state.activePoints[listIndex];
				var found = false;

				for (var k = 0; k < pointsPerIteration; k++)
					found |= AddNextPoint(point, ref settings, ref state);

				if (!found)
					state.activePoints.RemoveAt(listIndex);

				iterations += 1;
			}

			return state.points;
		}

		private static void AddFirstPoint(ref Settings settings, ref State state)
		{
			var added = false;
			while (!added)
			{
				var d = RandomHelper.NextFloat();
				var xr = settings.TopLeft.x + settings.Dimensions.x*d;

				d = RandomHelper.NextFloat();
				var yr = settings.TopLeft.y + settings.Dimensions.y * d;

				d = RandomHelper.NextFloat();
				var zr = settings.TopLeft.z + settings.Dimensions.z * d;

				var p = new Vector3(xr, yr, zr);
				if (settings.RejectionSqDistance != null &&
				    MathS.Vector3DistanceSquared(settings.Center, p) > settings.RejectionSqDistance)
					continue;
				added = true;

				var index = Denormalize(p, settings.TopLeft, settings.CellSize);

				state.grid[(int) index.x, (int) index.y, (int) index.z] = p;

				state.activePoints.Add(p);
				state.points.Add(p);
			}
		}

		private static bool AddNextPoint(Vector3 point, ref Settings settings, ref State state)
		{
			var found = false;
			var q = GenerateRandomAround(point, settings.MinimumDistance);

			if (q.x >= settings.TopLeft.x && q.x < settings.LowerRight.x &&
			    q.y > settings.TopLeft.y && q.y < settings.LowerRight.y &&
			    q.z > settings.TopLeft.z && q.z < settings.LowerRight.z &&
			    (settings.RejectionSqDistance == null ||
			     MathS.Vector3DistanceSquared(settings.Center, q) <= settings.RejectionSqDistance))
			{
				var qIndex = Denormalize(q, settings.TopLeft, settings.CellSize);
				var tooClose = false;

				
				int maxX = Mathf.Max(0, qIndex.x - 2);
				int maxY = Mathf.Max(0, qIndex.y - 2);
				int maxZ = Mathf.Max(0, qIndex.z - 2);

				int minX = Mathf.Min(settings.GridWidth, qIndex.x + 3);
				int minY = Mathf.Min(settings.GridHeight, qIndex.y + 3);
				int minZ = Mathf.Min(settings.GridDepth, qIndex.z + 3);

				float minimumDistance = settings.MinimumDistance;
				minimumDistance += minimumDistance * (Noise.Perlin3D(q, settings.Frequency)+0.5f) * 1.2f;

				for (var i = maxX; i < minX && !tooClose; i++)
				{
					for (var j = maxY; j < minY && !tooClose; j++)
					{
						for (var k = maxZ; k < minZ && !tooClose; k++)
		//						if (state.grid[i, j, k].HasValue && Vector3.Distance(state.grid[i, j, k].Value, q) < settings.MinimumDistance)
								if (state.grid[i, j, k].HasValue && Vector3.Distance(state.grid[i, j, k].Value, q) < minimumDistance)
								tooClose = true;
					}
				}
				

				if (!tooClose)
				{
					found = true;
					state.activePoints.Add(q);
					state.points.Add(q);
					state.grid[(int) qIndex.x, (int) qIndex.y, (int) qIndex.z] = q;
				}
			}

			return found;
		}

		private static Vector3 GenerateRandomAround(Vector3 center, float minimumDistance)
		{
			//float sample = Noise.Perlin3D(center, FREQUENCY) +0.5f;

			var radius = (minimumDistance + minimumDistance* RandomHelper.NextFloat() * 3f);

			return center + (Quaternion.Euler(RandomHelper.NextFloat() * 360f, RandomHelper.NextFloat() * 360f, RandomHelper.NextFloat() * 360f) * Vector3.one).normalized * radius;
		}

		static Vector3i Denormalize(Vector3 point, Vector3 origin, float cellSize)
		{
			return new Vector3i
			{
				x = MathS.FloorToInt((point.x - origin.x)/cellSize),
				y = MathS.FloorToInt((point.y - origin.y)/cellSize),
				z = MathS.FloorToInt((point.z - origin.z)/cellSize)
			};
		}

		public struct PoissonPoint
		{
			public Vector3 position;
			public float noiseSample;
		}

		private static class RandomHelper
		{
			public static System.Random Random = new System.Random();

			public static float NextFloat()
			{
				return (float)Random.NextDouble();
			}
		}
	}
}