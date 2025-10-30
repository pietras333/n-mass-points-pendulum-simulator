using UnityEngine;

[System.Serializable]
public class MassPoint
{
    public Vector2 Position;
    public float Mass = 1f;
    public float AttachedRodLength = 1f;

    public float InitialAngleDegrees = 45f;
    public float AngularVelocity = 0f;

    public float Angle;

    public MassPoint Clone()
    {
        return new MassPoint()
        {
            Position = this.Position,
            Mass = this.Mass,
            AttachedRodLength = this.AttachedRodLength,
            Angle = this.InitialAngleDegrees * Mathf.Deg2Rad,
            AngularVelocity = this.AngularVelocity
        };
    }

}
