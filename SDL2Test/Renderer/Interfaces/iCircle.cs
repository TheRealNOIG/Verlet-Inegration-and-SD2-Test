using SDL2;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using static SDL2.SDL;

public interface ICircle : IDrawable
{
    public PointF Center { get; set; }
    public int Radius { get; set; }
    public Color FillColor { get; set; }

    public IntPtr CircleTexture { get; set; }

    public void Draw(IntPtr renderer) { }
}

public static class CircleDrawExtension
{
    public static void Draw(this ICircle circle, IntPtr renderer)
    {
        int diameter = circle.Radius * 2;
        SDL.SDL_Rect renderQuad = new SDL.SDL_Rect() { x = (int)circle.Center.X - circle.Radius, y = (int)circle.Center.Y - circle.Radius, w = diameter, h = diameter };
        SDL.SDL_RenderCopy(renderer, circle.CircleTexture, IntPtr.Zero, ref renderQuad);
    }

    public static void CreateTexture(this ICircle circle, IntPtr renderer)
    {
        // Create the texture if it hasn't been created yet
        if (circle.CircleTexture == IntPtr.Zero)
        {
            int diameter = circle.Radius * 2;
            circle.CircleTexture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, diameter, diameter);
        }
        // Set blend mode to enable alpha blending
        SDL.SDL_SetTextureBlendMode(circle.CircleTexture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

        // Set texture alpha modulation to enable transparency
        SDL.SDL_SetTextureAlphaMod(circle.CircleTexture, circle.FillColor.A);

        // Set the renderer's target texture to the texture we want to draw to
        SDL.SDL_SetRenderTarget(renderer, circle.CircleTexture);

        // Draw the circle using the provided renderer
        SDL.SDL_SetRenderDrawColor(renderer, circle.FillColor.R, circle.FillColor.G, circle.FillColor.B, circle.FillColor.A);
        int x = circle.Radius;
        int y = 0;
        int radiusError = 1 - x;

        while (x >= y)
        {
            int offsetX = circle.Radius;
            int offsetY = circle.Radius;

            SDL.SDL_RenderDrawLine(renderer, x + offsetX, y + offsetY, -x + offsetX, y + offsetY);
            SDL.SDL_RenderDrawLine(renderer, x + offsetX, -y + offsetY, -x + offsetX, -y + offsetY);
            SDL.SDL_RenderDrawLine(renderer, y + offsetX, x + offsetY, -y + offsetX, x + offsetY);
            SDL.SDL_RenderDrawLine(renderer, y + offsetX, -x + offsetY, -y + offsetX, -x + offsetY);
            y++;
            if (radiusError < 0)
            {
                radiusError += 2 * y + 1;
            }
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
        }

        // Reset the renderer's target texture to the default rendering target (the screen)
        SDL.SDL_SetRenderTarget(renderer, IntPtr.Zero);

        circle.textureCreated = true;
    }


    /*    public static void Draw(this iCircle circle, IntPtr renderer)
    {
        SDL.SDL_SetRenderDrawColor(renderer, circle.FillColor.R, circle.FillColor.G, circle.FillColor.B, circle.FillColor.A);
        int x = circle.Radius;
        int y = 0;
        int radiusError = 1 - x;

        while (x >= y)
        {
            SDL.SDL_RenderDrawLine(renderer, (int)circle.Center.X - x, (int)circle.Center.Y + y, (int)circle.Center.X + x, (int)circle.Center.Y + y);
            SDL.SDL_RenderDrawLine(renderer, (int)circle.Center.X - x, (int)circle.Center.Y - y, (int)circle.Center.X + x, (int)circle.Center.Y - y);
            SDL.SDL_RenderDrawLine(renderer, (int)circle.Center.X - y, (int)circle.Center.Y + x, (int)circle.Center.X + y, (int)circle.Center.Y + x);
            SDL.SDL_RenderDrawLine(renderer, (int)circle.Center.X - y, (int)circle.Center.Y - x, (int)circle.Center.X + y, (int)circle.Center.Y - x);
            y++;
            if (radiusError < 0)
            {
                radiusError += 2 * y + 1;
            }
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
        }
    }*/


    /*    public static void Draw(this iCircle circle, IntPtr renderer)
        {
            int diameter = circle.Radius * 2;

            // Create a texture with alpha channel for the circle
            IntPtr texture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, diameter, diameter);
            if (texture == IntPtr.Zero)
            {
                throw new Exception("Failed to create texture: " + SDL.SDL_GetError());
            }

            // Set the texture as the rendering target
            SDL.SDL_SetRenderTarget(renderer, texture);

            // Clear the texture to transparent
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
            SDL.SDL_RenderClear(renderer);

            // Draw the filled circle to the texture
            SDL.SDL_SetRenderDrawColor(renderer, circle.FillColor.R, circle.FillColor.G, circle.FillColor.B, circle.FillColor.A);
            int x = circle.Radius;
            int y = 0;
            int radiusError = 1 - x;

            while (x >= y)
            {
                int offsetX = circle.Radius;
                int offsetY = circle.Radius;

                SDL.SDL_RenderDrawLine(renderer, x + offsetX, y + offsetY, -x + offsetX, y + offsetY);
                SDL.SDL_RenderDrawLine(renderer, x + offsetX, -y + offsetY, -x + offsetX, -y + offsetY);
                SDL.SDL_RenderDrawLine(renderer, y + offsetX, x + offsetY, -y + offsetX, x + offsetY);
                SDL.SDL_RenderDrawLine(renderer, y + offsetX, -x + offsetY, -y + offsetX, -x + offsetY);

                y++;
                if (radiusError < 0)
                {
                    radiusError += 2 * y + 1;
                }
                else
                {
                    x--;
                    radiusError += 2 * (y - x + 1);
                }
            }

            // Reset the rendering target
            SDL.SDL_SetRenderTarget(renderer, IntPtr.Zero);

            // Set the rendering area and render the texture
            SDL.SDL_Rect renderQuad = new SDL.SDL_Rect() { x = (int)circle.Center.X - circle.Radius, y = (int)circle.Center.Y - circle.Radius, w = diameter, h = diameter };
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref renderQuad);

            // Free resources
            SDL.SDL_DestroyTexture(texture);
        }*/


