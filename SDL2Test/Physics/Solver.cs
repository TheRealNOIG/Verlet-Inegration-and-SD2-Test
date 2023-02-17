using System;
using System.Collections.Generic;
using System.Drawing;

public class Solver : iUpdatable
{
    private Dictionary<string, VerletCircle> verlets = new Dictionary<string, VerletCircle>();
    private bool debug;
    private SDL2Window window;

    private float circleRadius;
    private PointF circleCenter;

    public Solver(int width = 800, int height = 600, float circleRadius = 0.45f, bool debug = false, SDL2Window window = null)
    {
        this.debug = debug;
        this.window = window;
        this.circleRadius = Math.Min(width, height) * circleRadius;
        this.circleCenter = new PointF(width / 2, height / 2);
    }

    public Solver(float circleRadius, PointF circleCenter, bool debug = false, SDL2Window window = null)
    {
        this.debug = debug;
        this.window = window;
        this.circleRadius = circleRadius;
        this.circleCenter = circleCenter;
    }

    public Solver(Constrainable constrainable, bool debug = false, SDL2Window window = null)
    {
        this.debug = debug;
        this.window = window;
        this.circleRadius = constrainable.circleRadius;
        this.circleCenter = constrainable.circleCenter;
    }

    public string AddVerlet(VerletCircle verlet)
    {
        string id = Guid.NewGuid().ToString();
        verlets.Add(id, verlet);
        if (debug)
        {
            window?.AddDrawable(verlet);
        }
        return id;
    }

    public void RemoveVerlet(string id)
    {
        VerletCircle verlet = null;
        if (verlets.TryGetValue(id, out verlet))
        {
            if (debug)
            {
                window?.RemoveDrawable(id);
            }
            verlets.Remove(id);
        }
    }

    public VerletCircle GetVerlet(string id)
    {
        VerletCircle verlet = null;
        verlets.TryGetValue(id, out verlet);
        return verlet;
    }

    public void Update(float deltaTime)
    {
        // Determine the smallest and largest circle sizes
        float minRadius = float.MaxValue;
        float maxRadius = float.MinValue;
        foreach (VerletCircle circle in verlets.Values)
        {
            float radius = circle.Radius;
            if (radius < minRadius)
            {
                minRadius = radius;
            }
            if (radius > maxRadius)
            {
                maxRadius = radius;
            }
        }

        // Calculate the size of each grid cell based on the larger of the two sizes
        int gridSize = (int)Math.Ceiling(maxRadius * 2f);

        // Create a dictionary to store circles in each grid cell
        Dictionary<Point, List<VerletCircle>> grid = new Dictionary<Point, List<VerletCircle>>();

        // Add circles to the grid based on their position
        foreach (VerletCircle circle in verlets.Values)
        {
            // Calculate the grid cell coordinates for the circle's position
            int x = (int)Math.Floor((circle.Position.X - circleCenter.X + circleRadius) / gridSize);
            int y = (int)Math.Floor((circle.Position.Y - circleCenter.Y + circleRadius) / gridSize);
            Point cell = new Point(x, y);

            // Add the circle to the grid cell
            if (!grid.ContainsKey(cell))
            {
                grid.Add(cell, new List<VerletCircle>());
            }
            grid[cell].Add(circle);
        }

        // Perform collision detection on circles in adjacent grid cells
        int substeps = 8;
        float subdelta = deltaTime / substeps;

        for (int i = 0; i < substeps; i++)
        {
            foreach (KeyValuePair<Point, List<VerletCircle>> cell in grid)
            {
                int x = cell.Key.X;
                int y = cell.Key.Y;

                // Check adjacent grid cells for collisions
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        Point neighbor = new Point(x + j, y + k);
                        if (grid.TryGetValue(neighbor, out List<VerletCircle> neighbors))
                        {
                            foreach (VerletCircle circle in cell.Value)
                            {
                                foreach (VerletCircle other in neighbors)
                                {
                                    if (circle != other)
                                    {
                                        SolveCircleCollision(circle, other, subdelta);
                                    }
                                }
                            }
                        }
                    }
                }
            }


            // Update circle positions
            foreach (VerletCircle circle in verlets.Values)
            {
                circle.Accelerate(new PointF(0f, 9.8f));
                circle.ConstrainToCircle(circleCenter, circleRadius);
                circle.Update(subdelta);
            }
        }
    }


    public void SolveCircleCollision(VerletCircle circle1, VerletCircle circle2, float deltaTime)
    {
        // Calculate the vector between the centers of the two circles
        float dx = circle2.Position.X - circle1.Position.X;
        float dy = circle2.Position.Y - circle1.Position.Y;
        float distance = (float)Math.Sqrt(dx * dx + dy * dy);

        // Calculate the maximum distance at which the two circles are just touching
        float maxDistance = circle1.Radius + circle2.Radius;

        if (distance < maxDistance)
        {
            // Calculate the amount of overlap between the two circles
            float overlap = maxDistance - distance;

            // Calculate the unit vector pointing from circle 1 to circle 2
            float ux = dx / distance;
            float uy = dy / distance;

            float moveDistance = overlap * 50f * deltaTime;

            // Move the circles apart by the full amount of overlap in opposite directions along the unit vector
            circle1.Position = new PointF(circle1.Position.X - ux * moveDistance, circle1.Position.Y - uy * moveDistance);
            circle2.Position = new PointF(circle2.Position.X + ux * moveDistance, circle2.Position.Y + uy * moveDistance);
        }
    }

}
