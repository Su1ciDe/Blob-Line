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
			// ParticlePooler.Instance.Spawn("Breakable", transform.position);
		}
	}
}