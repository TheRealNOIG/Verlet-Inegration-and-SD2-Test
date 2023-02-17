using SDL2;
using System.Drawing;

public class VerletCircle : VerletObject, ICircle
{
    public PointF Center { get; set; }
    public int Radius { get; set; }
    public Color FillColor { get; set; }

    public IntPtr CircleTexture { get; set; }

    public bool textureCreated { get; set; }

    public VerletCircle(PointF center, int radius, Color fillColor) : base(center)
    {
        this.Center = center;
        this.Radius = radius;
        this.FillColor = fillColor;
    }

    public void CreateTexture(IntPtr renderer)
    {
        CircleDrawExtension.CreateTexture(this, renderer);
    }

    public void Draw(IntPtr renderer)
    {
        CircleDrawExtension.Draw(this, renderer);
    }


    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        Center = Position;
    }


    public void ConstrainToCircle(PointF center, float radius)
    {
        // Calculate the vector from the circle center to the current position
        PointF vector = new PointF(Position.X - center.X, Position.Y - center.Y);
        float distance = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);

        // If the distance is greater than the radius minus the object's radius, move the object back to the circle
        if (distance > radius - Radius)
        {
            vector.X *= (radius - Radius) / distance;
            vector.Y *= (radius - Radius) / distance;
            Position = new PointF(center.X + vector.X, center.Y + vector.Y);
        }
    }

    public void DistroyTexture()
    {
        SDL.SDL_DestroyTexture(this.CircleTexture);
    }
}