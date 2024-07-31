using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities.Extensions;
using GamePlay.Blobs;
using UnityEngine;

namespace GamePlay.Player
{
	public class LineController : MonoBehaviour
	{
		public List<Blob> BlobsInLine { get; private set; } = new List<Blob>();
		public Blob CurrentSelectedBlob { get; private set; }

		[SerializeField] private LineRenderer lineRenderer;
		[SerializeField] private LineRenderer fakeLineRenderer;
		[SerializeField] private Collider col;

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
					else if (CurrentSelectedBlob.CellType == blob.CellType)
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

		private void OnTriggerExit(Collider other)
		{
			// if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Blob blob))
			// {
			// 	if (BlobsInLine.Contains(blob) && BlobsInLine.IndexOf(blob).Equals(BlobsInLine.Count - 2))
			// 	{
			// 		BlobsInLine[^1].OnRemovedFromLine();
			// 		BlobsInLine.RemoveAt(BlobsInLine.Count - 1);
			// 	}
			// 	
			// 	 CurrentSelectedBlob = BlobsInLine.Count > 0 ? BlobsInLine[^1] : null;
			// }
		}

		private void OnBlobAdded(Blob blob)
		{
			BlobsInLine.Add(blob);
			blob.OnAddedToLine();
			CurrentSelectedBlob = blob;

			lineRenderer.Queue(blob.transform.position);
			// lineRenderer.SetPosition(BlobsInLine.Count - 1, blob.transform.position);
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
		}

		private void OnInputDown(Vector3 pos)
		{
			transform.position = pos;
			// lineRenderer.Queue();
		}

		private void OnInputMove(Vector3 pos)
		{
			transform.position = pos;

			if (BlobsInLine.Count > 0)
			{
				// lineRenderer.positionCount = BlobsInLine.Count + 1;
				// lineRenderer.SetPosition(BlobsInLine.Count, transform.position);

				// lineRenderer.GetPosition(lineRenderer.positionCount - 1);
			}
		}

		private void OnInputUp(Vector3 pos)
		{
			// TODO: goal

			lineRenderer.Clear();
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