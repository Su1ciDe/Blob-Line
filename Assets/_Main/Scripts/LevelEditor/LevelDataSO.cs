using System;
using System.Collections.Generic;
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

		[Group("Randomizer"), Button]
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

		[TableList(Draggable = false, AlwaysExpanded = true, HideAddButton = true, HideRemoveButton = true, ShowElementLabels = false)]
		[SerializeField] private List<GoalCount> goalCounts;

		[Serializable]
		[DeclareHorizontalGroup("spawner")]
		public class GridSpawnerOptions
		{
			[GUIColor("$GetColor"), ValidateInput(nameof(ValidateCellType))]
			[Group("spawner")] public CellType CellType = CellType.Blue;
			[Range(1, 100)]
			[Group("spawner")] public int Weight = 1;
			[Group("spawner"), DisplayAsString, HideLabel] public string Chance;

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
						CellType.FilledGrid => Color.white,
						CellType.Empty => Color.white,
						CellType.StaticObstacle => Color.white,
						CellType.C_BreakableObstacle => Color.white,
						_ => throw new ArgumentOutOfRangeException()
					};

					return color;
				}
			}

			private TriValidationResult ValidateCellType()
			{
				if (CellType is CellType.C_BreakableObstacle or CellType.StaticObstacle or CellType.Empty or CellType.FilledGrid)
					return TriValidationResult.Error("This value must be color!");

				return TriValidationResult.Valid;
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
			[GUIColor("$GetColor"), ValidateInput(nameof(ValidateCellType))]
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
						CellType.FilledGrid => Color.white,
						CellType.Empty => Color.white,
						CellType.StaticObstacle => Color.white,
						CellType.C_BreakableObstacle => Color.white,
						_ => throw new ArgumentOutOfRangeException()
					};

					return color;
				}
			}

			private TriValidationResult ValidateCellType()
			{
				if (GaolColor is CellType.C_BreakableObstacle or CellType.StaticObstacle or CellType.Empty or CellType.FilledGrid)
					return TriValidationResult.Error("This value must be color!");

				return TriValidationResult.Valid;
			}
		}

		[Serializable]
		private class GoalCount
		{
			[GUIColor("$GetColor")]
			[ReadOnly] public CellType GaolColor;
			[ReadOnly] public int TotalCount;

			public GoalCount(CellType gaolColor, int totalCount)
			{
				GaolColor = gaolColor;
				TotalCount = totalCount;
			}

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
						CellType.FilledGrid => Color.white,
						CellType.Empty => Color.white,
						CellType.StaticObstacle => Color.white,
						CellType.C_BreakableObstacle => Color.white,
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
			{
				totalWeight += gridSpawnerOptions.Weight;
			}

			foreach (var gridSpawnerOption in GridSpawner)
			{
				gridSpawnerOption.Chance = ((float)gridSpawnerOption.Weight / totalWeight * 100).ToString("F2") + "%";
			}

			CalculateGoalCount();
		}

		private void CalculateGoalCount()
		{
			goalCounts.Clear();
			foreach (var goalOption in Goals)
			{
				var found = false;

				var goalCount = goalCounts.Where(x => x.GaolColor == goalOption.GaolColor);
				foreach (var count in goalCount)
				{
					count.TotalCount += goalOption.Count;
					found = true;
				}

				if (!found)
				{
					goalCounts.Add(new GoalCount(goalOption.GaolColor, goalOption.Count));
				}
			}
		}
	}
}