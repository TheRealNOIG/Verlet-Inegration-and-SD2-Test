using SDL2;
using System;
using System.Numerics;

public class Circle
{
    // Define constants
    private const float MASS = 1f;
    private const float DAMPING = 0.98f;
    private const int CIRCLE_SEGMENTS = 32;

    // Define private variables 
    private Vector2 position;
    private Vector2 oldPosition;
    private Vector2 velocity;
    private float radius;

    // Constructor
    public Circle(Vector2 initialPosition, float initialRadius)
    {
        position = initialPosition;
        oldPosition = initialPosition;
        velocity = Vector2.Zero;
        radius = initialRadius;
    }

    // Update method
    public void Update(float deltaTime)
    {
        // Update position using Verlet interpolation
        Vector2 acceleration = CalculateAcceleration(position);
        Vector2 newPosition = position + velocity * deltaTime + 0.5f * acceleration * deltaTime * deltaTime;
        velocity = (newPosition - oldPosition) / (2.0f * deltaTime);
        oldPosition = position;
        position = newPosition;

        // Apply damping
        velocity *= DAMPING;
    }

    // Render method
    public void Render(IntPtr renderer)
    {
        // Draw filled circle using renderer
        SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
        float deltaAngle = 2.0f * (float)Math.PI / CIRCLE_SEGMENTS;
        float angle = 0;
        for (int i = 0; i < CIRCLE_SEGMENTS; i++)
        {
            Vector2 startPoint = position + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
            Vector2 endPoint = position + new Vector2((float)Math.Cos(angle + deltaAngle), (float)Math.Sin(angle + deltaAngle)) * radius;
            SDL.SDL_RenderDrawLine(renderer, (int)startPoint.X, (int)startPoint.Y, (int)endPoint.X, (int)endPoint.Y);
            angle += deltaAngle;
        }
    }

    // Helper method to calculate acceleration
    private Vector2 CalculateAcceleration(Vector2 pos)
    {
        // Calculate acceleration based on forces acting on circle
        // For example, if the circle is subject to gravity:
        Vector2 acceleration = new Vector2(0, 981f);
        return acceleration;
    }

    // Method to constrain circle inside another circle
    public void ConstrainInsideCircle(Vector2 center, float constraintRadius)
    {
        // Calculate distance between center of constraint circle and circle object
        float distance = Vector2.Distance(center, position);
        float maxDistance = constraintRadius - radius;
        if (distance > maxDistance)
        {
            // Circle is outside constraint circle, so move it back inside
            Vector2 direction = Vector2.Normalize(position - center);
            position = center + direction * maxDistance;
            oldPosition = position;
        }
    }

    // Method to resolve collisions with another circle
    public void ResolveCollision(Circle other)
    {
        // Calculate distance between centers of circles
        float distance = Vector2.Distance(position, other.position);

        // If circles are overlapping
        if (distance < radius + other.radius)
        {
            // Calculate direction from this circle to other circle
            Vector2 direction = Vector2.Normalize(other.position - position);

            // Calculate penetration depth
            float penetrationDepth = radius + other.radius - distance;

            // Move circles apart by half the penetration depth in opposite directions
            position -= direction * (penetrationDepth / 2.0f);
            oldPosition = position;
            other.position += direction * (penetrationDepth / 2.0f);

            // Calculate relative velocity of circles
            Vector2 relativeVelocity = velocity - other.velocity;

            // Calculate relative velocity in direction of collision
            float collisionSpeed = Vector2.Dot(relativeVelocity, direction);

            // If circles are moving towards each other
            if (collisionSpeed < 0)
            {
                // Calculate impulse to apply to circles
                float impulseMagnitude = -(1.0f + DAMPING) * collisionSpeed / (1.0f / Circle.MASS + 1.0f / Circle.MASS);
                Vector2 impulse = impulseMagnitude * direction;

                // Apply impulse to this circle
                velocity += impulse / Circle.MASS;

                // Apply impulse to other circle
                other.velocity -= impulse / Circle.MASS;
            }
        }
    }

}
