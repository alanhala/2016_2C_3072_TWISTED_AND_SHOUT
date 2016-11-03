using System;
using System.Drawing;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    class Velocimetro
    {
        public void render(TgcText2D text, float velocityFloat)
        {
            var velocity = Math.Round(velocityFloat / 10);
            text.drawText(velocity.ToString() + " KM/H", 0,600, Color.Yellow);
            //text.Size = new Size(100, 100);
            var size = text.Size;

            var lala = "llala";
        }
    }
}