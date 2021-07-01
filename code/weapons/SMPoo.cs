using Sandbox;
using System;

[Library( "dm_smg", Title = "SMPoo" )]
[Hammer.EditorModel( "weapons/rust_smg/rust_smg.vmdl" )]
partial class SMPoo : BaseDmWeapon
{ 
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override int ClipSize => 30;
	public override float ReloadTime => 4.0f;
	public override int Bucket => 2;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_smg/rust_smg.vmdl" );
		AmmoClip = 100;
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		(Owner as AnimEntity).SetAnimBool( "b_attack", true );

		//
		// Tell the clients to play the shoot effects
		//
		ShootEffects();
		PlaySound( "shoot" );

		//
		// Shoot the bullets
		//
		//ShootBullet( 0.1f, 1.5f, 5.0f, 3.0f );
		if (IsClient) return;
		ShootShit(true);

	}

	public override void AttackSecondary()
	{
		// Grenade lob
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );
		Particles.Create( "particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point" );

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 2 ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}

	public override void Simulate(Client owner)
	{
		base.Simulate(owner);
	}

	void ShootShit(bool isBig = false)
	{
		var ent = new Poojectile
		{
			Position = Owner.EyePos + Owner.EyeRot.Forward * (isBig ? 70 : 40),
			Rotation = Owner.EyeRot,
			Weapon = this
		};

		ent.SetModel($"models/poopemoji/poopemoji_small.vmdl");
		ent.Velocity = Owner.EyeRot.Forward * 10000;
		ent.Scale = .2f;
	}
}
