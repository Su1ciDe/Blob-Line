using GamePlay.Blobs;
using UnityEngine;

namespace GamePlay.Obstacles
{
	public class BreakableObstacle : BaseObstacle
	{
		public override void OnBlastNear(Blob blob)
		{
			base.OnBlastNear(blob);

			CurrentGridCell.CurrentNode = null;

			Destroy(gameObject);
			// breakableObject.Break();
			//
			// navMeshObstacle.enabled = false;
			//
			// ParticlePooler.Instance.Spawn("Breakable", transform.position);
		}
	}
}