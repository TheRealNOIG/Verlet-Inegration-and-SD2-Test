public interface IDrawable
{
    void Draw(IntPtr renderer);
    void CreateTexture(IntPtr renderer);
    void DistroyTexture();

    bool textureCreated { get; set; }
}