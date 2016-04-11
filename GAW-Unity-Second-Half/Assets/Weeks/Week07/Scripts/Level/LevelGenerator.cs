using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Week07
{
	public class LevelGenerator : MonoBehaviour
	{
		private Level _level;

		public Chunk blablaTest;

		// Use this for early referencing
		protected void Awake()
		{
			GameGlobals.levelGenerator = this;
		}

		protected void OnEnable()
		{
			GameGlobals.levelGenerator = this;
		}

		public void CreateInitialLevel()
		{
			const short lSize = 12;

			var realLevelChunks = new List<Chunk>();

			_level = new Level();

			var startPosition = new Vector2s(0,0);

			{
				var firstChunk = new Chunk(startPosition, ChunkData.SquareRoomEmpty());
				_level[startPosition] = firstChunk;

				realLevelChunks.Add(firstChunk);
			}

			var previousPosition = startPosition;

			const int Num_Walks = 12;

			for (int number = 0; number < Num_Walks; number++)
			{
				Vector2s newPos;
				if (Random.value > 0.5f)
					newPos = new Vector2s((short)(previousPosition.x + 1), previousPosition.y);
				else
					newPos = new Vector2s(previousPosition.x, (short)(previousPosition.y + 1));

				Chunk testChunk;

				if (number == Num_Walks - 1)
					testChunk = new Chunk(newPos, ChunkData.TreasureRoom());
				else
					testChunk = new Chunk(newPos, ChunkData.SquareRoomRandom());

				_level[newPos] = testChunk;

				realLevelChunks.Add(testChunk);

				previousPosition = new Vector2s(newPos.x, newPos.y);
			}

			foreach (var chunk in realLevelChunks.ToArray())
			{
				chunk.Place();

				var neighbours = _level.GetNeighbours(chunk.position);

				foreach (var keyValuePair in neighbours)
				{
					if (keyValuePair.Value == null)
					{
						if (Random.value > 0.6f)
						{
							var newNeighbour = new Chunk(keyValuePair.Key, ChunkData.SquareRoomRandom());
							_level[keyValuePair.Key] = newNeighbour;

							newNeighbour.Place();

							realLevelChunks.Add(newNeighbour);

						}


					}
				}
			}

			foreach (var chunk in realLevelChunks.ToArray())
			{
				chunk.Place();

				var neighbours = _level.GetNeighbours(chunk.position);

				foreach (var keyValuePair in neighbours)
				{
					if (keyValuePair.Value == null)
					{
						if (Random.value > 0.7f)
						{
							var newNeighbour = new Chunk(keyValuePair.Key, ChunkData.SquareRoomRandom());
							_level[keyValuePair.Key] = newNeighbour;

							newNeighbour.Place();

							realLevelChunks.Add(newNeighbour);

						}


					}
				}
			}


			foreach (var chunk in realLevelChunks.ToArray())
			{
				chunk.Place();

				var neighbours = _level.GetNeighbours(chunk.position);

				foreach (var keyValuePair in neighbours)
				{
					if (keyValuePair.Value == null)
					{

						var newNeighbour = new Chunk(keyValuePair.Key, ChunkData.SolidRoom());
						_level[keyValuePair.Key] = newNeighbour;

						newNeighbour.Place();


					}
				}
			}


			/*
			for (short i = -lSize; i < lSize; i++)
			{
				for (short j = -lSize; j < lSize; j++)
				{
					var testPosition = new Vector2s(i, j);
					var testChunk = new Chunk(testPosition, ChunkData.SquareRoomRandom());
					_level[testPosition] = testChunk;

					testChunk.Place();

				}
			}
			*/
		}
	}

	public class Level
	{
		private Dictionary<Vector2s, Chunk> _chunks;

		public Level()
		{
			this._chunks = new Dictionary<Vector2s, Chunk>();
		}
		
		public KeyValuePair<Vector2s, Chunk>[] GetNeighbours(Vector2s pos)
		{

			var pos0 = new Vector2s( (short)(pos.x - 1), (short)(pos.y - 1) );
			var pos1 = new Vector2s( (short)(pos.x    ), (short)(pos.y - 1) );
			var pos2 = new Vector2s( (short)(pos.x + 1), (short)(pos.y - 1) );
			var pos3 = new Vector2s( (short)(pos.x - 1), (short)(pos.y    ) );
			var pos4 = new Vector2s( (short)(pos.x + 1), (short)(pos.y    ) );
			var pos5 = new Vector2s( (short)(pos.x - 1), (short)(pos.y + 1) );
			var pos6 = new Vector2s( (short)(pos.x    ), (short)(pos.y + 1) );
			var pos7 = new Vector2s( (short)(pos.x + 1), (short)(pos.y + 1) );	

			return new KeyValuePair<Vector2s, Chunk>[8]
			{
				new KeyValuePair<Vector2s, Chunk> (pos0, this[pos0]),
				new KeyValuePair<Vector2s, Chunk> (pos1, this[pos1]),
				new KeyValuePair<Vector2s, Chunk> (pos2, this[pos2]),
				new KeyValuePair<Vector2s, Chunk> (pos3, this[pos3]),
				new KeyValuePair<Vector2s, Chunk> (pos4, this[pos4]),
				new KeyValuePair<Vector2s, Chunk> (pos5, this[pos5]),
				new KeyValuePair<Vector2s, Chunk> (pos6, this[pos6]),
				new KeyValuePair<Vector2s, Chunk> (pos7, this[pos7]),
			};
		}

		public KeyValuePair<Vector2s, Chunk>[] GetNeighbours(Chunk chunk)
		{
			return GetNeighbours(chunk.position);
		}

		public Chunk this[Vector2s position]
		{
			get
			{
				if (_chunks.ContainsKey(position))
					return _chunks[position];
				
				return null;
			}
			set
			{
				if (!_chunks.ContainsKey(position))
					_chunks[position] = value;
				else
				{
					// Check if same position
					foreach (var positionKey in _chunks.Keys)
					{
						if (positionKey.Equals(positionKey, position))
						{
							Debug.LogError("Tried to place duplicate chunk at x:" + position.x + ", y:" + position.y);
							return;
						}
					}
					// Actually a hash collision, let's find it
					foreach (var positionKey in _chunks.Keys)
					{
						if (positionKey.GetHashCode() == position.GetHashCode())
						{
							Debug.LogError("Hash collision for pos x:" + position.x + ", y:" + position.y + " && x:"+positionKey.x+", y:"+positionKey.y);
							return;
						}
					}

				}


			}
		}
	}
}
