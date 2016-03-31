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

			var testPosition = new Vector2s(0,0);
			var testChunk = new Chunk(testPosition, ChunkData.RoundRoom());

			blablaTest = testChunk;

			testChunk.Place();
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
				_chunks[position] = value;
			}
		}
	}
}
