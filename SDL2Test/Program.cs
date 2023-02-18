using System.Drawing;
using System.Numerics;
using SDL2;

float lastFrameTime = 0;

// Initialize SDL
SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

// Create window
IntPtr window = SDL.SDL_CreateWindow("Circle Collision", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

// Create renderer
IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

// Create list of circles
List<Circle> circles = new List<Circle>();

// Generate random number of circles
Random rand = new Random();
int numCircles = rand.Next(25, 35);

// Generate circles
for (int i = 0; i < numCircles; i++)
{
    // Generate random position and radius for circle
    float xPos = (float)rand.NextDouble() * 700.0f + 50.0f;
    float yPos = (float)rand.NextDouble() * 500.0f + 50.0f;
    float radius = (float)rand.NextDouble() * 50.0f + 10.0f;

    // Create new circle
    Circle circle = new Circle(new Vector2(xPos, yPos), radius);

    // Add circle to list
    circles.Add(circle);
}

// Run main loop
bool quit = false;
while (!quit)
{
    // Handle events
    SDL.SDL_Event e;
    while (SDL.SDL_PollEvent(out e) != 0)
    {
        if (e.type == SDL.SDL_EventType.SDL_QUIT)
        {
            quit = true;
        }
    }

    // Clear screen
    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
    SDL.SDL_RenderClear(renderer);

    // Update and render circles
    foreach (Circle circle in circles)
    {
        // Resolve collisions with other circles
        foreach (Circle other in circles)
        {
            if (circle != other)
            {
                circle.ResolveCollision(other);
            }
        }

        // Update and constrain circle
        circle.Update(1.0f / 60.0f);
        circle.ConstrainInsideCircle(new Vector2(400, 300), 250);

        // Render circle
        circle.Render(renderer);
    }

    // Update screen
    SDL.SDL_RenderPresent(renderer);
}

// Clean up
SDL.SDL_DestroyRenderer(renderer);
SDL.SDL_DestroyWindow(window);
SDL.SDL_Quit();

float CalculateDeltaTime()
{
    // Calculate the time that has elapsed since the last frame
    uint currentFrameTime = SDL.SDL_GetTicks();
    float deltaTimeInSeconds = (currentFrameTime - lastFrameTime);
    lastFrameTime = currentFrameTime;
    Console.WriteLine("DeltaTime: " + deltaTimeInSeconds + " ms");
    return deltaTimeInSeconds;
}


/*// Create a window object
SDL2Window window = new SDL2Window("Example Window", 1920, 1080);

// Create a Constrainable object with a circle radius based on the window dimensions
Constrainable constrainable = new Constrainable(Math.Min(window.width, window.height) * 0.45f, new PointF(window.width / 2, window.height / 2));

// Create a Solver object using the Constrainable object and add it as an updatable to the window
Solver solver = new Solver(constrainable, true, window);
window.AddUpdatable(solver);

// Create a random number of VerletCircle objects and add them to the Solver object
Random rand = new Random();
int numCircles = rand.Next(2000, 2000);

for (int i = 0; i < numCircles; i++)
{
    // Generate a random position for the VerletCircle object and create it with a random radius and color
    float radius = (float)rand.NextDouble() * constrainable.circleRadius / 2f;
    float angle = (float)rand.NextDouble() * 2f * (float)Math.PI;
    float x = constrainable.circleCenter.X + radius * (float)Math.Cos(angle);
    float y = constrainable.circleCenter.Y + radius * (float)Math.Sin(angle);

    Color color = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 256), rand.Next(0, 256));
    VerletCircle circle = new VerletCircle(new Point((int)x, (int)y), rand.Next(5, 10), color);
    solver.AddVerlet(circle);
}

// Run the window loop
window.Run();
*/