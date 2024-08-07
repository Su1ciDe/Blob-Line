using GridSystem;
using UnityEngine;

namespace Interfaces
{
	public interface INode
	{
		public GridCell CurrentGridCell { get; set; }
		public void Init(GridCell placedCell);
		public Transform GetTransform();
	}
}