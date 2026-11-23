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
        private Image DisplayableTexture(Bitmap multiplier, uint[] maskTexture, List<object> patterns, bool overlay)
        {
            Rectangle rect = new Rectangle(0, 0, multiplier.Width, multiplier.Height);
            BitmapData bmpData = multiplier.LockBits(rect, ImageLockMode.ReadWrite, multiplier.PixelFormat);
            IntPtr ptr;
            if (bmpData.Stride > 0) ptr = bmpData.Scan0;
            else ptr = bmpData.Scan0 + bmpData.Stride * (multiplier.Height - 1);
            int bytes = Math.Abs(bmpData.Stride) * multiplier.Height;
            byte[] mult = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, mult, 0, bytes);

            for (int i = 0; i < mult.Length; i += 4)
            {
                float gray = ((mult[i] + mult[i + 1] + mult[i + 2]) / 3f) / (overlay ? 255f : 127);
                byte[] mask = BitConverter.GetBytes(maskTexture[i / 4]);
                byte[] maskControl = new byte[] { mask[2], mask[1], mask[0], mask[3] };
                for (int p = 0; p < patterns.Count; p++)     //for each pattern/color 
                {
                    float blend = maskControl[p] / 255f;
                    if (patterns[p] != null && maskControl[p] > 0)
                    {
                        if (patterns[p] is float[]) 
                        {
                            float[] color = (float[])patterns[p];
                            for (int k = 0; k < 3; k++)
                            {
                                float tmp;
                                tmp = gray * color[2 - k];
                                if (tmp < 0) tmp = 0;
                                if (tmp > 1) tmp = 1;
                                if (k == 0 || !overlay) mult[i + k] = (byte)(tmp * 255f);
                                else mult[i + k] = (byte)(((blend * tmp) + ((1f - blend) * (mult[i + k] / 255f))) * 255f);
                            }
                        }
                        else if (patterns[p] is Image)
                        {
                            Bitmap rgb = (Bitmap)patterns[p];
                            int currentX;
                            int currentY = Math.DivRem(i / 4, multiplier.Width, out currentX);
                            int w, h;
                            int x = Math.DivRem(currentX, rgb.Width, out w);
                            int y = Math.DivRem(currentY, rgb.Height, out h);
                            Color c = rgb.GetPixel(w, h);
                            float[] color = new float[] { c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f };
                            for (int k = 0; k < 3; k++)
                            {
                                float tmp;
                                tmp = gray * color[2 - k];
                                //if (k == 0) tmp = gray * blend * color[2 - k];
                                //else tmp = (gray * blend * color[2 - k]) + ((1f - blend) * (mult[i + k] / 255f));
                                if (tmp < 0) tmp = 0;
                                if (tmp > 1) tmp = 1;
                                if (k == 0 || !overlay) mult[i + k] = (byte)(tmp * 255f);
                                else mult[i + k] = (byte)(((blend * tmp) + ((1f - blend) * (mult[i + k] / 255f))) * 255f);
                            }
                        }
                    }
                }
              //  if (overlay) mult[i + 3] = opacity;
            }
            System.Runtime.InteropServices.Marshal.Copy(mult, 0, ptr, bytes);
            multiplier.UnlockBits(bmpData);
           // maskImage.UnlockBits(bmpData2);

            return multiplier;
        }

        private Bitmap DisplayableRGBPattern(PatternInfo pattern)
        {
            int width = 256, height = 256;
            float[][] colors = pattern.RGBColors;
            uint[] maskArray = FetchImageArrayFromDDS(pattern.rgbMask, ref errorList, width, height);

            Bitmap texture = new Bitmap(width, height);
            Rectangle rect = new Rectangle(0, 0, width, height);
            System.Drawing.Imaging.BitmapData bmpData = texture.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr;
            if (bmpData.Stride > 0) ptr = bmpData.Scan0;
            else ptr = bmpData.Scan0 + bmpData.Stride * (texture.Height - 1);
            int bytes = Math.Abs(bmpData.Stride) * texture.Height;
            byte[] textureArray = new byte[bytes];
            if (maskArray != null)
                for (int i = 0; i < maskArray.Length; i++)
                {
                    byte[] mask = BitConverter.GetBytes(maskArray[i]);
                    byte[] maskControl = new byte[] { mask[2], mask[1], mask[0], mask[3] };
                    for (int p = 0; p < colors.GetLength(0); p++)     //for each color
                    {
                        if (colors[p] != null && maskControl[p] > 0)
                        {
                            float blend = maskControl[p] / 255f;
                            if (colors[p] is float[])
                            {
                                float[] color = colors[p];
                                for (int k = 0; k < 3; k++)
                                {
                                    float tmp;
                                    if (p == 0) tmp = color[2 - k];
                                    else tmp = (blend * color[2 - k]) + ((1f - blend) * (textureArray[i * 4 + k] / 255f));
                                    if (tmp < 0) tmp = 0;
                                    if (tmp > 1) tmp = 1;
                                    textureArray[i * 4 + k] = (byte)(tmp * 255);
                                }
                            }
                        }
                    }
                    //textureArray[i * 4 + 2] = 255;
                    //textureArray[i * 4 + 1] = 0;
                    //textureArray[i * 4] = 0;
                    textureArray[i * 4 + 3] = 255;
                }
            System.Runtime.InteropServices.Marshal.Copy(textureArray, 0, ptr, bytes);
            texture.UnlockBits(bmpData);
            return texture;

            //Bitmap maskImage = FetchGameImageFromDDS(pattern.rgbMask, ref errorList, 256, 256);
            //Rectangle rect2 = new Rectangle(0, 0, maskImage.Width, maskImage.Height);
            //System.Drawing.Imaging.BitmapData bmpData2 = maskImage.LockBits(rect2, ImageLockMode.ReadOnly, maskImage.PixelFormat);
            //IntPtr ptr2;
            //if (bmpData2.Stride > 0) ptr2 = bmpData2.Scan0;
            //else ptr2 = bmpData2.Scan0 + bmpData2.Stride * (maskImage.Height - 1);
            //int bytes2 = Math.Abs(bmpData2.Stride) * maskImage.Height;
            //byte[] mask = new byte[bytes2];
            //System.Runtime.InteropServices.Marshal.Copy(ptr2, mask, 0, bytes2);

            //// argbValues1[i] = blue
            //// argbValues1[i + 1] = green
            //// argbValues1[i + 2] = red
            //// argbValues1[i + 3] = alpha

            //for (int i = 0; i < mask.Length; i += 4)
            //{
            //    byte[] maskControl = new byte[] { mask[i + 2], mask[i + 1], mask[i], mask[i + 3] };
            //    for (int p = 0; p < colors.GetLength(0); p++)     //for each color
            //    {
            //        if (colors[p] != null && maskControl[p] > 0)
            //        {
            //            float blend = maskControl[p] / 255f;
            //            if (colors[p] is float[])
            //            {
            //                float[] color = (float[])colors[p];
            //                for (int k = 0; k < 3; k++)
            //                {
            //                    float tmp;
            //                    if (p == 0) tmp = color[2 - k];
            //                    else tmp = (blend * color[2 - k]) + ((1f - blend) * (mask[i + k] / 255f));
            //                    if (tmp < 0) tmp = 0;
            //                    if (tmp > 1) tmp = 1;
            //                    mask[i + k] = (byte)(tmp * 255);
            //                }
            //            }
            //        }
            //    }
            //    mask[i + 3] = 255;
            //}
            //System.Runtime.InteropServices.Marshal.Copy(mask, 0, ptr2, bytes2);
            //maskImage.UnlockBits(bmpData2);
            //return maskImage;
        }

        private Bitmap DisplayableHSVPattern(PatternInfo pattern)
        {
            //if (pattern.hsvBaseBg != null && pattern.hsvBg != null && pattern.hsvShiftBg != null &&
            //    pattern.channels == null && pattern.hsv == null && pattern.hsvShift == null)
            //{
            //    Bitmap patternImage = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            //    Bitmap background = FetchGameImageFromDDS(pattern.background, ref errorList, 256, 256);

            //    BitmapData bmpData0 = patternImage.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, patternImage.PixelFormat);
            //    IntPtr ptr0;
            //    if (bmpData0.Stride > 0) ptr0 = bmpData0.Scan0;
            //    else ptr0 = bmpData0.Scan0 + bmpData0.Stride * (patternImage.Height - 1);
            //    byte[] finalArray = new byte[Math.Abs(bmpData0.Stride) * patternImage.Height];
            //    System.Runtime.InteropServices.Marshal.Copy(ptr0, finalArray, 0, finalArray.Length);

            //    BitmapData bmpData1 = background.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, background.PixelFormat);
            //    IntPtr ptr1;
            //    if (bmpData1.Stride > 0) ptr1 = bmpData1.Scan0;
            //    else ptr1 = bmpData1.Scan0 + bmpData1.Stride * (background.Height - 1);
            //    byte[] backArray = new byte[Math.Abs(bmpData1.Stride) * background.Height];
            //    System.Runtime.InteropServices.Marshal.Copy(ptr1, backArray, 0, backArray.Length);

            //    HSVColor backBase = pattern.hsvBaseBg != null ? new HSVColor(pattern.hsvBaseBg[0] * 360f, pattern.hsvBaseBg[1], pattern.hsvBaseBg[2]) : new HSVColor(0, 0, 0);
            //    HSVColor backChannel = pattern.hsvBg != null ? new HSVColor(pattern.hsvBg[0] * 360f, pattern.hsvBg[1], pattern.hsvBg[2]) : new HSVColor(0, 0, 0);
            //    HSVColor backShift = pattern.hsvShiftBg != null ? new HSVColor(pattern.hsvShiftBg[0] * 360f, pattern.hsvShiftBg[1], pattern.hsvShiftBg[2]) : new HSVColor(0, 0, 0);

            //    for (int i = 0; i < finalArray.Length; i += 4)
            //    {
            //        HSVColor hsv = new HSVColor(backArray[i + 2], backArray[i + 1], backArray[i]);
            //        byte[] color = (hsv + backChannel + backShift).ToRGBColor();
            //        //byte[] color = (hsv + baseHSV + backHSV + backShift).ToRGBColor();
            //        finalArray[i + 2] = color[0];
            //        finalArray[i + 1] = color[1];
            //        finalArray[i] = color[2];
            //    }
            //    System.Runtime.InteropServices.Marshal.Copy(finalArray, 0, ptr0, finalArray.Length);
            //    patternImage.UnlockBits(bmpData0);
            //    background.UnlockBits(bmpData1);

            //    return patternImage;
            //}
            //else
            //{

            Bitmap patternImage = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            Bitmap background = FetchGameImageFromDDS(pattern.background, ref errorList, 256, 256);
            uint[] rgb = FetchImageArrayFromDDS(pattern.rgbMask, ref errorList, 256, 256);
            Bitmap[] patBack = new Bitmap[3];
            if (pattern.channels != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (pattern.channels[i] != null) patBack[i] = FetchGameImageFromDDS(pattern.channels[i], ref errorList, 256, 256);
                    // if (patBack[i] == null) patBack[i] = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
                }
            }

            BitmapData bmpData0 = patternImage.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, patternImage.PixelFormat);
            IntPtr ptr0;
            if (bmpData0.Stride > 0) ptr0 = bmpData0.Scan0;
            else ptr0 = bmpData0.Scan0 + bmpData0.Stride * (patternImage.Height - 1);
            byte[] finalArray = new byte[Math.Abs(bmpData0.Stride) * patternImage.Height];
            System.Runtime.InteropServices.Marshal.Copy(ptr0, finalArray, 0, finalArray.Length);

            BitmapData bmpData1 = background.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, background.PixelFormat);
            IntPtr ptr1;
            if (bmpData1.Stride > 0) ptr1 = bmpData1.Scan0;
            else ptr1 = bmpData1.Scan0 + bmpData1.Stride * (background.Height - 1);
            byte[] backArray = new byte[Math.Abs(bmpData1.Stride) * background.Height];
            System.Runtime.InteropServices.Marshal.Copy(ptr1, backArray, 0, backArray.Length);

            //BitmapData bmpData2 = rgb.LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, rgb.PixelFormat);
            //IntPtr ptr2;
            //if (bmpData2.Stride > 0) ptr2 = bmpData2.Scan0;
            //else ptr2 = bmpData2.Scan0 + bmpData2.Stride * (rgb.Height - 1);
            //byte[] maskArray = new byte[Math.Abs(bmpData2.Stride) * rgb.Height];
            //System.Runtime.InteropServices.Marshal.Copy(ptr2, maskArray, 0, maskArray.Length);

            BitmapData bmpData3 = null;
            byte[] greenArray = null;
            if (pattern.channelEnabled[0])
            {
                bmpData3 = patBack[0].LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, patBack[0].PixelFormat);
                IntPtr ptr3;
                if (bmpData3.Stride > 0) ptr3 = bmpData3.Scan0;
                else ptr3 = bmpData3.Scan0 + bmpData3.Stride * (patBack[0].Height - 1);
                greenArray = new byte[Math.Abs(bmpData3.Stride) * patBack[0].Height];
                System.Runtime.InteropServices.Marshal.Copy(ptr3, greenArray, 0, greenArray.Length);
            }

            BitmapData bmpData4 = null;
            byte[] blueArray = null;
            if (pattern.channelEnabled[1])
            {
                bmpData4 = patBack[1].LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, patBack[1].PixelFormat);
                IntPtr ptr4;
                if (bmpData4.Stride > 0) ptr4 = bmpData4.Scan0;
                else ptr4 = bmpData4.Scan0 + bmpData4.Stride * (patBack[1].Height - 1);
                blueArray = new byte[Math.Abs(bmpData4.Stride) * patBack[1].Height];
                System.Runtime.InteropServices.Marshal.Copy(ptr4, blueArray, 0, blueArray.Length);
            }

            BitmapData bmpData5 = null;
            byte[] alphaArray = null;
            if (pattern.channelEnabled[2])
            {
                bmpData5 = patBack[2].LockBits(new Rectangle(0, 0, 256, 256), ImageLockMode.ReadOnly, patBack[2].PixelFormat);
                IntPtr ptr5;
                if (bmpData5.Stride > 0) ptr5 = bmpData5.Scan0;
                else ptr5 = bmpData5.Scan0 + bmpData5.Stride * (patBack[2].Height - 1);
                alphaArray = new byte[Math.Abs(bmpData5.Stride) * patBack[2].Height];
                System.Runtime.InteropServices.Marshal.Copy(ptr5, alphaArray, 0, alphaArray.Length);
            }

            // argbValues1[i] = blue
            // argbValues1[i + 1] = green
            // argbValues1[i + 2] = red
            // argbValues1[i + 3] = alpha

            //HSVColor backBase = pattern.hsvBaseBg != null ? new HSVColor(pattern.hsvBaseBg[0] * 360f, pattern.hsvBaseBg[1], pattern.hsvBaseBg[2]) : new HSVColor(0, 0, 0);
            //HSVColor greenBase = pattern.hsvBase != null && pattern.hsvBase[0] != null ? new HSVColor(pattern.hsvBase[0][0] * 360f, pattern.hsvBase[0][1], pattern.hsvBase[0][2]) : new HSVColor(0, 0, 0);
            //HSVColor blueBase = pattern.hsvBase != null && pattern.hsvBase[1] != null ? new HSVColor(pattern.hsvBase[1][0] * 360f, pattern.hsvBase[1][1], pattern.hsvBase[1][2]) : new HSVColor(0, 0, 0);
            //HSVColor alphaBase = pattern.hsvBase != null && pattern.hsvBase[2] != null ? new HSVColor(pattern.hsvBase[2][0] * 360f, pattern.hsvBase[2][1], pattern.hsvBase[2][2]) : new HSVColor(0, 0, 0);
            HSVColor backChannel = pattern.hsvBg != null ? new HSVColor(pattern.hsvBg[0] * 360f, pattern.hsvBg[1], pattern.hsvBg[2]) : new HSVColor(0, 0, 0);
            //HSVColor backShift = pattern.hsvShiftBg != null ? new HSVColor(pattern.hsvShiftBg[0] * 360f, pattern.hsvShiftBg[1], pattern.hsvShiftBg[2]) : new HSVColor(0, 0, 0);
            HSVColor greenChannel = pattern.hsv != null && pattern.hsv[0] != null ? new HSVColor(pattern.hsv[0][0] * 360f, pattern.hsv[0][1], pattern.hsv[0][2]) : new HSVColor(0, 0, 0);
            //HSVColor greenShift = pattern.hsvShift != null && pattern.hsvShift[0] != null ? new HSVColor(pattern.hsvShift[0][0] * 360f, pattern.hsvShift[0][1], pattern.hsvShift[0][2]) : new HSVColor(0, 0, 0);
            HSVColor blueChannel = pattern.hsv != null && pattern.hsv[1] != null ? new HSVColor(pattern.hsv[1][0] * 360f, pattern.hsv[1][1], pattern.hsv[1][2]) : new HSVColor(0, 0, 0);
            //HSVColor blueShift = pattern.hsvShift != null && pattern.hsvShift[1] != null ? new HSVColor(pattern.hsvShift[1][0] * 360f, pattern.hsvShift[1][1], pattern.hsvShift[1][2]) : new HSVColor(0, 0, 0);
            HSVColor alphaChannel = pattern.hsv != null && pattern.hsv[2] != null ? new HSVColor(pattern.hsv[2][0] * 360f, pattern.hsv[2][1], pattern.hsv[2][2]) : new HSVColor(0, 0, 0);
            //HSVColor alphaShift = pattern.hsvShift != null && pattern.hsvShift[2] != null ? new HSVColor(pattern.hsvShift[2][0] * 360f, pattern.hsvShift[2][1], pattern.hsvShift[2][2]) : new HSVColor(0, 0, 0);

            for (int i = 0; i < finalArray.Length; i += 4)
            {
                byte[] maskArray = BitConverter.GetBytes(rgb[i / 4]);
                HSVColor hsv = new HSVColor(backArray[i + 2], backArray[i + 1], backArray[i]);
                byte[] color = (hsv + backChannel).ToRGBColor();

                if (pattern.channelEnabled[0] && maskArray[1] > 0)       //green channel
                {
                    HSVColor hsv1 = new HSVColor(greenArray[i + 2], greenArray[i + 1], greenArray[i]);
                    byte[] color1 = (hsv1 + greenChannel).ToRGBColor();
                    float weight = maskArray[1] / 255f;
                    for (int c = 0; c < 3; c++)
                    {
                        color[c] = (byte)((color1[c] * weight) + (color[c] * (1f - weight)));
                    }
                }
                if (pattern.channelEnabled[1] && maskArray[0] > 0)       //blue channel
                {
                    HSVColor hsv1 = new HSVColor(blueArray[i + 2], blueArray[i + 1], blueArray[i]);
                    byte[] color1 = (hsv1 + blueChannel).ToRGBColor();
                    float weight = maskArray[0] / 255f;
                    for (int c = 0; c < 3; c++)
                    {
                        color[c] = (byte)((color1[c] * weight) + (color[c] * (1f - weight)));
                    }
                }
                if (pattern.channelEnabled[2] && maskArray[3] > 0)       //alpha channel
                {
                    HSVColor hsv1 = new HSVColor(alphaArray[i + 2], alphaArray[i + 1], alphaArray[i]);
                    byte[] color1 = (hsv1 + alphaChannel).ToRGBColor();
                    float weight = maskArray[3] / 255f;
                    for (int c = 0; c < 3; c++)
                    {
                        color[c] = (byte)((color1[c] * weight) + (color[c] * (1f - weight)));
                    }
                }
                finalArray[i + 2] = color[0];
                finalArray[i + 1] = color[1];
                finalArray[i] = color[2];
            }
            System.Runtime.InteropServices.Marshal.Copy(finalArray, 0, ptr0, finalArray.Length);
            patternImage.UnlockBits(bmpData0);
            background.UnlockBits(bmpData1);
           // rgb.UnlockBits(bmpData2);
            if (pattern.channelEnabled[0]) patBack[0].UnlockBits(bmpData3);
            if (pattern.channelEnabled[1]) patBack[1].UnlockBits(bmpData4);
            if (pattern.channelEnabled[2]) patBack[2].UnlockBits(bmpData5);

            return patternImage;

            //}
        }

        public class HSVColor
        {
            private float h, s, v;
            public float Hue {
                get { return this.h; }
                set
                {
                    this.h = value;
                    while (this.h > 360)
                    {
                        this.h -= 360;
                    }
                    while (this.h < 0)
                    {
                        this.h += 360;
                    }
                }
            }
            public float Saturation {
                get { return this.s; }
                set { this.s = value;
                    if (this.s > 1) this.s = 1;
                    if (this.s < 0) this.s = 0;
                } }
            public float Value {
                get { return this.v; }
                set { this.v = value;
                    if (this.v > 1) this.v = 1;
                    if (this.v < 0) this.v = 0;
                } }

            public HSVColor(float hue, float saturation, float value)
            {
                this.h = hue;
                this.s = saturation;
                this.v = value;
            }

            public HSVColor(float hue, float saturation, float value, bool corrected)
            {
                this.Hue = hue;
                this.Saturation = saturation;
                this.Value = value;
            }

            /// <summary>
            /// Create HSV color from red, green, blue values
            /// </summary>
            /// <param name="red">0-255</param>
            /// <param name="green">0-255</param>
            /// <param name="blue">0-255</param>
            /// <returns></returns>
            public HSVColor(byte red, byte green, byte blue)
            {
                double val1 = (double)red / (double)byte.MaxValue;
                double val2_1 = (double)green / (double)byte.MaxValue;
                double val2_2 = (double)blue / (double)byte.MaxValue;
                double num1 = Math.Min(Math.Min(val1, val2_1), val2_2);
                double num2 = Math.Max(Math.Max(val1, val2_1), val2_2);
                double h = 0.0;
                if (num1 != num2)
                {
                    if (val1 == num2 && val2_1 >= val2_2)
                        h = 60.0 * ((val2_1 - val2_2) / (num2 - num1)) + 0.0;
                    else if (val1 == num2 && val2_1 < val2_2)
                        h = 60.0 * ((val2_1 - val2_2) / (num2 - num1)) + 360.0;
                    else if (val2_1 == num2)
                        h = 60.0 * ((val2_2 - val1) / (num2 - num1)) + 120.0;
                    else if (val2_2 == num2)
                        h = 60.0 * ((val1 - val2_1) / (num2 - num1)) + 240.0;
                }
                this.Hue = (float)h;
                this.Value = (float)num2;
                if (num2 == 0.0)
                    this.Saturation = 0.0f;
                else
                    this.Saturation = (float)((num2 - num1) / num2);

                //int max = Math.Max(red, Math.Max(green, blue));
                //int min = Math.Min(red, Math.Min(green, blue));
                //float hue = 0f;
                //if (min == max)
                //{
                //    hue = 0;
                //}
                //else if (max == red)
                //{
                //    hue = (green - blue) / (max - min);

                //}
                //else if (max == green)
                //{
                //    hue = 2f + (blue - red) / (max - min);

                //}
                //else
                //{
                //    hue = 4f + (red - green) / (max - min);
                //}
                //hue = hue * 60f;
                //if (hue < 0) hue = hue + 360f;
                //float saturation = (max == 0) ? 0 : 1f - (1f * min / max);
                //float value = max / 255f;
                //this.Hue = hue;
                //this.Saturation = saturation;
                //this.Value = value;
            }

            /// <summary>
            /// Returns byte array of red (0-255), green (0-255), blue (0-255)
            /// </summary>
            public byte[] ToRGBColor()
            {
                double num1 = this.h;
                while (num1 < 0.0)
                    num1 += 360.0;
                while (num1 >= 360.0)
                    num1 -= 360.0;
                double num2;
                double num3;
                double num4;
                if (this.v <= 0.0)
                {
                    double num5;
                    num2 = num5 = 0.0;
                    num3 = num5;
                    num4 = num5;
                }
                else if (this.s <= 0.0)
                {
                    double num5;
                    num2 = num5 = this.v;
                    num3 = num5;
                    num4 = num5;
                }
                else
                {
                    double d = num1 / 60.0;
                    int num5 = (int)Math.Floor(d);
                    double num6 = d - (double)num5;
                    double num7 = this.v * (1.0 - this.s);
                    double num8 = this.v * (1.0 - this.s * num6);
                    double num9 = this.v * (1.0 - this.s * (1.0 - num6));
                    switch (num5)
                    {
                        case -1:
                            num4 = this.v;
                            num3 = num7;
                            num2 = num8;
                            break;
                        case 0:
                            num4 = this.v;
                            num3 = num9;
                            num2 = num7;
                            break;
                        case 1:
                            num4 = num8;
                            num3 = this.v;
                            num2 = num7;
                            break;
                        case 2:
                            num4 = num7;
                            num3 = this.v;
                            num2 = num9;
                            break;
                        case 3:
                            num4 = num7;
                            num3 = num8;
                            num2 = this.v;
                            break;
                        case 4:
                            num4 = num9;
                            num3 = num7;
                            num2 = this.v;
                            break;
                        case 5:
                            num4 = this.v;
                            num3 = num7;
                            num2 = num8;
                            break;
                        case 6:
                            num4 = this.v;
                            num3 = num9;
                            num2 = num7;
                            break;
                        default:
                            double num10;
                            num2 = num10 = this.v;
                            num3 = num10;
                            num4 = num10;
                            break;
                    }
                }
                return new byte[] { Clamp((int)(num4 * (double)byte.MaxValue)), Clamp((int)(num3 * (double)byte.MaxValue)), Clamp((int)(num2 * (double)byte.MaxValue)) };

                //int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
                //double f = h / 60 - Math.Floor(h / 60);

                //int val = (int)(this.v * 255);
                //byte v = (byte)val;
                //byte p = (byte)(val * (1 - s));
                //byte q = (byte)(val * (1 - f * s));
                //byte t = (byte)(val * (1 - (1 - f) * s));

                //if (hi == 0)
                //    return new byte[] { v, t, p };
                //else if (hi == 1)
                //    return new byte[] { q, v, p };
                //else if (hi == 2)
                //    return new byte[] { p, v, t };
                //else if (hi == 3)
                //    return new byte[] { p, q, v };
                //else if (hi == 4)
                //    return new byte[] { t, p, v };
                //else
                //    return new byte[] { v, p, q };
            }

            private static byte Clamp(int i)
            {
                if (i < 0)
                    return 0;
                if (i > byte.MaxValue)
                    return byte.MaxValue;
                return (byte)i;
            }

            public static HSVColor operator +(HSVColor hsv1, HSVColor hsv2)
            {
                return new HSVColor(hsv1.h + hsv2.h, hsv1.s + hsv2.s, hsv1.v + hsv2.v, true);
            }

            public static HSVColor operator -(HSVColor hsv1, HSVColor hsv2)
            {
                return new HSVColor(hsv1.h - hsv2.h, hsv1.s - hsv2.s, hsv1.v - hsv2.v, true);
            }
        }
    }
}
