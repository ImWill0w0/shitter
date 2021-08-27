using Sandbox;

[Library( "shitthrower", Title = "Throw Shit" )]
[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
partial class shitthrower : BaseDmWeapon
{ 
	public override string ViewModelPath => "models/poopemoji/poopemoji_v.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override AmmoType AmmoType => AmmoType.Shit;
	public override int ClipSize => 69;
	public override float ReloadTime => 3.0f;

	public override int Bucket => 1;

	public TimeSince TimeSinceChargeStart;
	public float ChargeTime => 2.0f; // How long the user needs to press Attack2 before shitting
	private Particles ChargeParticles;

	public override void Spawn()
	{
		base.Spawn();

		SetModel("models/poopemoji/poopemoji_w.vmdl");
		AmmoClip = 100;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override bool CanSecondaryAttack()
	{
		return Input.Down( InputButton.Attack2 );
	}

	public void StartCharge()
	{
		// Change this to your charge particle. Make its life the same rate as your ChargeTime
		ChargeParticles = Particles.Create( "particles/shit_buildup.vpcf", this, "muzzle" );
	}

	public void StopCharge()
	{
		ChargeParticles?.Destroy( true );
		TimeSinceChargeStart = 0;
	}
	
	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = -0.5f;

		(Owner as AnimEntity)?.SetAnimBool("b_attack", true);

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}
		
		//
		// Tell the clients to play the shoot effects
		//
		//ShootEffects();
		//PlaySound( "shoot" );

		//
		// Shoot the bullets
		//
		//ShootBullet( 0.05f, 1.5f, 9.0f, 3.0f );
		if (IsClient) return;
		ShootShit();

	}

	public override void AttackSecondary()
    {
	    if ( TimeSinceChargeStart < ChargeTime )
		    return;
		
	    TimeSincePrimaryAttack = 0;
	    TimeSinceChargeStart = 0;

	    if ( !TakeAmmo( 1 ) )
	    {
		    DryFire();
		    return;
	    }
		
	    // Do your charge fire stuff here, call ShootEffects, play sounds, etc

		if ( !TakeAmmo (5) )
        {
			DryFire();
			return;
        }

		ShootEffects();
		PlaySound ( "shoot_big" );

		if ( IsClient ) return;
		ShootShit(true);
    }

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( IsReloading )
		{
			// Reset timers so that people don't mess with them while reloading
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;
			TimeSinceChargeStart = 0;
			return;
		}
		
		// Don't do anything if we don't have the ammo for it or player is dead
		if ( AvailableAmmo() < 5 || Owner.Health <= 0 || !Owner.IsValid() )
		{
			StopCharge();
			return;
		}
		
		// We just started charging, so spawn particles & stuff
		if ( Input.Pressed( InputButton.Attack2 ) || TimeSinceChargeStart.Relative.AlmostEqual( 0 ) )
			StartCharge();
		
		if ( !Input.Down( InputButton.Attack2 ) )
			TimeSinceChargeStart = 0;

		// No longer charging
		if ( Input.Released( InputButton.Attack2 ) )
			StopCharge();
	}

	public override void SimulateAnimator(PawnAnimator anim)
	{
		anim.SetParam("holdtype", 4); // TODO this is shit
		anim.SetParam("aimat_weight", 1.0f);
		anim.SetParam("holdtype_handedness", 0);
		anim.SetParam("holdtype_pose", 2.25f);
		anim.SetParam("holdtype_pose_hand", 0.01f);
	}

	void ShootShit(bool isBig = false)
	{
		var ent = new Poojectile
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * (isBig ? 70 : 40),
			Rotation = Owner.EyeRot,
			Weapon = this
		};

		ent.SetModel($"models/poopemoji/poopemoji{(isBig ? "_big" : "")}.vmdl");
		ent.Velocity = Owner.EyeRot.Forward * 10000;
	}
}
