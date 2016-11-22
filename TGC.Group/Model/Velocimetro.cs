using Microsoft.DirectX;
using System;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Text;
using TGC.Core.Utils;
using TGC.Group.Model.HUD;

namespace TGC.Group.Model
{
    class Velocimetro
    {

        private CustomSprite ticksSprite;
        private CustomSprite needleSprite;
        private CustomSprite numbersSprite;
        private CustomSprite circleSprite;

        private Drawer2D drawer2D;

        public Velocimetro()
        {
           
             ticksSprite = new CustomSprite();
            ticksSprite.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\ticks.png", D3DDevice.Instance.Device);
            ticksSprite.Scaling = new Vector2(0.5f, 0.5f);
            var textureSize = ticksSprite.Bitmap.Size;
            ticksSprite.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width/2, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height/2, 0));

            numbersSprite = new CustomSprite();
            numbersSprite.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\numbers.png", D3DDevice.Instance.Device);
            numbersSprite.Scaling = new Vector2(0.5f, 0.5f);
            textureSize = numbersSprite.Bitmap.Size;
            numbersSprite.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width/2, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height/2, 0));

            needleSprite = new CustomSprite();
            needleSprite.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\aguja.png", D3DDevice.Instance.Device);
            needleSprite.Scaling = new Vector2(0.3f, 0.3f);
            textureSize = needleSprite.Bitmap.Size;
            needleSprite.Position = new Vector2(numbersSprite.Position.X + numbersSprite.Bitmap.Size.Width/4, numbersSprite.Position.Y + numbersSprite.Bitmap.Size.Height / 4);

            circleSprite = new CustomSprite();
            circleSprite.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\circle.png", D3DDevice.Instance.Device);
            circleSprite.Scaling = new Vector2(0.86f, 0.86f);
            circleSprite.Position = new Vector2(numbersSprite.Position.X + 16, numbersSprite.Position.Y + 18);
            drawer2D = new Drawer2D();
        }

        public void render(TgcText2D text, float velocityFloat)
        {
            if (velocityFloat < 0)
                needleSprite.Rotation = (FastMath.PI / 4 * velocityFloat * 0.05f);
            else
                needleSprite.Rotation = FastMath.PI / 4 * velocityFloat * 0.05f;

            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(circleSprite);
            drawer2D.DrawSprite(ticksSprite);
            drawer2D.DrawSprite(numbersSprite);
            drawer2D.DrawSprite(needleSprite);
            drawer2D.EndDrawSprite();
        }
    }
}