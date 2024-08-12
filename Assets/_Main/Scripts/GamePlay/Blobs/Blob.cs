using System.Collections;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.AudioSystem;
using GamePlay.Obstacles;
using GridSystem;
using HolderSystem;
using Interfaces;
using LevelEditor;
using Lofelt.NiceVibrations;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace GamePlay.Blobs
{
	[SelectionBase]
	public class Blob : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }
		public Holder CurrentHolder { get; set; }
		public CellType CellType { get; private set; }

		public bool IsMoving { get; private set; }
		public bool IsFalling { get; private set; }
		public bool IsInGrid { get; private set; }

		[SerializeField] private Transform model;
		[SerializeField] private GameObject bubble;
		[SerializeField] private Renderer[] renderers;
		[SerializeField] private Animator animator;

		[Space]
		[SerializeField] private Vector3 positionOffset = new Vector3(0, 0.5f, 0);
		public Vector3 PositionOffset => positionOffset;

		[Space]
		[SerializeField] private AnimationCurve jumpCurve;

		public static float JUMP_POWER = 10;
		public static float JUMP_DURATION = .5F;
		private const float ANIM_DURATION = .35F;

		public void Setup(CellType cellType, GridCell cell = null)
		{
			CellType = cellType;
			IsInGrid = true;
			if (cell)
				Init(cell);

			var mat = GameManager.Instance.BlobMaterialsSO.BlobMaterials[cellType];
			foreach (var r in renderers)
				r.material = mat;
		}

		public void Init(GridCell placedCell)
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
			model.DOScale(1.1f, ANIM_DURATION).SetEase(Ease.OutElastic);
		}

		public void OnRemovedFromLine()
		{
			model.DOComplete();
			model.transform.localScale = Vector3.one;
			model.DOScale(0.8f * Vector3.one, ANIM_DURATION / 4f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
		}

		public void PopBubble(int index, bool toGoal = false)
		{
			AudioManager.Instance.PlayAudio(AudioName.Pop1).SetPitch(0.75f + index * 0.05f);
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);
			ParticlePooler.Instance.Spawn("Bubble", transform.position);

			if (toGoal)
			{
				animator.SetBool(armsLegs, true);
			}

			bubble.SetActive(false);
		}

		public void OnJumpToGoal()
		{
			IsInGrid = false;
			IsMoving = true;

			transform.DOScale(0.75f, JUMP_DURATION);
			animator.SetBool(armsLegs, true);

			if (CurrentGridCell)
			{
				CheckBreakableObstacle();
				CurrentGridCell.CurrentNode = null;
			}

			if (CurrentHolder)
			{
				CurrentHolder.Blobs.Remove(this);
				CurrentHolder = null;
			}
		}

		public void OnJumpToHolder(Holder holder)
		{
			IsInGrid = false;
			animator.SetBool(armsLegs, false);

			if (CurrentHolder)
				CurrentHolder.Blobs.Remove(this);
			CurrentHolder = holder;

			if (CurrentGridCell)
			{
				CheckBreakableObstacle();
				CurrentGridCell.CurrentNode = null;
				CurrentGridCell = null;
			}
		}

		public void OnEnterToGoal()
		{
			IsMoving = false;
		}

		public Tween JumpTo(Vector3 position)
		{
			return transform.DOJump(position, JUMP_POWER, 1, JUMP_DURATION).SetEase(jumpCurve);
		}

		public void SwapCell(GridCell cell)
		{
			if (CurrentGridCell)
				CurrentGridCell.CurrentNode = null;

			PlaceToCell(cell);
		}

		public void PlaceToCell(GridCell cell)
		{
			CurrentGridCell = cell;
			CurrentGridCell.CurrentNode = this;

			transform.SetParent(cell.transform);
		}

		private const float FALL_SPEED = 20f;
		private const float ACCELERATION = .5f;
		private float velocity;
		private static readonly int armsLegs = Animator.StringToHash("ArmsLegs");

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

		private void CheckBreakableObstacle()
		{
			var gridCells = Grid.Instance.GridCells;
			if (CurrentGridCell.Coordinates.x - 1 >= 0 && gridCells[CurrentGridCell.Coordinates.x - 1, CurrentGridCell.Coordinates.y].CurrentNode is BreakableObstacle leftObstacle)
			{
				leftObstacle.OnBlastNear(this);
			}

			if (CurrentGridCell.Coordinates.x + 1 < gridCells.GetLength(0) &&
			    gridCells[CurrentGridCell.Coordinates.x + 1, CurrentGridCell.Coordinates.y].CurrentNode is BreakableObstacle rightObstacle)
			{
				rightObstacle.OnBlastNear(this);
			}

			if (CurrentGridCell.Coordinates.y - 1 >= 0 && gridCells[CurrentGridCell.Coordinates.x, CurrentGridCell.Coordinates.y - 1].CurrentNode is BreakableObstacle upObstacle)
			{
				upObstacle.OnBlastNear(this);
			}

			if (CurrentGridCell.Coordinates.y + 1 < gridCells.GetLength(1) && gridCells[CurrentGridCell.Coordinates.x, CurrentGridCell.Coordinates.y + 1].CurrentNode is BreakableObstacle downObstacle)
			{
				downObstacle.OnBlastNear(this);
			}
		}
	}
}