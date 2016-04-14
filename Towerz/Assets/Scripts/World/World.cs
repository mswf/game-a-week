using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World
{
	private Dictionary<Vector2s, WorldSlice> worldSlices;


	public World()
	{
		worldSlices = new Dictionary<Vector2s, WorldSlice>();




	}

	public int[][] GenerateBlocks(Vector2s position)
	{
		return new int[WorldSlice.DIMENSIONS][]
		{
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],
			new int[WorldSlice.DIMENSIONS],	
		};
	}

	public KeyValuePair<Vector2s, WorldSlice>[] GetNeighbours(Vector2s pos)
	{

		var pos0 = new Vector2s((short)(pos.x - 1), (short)(pos.y - 1));
		var pos1 = new Vector2s((short)(pos.x), (short)(pos.y - 1));
		var pos2 = new Vector2s((short)(pos.x + 1), (short)(pos.y - 1));
		var pos3 = new Vector2s((short)(pos.x - 1), (short)(pos.y));
		var pos4 = new Vector2s((short)(pos.x + 1), (short)(pos.y));
		var pos5 = new Vector2s((short)(pos.x - 1), (short)(pos.y + 1));
		var pos6 = new Vector2s((short)(pos.x), (short)(pos.y + 1));
		var pos7 = new Vector2s((short)(pos.x + 1), (short)(pos.y + 1));

		return new KeyValuePair<Vector2s, WorldSlice>[8]
		{
				new KeyValuePair<Vector2s, WorldSlice> (pos0, this[pos0]),
				new KeyValuePair<Vector2s, WorldSlice> (pos1, this[pos1]),
				new KeyValuePair<Vector2s, WorldSlice> (pos2, this[pos2]),
				new KeyValuePair<Vector2s, WorldSlice> (pos3, this[pos3]),
				new KeyValuePair<Vector2s, WorldSlice> (pos4, this[pos4]),
				new KeyValuePair<Vector2s, WorldSlice> (pos5, this[pos5]),
				new KeyValuePair<Vector2s, WorldSlice> (pos6, this[pos6]),
				new KeyValuePair<Vector2s, WorldSlice> (pos7, this[pos7]),
		};
	}

	public KeyValuePair<Vector2s, WorldSlice>[] GetNeighbours(WorldSlice worldSlice)
	{
		return GetNeighbours(worldSlice.position);
	}

	public WorldSlice this[Vector2s position]
	{
		get
		{
			if (worldSlices.ContainsKey(position))
				return worldSlices[position];

			return null;
		}
		set
		{
			if (!worldSlices.ContainsKey(position))
				worldSlices[position] = value;
			else
			{
				// Check if same position
				foreach (var positionKey in worldSlices.Keys)
				{
					if (positionKey.Equals(positionKey, position))
					{
						Debug.LogError("Tried to place duplicate chunk at x:" + position.x + ", y:" + position.y);
						return;
					}
				}
				// Actually a hash collision, let's find it
				foreach (var positionKey in worldSlices.Keys)
				{
					if (positionKey.GetHashCode() == position.GetHashCode())
					{
						Debug.LogError("Hash collision for pos x:" + position.x + ", y:" + position.y + " && x:" + positionKey.x + ", y:" + positionKey.y);
						return;
					}
				}

			}


		}
	}
}
