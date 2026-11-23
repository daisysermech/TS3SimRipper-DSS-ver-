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

                //if (age > AgeGender.Child)
                //{
                //    Bitmap cut = FetchGameImageFromDDS(skinSet.cutnessLink, ref errorList);
                //    if (cut != null)
                //    {
                //        alphaMatrix[3][3] = cutnessSlider;
                //        convert = new ColorMatrix(alphaMatrix);
                //        attributes = new ImageAttributes();
                //        attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //        gr.DrawImage(cut, new Rectangle(0, 0, details.Width, details.Height), 0, 0, cut.Width, cut.Height, GraphicsUnit.Pixel, attributes);
                //        cut.Dispose();
                //    }

                //    if ((gender & AgeGender.Female) > 0)
                //    {
                //        Bitmap cleavage = FetchGameImageFromDDS(skinSet.cleavageLink, ref errorList);
                //        if (cleavage != null)
                //        {
                //            alphaMatrix[3][3] = cleavageSlider;
                //            convert = new ColorMatrix(alphaMatrix);
                //            attributes = new ImageAttributes();
                //            attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                //            gr.DrawImage(cleavage, new Rectangle(0, 0, details.Width, details.Height), 0, 0, cleavage.Width, cleavage.Height, GraphicsUnit.Pixel, attributes);
                //            cleavage.Dispose();
                //        }
                //    }
                //}
                //convert = new ColorMatrix(redMatrix);
                //attributes = new ImageAttributes();
                //attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
              //  gr.DrawImage(details, new Rectangle(0, 0, details.Width, details.Height), 0, 0, details.Width, details.Height, GraphicsUnit.Pixel, attributes);
            }

            if (colorRamp == null) return details;
            Color skinColor = colorRamp.GetPixel(colorRamp.Width / 2, (int)(colorRamp.Height * colorSlider));
            float[] color = new float[] { skinColor.B / 255f, skinColor.G / 255f, skinColor.R / 255f, 1f };
            //ushort[] color = GetHSL(skinColor.R, skinColor.G, skinColor.B);
            //ushort hue = color[0];
            //ushort saturation = color[1];

            Rectangle rect = new Rectangle(0, 0, details.Width, details.Height);
            System.Drawing.Imaging.BitmapData bmpData = details.LockBits(rect, ImageLockMode.ReadWrite, details.PixelFormat);
            IntPtr ptr;
            if (bmpData.Stride > 0) ptr = bmpData.Scan0;
            else ptr = bmpData.Scan0 + bmpData.Stride * (details.Height - 1);
            int bytes = Math.Abs(bmpData.Stride) * details.Height;
            byte[] detail = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, detail, 0, bytes);

            // argbValues1[i] = blue
            // argbValues1[i + 1] = green
            // argbValues1[i + 2] = red
            // argbValues1[i + 3] = alpha
            float contrast = 1.25f;
            float midpoint = 0.75f;

            for (int i = 0; i < detail.Length; i += 4)
            {
                for (int j = 0; j < 3; j++)
                {
                    //increase contrast slightly
                    float tmp = detail[i + j] / 255f;
                    //tmp = (((tmp - midpoint) * contrast) + midpoint);
                    //if (tmp < 0f) tmp = 0;
                    //if (tmp > 255) tmp = 255;

                    //float tmp = detail[i + j];
                    //first pass softlight, details over color (keeps color)
                    //tmp = ((1f - 2f * (detail[i + j] / 255f)) * (color[j] / 255f) * (color[j] / 255f) + 2f * (detail[i + j] / 255f) * (color[j] / 255f)) * 255f;
                    //   tmp = (float)Math.Min(tmp * 1.2, 255.0);        //but it's too dark

                    //if (color[j] > 128)           //second pass overlay blend, details over color
                    //    tmp = 255f - ((255f - 2f * (detail[i + j] - 128f)) * (255f - color[j]) / 256f);
                    //else
                    //    tmp = (2f * detail[i + j] * color[j]) / 256f;

                    //tmp = ((tmp2 * pass2opacity) + (tmp * (1f - pass2opacity)));          // blend using 2nd pass opacity

                    //if (saturation <= 100)
                    //tmp = ((tmp / 255f) * (tmp + ((2f * rgbOver[2 - j]) / 255f) * (255f - tmp)));  //3rd pass is soft light blend, color over all

                    tmp = ((tmp * color[j]) * 255);
                    if (tmp < 0f) tmp = 0;
                    if (tmp > 255) tmp = 255;
                    // byte tmp = Math.Min(detail[i + j], color[j]);
                    detail[i + j] = (byte)tmp;
                }
                //ushort[] hsl = GetHSL(detail[i + 2], detail[i + 1], detail[i]);
                //byte[] rgb = GetRGB(hue, saturation, hsl[2]);
                //detail[i + 2] = rgb[0];
                //detail[i + 1] = rgb[1];
                //detail[i] = rgb[2];
                // Restore face-only transparent region: only clear alpha for Face partType
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