    /*    public static void Draw(this iCircle circle, IntPtr renderer)
        {
            int diameter = circle.Radius * 2;

            // Create a surface with alpha channel for the circle
            IntPtr surface = SDL.SDL_CreateRGBSurfaceWithFormat(0, diameter, diameter, 32, SDL.SDL_PIXELFORMAT_RGBA8888);
            if (surface == IntPtr.Zero)
            {
                throw new Exception("Failed to create surface: " + SDL.SDL_GetError());
            }

            // Fill the surface with the fill color
            SDL.SDL_Rect rect = new SDL.SDL_Rect() { x = 0, y = 0, w = diameter, h = diameter };

            SDL.SDL_Surface surfaceStruct = (SDL.SDL_Surface)Marshal.PtrToStructure(surface, typeof(SDL.SDL_Surface));
            IntPtr formatPtr = surfaceStruct.format;
            //SDL.SDL_FillRect(surface, ref rect, SDL.SDL_MapRGBA(formatPtr, circle.FillColor.R, circle.FillColor.G, circle.FillColor.B, circle.FillColor.A));

            DrawCircleOnSurface(surface, circle.Center, circle.Radius, SDL.SDL_MapRGBA(formatPtr, circle.FillColor.R, circle.FillColor.G, circle.FillColor.B, circle.FillColor.A));

            // Create a texture from the surface
            IntPtr texture = CreateTextureFromSurface(renderer, surface);
            if (texture == IntPtr.Zero)
            {
                SDL.SDL_FreeSurface(surface);
                throw new Exception("Failed to create texture: " + SDL.SDL_GetError());
            }

            // Set the rendering area and render the texture
            SDL.SDL_Rect renderQuad = new SDL.SDL_Rect() { x = (int)circle.Center.X - circle.Radius, y = (int)circle.Center.Y - circle.Radius, w = diameter, h = diameter };
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref renderQuad);

            // Free resources
            SDL.SDL_DestroyTexture(texture);
            SDL.SDL_FreeSurface(surface);
        }*/



    /* public static IntPtr CreateTextureFromSurface(IntPtr renderer, IntPtr surface)
     {
         SDL.SDL_Surface surfaceStruct = (SDL.SDL_Surface)Marshal.PtrToStructure(surface, typeof(SDL.SDL_Surface));
         int surfaceWidth = surfaceStruct.w;
         int surfaceHeight = surfaceStruct.h;

         // Allocate a managed byte array to hold the pixel data
         byte[] pixels = new byte[surfaceWidth * surfaceHeight * 4];

         // Copy the pixel data from the surface to the managed array
         IntPtr pixelsIntPtr = surfaceStruct.pixels;
         Marshal.Copy(pixelsIntPtr, pixels, 0, pixels.Length);

         // Allocate a new IntPtr for the texture
         IntPtr texture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_ABGR8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, surfaceWidth, surfaceHeight);

         // Lock the texture to get a pointer to its pixel data
         IntPtr texturePixels;
         int pitch;
         SDL.SDL_LockTexture(texture, IntPtr.Zero, out texturePixels, out pitch);

         // Copy the pixel data from the managed array to the texture
         Marshal.Copy(pixels, 0, texturePixels, pixels.Length);

         // Unlock the texture to release the pixel data
         SDL.SDL_UnlockTexture(texture);
         return texture;
     }


     public static void DrawCircleOnSurface(IntPtr surface, PointF center, int radius, uint color)
     {
         SDL.SDL_Surface surfaceStruct = (SDL.SDL_Surface)Marshal.PtrToStructure(surface, typeof(SDL.SDL_Surface));
         int surfaceWidth = surfaceStruct.w;
         int surfaceHeight = surfaceStruct.h;

         int diameter = radius * 2;
         SDL.SDL_Rect rect = new SDL.SDL_Rect() { x = 0, y = 0, w = diameter, h = diameter };

         // Lock the surface so we can write to it
         SDL.SDL_LockSurface(surface);

         int bbp = surfaceWidth * surfaceHeight * 4;
         byte[] pixels = new byte[bbp];

         for (int x = 0; x < diameter; x++)
         {
             for (int y = 0; y < diameter; y++)
             {
                 int dx = radius - x;
                 int dy = radius - y;
                 if (Math.Sqrt(dx * dx + dy * dy) <= radius)
                 {
                     // Calculate the pixel offset for this location
                     int pixelOffset = (y * surfaceWidth + x) * 4;

                     Marshal.WriteInt32(surface, pixelOffset, (int)color);
                 }
             }
         }

         // Unlock the surface to write changes
         SDL.SDL_UnlockSurface(surface);
     }*/


}
