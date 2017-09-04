using System.Drawing;

namespace Boerman.Core.Helpers
{
    public static class ImageHelper
    {
        public static Bitmap MergeImages(Bitmap bmp1, Bitmap bmp2, MergeMethod mergeMethod)
        {
            int bX = bmp1.Width > bmp2.Width ? bmp1.Width : bmp2.Width;
            int bY = bmp1.Height > bmp2.Height ? bmp1.Height : bmp2.Height;
            
            if (mergeMethod == MergeMethod.ToBottom) bX *= 2;
            if (mergeMethod == MergeMethod.ToBottom) bY *= 2;

            var result = new Bitmap(bmp1, bX, bY);

            var graphic = Graphics.FromImage(result);

            graphic.DrawImage(bmp1, new System.Drawing.Point(0, 0));

            if (mergeMethod == MergeMethod.ToBottom)
            {
                graphic.DrawImage(bmp2, new System.Drawing.Point(0, -bY/2));
            }
            if (mergeMethod == MergeMethod.ToRight)
            {
                graphic.DrawImage(bmp2, new System.Drawing.Point(-bX/2, 0));
            }
            if (mergeMethod == MergeMethod.Overlay)
            {
                graphic.DrawImage(bmp2, 0, 0);
            }

            return result;
        }

        public enum MergeMethod
        {
            Overlay,
            ToRight,
            ToBottom
        }
    }
}
