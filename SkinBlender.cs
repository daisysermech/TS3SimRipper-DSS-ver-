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
        private Image DisplayableSkintone(TONE tone, AgeGender age, AgeGender gender, PartTypeFlags partType, Bitmap colorRamp,
            float colorSlider, float cutnessSlider, float cleavageSlider)
        {
            if (tone == null) return null;

            float[][] alphaMatrix = {
                     new float[] {1, 0, 0, 0, 0},       // m00 = red scaling factor
                     new float[] {0, 1, 0, 0, 0},       // m11 = green scaling factor
                     new float[] {0, 0, 1, 0, 0},       // m22 = blue scaling factor
                     new float[] {0, 0, 0, 1, 0},       // m33 = alpha scaling factor
                     new float[] {0, 0, 0, 0, 1}        // increments for R, G, B, A
              };

            float[][] redMatrix = {
                     new float[] {1.05f, 0, 0, 0, 0},       // m00 = red scaling factor
                     new float[] {0, 1, 0, 0, 0},       // m11 = green scaling factor
                     new float[] {0, 0, 1, 0, 0},       // m22 = blue scaling factor
                     new float[] {0, 0, 0, 1, 0},       // m33 = alpha scaling factor
                     new float[] {0, 0, 0, 0, 1}        // increments for R, G, B, A
              };

            TONE.SkinSet skinSet = tone.GetSkinSet(Species.Human, age, gender, partType);
            Bitmap details = FetchGameImageFromDDS(skinSet.LightSkinLink, ref errorList);

            using (Graphics gr = Graphics.FromImage(details))
            {
                Bitmap dark = FetchGameImageFromDDS(skinSet.darkLink, ref errorList);
                alphaMatrix[3][3] = colorSlider;
                ColorMatrix convert = new ColorMatrix(alphaMatrix);
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gr.DrawImage(dark, new Rectangle(0, 0, details.Width, details.Height), 0, 0, dark.Width, dark.Height, GraphicsUnit.Pixel, attributes);
                dark.Dispose();

                Bitmap overlay = FetchGameImageFromDDS(skinSet.overlayLink, ref errorList);
                if (overlay != null)
                {
                    gr.DrawImage(overlay, new Rectangle(0, 0, details.Width, details.Height), 0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel);
                    overlay.Dispose();
                }

            }

            if (colorRamp == null) return details;
            Color skinColor = colorRamp.GetPixel(colorRamp.Width / 2, (int)(colorRamp.Height * colorSlider));
            float[] color = new float[] { skinColor.B / 255f, skinColor.G / 255f, skinColor.R / 255f, 1f };

            Rectangle rect = new Rectangle(0, 0, details.Width, details.Height);
            BitmapData bmpData = details.LockBits(rect, ImageLockMode.ReadWrite, details.PixelFormat);
            IntPtr ptr;
            if (bmpData.Stride > 0) ptr = bmpData.Scan0;
            else ptr = bmpData.Scan0 + bmpData.Stride * (details.Height - 1);
            int bytes = Math.Abs(bmpData.Stride) * details.Height;
            byte[] detail = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, detail, 0, bytes);

            float contrast = 1.25f;
            float midpoint = 0.75f;

            for (int i = 0; i < detail.Length; i += 4)
            {
                float luma = 0.114f * detail[i] + 0.587f * detail[i + 1] + 0.299f * detail[i + 2];

                for (int j = 0; j < 3; j++)
                {
                    float tmp = detail[i + j] / 255f;
                   
                    tmp = ((tmp * color[j]) * 255);
                    if (tmp < 0f) tmp = 0;
                    if (tmp > 255) tmp = 255;

                    detail[i + j] = (byte)tmp;
                    detail[i + j] = (byte)Math.Max(0, Math.Min(255, luma + (detail[i + j] - luma) * 1.2f));
                }
                if (partType == PartTypeFlags.Face)
                {
                    int col;
                    int row = Math.DivRem(i / 4, details.Width, out col);
                    if (row > 665 && row < 842 && col < 85) detail[i + 3] = 0;
                }
            }
            // Ensure scalp textures are fully opaque (no accidental transparency from source images)
            if (partType == PartTypeFlags.Scalp)
            {
                for (int a = 3; a < detail.Length; a += 4)
                    detail[a] = 255;
            }
            System.Runtime.InteropServices.Marshal.Copy(detail, 0, ptr, bytes);
            details.UnlockBits(bmpData);

            if (partType == PartTypeFlags.Face)
            {
                //Image faceOver = (age > AgeGender.Child && gender == AgeGender.Female) ? Properties.Resources.afFaceOverlay : Properties.Resources.amFaceOverlay;
                Image faceOver = Properties.Resources.amFaceOverlay;
                using (Graphics gr = Graphics.FromImage(details))
                {
                    gr.DrawImage(faceOver, 0, 0, details.Width, details.Height);
                }
            }
            return details;
        }
    }
}
