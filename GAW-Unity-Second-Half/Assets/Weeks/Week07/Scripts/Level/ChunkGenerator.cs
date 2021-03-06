﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Policy;
using UnityEngine.SceneManagement;

namespace Week07
{
	public static class ChunkGenerator
	{
		
	}

	[System.Serializable]
	public class Chunk
	{
		private const int ChunkDimension = ChunkData.ChunkDimension;

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



			const float tileScale = 3f;
			var initialPosition = new Vector3(_position.x * ChunkDimension * tileScale, tileScale / 2f, _position.y * ChunkDimension * tileScale);
			var initialScale = new Vector3(tileScale, tileScale, tileScale);
	
			for (int x = 0; x < ChunkDimension; x++)
			{
				for (int y = 0; y < ChunkDimension; y++)
				{
					var tileType = _chunkData[x][y];
					
					if (tileType == ChunkData.SOLID)
					{
						var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
						tile.transform.position = initialPosition + new Vector3(x* tileScale, 0, y*tileScale);
						tile.transform.localScale = initialScale;
						tile.AddComponent<HitCollider>();
					}
					else if (tileType == ChunkData.RANDOM)
						if (Random.value > 0.6f)
						{
							_chunkData[x][y] = ChunkData.SOLID;

							var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
							tile.transform.position = initialPosition + new Vector3(x*tileScale, 0, y*tileScale);
							tile.transform.localScale = initialScale;
							tile.AddComponent<HitCollider>();
						}
						else
						{
							_chunkData[x][y] = ChunkData.OPEN;

						}
				}
			}

			for (int x = 0; x < ChunkDimension; x++)
			{
				for (int y = 0; y < ChunkDimension; y++)
				{
					var tileType = _chunkData[x][y];

					if (tileType == ChunkData.OPEN)
					{
						int numSolidNeighbours = 0;

						var cardNeighbours = GetCardinalNeighbours(x, y);

						foreach (short neighbour in cardNeighbours)
						{
							if (neighbour == ChunkData.SOLID)
								numSolidNeighbours += 4;
						}


						var diagNeighbours = GetDiagonalNeighbours(x, y);

						foreach (short neighbour in diagNeighbours)
						{
							if (neighbour == ChunkData.SOLID)
								numSolidNeighbours += 1;
						}

						if (numSolidNeighbours >= 12)
						{
							float thresHold = 0.4f + 0.1f*numSolidNeighbours - 12;

							if (Random.value > thresHold)
							{
								_chunkData[x][y] = ChunkData.TREASURE;

								// place treasure
								var tile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
								tile.transform.position = initialPosition + new Vector3(x * tileScale, 0, y * tileScale);
								tile.transform.localScale = initialScale;
								tile.GetComponent<MeshRenderer>().material.color = Color.yellow;
								tile.AddComponent<HitCollider>();
							}
						}
					}
					else if (tileType == ChunkData.TREASURE)
					{
						// place treasure
						var tile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						tile.transform.position = initialPosition + new Vector3(x * tileScale, 0, y * tileScale);
						tile.transform.localScale = initialScale;
						tile.GetComponent<MeshRenderer>().material.color = Color.yellow;
						tile.AddComponent<HitCollider>();
					}
				}
			}

			_isPlaced = true;
		}

		public short[] GetNeighbours(int x, int y)
		{
			return new short[8]
			{
				GetTileAt(x - 1, y - 1),
				GetTileAt(x    , y - 1),
				GetTileAt(x + 1, y - 1),
				GetTileAt(x - 1, y    ),
				GetTileAt(x + 1, y    ),
				GetTileAt(x - 1, y + 1),
				GetTileAt(x    , y + 1),
				GetTileAt(x + 1, y + 1),
			};
		}

		public short[] GetCardinalNeighbours(int x, int y)
		{
			return new short[4]
			{
				GetTileAt(x    , y - 1),
				GetTileAt(x - 1, y    ),
				GetTileAt(x + 1, y    ),
				GetTileAt(x    , y + 1),
			};
		}

		public short[] GetDiagonalNeighbours(int x, int y)
		{
			return new short[4]
			{
				GetTileAt(x - 1, y - 1),
				GetTileAt(x + 1, y - 1),
				GetTileAt(x - 1, y + 1),
				GetTileAt(x + 1, y + 1),
			};
		}

		public short GetTileAt(int x, int y)
		{
			if (x > 0 && x < ChunkDimension &&
			    y > 0 && y < ChunkDimension)
			{
				return _chunkData[x][y];
			}
			else
			{
				return ChunkData.OPEN;
			}
		}
	}

	public static class ChunkData
	{
		public const int ChunkDimension = 8;

		// Open = O = 0
		public const short OPEN = 0;
		public const short O = OPEN;

		// Solid = S = 1
		public const short SOLID = 1;
		public const short S = SOLID;

		// Random = R = 2
		public const short RANDOM = 2;
		public const short R = RANDOM;

		public const short TREASURE = 10;
		public const short T = TREASURE;


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

		public static short[][] SquareRoom()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,S,S,S,S,S,S,S}
			};
		}

		public static short[][] TreasureRoom()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,O,O,S,S,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,T,T,O,O,S},
				new short[ChunkDimension] {O,O,T,T,T,T,O,O},
				new short[ChunkDimension] {O,O,T,T,T,T,O,O},
				new short[ChunkDimension] {S,O,O,T,T,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,S,S,O,O,S,S,S}
			};
		}

		public static short[][] SquareRoomEmpty()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,O,O,S,S,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,O},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,S,S,O,O,S,S,S}
			};
		}

		public static short[][] DiamondRoomEmpty()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,S,S,S,S,S},
				new short[ChunkDimension] {S,S,S,O,O,O,S,S},
				new short[ChunkDimension] {S,S,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,O,O,S},
				new short[ChunkDimension] {S,O,O,O,O,O,S,S},
				new short[ChunkDimension] {S,S,O,O,O,S,S,S},
				new short[ChunkDimension] {S,S,S,O,S,S,S,S}
			};
		}
		public static short[][] SquareRoomRandom()
		{
			return new short[ChunkDimension][]
			{
				new short[ChunkDimension] {S,S,S,R,O,R,R,S},
				new short[ChunkDimension] {S,O,O,O,O,O,R,R},
				new short[ChunkDimension] {S,R,R,R,R,O,O,R},
				new short[ChunkDimension] {O,O,R,O,O,O,O,O},
				new short[ChunkDimension] {O,O,O,O,O,R,O,O},
				new short[ChunkDimension] {R,O,O,R,R,R,O,S},
				new short[ChunkDimension] {R,R,O,O,O,R,R,S},
				new short[ChunkDimension] {S,R,R,R,O,S,S,S}
			};
		}
	}
}
