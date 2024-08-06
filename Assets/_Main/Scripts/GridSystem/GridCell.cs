using Interfaces;
using UnityEngine;

namespace GridSystem
{
	[SelectionBase]
	public class GridCell : MonoBehaviour
	{
		public Vector2Int Coordinates { get; private set; }

		public INode CurrentNode { get; set; }

		public void Setup(int x, int y, Vector2 size)
		{
			Coordinates = new Vector2Int(x, y);
		}
	}
}