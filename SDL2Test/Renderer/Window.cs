using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SDL2;

public class SDL2Window
{
    // Window and Renderer
    private IntPtr window; 
    public IntPtr renderer { get; private set; } 

    // Display and Buffer
    private IntPtr buffer; 
    public int width { get; private set; } 
    public int height { get; private set; }
    private SDL.SDL_DisplayMode displayMode;

    // Running state
    private bool isRunning; 

    // Drawable and Updatable Dictionaries
    private Dictionary<string, IDrawable> drawableObjects = new Dictionary<string, IDrawable>();
    private Dictionary<string, iUpdatable> updatableObjects = new Dictionary<string, iUpdatable>();

    // Constructor for SDL2Window
    public SDL2Window(string title, int width, int height)
    {
        // Initialize SDL2 video subsystem
        SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

        // Initialize SDL TTF
        SDL_ttf.TTF_Init();

        // Create SDL2 window
        window = SDL.SDL_CreateWindow(title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

        // Create SDL2 renderer
        renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        // Create texture buffer
        buffer = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);

        // Set window dimensions
        this.width = width;
        this.height = height;

        // Get the Display mode of the user's monitor
        SDL.SDL_GetWindowDisplayMode(window, out displayMode);
    }


    #region Drawable Managment
    // Method for adding a drawable object to the dictionary
    public string AddDrawable(VerletCircle drawable)
    {
        // Generate unique ID for drawable
        string id = Guid.NewGuid().ToString();

        // Add drawable to dictionary
        drawableObjects .Add(id, drawable);

        // Return ID of added drawable
        return id;
    }

    // Method for removing a drawable object from the dictionary
    public void RemoveDrawable(string id)
    {
        // Remove drawable from dictionary by ID
        drawableObjects .Remove(id);
    }

    // Method for getting a drawable object from the dictionary by ID
    public IDrawable GetDrawable(string id)
    {
        // Attempt to retrieve drawable from dictionary by ID
        IDrawable obj;
        drawableObjects .TryGetValue(id, out obj);

        // Return retrieved drawable object
        return obj;
    }
    #endregion

    #region Updatables Managment
    // Method for adding an updatable object to the dictionary
    public string AddUpdatable(iUpdatable updatable)
    {
        // Generate unique ID for updatable
        string id = Guid.NewGuid().ToString();

        // Add updatable to dictionary
        updatableObjects.Add(id, updatable);

        // Return ID of added updatable
        return id;
    }

    // Method for removing an updatable object from the dictionary
    public void RemoveUpdatable(string id)
    {
        // Remove updatable from dictionary by ID
        updatableObjects.Remove(id);
    }

    // Method for getting an updatable object from the dictionary by ID
    public iUpdatable GetUpdatable(string id)
    {
        // Attempt to retrieve updatable from dictionary by ID
        iUpdatable obj;
        updatableObjects.TryGetValue(id, out obj);

        // Return retrieved updatable object
        return obj;
    }
    #endregion

    public void Run()
    {
        isRunning = true;
        SDL.SDL_Event e;
        int frameTimeMs = (int)(1000.0 / displayMode.refresh_rate);
        long lastFrameTime = SDL.SDL_GetTicks();
        long lastFPSCountedTime = SDL.SDL_GetTicks();
        int framesCounted = 0;
        int fps = 0;

        while (isRunning)
        {
            long currentFrameTime = SDL.SDL_GetTicks();
            float deltaTime = (currentFrameTime - lastFrameTime) / 1000f;
            lastFrameTime = currentFrameTime;

            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    isRunning = false;
                }
            }

            // Set the color of the render draw to black
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            // Clear the renderer
            SDL.SDL_RenderClear(renderer);

            foreach (KeyValuePair<string, IDrawable> drawable in drawableObjects)
            {
                // Only create the texture if it hasn't been created yet
                if (!drawable.Value.textureCreated)
                {
                    drawable.Value.CreateTexture(renderer);

                }
                drawable.Value.Draw(renderer);
            }

            RenderText(renderer, $"FPS: {fps}", 0, 10);

            // Present the renderer to the screen
            SDL.SDL_RenderPresent(renderer);

            // Iterate through each updatable object and update them
            foreach (KeyValuePair<string, iUpdatable> updatable in updatableObjects)
            {
                updatable.Value.Update(deltaTime);
            }

            // Count the frames rendered
            framesCounted++;

            // Calculate FPS every second
            if (currentFrameTime >= lastFPSCountedTime + 1000)
            {
                fps = framesCounted;
                framesCounted = 0;
                lastFPSCountedTime = currentFrameTime;
            }

            // Wait for the appropriate amount of time to sync with the refresh rate
            /*            long elapsedFrameTime = SDL.SDL_GetTicks() - currentFrameTime;
                        long remainingFrameTime = frameTimeMs - elapsedFrameTime;

                        if (remainingFrameTime > 0)
                        {
                            SDL.SDL_Delay((uint)remainingFrameTime);
                        }*/
        }


        foreach (KeyValuePair<string, IDrawable> drawable in drawableObjects)
        {
            if (!drawable.Value.textureCreated)
            {
                drawable.Value.DistroyTexture();
            }
        }

        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
        SDL_ttf.TTF_Quit();
    }

    private void RenderText(IntPtr renderer, string text, int x, int y, int fontSize = 24, string fontName = "arial")
    {
        // Load font
        IntPtr font = SDL_ttf.TTF_OpenFont("Fonts/" + fontName + ".ttf", fontSize);
        if (font == IntPtr.Zero)
        {
            Console.WriteLine("Failed to load font: " + SDL.SDL_GetError());
            return;
        }

        // Set the text color to white
        SDL.SDL_Color color = new SDL.SDL_Color() { r = 255, g = 255, b = 255, a = 255 };

        // Render the text to a surface
        IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, text, color);
        if (surface == IntPtr.Zero)
        {
            Console.WriteLine("Failed to render text surface: " + SDL.SDL_GetError());
            SDL_ttf.TTF_CloseFont(font);
            return;
        }

        // Create a texture from the surface
        IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);
        if (texture == IntPtr.Zero)
        {
            Console.WriteLine("Failed to create texture: " + SDL.SDL_GetError());
            SDL.SDL_FreeSurface(surface);
            SDL_ttf.TTF_CloseFont(font);
            return;
        }

        // Get the dimensions of the rendered text
        int textWidth, textHeight;
        SDL_ttf.TTF_SizeText(font, text, out textWidth, out textHeight);

        // Set the rendering area and render the texture
        SDL.SDL_Rect renderQuad = new SDL.SDL_Rect() { x = x, y = y, w = textWidth, h = textHeight };
        SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref renderQuad);

        // Free resources
        SDL.SDL_DestroyTexture(texture);
        SDL.SDL_FreeSurface(surface);
        SDL_ttf.TTF_CloseFont(font);
    }

}