using UnityEngine;
using Random = UnityEngine.Random;
using Avrahamy;
using Avrahamy.Math;
using BitStrap;

namespace Flocking
{
	public class FollowerPeepController : MonoBehaviour
	{
		[SerializeField] private PeepController peep;
		[SerializeField] protected float separationWeight;
		[SerializeField] protected float alignmentWeight;
		[SerializeField] protected float cohesionWeight;
		[SerializeField] private float senseRadius = 2f;
		[SerializeField] private PassiveTimer navigationTimer;
		[SerializeField] private LayerMask navigationMask;
		[TagSelector] [SerializeField] private string peepTag;
		[SerializeField] private float playerFlockingWeight = 2;
		[SerializeField] private float wallFlockingWeight = 3;

		private PeepState _state = PeepState.Free;
		private Vector2 _thronePosition;
		private Vector3 _initialPosition;
		private Quaternion _initialRotation;

		private static readonly Collider[] colliderResults = new Collider[10];

		private bool Throned => _state != PeepState.Free;

		public void EnteredThrone(Vector2 newThronePosition)
		{
			_thronePosition = newThronePosition;
			_state = PeepState.EnterThrone;
		}

		public void ExitThrone()
		{
			_state = PeepState.ExitThrone;
		}

		public void ExitedThrone()
		{
			_state = PeepState.Free;
		}

		private enum PeepState
		{
			Free,
			EnterThrone,
			ExitThrone
		}

		protected void Reset()
		{
			if (peep == null)
				peep = GetComponent<PeepController>();
		}

		protected void OnEnable()
		{
			navigationTimer.Start();
			peep.DesiredVelocity = Random.insideUnitCircle.normalized;
		}

		private void Start()
		{
			_initialPosition = transform.position;
			_initialRotation = Quaternion.LookRotation(_initialPosition);
			transform.rotation = _initialRotation;
		}

		protected void Update()
		{
			if (navigationTimer.IsActive) return;
			navigationTimer.Start();

			var position = peep.Position;

			// Check for colliders in the sense radius.
			var hits = Physics.OverlapSphereNonAlloc(
				position,
				senseRadius,
				colliderResults,
				navigationMask.value);

			// There will always be at least one hit on our own collider.
			if (hits <= 1) return;
			var avgSeparationDirection = Vector3.zero;
			var avgForwardDirection = Vector3.zero;
			var avgPosition = Vector3.zero;
			var divisor = hits - 1;
			float weight = 1;
			for (int i = 0; i < hits; i++)
			{
				var hit = colliderResults[i];
				// Ignore self.
				if (hit.attachedRigidbody != null && hit.attachedRigidbody.gameObject == peep.gameObject) continue;

				// Always repel from walls.
				var repel = true;
				if (hit.tag.EndsWith(peepTag))
				{
					// repel = false;
					// Ignore throned peeps
					var otherPeepFollower = hit.attachedRigidbody.GetComponent<FollowerPeepController>();
					if ((otherPeepFollower && otherPeepFollower.Throned) || hit.gameObject == gameObject)
					{
						--divisor;
						break;
					}
					else if (!otherPeepFollower)
					{
						weight = playerFlockingWeight;
						++divisor;
					}

					avgSeparationDirection += SeparationDirection(hit, position, repel);
					avgForwardDirection += ForwardDirection(hit) * weight;
					avgPosition += hit.transform.position * weight;
				}
				else
				{
					avgSeparationDirection += SeparationDirection(hit, position, repel) * wallFlockingWeight;
					--divisor;
				}
			}

			var separationDirection = avgSeparationDirection.ToVector2XZ().normalized;
			// calculate the final average alignment direction 
			var alignmentDirection = avgForwardDirection.ToVector2XZ().normalized;
			var cohesionDirection =
				divisor <= 0 ? Vector2.zero : (avgPosition / divisor - position).ToVector2XZ().normalized;
			// calculate final direction
			var finalDirection = alignmentWeight * alignmentDirection + separationWeight * separationDirection +
			                     cohesionWeight * cohesionDirection;
			peep.DesiredVelocity = finalDirection.normalized;
			//print(peep.DesiredVelocity);
		}

		private Vector3 ForwardDirection(Collider hit)
		{
			if (hit.CompareTag("Wall")) return Vector3.zero;

			var otherPeep = hit.attachedRigidbody.GetComponent<PeepController>();

			if (otherPeep == null || otherPeep.Group != peep.Group)
				return Vector3.zero;
			return otherPeep.Forward;
		}


		private Vector3 SeparationDirection(Collider hit, Vector3 position, bool repel)
		{
			var closestPoint = hit.ClosestPoint(position);
			var avgDirection = Vector3.zero;
			closestPoint.y = 0f;
			DebugDraw.DrawLine(
				position + Vector3.up,
				closestPoint + Vector3.up,
				Color.cyan,
				navigationTimer.Duration / 2);
			var direction = closestPoint - position;

			var magnitude = direction.magnitude;
			var distancePercent = repel
				? Mathf.InverseLerp(peep.SelfRadius, senseRadius, magnitude)
				// Inverse attraction factor so peeps won't be magnetized to
				// each other without being able to separate.
				: Mathf.InverseLerp(senseRadius, peep.SelfRadius, magnitude);

			// Make sure the distance % is not 0 to avoid division by 0.
			distancePercent = Mathf.Max(distancePercent, 0.01f);

			// Force is stronger when distance percent is closer to 0 (1/x-1).
			var forceWeight = 1f / distancePercent - 1f;

			// Angle between forward to other collider.
			var angle = transform.forward.GetAngleBetweenXZ(direction);
			var absAngle = Mathf.Abs(angle);
			if (absAngle > 90f)
			{
				// Decrease weight of colliders that are behind the peep.
				// The closer to the back, the lower the weight.
				var t = Mathf.InverseLerp(180f, 90f, absAngle);
				forceWeight *= Mathf.Lerp(0.1f, 1f, t);
			}

			direction = direction.normalized * forceWeight;
			if (repel)
			{
				avgDirection -= direction;
				DebugDraw.DrawArrowXZ(
					position + Vector3.up,
					-direction * 3f,
					1f,
					30f,
					Color.magenta,
					navigationTimer.Duration / 2);
			}
			else
			{
				avgDirection += direction;
				DebugDraw.DrawArrowXZ(
					position + Vector3.up,
					direction * 3f,
					1f,
					30f,
					Color.green,
					navigationTimer.Duration / 2);
			}

			return avgDirection;
		}


		protected void OnDrawGizmos()
		{
			var angle = transform.forward.GetAngleXZ();
			DebugDraw.GizmosDrawSector(
				transform.position,
				senseRadius,
				180f + angle,
				-180f + angle,
				Color.green);
		}

		public void Init()
		{
			transform.position = _initialPosition;
			transform.rotation = _initialRotation;
			peep.Velocity = Vector2.zero;
			peep.DesiredVelocity = Vector2.zero;
		}
	}
}