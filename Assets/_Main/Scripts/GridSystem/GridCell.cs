using Interfaces;
using LevelEditor;
using UnityEngine;

namespace GridSystem
{
	[SelectionBase]
	public class GridCell : MonoBehaviour
	{
		public Vector2Int Coordinates { get; private set; }
		public CellType CellType { get; private set; }

		public INode CurrentNode { get; set; }

		public void Setup(int x, int y, Vector2 size, CellType cellType)
		{
			Coordinates = new Vector2Int(x, y);
			CellType = cellType;
			if (CellType == CellType.Empty)
				transform.GetChild(0).gameObject.SetActive(false);
		}
	}
}