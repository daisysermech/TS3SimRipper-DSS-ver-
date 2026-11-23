using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Resources;
using s3pi.ImageResource;

namespace TS3SimRipper
{
    public partial class Form1 : Form
    {
        private Image DisplayablePelt()
        //private Image DisplayablePelt(TS4SaveGame.PeltLayerData[] peltLayers, List<Image> overlays)
        {
            //Bitmap details = new Bitmap(2048, 1024);
            //if (details.Size != petTextureSize) details = new Bitmap(details, petTextureSize);

            //if (overlays.Count > 0)
            //{
            //  //  details = (Bitmap)overlays[0];
            //    using (Graphics g = Graphics.FromImage(details))
            //    {
            //        for (int i = 0; i < overlays.Count; i++)
            //        {
            //            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //            g.DrawImage(overlays[i], 0, 0, details.Width, details.Height);
            //        }
            //    }
            //}

            //Bitmap pelt = null;
            //for (int p = 0; p < peltLayers.Length; p++)
            //{
            //    PeltLayer peltLayer = FetchGamePeltLayer(new TGI((uint)ResourceTypes.PeltLayer, 0, peltLayers[p].layer_id), ref errorList);
            //    Bitmap alpha = FetchGameImageFromRLE(new TGI((uint)ResourceTypes.RLE2, 0, peltLayer.TextureKey), ref errorList);
            //    uint color = peltLayers[p].color;
            //    Bitmap layer = new Bitmap(alpha.Width, alpha.Height);
            //    using (Graphics gc = Graphics.FromImage(layer))
            //    {
            //        using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)color)))
            //        {
            //            gc.FillRectangle(brush, 0, 0, layer.Width, layer.Height);
            //        }
            //    }
            //    layer.SetAlphaFromImage(alpha);

            //    if (pelt == null)
            //    {
            //        pelt = new Bitmap(layer);
            //    }
            //    else
            //    {
            //        using (Graphics gr = Graphics.FromImage(pelt))
            //        {
            //            gr.DrawImage(layer, new Point(0, 0));
            //        }
            //    }
            //    layer.Dispose();
            //}

            ////WriteImage("Save details", details, "");
            ////WriteImage("Save pelt", pelt, "");

            //Rectangle rect1 = new Rectangle(0, 0, pelt.Width, pelt.Height);
            //System.Drawing.Imaging.BitmapData bmpData1 = pelt.LockBits(rect1, ImageLockMode.ReadWrite, details.PixelFormat);
            //IntPtr ptr1;
            //if (bmpData1.Stride > 0) ptr1 = bmpData1.Scan0;
            //else ptr1 = bmpData1.Scan0 + bmpData1.Stride * (pelt.Height - 1);
            //int bytes1 = Math.Abs(bmpData1.Stride) * pelt.Height;
            //byte[] argbValues1 = new byte[bytes1];
            //System.Runtime.InteropServices.Marshal.Copy(ptr1, argbValues1, 0, bytes1);

            //Rectangle rect2 = new Rectangle(0, 0, details.Width, details.Height);
            //System.Drawing.Imaging.BitmapData bmpData2 = details.LockBits(rect2, ImageLockMode.ReadWrite, details.PixelFormat);
            //IntPtr ptr2;
            //if (bmpData2.Stride > 0) ptr2 = bmpData2.Scan0;
            //else ptr2 = bmpData2.Scan0 + bmpData2.Stride * (details.Height - 1);
            //int bytes2 = Math.Abs(bmpData2.Stride) * details.Height;
            //byte[] argbValues2 = new byte[bytes2];
            //System.Runtime.InteropServices.Marshal.Copy(ptr2, argbValues2, 0, bytes2);

            //// argbValues1[i] = blue
            //// argbValues1[i + 1] = green
            //// argbValues1[i + 2] = red
            //// argbValues1[i + 3] = alpha

            //for (int i = 0; i < argbValues1.Length; i += 4)
            //{
            //    //ushort[] hsl1 = GetHSL(argbValues1[i + 2], argbValues1[i + 1], argbValues1[i]);
            //    //ushort[] hsl2 = GetHSL(argbValues2[i + 2], argbValues2[i + 1], argbValues2[i]);
            //    //byte[] rgb = GetRGB(hsl1[0], hsl1[1], hsl2[2]);
            //    //argbValues1[i + 2] = rgb[0];
            //    //argbValues1[i + 1] = rgb[1];
            //    //argbValues1[i] = rgb[2];
            //    //argbValues1[i + 3] = 255;

            //    for (int j = 0; j < 3; j++)
            //    {
            //        float tmp;
            //        if (argbValues1[i + j] > 128)           //hard light blend, color over details
            //            tmp = 255 - ((255f - 2f * (argbValues1[i + j] - 128f)) * (255f - argbValues2[i + j]) / 256f);
            //        else
            //            tmp = (2f * argbValues1[i + j] * argbValues2[i + j]) / 256f;
            //        argbValues1[i + j] = (byte)tmp;
            //    }
            //}
            //System.Runtime.InteropServices.Marshal.Copy(argbValues1, 0, ptr1, bytes1);
            //pelt.UnlockBits(bmpData1);
            //details.UnlockBits(bmpData2);
            //details.Dispose();

            //return pelt;
            return new Bitmap(100, 100);
        }
    }
}
