using System.Drawing;

public class Constrainable
{
    public float circleRadius;
    public PointF circleCenter;

    // Constructor for Constrainable
    public Constrainable(float circleRadius, PointF circleCenter)
    {
        this.circleRadius = circleRadius;
        this.circleCenter = circleCenter;
    }

    // Method to get the minimum x position a circle can have
    public float GetMinX()
    {
        return circleCenter.X - circleRadius;
    }

    // Method to get the maximum x position a circle can have
    public float GetMaxX()
    {
        return circleCenter.X + circleRadius;
    }

    // Method to get the minimum y position a circle can have
    public float GetMinY()
    {
        return circleCenter.Y - circleRadius;
    }

    // Method to get the maximum y position a circle can have
    public float GetMaxY()
    {
        return circleCenter.Y + circleRadius;
    }
}
