using Sandbox;
using System;

class Poojectile : Prop
{
	public Entity Weapon { get; set; }	

	protected override void OnPhysicsCollision(CollisionEventData eventData)
	{
		var propData = GetModelPropData();

		var minImpactSpeed = propData.MinImpactDamageSpeed;
		if (minImpactSpeed <= 0.0f) minImpactSpeed = 500;

		var impactDmg = propData.ImpactDamage;
		if (impactDmg <= 0.0f) impactDmg = 10;

		float speed = eventData.Speed;

		if (speed > minImpactSpeed)
		{
			// I take damage from high speed impacts
			if (Health > 0)
			{
				var damage = speed / minImpactSpeed * impactDmg;
				TakeDamage(DamageInfo.Generic(damage).WithFlag(DamageFlags.PhysicsImpact));
			}

			// Whatever I hit takes more damage
			if (eventData.Entity.IsValid() && eventData.Entity != this)
			{
				var damage = speed / minImpactSpeed * impactDmg * 1.2f;
				eventData.Entity.TakeDamage(DamageInfo.Generic(damage)
					.WithFlag(DamageFlags.PhysicsImpact)
					.WithWeapon(Weapon)
					.WithAttacker(Weapon.Owner)
					.WithPosition(eventData.Pos)
					.WithForce(eventData.PreVelocity));
			}
		}
	}
}

