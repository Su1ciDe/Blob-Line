using System.Collections.Generic;
using GamePlay.Blobs;
using UnityEngine;

namespace Interfaces
{
	public interface ISlot
	{
		public List<Blob> Blobs { get; set; }
		public int Index { get; set; }
		public Transform GetTransform();
	}
}