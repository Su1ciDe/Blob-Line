using DG.Tweening;
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

		public bool IsMoving { get; set; }

		[SerializeField] private Transform model;
		[SerializeField] private MeshRenderer meshRenderer;

		public static float JUMP_POWER = 2;
		public static float JUMP_DURATION = .25F;

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
			transform.localPosition = new Vector3(0, 0.5f, 0);
		}

		public void OnAddedToLine()
		{
			model.DOKill();
			model.transform.localScale = 0.5f * Vector3.one;
			model.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack, 3f);
		}

		public void OnRemovedFromLine()
		{
			model.DOKill();
			model.DOScale(Vector3.one, 0.25f).SetEase(Ease.InBack);
		}

		public void OnJumpToGoal()
		{
			IsMoving = true;
		}

		public void OnEnterToGoal()
		{
			IsMoving = false;
		}

		public Tween JumpTo(Vector3 position)
		{
			return transform.DOJump(position, JUMP_POWER, 1, JUMP_DURATION);
		}
	}
}