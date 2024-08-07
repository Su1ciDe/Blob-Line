using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.Utilities.Extensions;
using GamePlay.Blobs;
using GamePlay.Obstacles;
using GoalSystem;
using HolderSystem;
using LevelEditor;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace GridSystem
{
	public class Grid : Singleton<Grid>
	{
		public GridCell[,] GridCells => gridCells;
		private GridCell[,] gridCells;

		[SerializeField] private Vector2 nodeSize;
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		[SerializeField] private GridCell cellPrefab;
		private Vector2Int size = new Vector2Int(4, 8);

		[Space]
		[SerializeField] private Blob blobPrefab;
		[Space]
		[SerializeField] private ObstaclesSO obstaclesSO;

		private float xOffset, yOffset;

		public static event UnityAction OnFallFillFinish;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;

			GoalManager.OnGoalSequenceComplete += OnGoalSequenceCompleted;
			HolderManager.OnHolderSequenceComplete += OnHolderSequenceCompleted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;

			GoalManager.OnGoalSequenceComplete -= OnGoalSequenceCompleted;
			HolderManager.OnHolderSequenceComplete -= OnHolderSequenceCompleted;
		}

		private void OnLevelLoaded()
		{
			Setup(LevelManager.Instance.CurrentLevelData.Grid);
		}

		public void Setup(Array2DGrid grid)
		{
			size = grid.GridSize;
			gridCells = new GridCell[size.x, size.y];

			xOffset = (nodeSize.x * size.x + xSpacing * (size.x - 1)) / 2f - nodeSize.x / 2f;
			yOffset = (nodeSize.y * size.y + ySpacing * (size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var gridCell = grid.GetCell(x, y);
					// if (gridCell == CellType.Empty) continue;

					var cell = Instantiate(cellPrefab, transform);
					cell.Setup(x, y, nodeSize, gridCell);
					cell.gameObject.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
					gridCells[x, y] = cell;

					if (gridCell is CellType.StaticObstacle or CellType.C_BreakableObstacle)
					{
						var obstacle = Instantiate(obstaclesSO.Obstacles[gridCell], cell.transform);
						obstacle.Init(cell);
					}
					else if (gridCell != CellType.Empty)
					{
						var blob = Instantiate(blobPrefab, cell.transform);
						blob.Setup(gridCell, cell);
					}
				}
			}
		}

		public void Fall()
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = size.y - 1; y >= 0; y--)
				{
					var cell = gridCells[x, y];
					if (!cell) continue;
					if (cell.CellType == CellType.Empty) continue;
					if (cell.CurrentNode is not Blob blob) continue;

					// Check if there is any empty cell under
					int emptyY = GetFirstEmptyRow(x, y);
					if (emptyY.Equals(blob.CurrentGridCell.Coordinates.y)) continue;
					blob.SwapCell(gridCells[x, emptyY]);
					blob.Fall(gridCells[x, emptyY].transform.position);
				}
			}

			FallUnderObstacles();
		}

		private void FallUnderObstacles()
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = size.y - 1; y >= 0; y--)
				{
					var cell = gridCells[x, y];
					if (!cell) continue;
					if (cell.CellType == CellType.Empty) continue;
					if (cell.CurrentNode is not Blob blob) continue;

					// Check if there is any empty cell under
					int emptyY = GetFirstEmptyRow(x, y);
					blob.SwapCell(gridCells[x, emptyY]);
					var pos = gridCells[x, emptyY].transform.position;

					var obstacle = GetObstacleInColumn(x);
					if (obstacle)
					{
						int emptyObstacleY = GetFirstEmptyRow(obstacle.CurrentGridCell.Coordinates);
						if (emptyY < emptyObstacleY)
						{
							blob.Fall(pos, gridCells[obstacle.CurrentGridCell.Coordinates.x, emptyObstacleY].transform.position);
							blob.SwapCell(gridCells[obstacle.CurrentGridCell.Coordinates.x, emptyObstacleY]);
						}
						else
						{
							blob.Fall(pos);
						}
					}
					else
					{
						blob.Fall(pos);
					}
				}
			}
		}

		public void Fill()
		{
			var types = LevelManager.Instance.CurrentLevelData.GridSpawner.Select(x => x.CellType).ToList();
			var weights = LevelManager.Instance.CurrentLevelData.GridSpawner.Select(x => x.Weight).ToList();

			FillEmptyCells(types, weights);
			FallUnderObstacles();
			FillEmptyCells(types, weights);
		}

		private void FillEmptyCells(List<CellType> cellTypes, List<int> weights)
		{
			for (int x = 0; x < size.x; x++)
			{
				var emptyRowCount = GetEmptyRows(x);
				for (int i = 0; i < emptyRowCount; i++)
				{
					var emptyCellY = GetFirstEmptyRow(x, 0);

					var blob = SpawnRandomBlob(cellTypes, weights);
					blob.transform.position = gridCells[x, 0].transform.position + new Vector3(0, blob.PositionOffset.y, 1.5f);

					blob.PlaceToCell(gridCells[x, emptyCellY]);
					blob.Fall(gridCells[x, emptyCellY].transform.position);
				}
			}
		}

		private Blob SpawnRandomBlob(List<CellType> cellTypes, List<int> weights)
		{
			var randomType = cellTypes.WeightedRandom(weights);
			var blob = Instantiate(blobPrefab);
			blob.Setup(randomType);
			return blob;
		}

		private void OnGoalSequenceCompleted()
		{
			Fall();
			Fill();

			StartCoroutine(WaitFallFill());
		}

		private void OnHolderSequenceCompleted()
		{
			Fall();
			Fill();

			StartCoroutine(WaitFallFill());
		}

		private IEnumerator WaitFallFill()
		{
			yield return new WaitUntil(() => !IsAnyBlobFalling());
			yield return null;
			yield return new WaitUntil(() => !IsAnyBlobFalling());

			OnFallFillFinish?.Invoke();
		}

		public void BubbleFeedback(List<Blob> blobs)
		{
			var tempBlobs = new List<Blob>(blobs);
			StartCoroutine(BubbleFeedbackCoroutine(tempBlobs));
		}

		private readonly WaitForSeconds waitFeedback = new WaitForSeconds(0.05f);

		private IEnumerator BubbleFeedbackCoroutine(List<Blob> blobs)
		{
			for (int i = 0; i < blobs.Count; i++)
			{
				blobs[i].PopBubble(i);
				yield return waitFeedback;
			}
		}

		private bool IsAnyBlobFalling()
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					if (gridCells[x, y]?.CurrentNode is Blob { IsFalling: true })
					{
						return true;
					}
				}
			}

			return false;
		}

		public int GetEmptyRows(int x)
		{
			int count = 0;
			for (int y = 0; y < size.y; y++)
			{
				if (gridCells[x, y].CellType != CellType.Empty && gridCells[x, y].CurrentNode is null)
				{
					count++;
				}
				else if (gridCells[x, y].CurrentNode is BaseObstacle)
				{
					break;
				}
			}

			return count;
		}

		private int GetFirstEmptyRow(int x, int y)
		{
			var yy = y;
			for (var i = y + 1; i < size.y; i++)
			{
				if (gridCells[x, i].CellType == CellType.Empty)
				{
					if (gridCells[x, i + 1].CurrentNode is not null) break;
					yy++;
					continue;
				}

				if (gridCells[x, i].CurrentNode is not null) break;
				yy++;
			}

			return yy;
		}

		private int GetFirstEmptyRow(Vector2Int coordinates)
		{
			return GetFirstEmptyRow(coordinates.x, coordinates.y);
		}

		private BaseObstacle GetObstacleInColumn(int x)
		{
			int xx;
			if (x - 1 >= 0)
			{
				xx = x - 1;
				for (int y = size.y - 1; y >= 0; y--)
				{
					if (gridCells[xx, y] && gridCells[xx, y].CurrentNode is not null && gridCells[xx, y].CurrentNode is BaseObstacle obstacle && y + 1 <= size.y &&
					    gridCells[xx, y + 1].CurrentNode is null)
					{
						return obstacle;
					}
				}
			}

			if (x + 1 < size.x)
			{
				xx = x + 1;

				for (int y = size.y - 1; y >= 0; y--)
				{
					if (gridCells[xx, y] && gridCells[xx, y].CurrentNode is not null && gridCells[xx, y].CurrentNode is BaseObstacle obstacle && y + 1 <= size.y &&
					    gridCells[xx, y + 1].CurrentNode is null)
					{
						return obstacle;
					}
				}
			}

			return null;
		}
	}
}