using GridSystem;

namespace Interfaces
{
	public interface INode
	{
		public GridCell CurrentGridCell { get; set; }
		public void Place(GridCell placedCell);
	}
}