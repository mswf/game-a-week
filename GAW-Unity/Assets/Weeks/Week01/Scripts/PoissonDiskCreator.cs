using UnityEngine;
using System.Collections.Generic;
using DG.Tweening.Plugins.Options;

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

		[Range(0.1f, 4f)]
		public float particleSize = 1.5f;

		private float _particleStartTime = 0f;

		private void OnEnable()
		{
			//Refresh();
		}

		private void Awake()
		{
			transform.hasChanged = false;
			
		}

		private bool _isInitializedOnce = false;

		private void Update()
		{
			if (_isInitializedOnce == false && Input.GetKeyUp(KeyCode.Y))
			{
				_isInitializedOnce = true;
		
				Refresh();

			}

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
			//float initialTime = Time.realtimeSinceStartup;
			RandomHelper.Random = new System.Random(seed);
			
			_points = SamplePoints(transform.TransformPoint(Vector3.zero), transform.TransformPoint(dimensions), null, minDistance, frequency, samplingCount).ToArray();

			if (_particleSystem == null)
			{
				_particleSystem = GetComponent<ParticleSystem>();
			}

			InitializeParticles();
			//Log.Steb(Time.realtimeSinceStartup - initialTime);
			transform.hasChanged = false;
		}

		private void InitializeParticles()
		{
			_particleSystem.Stop();
			_particleSystem.Clear();

			_particleStartTime = Time.time;


			var tempEmitParams = new List<ParticleSystem.EmitParams>(_points.Length / 2);

			var tempEmitParam = new ParticleSystem.EmitParams();

			for (var i = 0; i < _points.Length; i++)
			{
				var sample = Noise.Perlin3D(_points[i], frequency) + 0.5f;

				var color = gizmoGradient.Evaluate(sample);

				if (color.a > 0.0005f)
				{
					//sample *= 2f;



					tempEmitParam.position = _points[i];
					tempEmitParam.velocity = Vector3.zero;
					tempEmitParam.startLifetime = particleLifeTime;
					tempEmitParam.startSize = Mathf.Sqrt(sample * -1f + 1f) * particleSize;
					tempEmitParam.rotation = sample * 10f;
					tempEmitParam.velocity = Vector3.up*Mathf.Sin(sample)*-0.06f;
					tempEmitParam.startColor = color;

					tempEmitParams.Add(tempEmitParam);
					//particleSystem.Emit(emit, 1);
				}

			}

			_emitParams = tempEmitParams.ToArray();
			_particleSystem.maxParticles = _emitParams.Length;


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
					_particleSystem.Emit(_emitParams[i], 1);
				for (int i = 0; i < endRange; i++)
					_particleSystem.Emit(_emitParams[i], 1);
			}
			else if (endRange > length)
			{
				for (int i = startRange; i < length; i++)
					_particleSystem.Emit(_emitParams[i], 1);
				for (int i = 0; i < length - endRange; i++)
					_particleSystem.Emit(_emitParams[i], 1);
			}
			else
			{
				for (int i = startRange; i < endRange; i++)
					_particleSystem.Emit(_emitParams[i], 1);
			}
		}

		
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			
			Gizmos.DrawWireCube(transform.TransformPoint(dimensions * 0.5f), dimensions);
			
		}
		

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
			public Bounds bounds;
			
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
				bounds = new Bounds(topLeft + dimensions*.5f, dimensions),
				rejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance,
				minimumDistance = minimumDistance,
				cellSize = cellSize,
				gridWidth = (int)(dimensions.x / cellSize) + 1,
				gridHeight = (int)(dimensions.y / cellSize) + 1,
				gridDepth = (int)(dimensions.z / cellSize) + 1,
				frequency = frequency
			};


			//int guesstimateSize = (int) (settings.gridWidth * settings.gridHeight * settings.gridDepth * cellSize);
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
					found |= AddNextPoint(ref point, ref settings, ref state);

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
				var xr = settings.bounds.min.x + settings.bounds.size.x*d;

				d = RandomHelper.NextFloat();
				var yr = settings.bounds.min.y + settings.bounds.size.y * d;

				d = RandomHelper.NextFloat();
				var zr = settings.bounds.min.z + settings.bounds.size.z * d;

				var p = new Vector3(xr, yr, zr);
				if (settings.rejectionSqDistance != null &&
				    MathS.Vector3DistanceSquared(settings.bounds.center, p) > settings.rejectionSqDistance)
					continue;
				added = true;

				var index = Denormalize(ref p, settings.bounds.min, settings.cellSize);

				state.grid[(int) index.x, (int) index.y, (int) index.z] = p;

				state.activePoints.Add(p);
				state.points.Add(p);
			}
		}

		private static bool AddNextPoint(ref Vector3 point, ref Settings settings, ref State state)
		{
			var q = GenerateRandomAround(ref point, settings.minimumDistance);

			if (settings.bounds.Contains(q) &&
			    (settings.rejectionSqDistance == null ||
			     MathS.Vector3DistanceSquared(settings.bounds.center, q) <= settings.rejectionSqDistance))
			{
				var qIndex = Denormalize(ref q, settings.bounds.min, settings.cellSize);

				float minimumDistance = settings.minimumDistance;
				minimumDistance += minimumDistance * (Noise.Perlin3D(q, settings.frequency) + 0.5f) * 1.2f;


				var tooClose = isTooClose(ref state.grid, ref q, 
					Mathf.Max(0, qIndex.x - 2), Mathf.Max(0, qIndex.y - 2), Mathf.Max(0, qIndex.z - 2), 
					
					Mathf.Min(settings.gridWidth, qIndex.x + 3), 
					Mathf.Min(settings.gridHeight, qIndex.y + 3), 
					Mathf.Min(settings.gridDepth, qIndex.z + 3), 
					
					minimumDistance);

				if (!tooClose)
				{
					state.activePoints.Add(q);
					state.points.Add(q);
					state.grid[qIndex.x, qIndex.y, qIndex.z] = q;

					return true;
				}
			}

			return false;
		}

		private static bool isTooClose(ref Vector3?[,,] grid, ref Vector3 q, int maxX, int maxY, int maxZ, int minX, int minY, int minZ, float minimumDistance)
		{
			for (var i = maxX; i < minX; i++)
			{
				for (var j = maxY; j < minY; j++)
				{
					for (var k = maxZ; k < minZ; k++)
		//				if (state.grid[i, j, k].HasValue && Vector3.Distance(state.grid[i, j, k].Value, q) < settings.MinimumDistance)
						if (grid[i, j, k].HasValue && Vector3.Distance(grid[i, j, k].Value, q) < minimumDistance)
							return true;
				}
			}

			return false;
		}

		private static Vector3 GenerateRandomAround(ref Vector3 center, float minimumDistance)
		{
			//float sample = Noise.Perlin3D(center, FREQUENCY) +0.5f;

			var radius = (minimumDistance + minimumDistance* RandomHelper.NextFloat() * 3f);

			return center + (Random.rotation * Vector3.one).normalized * radius;
		}

		static Vector3i Denormalize(ref Vector3 point, Vector3 origin, float cellSize)
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