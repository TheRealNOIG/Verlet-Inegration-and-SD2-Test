using System.Drawing;
using SDL2;

// Create a window object
SDL2Window window = new SDL2Window("Example Window", 1920, 1080);

// Create a Constrainable object with a circle radius based on the window dimensions
Constrainable constrainable = new Constrainable(Math.Min(window.width, window.height) * 0.45f, new PointF(window.width / 2, window.height / 2));

// Create a Solver object using the Constrainable object and add it as an updatable to the window
Solver solver = new Solver(constrainable, true, window);
window.AddUpdatable(solver);

// Create a random number of VerletCircle objects and add them to the Solver object
Random rand = new Random();
int numCircles = rand.Next(800, 800);

for (int i = 0; i < numCircles; i++)
{
    // Generate a random position for the VerletCircle object and create it with a random radius and color
    float radius = (float)rand.NextDouble() * constrainable.circleRadius / 2f;
    float angle = (float)rand.NextDouble() * 2f * (float)Math.PI;
    float x = constrainable.circleCenter.X + radius * (float)Math.Cos(angle);
    float y = constrainable.circleCenter.Y + radius * (float)Math.Sin(angle);

    Color color = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
    VerletCircle circle = new VerletCircle(new Point((int)x, (int)y), rand.Next(5, 15), color);
    solver.AddVerlet(circle);
}

// Run the window loop
window.Run();
