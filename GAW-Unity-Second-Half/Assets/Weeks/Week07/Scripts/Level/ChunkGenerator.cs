using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

namespace Week07
{
	public static class ChunkGenerator
	{
		
	}

	[System.Serializable]
	public class Chunk
	{
		public const int ChunkDimension = 8;

		[SerializeField]
		private readonly short[][] _chunkData;
		[SerializeField]
		private readonly Vector2s _position;

		private bool _isPlaced;

		public Vector2s position
		{
			get { return _position; }
		}

		public Chunk(Vector2s position, short[][] chunkData)
		{
			this._position = position;
			this._chunkData = chunkData;

			_isPlaced = false;
		}

		public void Place()
		{
			if (_isPlaced == true)
				return;



			const float scale = 3f;
			var initialPosition = new Vector3(_position.x, scale / 2f, _position.y);
			var initialScale = new Vector3(scale, scale, scale);
	
			for (int x = 0; x < ChunkDimension; x++)
			{
				for (int y = 0; y < ChunkDimension; y++)
				{
					var tileType = _chunkData[x][y];

					if (tileType == 1)
					{
						var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
						tile.transform.position = initialPosition + new Vector3(x* scale, 0, y*scale);
						tile.transform.localScale = initialScale;
						tile.AddComponent<HitCollider>();
					}
				}
			}
			
			_isPlaced = true;
		}
	}

	public static class ChunkData
	{
		private const int ChunkDimension = Chunk.ChunkDimension;

		// Open = O = 0
		public const short OPEN = 0;
		public const short O = OPEN;

		// Solid = S = 1
		public const short SOLID = 1;
		public const short S = SOLID;

		public static short[][] EmptyRoom()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O}
			};
		}

		public static short[][] SolidRoom()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S}
			};
		}

		public static short[][] RoundRoom()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,O,O,S,S,S},
				new short[ChunkDimension] {S,S,O,O,O,O,S,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,S,O,O,O,O,S,S},
				new short[ChunkDimension] {S,S,S,O,O,S,S,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S}
			};
		}
	}
}
