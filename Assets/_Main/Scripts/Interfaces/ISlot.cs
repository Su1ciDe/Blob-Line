using GamePlay.Blobs;
using UnityEngine;

namespace Interfaces
{
	public interface ISlot
	{
		public Blob Blob { get; set; }
		public int Index { get; set; }
		public void SetBlob(Blob blob, bool setPosition = true);
		public Transform GetTransform();
	}
}