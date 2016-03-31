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
			_level = new Level();

			for (short i = -4; i < 4; i++)
			{
				for (short j = -4; j < 4; j++)
				{
					var testPosition = new Vector2s(i, j);
					var testChunk = new Chunk(testPosition, ChunkData.SquareRoomEmpty());
					_level[testPosition] = testChunk;

					testChunk.Place();

				}
			}
		}
	}

	public class Level
	{
		private Dictionary<Vector2s, Chunk> _chunks;

		public Level()
		{
			this._chunks = new Dictionary<Vector2s, Chunk>();
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
