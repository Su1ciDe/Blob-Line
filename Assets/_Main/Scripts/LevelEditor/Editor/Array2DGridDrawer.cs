using Array2DEditor;
using UnityEditor;

namespace LevelEditor
{
	public class Array2DGridDrawer
	{
		[CustomPropertyDrawer(typeof(Array2DGrid))]
		public class Array2DExampleEnumDrawer : Array2DEnumDrawer<CellType>
		{
		}
	}
}