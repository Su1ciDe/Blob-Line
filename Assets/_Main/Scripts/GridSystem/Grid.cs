using UnityEngine;

namespace GridSystem
{
	public class Grid : MonoBehaviour
	{
		[SerializeField] private Vector2Int size = new Vector2Int(4, 8);
		[SerializeField] private Vector2 nodeSize;
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		[SerializeField] private GridCell cellPrefab;
		
		private GridCell[,] gridCells;
		public GridCell[,] GridCells => gridCells;

		private void Awake()
		{
			Setup();
		}

		private void Setup()
		{
			gridCells = new GridCell[size.x, size.y];

			var xOffset = (nodeSize.x * size.x + xSpacing * (size.x - 1)) / 2f - nodeSize.x / 2f;
			var yOffset = (nodeSize.y * size.y + ySpacing * (size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var cell = Instantiate(cellPrefab, transform);
					cell.Setup(x, y, nodeSize);
					cell.gameObject.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
					gridCells[x, y] = cell;
				}
			}
		}
	}
}