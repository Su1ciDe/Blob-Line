using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities.Extensions;
using GamePlay.Blobs;
using UnityEngine;
using UnityEngine.Events;

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

		public static event UnityAction<List<Blob>> OnLineComplete;

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
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Blob blob))
			{
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
			BlobsInLine.Add(blob);
			blob.OnAddedToLine();
			CurrentSelectedBlob = blob;

			var blobPos = blob.transform.position;
			lineRenderer.Queue(blobPos);
			fakeLineRenderer.SetPosition(0, blobPos);
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
		}

		private void OnInputDown(Vector3 pos)
		{
			transform.position = pos;
		}

		private void OnInputMove(Vector3 pos)
		{
			transform.position = pos;

			if (BlobsInLine.Count > 0)
			{
				fakeLineRenderer.SetPosition(1, transform.position);
			}
		}

		private void OnInputUp(Vector3 pos)
		{
			if (BlobsInLine.Count > 2)
			{
				OnLineComplete?.Invoke(BlobsInLine);
			}

			lineRenderer.Clear();
			fakeLineRenderer.SetPosition(0, Vector3.zero);
			fakeLineRenderer.SetPosition(1, Vector3.zero);
			BlobsInLine.Clear();
		}

		private void OnLevelStarted()
		{
			lineRenderer.Clear();
			BlobsInLine.Clear();
		}

		private void OnLevelWon()
		{
			BlobsInLine.Clear();
		}

		private void OnLevelLost()
		{
			BlobsInLine.Clear();
		}
	}
}