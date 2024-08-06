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

		[Title("Goals")]
		[Range(1, 4)] public int ActiveGoalCount = 1;
		[InlineProperty, TableList] public GoalOptions[] Goals;

		[Serializable]
		[DeclareHorizontalGroup("spawner")]
		public class GridSpawnerOptions
		{
			[GUIColor("$GetColor")]
			[Group("spawner")] public CellType CellType = CellType.Blue;
			[Range(1, 100)]
			[Group("spawner")] public int Weight = 1;

			private Color GetColor
			{
				get
				{
					var color = CellType switch
					{
						CellType.Blue => Color.blue,
						CellType.Green => Color.green,
						CellType.Orange => new Color(1f, 0.5f, 0),
						CellType.Pink => Color.magenta,
						CellType.X_Purple => new Color(.7f, .25f, 1f),
						CellType.Red => Color.red,
						CellType.Yellow => Color.yellow,
						CellType.Grid => Color.white,
						CellType.Empty => Color.white,
						CellType.BasicObstacle => Color.white,
						CellType.BreakableObstacle => Color.white,
						_ => throw new ArgumentOutOfRangeException()
					};

					return color;
				}
			}
		}

		[Serializable]
		private struct RandomCell
		{
			public CellType CellType;
			[Range(1, 100)]
			public int Weight;
		}

		[Serializable]
		public class GoalOptions
		{
			[GUIColor("$GetColor")]
			public CellType GaolColor = CellType.Blue;
			public int Count;

			private Color GetColor
			{
				get
				{
					var color = GaolColor switch
					{
						CellType.Blue => Color.blue,
						CellType.Green => Color.green,
						CellType.Orange => new Color(1f, 0.5f, 0),
						CellType.Pink => Color.magenta,
						CellType.X_Purple => new Color(.7f, .25f, 1f),
						CellType.Red => Color.red,
						CellType.Yellow => Color.yellow,
						CellType.Grid => Color.white,
						CellType.Empty => Color.white,
						CellType.BasicObstacle => Color.white,
						CellType.BreakableObstacle => Color.white,
						_ => throw new ArgumentOutOfRangeException()
					};

					return color;
				}
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