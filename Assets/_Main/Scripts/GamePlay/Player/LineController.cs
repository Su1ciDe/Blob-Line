using System.Collections.Generic;
using Fiber.Managers;
using Fiber.AudioSystem;
using Fiber.Utilities.Extensions;
using GamePlay.Blobs;
using GoalSystem;
using LevelEditor;
using Lofelt.NiceVibrations;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;

namespace GamePlay.Player
{
	public class LineController : MonoBehaviour
	{
		public List<Blob> BlobsInLine { get; private set; } = new List<Blob>();
		public Blob CurrentSelectedBlob { get; private set; }

		[SerializeField] private LineRenderer lineRenderer;
		[SerializeField] private LineRenderer fakeLineRenderer;
		[SerializeField] private Collider col;

		[Space]
		[SerializeField] private float distanceThreshold = 2;

		public static event UnityAction<List<Blob>, Goal> OnLineToGoal;
		public static event UnityAction<List<Blob>> OnLineToHolder;
		public static event UnityAction<List<Blob>> OnLineComplete;
		public static event UnityAction<List<Blob>> OnBlobAddedToLine;
		public static event UnityAction<List<Blob>> OnBlobRemovedFromLine;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;

			PlayerInputs.OnDown += OnInputDown;
			PlayerInputs.OnMove += OnInputMove;
			PlayerInputs.OnUp += OnInputUp;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;

			PlayerInputs.OnDown -= OnInputDown;
			PlayerInputs.OnMove -= OnInputMove;
			PlayerInputs.OnUp -= OnInputUp;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Blob blob) && blob.IsInGrid)
			{
				if (TutorialManager.Instance && TutorialManager.Instance.TutorialColorPredicate != CellType.Empty && blob.CellType == TutorialManager.Instance.TutorialColorPredicate)
					return;

				if (!BlobsInLine.Contains(blob))
				{
					if (!CurrentSelectedBlob)
					{
						OnBlobAdded(blob);
					}
					else if (CurrentSelectedBlob.CellType == blob.CellType &&
					         Mathf.Abs((CurrentSelectedBlob.transform.position - blob.transform.position).sqrMagnitude) < distanceThreshold * distanceThreshold)
					{
						OnBlobAdded(blob);
					}
				}
				else if (BlobsInLine.IndexOf(blob) < BlobsInLine.Count - 1)
				{
					OnBlobRemoved(blob);
				}
			}
		}

		private void OnBlobAdded(Blob blob)
		{
			var color = GameManager.Instance.BlobMaterialsSO.BlobMaterials[blob.CellType].color;
			lineRenderer.startColor = lineRenderer.endColor = fakeLineRenderer.startColor = fakeLineRenderer.endColor = color;

			BlobsInLine.Add(blob);
			blob.OnAddedToLine();
			CurrentSelectedBlob = blob;

			var blobPos = blob.transform.position;
			lineRenderer.Queue(blobPos);
			fakeLineRenderer.SetPosition(0, blobPos);

			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);
			AudioManager.Instance.PlayAudio(AudioName.Pop2).SetPitch(.75f + BlobsInLine.Count * .1f);

			OnBlobAddedToLine?.Invoke(BlobsInLine);
		}

		private void OnBlobRemoved(Blob blob)
		{
			var index = BlobsInLine.IndexOf(blob);
			var count = BlobsInLine.Count;
			for (int i = count - 1; i > index; i--)
			{
				BlobsInLine[^1].OnRemovedFromLine();
				BlobsInLine.RemoveAt(BlobsInLine.Count - 1);
				lineRenderer.Pop();
			}

			CurrentSelectedBlob = BlobsInLine[^1];
			fakeLineRenderer.SetPosition(0, CurrentSelectedBlob.transform.position);

			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.SoftImpact);
			AudioManager.Instance.PlayAudio(AudioName.Pop2).SetPitch(.75f + BlobsInLine.Count * .1f);

			OnBlobRemovedFromLine?.Invoke(BlobsInLine);
		}

		private void OnInputDown(Vector3 pos)
		{
			col.enabled = true;
			transform.position = pos;
		}

		private void OnInputMove(Vector3 pos)
		{
			transform.position = pos;

			if (BlobsInLine.Count > 0)
			{
				var startPos = fakeLineRenderer.GetPosition(0);
				var dir = (transform.position - startPos).normalized;
				fakeLineRenderer.SetPosition(1, startPos + distanceThreshold * dir);
			}
		}

		private void OnInputUp(Vector3 pos)
		{
			if (TutorialManager.Instance && (TutorialManager.Instance.TutorialCountPredicate.Equals(0) || TutorialManager.Instance.TutorialCountPredicate.Equals(BlobsInLine.Count)))
			{
				if (BlobsInLine.Count > 2)
				{
					var goal = GoalManager.Instance.GetCurrentGoalByType(CurrentSelectedBlob.CellType);
					if (goal)
						OnLineToGoal?.Invoke(BlobsInLine, goal);
					else
						OnLineToHolder?.Invoke(BlobsInLine);

					Player.Instance.Inputs.CanInput = false;

					OnLineComplete?.Invoke(BlobsInLine);
				}
			}

			lineRenderer.Clear();
			fakeLineRenderer.SetPosition(0, Vector3.zero);
			fakeLineRenderer.SetPosition(1, Vector3.zero);
			for (var i = 0; i < BlobsInLine.Count; i++)
				BlobsInLine[i].OnRemovedFromLine();

			BlobsInLine.Clear();
			CurrentSelectedBlob = null;

			col.enabled = false;

			OnBlobRemovedFromLine?.Invoke(BlobsInLine);
		}

		private void OnLevelStarted()
		{
			CurrentSelectedBlob = null;
			lineRenderer.Clear();
			BlobsInLine.Clear();
		}

		private void OnLevelWon()
		{
			CurrentSelectedBlob = null;
			BlobsInLine.Clear();

			OnInputUp(default);
		}

		private void OnLevelLost()
		{
			CurrentSelectedBlob = null;
			BlobsInLine.Clear();

			OnInputUp(default);
		}
	}
}