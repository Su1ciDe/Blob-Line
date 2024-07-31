using GamePlay.Blobs;
using GridSystem;
using Interfaces;
using UnityEngine;

namespace GamePlay.Obstacles
{
	public abstract class BaseObstacle : MonoBehaviour, INode
	{
		public GridCell CurrentGridCell { get; set; }

		public void Place(GridCell placedCell)
		{
			CurrentGridCell = placedCell;
			CurrentGridCell.CurrentNode = this;

			transform.SetParent(placedCell.transform);
			transform.localPosition = Vector3.zero;
		}

		public virtual void OnBlastNear(Blob blob)
		{
		}
	}
}