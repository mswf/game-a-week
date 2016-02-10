using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Week01
{
	public class PoissonDiskCreator : MonoBehaviour
	{
		[Range(1f, 10f)]
		public float rejectionDistance = 1f;

		[Range(1f,20f)]
		public float minDistance = 1f;

		[Range(10, 50)]
		public int samplingCount = 30;

		public Vector3 topLeft;
		public Vector3 lowerRight;


		[Range(1, 200)]
		public int earlyOut = 10;

		private Vector3[] points;

		private void OnEnable()
		{
			Refresh();
		}


		public void Refresh()
		{

			//float cellSize = minDistance/Mathf.Sqrt(2f);

			points = SamplePoints(transform.TransformPoint(topLeft), transform.TransformPoint(lowerRight), null, minDistance, samplingCount, earlyOut).ToArray();

			Log.Steb(points.Length);
		}

		// Update is called once per frame
		void Update()
		{
			if (transform.hasChanged)
			{
				transform.hasChanged = false;
				Refresh();
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (points != null)
			{
				float scale = 1f / points.Length;
				Gizmos.color = Color.yellow;
				for (int v = 0; v < points.Length; v++)
				{
					Gizmos.DrawSphere(points[v], 0.5f);
				}
			}
		}


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

		}


		static List<Vector3> SamplePoints(Vector3 topLeft, Vector3 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration, int earlyOut)
		{
			RandomHelper.Random = new System.Random(0);

			var settings = new Settings()
			{
				TopLeft = topLeft,
				LowerRight = lowerRight,
				Dimensions = lowerRight - topLeft,
				Center = (topLeft + lowerRight) / 2f,
				CellSize = minimumDistance / SquareRootTwo,
				MinimumDistance = minimumDistance,
				RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance
			};
			
			settings.GridWidth = (int)(settings.Dimensions.x / settings.CellSize) + 1;
			settings.GridHeight = (int)(settings.Dimensions.y / settings.CellSize) + 1;
			settings.GridDepth = (int)(settings.Dimensions.z / settings.CellSize) + 1;

			int guesstimateSize = (settings.GridWidth* settings.GridHeight * settings.GridDepth * 2);
			Log.Steb(guesstimateSize);
			var state = new State()
			{
				grid = new Vector3?[settings.GridWidth, settings.GridHeight, settings.GridDepth],
				activePoints = new List<Vector3>(15000),
				points = new List<Vector3>(15000)
			};

			AddFirstPoint(ref settings, ref state);


			int iterations = 0;

			while (state.activePoints.Count != 0 && iterations < earlyOut)
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

				for (var i = maxX; i < Mathf.Min(settings.GridWidth, qIndex.x + 3) && !tooClose; i++)
				{
					for (var j = maxY; j < Mathf.Min(settings.GridHeight, qIndex.y+3) && !tooClose; j++)
					{
						for (var k = maxZ; k < Mathf.Min(settings.GridHeight, qIndex.y + 3) && !tooClose; k++)
							if (state.grid[i, j, k].HasValue && Vector3.Distance(state.grid[i, j, k].Value, q) < settings.MinimumDistance)
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
			var d = RandomHelper.NextFloat();
			var radius = minimumDistance + minimumDistance*d;

			d = RandomHelper.NextFloat();
			var randomX = d*360f;
			d = RandomHelper.NextFloat();
			var randomY = d*360f;
			d = RandomHelper.NextFloat();
			var randomZ = d*360f;

	//		Log.Steb((Quaternion.Euler(randomX, randomY, randomZ) * Vector3.one).magnitude);

			return center + (Quaternion.Euler(randomX, randomY, randomZ) * Vector3.one).normalized * radius;
		}

		static Vector3i Denormalize(Vector3 point, Vector3 origin, float cellSize)
		{
			return new Vector3i
			{
				x = (int) ((point.x - origin.x)/cellSize),
				y = (int) ((point.y - origin.y)/cellSize),
				z = (int) ((point.z - origin.z)/cellSize)
			};
		}

		private struct Vector3i
		{
			public int x;
			public int y;
			public int z;
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