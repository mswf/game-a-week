using UnityEngine;
using System.Collections;


public class WorldSlice
{
	public enum GenerationStatus
	{
		None,
		Data,
		Spawned
	}

	public const int DIMENSIONS = 16;

	private World _world;
	private int[][] _blocks;

	public readonly Vector2s position;

	private WorldSlice()
	{}

	public WorldSlice(World world, Vector2s position)
	{
		this._world = world;
		this.position = position;

		this._blocks = world.GenerateBlocks(position);
	}

}
