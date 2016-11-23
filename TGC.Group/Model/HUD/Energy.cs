using Microsoft.DirectX;
using System;
using System.Collections.Generic;
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
            //energyBorder.Scaling = new Vector2(0.5f, 0.5f);
            var textureSize = energyBorder.Bitmap.Size;
            energyBorder.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width *4, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height * 4, 0));

            energyBar = new CustomSprite();
            energyBar.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\energy.png", D3DDevice.Instance.Device);
            //energyBorder.Scaling = new Vector2(0.5f, 0.5f);
            textureSize = energyBar.Bitmap.Size;
            energyBar.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width * 4, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height * 4, 0));

            heart = new CustomSprite();
            heart.Bitmap = new CustomBitmap(Game.Default.MediaDirectory + "\\heart.png", D3DDevice.Instance.Device);
            //energyBorder.Scaling = new Vector2(0.5f, 0.5f);
            textureSize = heart.Bitmap.Size;
            heart.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width * 4, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height * 4, 0));

            drawer2D = new Drawer2D();
        }

        public void render()
        {

            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(energyBar);
            drawer2D.DrawSprite(energyBorder);
            drawer2D.DrawSprite(heart);
            drawer2D.EndDrawSprite();
        }
    }
}
