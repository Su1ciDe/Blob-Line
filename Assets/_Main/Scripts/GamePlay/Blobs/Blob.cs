using Fiber.Managers;
using GridSystem;
using Interfaces;
using LevelEditor;
using UnityEngine;

namespace GamePlay.Blobs
{
	public class Blob : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }
		public CellType CellType { get; private set; }

		[SerializeField] private MeshRenderer meshRenderer;

		public void Setup(CellType cellType, GridCell cell)
		{
			CellType = cellType;
			Place(cell);

			meshRenderer.material = GameManager.Instance.BlobMaterialsSO.BlobMaterials[cellType];
		}

		public void Place(GridCell placedCell)
		{
			CurrentGridCell = placedCell;
			CurrentGridCell.CurrentNode = this;

			transform.SetParent(placedCell.transform);
			transform.localPosition = Vector3.zero;
		}
	}
}