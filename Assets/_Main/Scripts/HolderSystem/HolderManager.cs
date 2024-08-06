using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Blobs;
using GamePlay.Player;
using LevelEditor;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HolderSystem
{
	public class HolderManager : Singleton<HolderManager>
	{
		[SerializeField] private int holderCount = 5;

		[Title("References")]
		[SerializeField] private Holder holderPrefab;

		private List<Holder> holders = new List<Holder>();

		public const int MAX_STACK_COUNT = 5;

		public static event UnityAction OnHolderSequenceComplete;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += Setup;

			LineController.OnLineToHolder += OnBlobsToHolder;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= Setup;

			LineController.OnLineToHolder -= OnBlobsToHolder;
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
				holders.Add(holder);
			}
		}

		private readonly WaitForSeconds holderDelay = new WaitForSeconds(.1f);

		public void OnBlobsToHolder(List<Blob> blobs)
		{
			StartCoroutine(OnBlobsToHolderCoroutine(blobs));
		}

		private IEnumerator OnBlobsToHolderCoroutine(List<Blob> blobs)
		{
			var tempBlobs = new List<Blob>(blobs);
			var count = tempBlobs.Count;
			var holder = GetFirstHolder(tempBlobs[0].CellType);
			if (!holder)
			{
				LevelManager.Instance.Lose();
				yield break;
			}

			for (int i = 0; i < count; i++)
			{
				if (holder.Blobs.Count < MAX_STACK_COUNT)
				{
					var blob = tempBlobs[0];
					holder.SetBlob(blob);
					tempBlobs.RemoveAt(0);
					blob.JumpTo(new Vector3(holder.transform.position.x, holder.transform.position.y + Holder.OFFSET * holder.Blobs.Count, holder.transform.position.z)).OnComplete(() =>
					{
						//
					});

					yield return holderDelay;
				}
				else
				{
					StartCoroutine(OnBlobsToHolderCoroutine(tempBlobs));
					yield break;
				}
			}

			if (tempBlobs.Count.Equals(0))
			{
				OnHolderSequenceComplete?.Invoke();
			}
		}

		public void CheckForCompletedStacks()
		{
			
		}

		public Holder GetFirstHolder(CellType cellType)
		{
			for (var i = 0; i < holders.Count; i++)
			{
				var holder = holders[i];
				if (holder.Blobs.Count.Equals(MAX_STACK_COUNT)) continue;
				if (holder.Blobs.Count.Equals(0))
				{
					return holders[i];
				}
				else if (holder.Blobs.Peek().CellType == cellType)
				{
					return holders[i];
				}
			}

			return null;
		}

		// public IEnumerable<Blob> GetBlobsByType(CellType cellType)
		// {
		// 	for (var i = 0; i < holders.Count; i++)
		// 	{
		// 		if (holders[i].Blob && holders[i].Blob.CellType == cellType)
		// 			yield return holders[i].Blob;
		// 	}
		// }
		//
		// public int GetEmptyHolderCount()
		// {
		// 	int count = 0;
		// 	for (var i = 0; i < holders.Count; i++)
		// 	{
		// 		if (!holders[i].Blob)
		// 			count++;
		// 	}
		//
		// 	return count;
		// }
	}
}