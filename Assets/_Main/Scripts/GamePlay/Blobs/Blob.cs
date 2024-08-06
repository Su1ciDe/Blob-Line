using System.Collections;
using DG.Tweening;
using Fiber.Managers;
using GridSystem;
using Interfaces;
using LevelEditor;
using UnityEngine;

namespace GamePlay.Blobs
{
	[SelectionBase]
	public class Blob : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }
		public CellType CellType { get; private set; }

		public bool IsMoving { get; private set; }
		public bool IsFalling { get; private set; }

		[SerializeField] private Transform model;
		[SerializeField] private Renderer[] renderers;

		[Space]
		[SerializeField] private Vector3 positionOffset = new Vector3(0, 0.5f, 0);

		public static float JUMP_POWER = 3;
		public static float JUMP_DURATION = .25F;
		private const float ANIM_DURATION = .35F;

		public void Setup(CellType cellType, GridCell cell)
		{
			CellType = cellType;
			Place(cell);

			var mat = GameManager.Instance.BlobMaterialsSO.BlobMaterials[cellType];
			foreach (var r in renderers)
				r.material = mat;
		}

		public void Place(GridCell placedCell)
		{
			CurrentGridCell = placedCell;
			CurrentGridCell.CurrentNode = this;

			transform.SetParent(placedCell.transform);
			transform.localPosition = positionOffset;
		}

		public Transform GetTransform() => transform;

		public void OnAddedToLine()
		{
			model.DOKill();
			model.transform.localScale = 0.5f * Vector3.one;
			model.DOScale(Vector3.one, ANIM_DURATION).SetEase(Ease.OutElastic, 3f);
		}

		public void OnRemovedFromLine()
		{
			model.DOComplete();
			model.DOScale(1.25f * Vector3.one, ANIM_DURATION / 2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
		}

		public void OnJumpToGoal()
		{
			IsMoving = true;

			CurrentGridCell.CurrentNode = null;
		}

		public void OnEnterToGoal()
		{
			IsMoving = false;
		}

		public Tween JumpTo(Vector3 position)
		{
			return transform.DOJump(position, JUMP_POWER, 1, JUMP_DURATION);
		}

		public void SwapCell(GridCell cell)
		{
			CurrentGridCell.CurrentNode = null;

			CurrentGridCell = cell;
			CurrentGridCell.CurrentNode = this;

			transform.SetParent(cell.transform);
		}

		private const float FALL_SPEED = 20f;
		private const float ACCELERATION = .5f;
		private float velocity;

		public void Fall(Vector3 position, Vector3? secondPosition = null)
		{
			StartCoroutine(FallCoroutine(position, secondPosition));
		}

		private IEnumerator FallCoroutine(Vector3 position, Vector3? secondPosition)
		{
			yield return new WaitUntil(() => !IsFalling);

			IsFalling = true;

			var currentPos = transform.position;
			while (currentPos.z > position.z)
			{
				velocity += ACCELERATION;
				velocity = velocity >= FALL_SPEED ? FALL_SPEED : velocity;

				currentPos = transform.position;

				currentPos.z -= velocity * Time.deltaTime;
				transform.position = currentPos;
				yield return null;
			}

			currentPos.z = position.z;
			transform.position = currentPos;
			velocity = 0;

			if (secondPosition is not null)
				yield return StartCoroutine(FallToTheSecondPosition((Vector3)secondPosition));

			IsFalling = false;
		}

		public IEnumerator FallToTheSecondPosition(Vector3 secondPosition)
		{
			yield return transform.DOMove(secondPosition + positionOffset, FALL_SPEED / 2f).SetSpeedBased(true).SetEase(Ease.OutSine).WaitForCompletion();
		}
	}
}