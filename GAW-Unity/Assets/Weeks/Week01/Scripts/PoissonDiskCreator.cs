using UnityEngine;
using System.Collections.Generic;

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

		private Vector3[] _points;

		private ParticleSystem _particleSystem;
		private ParticleSystem.EmitParams[] _emitParams;

		[Range(0.1f, 100f)]
		public float spawnTimeFrame = 1f;

		[Range(0.1f, 100f)]
		public float particleLifeTime = 1f;

		private float _particleStartTime = 0f;

		private void OnEnable()
		{
			Refresh();
		}

		private void Update()
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
			RandomHelper.Random = new System.Random(seed);
			
			_points = SamplePoints(transform.TransformPoint(Vector3.zero), transform.TransformPoint(dimensions), null, minDistance, frequency, samplingCount).ToArray();

			if (_particleSystem == null)
			{
				_particleSystem = GetComponent<ParticleSystem>();
			}

			InitializeParticles();
		}

		private void InitializeParticles()
		{
			_particleSystem.Stop();
			_particleSystem.Clear();

			_particleStartTime = Time.time;

			_particleSystem.maxParticles = _points.Length;

			_emitParams = new ParticleSystem.EmitParams[_points.Length];

			for (var i = 0; i < _emitParams.Length; i++)
			{
				var sample = Noise.Perlin3D(_points[i], frequency) + 0.5f;

				//if (sample < 0.5f)
				{
					//sample *= 2f;

					_emitParams[i].position = _points[i];
					_emitParams[i].velocity = Vector3.zero;
					_emitParams[i].startLifetime = particleLifeTime;
					_emitParams[i].startSize = (sample * -1f + 1f) * 1.5f;
					_emitParams[i].rotation = sample * 10f;
					_emitParams[i].velocity = Vector3.up*Mathf.Sin(sample)*0.06f;
					_emitParams[i].startColor = gizmoGradient.Evaluate(sample);
					//particleSystem.Emit(emit, 1);
				}

			}

			Helper.RandomizeArray(ref _emitParams);

			_particleSystem.Play();
		}

		private void PositionParticles()
		{
			if (_points == null)
				return;
			

			float deltaTime = (Time.time - _particleStartTime) % spawnTimeFrame;
			float rateStart = deltaTime/spawnTimeFrame;
			float rateEnd = (deltaTime+Time.deltaTime) / spawnTimeFrame;
			
			int length = _emitParams.Length;
			int startRange = MathS.FloorToInt(length * rateStart);
			int endRange = MathS.FloorToInt(length * rateEnd);
			
			if (endRange < startRange)
			{
				for (int i = startRange; i < length; i++)
				{
					_particleSystem.Emit(
						_emitParams[i], 1
					);
				}
				for (int i = 0; i < endRange; i++)
				{
					_particleSystem.Emit(
						_emitParams[i], 1
					);
				}
			}
			else if (endRange > length)
			{
				for (int i = startRange; i < length; i++)
				{
					_particleSystem.Emit(
						_emitParams[i], 1
					);
				}
				for (int i = 0; i < length - endRange; i++)
				{
					_particleSystem.Emit(
						_emitParams[i], 1
					);
				}
			}
			else
			{
				for (int i = startRange; i < endRange; i++)
				{
					_particleSystem.Emit(
						_emitParams[i], 1
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

		private static readonly float SquareRootTwo = (float)System.Math.Sqrt(2);

		private struct State
		{
			public Vector3?[,,] grid;
			public List<Vector3> activePoints, points;
		}

		private struct Settings
		{
			public Vector3 topLeft, lowerRight, center;
			public Vector3 dimensions;
			public float? rejectionSqDistance;
			public float minimumDistance;
			public float cellSize;
			public int gridWidth;
			public int gridHeight;
			public int gridDepth;

			public float frequency;

		}

		private static List<Vector3> SamplePoints(Vector3 topLeft, Vector3 lowerRight, float? rejectionDistance, float minimumDistance, float frequency, int pointsPerIteration)
		{
			var dimensions = lowerRight - topLeft;
			var cellSize = minimumDistance/SquareRootTwo;

			var settings = new Settings
			{
				topLeft = topLeft,
				lowerRight = lowerRight,
				center = (topLeft + lowerRight) / 2f,
				dimensions = dimensions,
				rejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance,
				minimumDistance = minimumDistance,
				cellSize = cellSize,
				gridWidth = (int)(dimensions.x / cellSize) + 1,
				gridHeight = (int)(dimensions.y / cellSize) + 1,
				gridDepth = (int)(dimensions.z / cellSize) + 1,
				frequency = frequency
			};
			


			//int guesstimateSize = (settings.GridWidth* settings.GridHeight * settings.GridDepth * 2);
			//Log.Steb(guesstimateSize);
			//TODO: proper guesstimate, prolly research this
			var state = new State()
			{
				grid = new Vector3?[settings.gridWidth, settings.gridHeight, settings.gridDepth],
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
				var xr = settings.topLeft.x + settings.dimensions.x*d;

				d = RandomHelper.NextFloat();
				var yr = settings.topLeft.y + settings.dimensions.y * d;

				d = RandomHelper.NextFloat();
				var zr = settings.topLeft.z + settings.dimensions.z * d;

				var p = new Vector3(xr, yr, zr);
				if (settings.rejectionSqDistance != null &&
				    MathS.Vector3DistanceSquared(settings.center, p) > settings.rejectionSqDistance)
					continue;
				added = true;

				var index = Denormalize(p, settings.topLeft, settings.cellSize);

				state.grid[(int) index.x, (int) index.y, (int) index.z] = p;

				state.activePoints.Add(p);
				state.points.Add(p);
			}
		}

		private static bool AddNextPoint(Vector3 point, ref Settings settings, ref State state)
		{
			var found = false;
			var q = GenerateRandomAround(point, settings.minimumDistance);

			if (q.x >= settings.topLeft.x && q.x < settings.lowerRight.x &&
			    q.y > settings.topLeft.y && q.y < settings.lowerRight.y &&
			    q.z > settings.topLeft.z && q.z < settings.lowerRight.z &&
			    (settings.rejectionSqDistance == null ||
			     MathS.Vector3DistanceSquared(settings.center, q) <= settings.rejectionSqDistance))
			{
				var qIndex = Denormalize(q, settings.topLeft, settings.cellSize);
				var tooClose = false;

				
				int maxX = Mathf.Max(0, qIndex.x - 2);
				int maxY = Mathf.Max(0, qIndex.y - 2);
				int maxZ = Mathf.Max(0, qIndex.z - 2);

				int minX = Mathf.Min(settings.gridWidth, qIndex.x + 3);
				int minY = Mathf.Min(settings.gridHeight, qIndex.y + 3);
				int minZ = Mathf.Min(settings.gridDepth, qIndex.z + 3);

				float minimumDistance = settings.minimumDistance;
				minimumDistance += minimumDistance * (Noise.Perlin3D(q, settings.frequency)+0.5f) * 1.2f;

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