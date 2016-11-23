using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Utils;

namespace TGC.Group.Model.HUD
{
    public class Energy
    {
        private CustomSprite energyBorder;
        private CustomSprite energyBar;
        private CustomSprite heart;

        private Drawer2D drawer2D;

        public Energy()
        {
            energyBorder = new CustomSprite();
            energyBorder.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\energy-border.png", D3DDevice.Instance.Device);
            energyBorder.Scaling = new Vector2(0.5f, 0.5f);
            var textureSize = energyBorder.Bitmap.Size;
            energyBorder.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width * 1.2f, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height * 1.3f, 0));

            energyBar = new CustomSprite();
            energyBar.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\energy.png", D3DDevice.Instance.Device);
            energyBar.Scaling = new Vector2(0.5f, 0.5f);
            energyBar.Position = new Vector2(energyBorder.Position.X + 3, energyBorder.Position.Y + 3);

            drawer2D = new Drawer2D();
        }

        public void render(double energy)
        {

            if(energy < 0)
            {
                energy = 0;
            }

            int red = (((int)energy - 100) * 255) / -100;
            int green = ((int)energy * 255) / 100;
            energyBar.Scaling = new Vector2( (float)energy / 205f, 0.4f);
            var color = Color.FromArgb(red, green, (int)0);
            energyBar.Color = color;

            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(energyBorder);
            drawer2D.DrawSprite(energyBar);
            drawer2D.EndDrawSprite();
        }
    }
}
