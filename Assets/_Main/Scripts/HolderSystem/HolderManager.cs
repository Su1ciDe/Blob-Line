using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Blobs;
using GamePlay.Player;
using GoalSystem;
using LevelEditor;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HolderSystem
{
	public class HolderManager : Singleton<HolderManager>
	{
		public bool IsBusy { get; private set; }

		[SerializeField] private int holderCount = 5;

		[Title("References")]
		[SerializeField] private Holder holderPrefab;

		public List<Holder> Holders { get; } = new List<Holder>();

		public const int MAX_STACK_COUNT = 10;

		public static event UnityAction OnHolderSequenceComplete;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += Setup;

			LineController.OnLineToHolder += OnBlobsToHolder;
			GoalManager.OnBeforeNewGoal += OnNewGoal;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= Setup;

			LineController.OnLineToHolder -= OnBlobsToHolder;
			GoalManager.OnBeforeNewGoal -= OnNewGoal;
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		private void Setup()
		{
			var offset = holderCount * holderPrefab.Size / 2f - holderPrefab.Size / 2f;
			for (int i = 0; i < holderCount; i++)
			{
				var holder = Instantiate(holderPrefab, transform);
				holder.transform.localPosition = new Vector3(i * holderPrefab.Size - offset, 0, 0);
				Holders.Add(holder);
			}
		}

		private readonly WaitForSeconds holderDelay = new WaitForSeconds(.1f);

		public void OnBlobsToHolder(List<Blob> blobs)
		{
			StartCoroutine(OnBlobsToHolderCoroutine(blobs));
		}

		private List<Blob> excessBlobs = new List<Blob>();

		private IEnumerator OnBlobsToHolderCoroutine(List<Blob> blobs)
		{
			var tempBlobs = new List<Blob>(blobs);
			var count = tempBlobs.Count;
			var holder = GetFirstHolder(tempBlobs[0].CellType);
			if (!holder)
			{
				StopCheckFail();
				checkFailCoroutine = StartCoroutine(CheckFail());
				excessBlobs = new List<Blob>(blobs);

				yield break;
			}

			for (int i = 0; i < count; i++)
			{
				if (holder.Blobs.Count < MAX_STACK_COUNT)
				{
					var blob = tempBlobs[0];

					blob.OnJumpToHolder(holder);
					holder.SetBlob(blob);
					tempBlobs.RemoveAt(0);
					blob.JumpTo(new Vector3(holder.transform.position.x, holder.transform.position.y + Holder.OFFSET * holder.Blobs.Count, holder.transform.position.z))
						.OnComplete(() => blob.OnEnterToHolder());

					yield return holderDelay;
				}
				else
				{
					// holder.Complete();
					StartCoroutine(OnBlobsToHolderCoroutine(tempBlobs));
					yield break;
				}
			}

			if (tempBlobs.Count.Equals(0))
			{
				OnHolderSequenceComplete?.Invoke();
			}
		}

		private Coroutine checkFailCoroutine;

		private IEnumerator CheckFail()
		{
			yield return new WaitForSeconds(2);
			LevelManager.Instance.Lose();

			checkFailCoroutine = null;
		}

		public void StopCheckFail()
		{
			if (checkFailCoroutine is not null)
			{
				StopCoroutine(checkFailCoroutine);
				checkFailCoroutine = null;
			}
		}

		private void OnNewGoal(Goal newGoal)
		{
			StartCoroutine(CheckForCompletedStacks(newGoal));
		}

		private IEnumerator CheckForCompletedStacks(Goal newGoal)
		{
			yield return new WaitUntil(() => !IsBusy);
			IsBusy = true;

			yield return new WaitForSeconds(0.5f);

			for (int i = holderCount - 1; i >= 0; i--)
			{
				if (Holders[i].Blobs.Count.Equals(0)) continue;
				if (Holders[i].Blobs[0].CellType != newGoal.CellType) continue;

				StopCheckFail();
				Holders[i].StopComplete();

				var blobsReversed = new List<Blob>(Holders[i].Blobs);
				blobsReversed.Reverse();
				GoalManager.Instance.OnBlobsToGoal(blobsReversed, newGoal);

				var goalCounter = newGoal.NeededAmount - newGoal.CurrentAmount;
				var holderCounter = blobsReversed.Count;
				var count = goalCounter < holderCounter ? goalCounter : holderCounter;
				yield return new WaitForSeconds(0.1f * count + Blob.JUMP_DURATION);
			}

			if (excessBlobs.Count > 0)
			{
				if (excessBlobs[0].CellType == newGoal.CellType)
				{
					GoalManager.Instance.OnBlobsToGoal(excessBlobs, newGoal);
				}
				else
				{
					OnBlobsToHolder(excessBlobs);
				}

				excessBlobs.Clear();
			}

			IsBusy = false;
		}

		public Holder GetFirstHolder(CellType cellType)
		{
			for (var i = 0; i < Holders.Count; i++)
			{
				var holder = Holders[i];
				if (holder.Blobs.Count.Equals(MAX_STACK_COUNT)) continue;
				if (holder.Blobs.Count.Equals(0))
				{
					return Holders[i];
				}
				else if (holder.Blobs[0].CellType == cellType)
				{
					return Holders[i];
				}
			}

			return null;
		}
	}
}