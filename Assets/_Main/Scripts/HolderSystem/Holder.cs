using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GamePlay.Blobs;
using Interfaces;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : MonoBehaviour, ISlot
	{
		public List<Blob> Blobs { get; set; } = new List<Blob>();
		public int Index { get; set; }

		[SerializeField] private float size;
		public float Size => size;

		public static float OFFSET = .25f;

		public void SetBlob(Blob blob)
		{
			Blobs.Add(blob);

			if (Blobs.Count.Equals(HolderManager.MAX_STACK_COUNT))
			{
				Complete();
			}
		}

		public Transform GetTransform() => transform;

		private void Complete()
		{
			CompleteAsync();
		}

		private readonly CancellationTokenSource completeCancellationToken = new CancellationTokenSource();

		public void StopComplete()
		{
			completeCancellationToken.Cancel();
		}

		private async void CompleteAsync()
		{
			try
			{
				await UniTask.WaitUntil(() => !Blobs.Any(x => x.IsMoving), PlayerLoopTiming.Update, completeCancellationToken.Token, true);
			}
			catch (OperationCanceledException e)
			{
			}

			for (var i = 0; i < Blobs.Count; i++)
			{
				Blobs[i].transform.DOScale(1.35f, .1f).SetDelay(0.2f + i * 0.05f).SetLoops(2, LoopType.Yoyo);
			}
		}
	}
}