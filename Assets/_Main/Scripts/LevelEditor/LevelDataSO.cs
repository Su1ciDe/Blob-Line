using System;
using System.Linq;
using Fiber.Utilities.Extensions;
using TriInspector;
using UnityEngine;

namespace LevelEditor
{
	[CreateAssetMenu(fileName = "Level_000", menuName = "Blob Line/Level Data", order = 0)]
	[DeclareFoldoutGroup("Randomizer")]
	[DeclareBoxGroup("Goal")]
	public class LevelDataSO : ScriptableObject
	{
		[Title("Grid")]
		public Array2DGrid Grid;

		[Space]
		[Group("Randomizer"), TableList] [SerializeField] private RandomCell[] randomCells;

		[Group("Randomizer"), Button(ButtonSizes.Medium)]
		private void Randomize()
		{
			var randomCellTypes = randomCells.Select(x => x.CellType).ToList();
			var randomCellWeights = randomCells.Select(y => y.Weight).ToList();

			var size = Grid.GridSize;
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					Grid.SetCell(x, y, randomCellTypes.WeightedRandom(randomCellWeights));
				}
			}
		}

		[Title("Grid Spawner")]
		public GridSpawnerOptions[] GridSpawner;
		[SerializeField, DisplayAsString] private int totalWeight;

		[Title("Goals"),InlineProperty]
		public GoalStage[] GoalStages;

		[Serializable]
		public class GridSpawnerOptions
		{
			public CellType CellType = CellType.Grid;
			[Range(1, 100)]
			public int Weight = 1;
		}

		[Serializable]
		private struct RandomCell
		{
			public CellType CellType;
			[Range(1, 100)]
			public int Weight;
		}

		[Serializable]
		public class GoalStage
		{
			[TableList(Draggable = true, AlwaysExpanded = true)]
			public GoalOptions[] Goals;

			[Serializable]
			public class GoalOptions
			{
				public CellType GaolColor = CellType.Blue;
				public int Count;
			}
		}

		private void OnValidate()
		{
			totalWeight = 0;
			foreach (var gridSpawnerOptions in GridSpawner)
				totalWeight += gridSpawnerOptions.Weight;
		}
	}
}