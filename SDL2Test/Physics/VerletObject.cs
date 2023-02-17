using System.Drawing;

public class VerletObject
{
    // Public properties
    public PointF Position { get; set; }
    public PointF PreviousPosition { get; set; }
    public PointF CurrentAcceleration { get; set; }
    public float Damping { get; set; }

    // Constructor for VerletObject
    public VerletObject(PointF position)
    {
        // Initialize object properties
        Position = position;
        PreviousPosition = position;
        CurrentAcceleration = new PointF(0, 0);
        Damping = .99f;
    }

    // Method for accelerating the object
    public void Accelerate(PointF force)
    {
        // Update the acceleration
        CurrentAcceleration = new PointF(CurrentAcceleration.X + force.X, CurrentAcceleration.Y + force.Y);
    }

    // Method for updating the object's position
    public virtual void Update(float deltaTime)
    {
        // Calculate the displacement of the object
        float xDisplacement = (Position.X - PreviousPosition.X) * Damping + CurrentAcceleration.X * deltaTime;
        float yDisplacement = (Position.Y - PreviousPosition.Y) * Damping + CurrentAcceleration.Y * deltaTime;

        // Store the current position in a temporary variable
        PointF temp = Position;

        // Update the object's position and previous position
        Position = new PointF(Position.X + xDisplacement, Position.Y + yDisplacement);
        PreviousPosition = temp;

        // Reset the object's acceleration
        CurrentAcceleration = new PointF(0, 0);
    }
}
