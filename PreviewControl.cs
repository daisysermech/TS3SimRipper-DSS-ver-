/* TS4 SimRipper, a tool for creating custom content for The Sims 4,
   Copyright (C) 2014  C. Marinetti

   This program is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.

   You should have received a copy of the GNU General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
   The author may be contacted at modthesims.info, username cmarNYC. */

using Collada141;
using s3pi.ImageResource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using static TS3SimRipper.Form1;

namespace TS3SimRipper
{
    public partial class Form1 : Form
    {
        ResourceManager rm = Properties.Resources.ResourceManager;
        GEOM currentBodyModel = null;
        GEOM currentGlassModel = null;
        GEOM currentHairModel = null;
        GEOM currentFaceModel = null;
        GEOM currentScalpModel = null;
        List<GEOM> currentAccessories = null;
        string currentName;
        Species currentSpecies;
        AgeGender currentAge;
        AgeGender currentGender;
        RIG currentRig;
        List<OutfitInfo> currentOutfits;
        List<Image> outfitOverlays;
        List<ImageStack> imageStack;
        List<ImageStack> glassStack;
        int outfitIndex;
        Image currentBodyTexture;
        Image currentBodySpecular;
        Image currentBodyBump;
        Image currentGlassTexture;
        Image currentGlassSpecular;
        Image currentGlassBump;
        Image currentHairTexture;
        Image currentHairSpecular;
        Image currentHairBump;
        Image currentFaceTexture;
        Image currentFaceSpecular;
        Image currentFaceBump;
        Image currentScalpTexture;
        Image currentScalpSpecular;
        Image currentScalpBump;
        List<Image> currentAccessoryTexture;
        List<Image> currentAccessorySpecular;
        List<Image> currentAccessoryBump;
        List<string> currentAccessoryName;
        GEOM BaseModel;
        GEOM BaseGlass;
        PartTypeFlags[] PartTypes = new PartTypeFlags[] { PartTypeFlags.Body, PartTypeFlags.Face, PartTypeFlags.Hair, PartTypeFlags.Scalp, PartTypeFlags.Accessory };
        string[] Patterns = new string[] { "Pattern A", "Pattern B", "Pattern C", "Pattern D" };
        string[] HairColors = new string[] { "Root Color", "Highlight Color", "Tip Color" };
        string[] MultiTintColors = new string[] { "Tint Color", "Tint Color A", "Tint Color B", "Tint Color C", "Tint Color D" };

        public static GEOM LoadBONDMorph(GEOM baseMesh, BOND boneDelta, RIG rig, ref string errorMsg)
        {
            if (baseMesh == null) return null;
            if (boneDelta == null) return baseMesh;
            GEOM morphMesh = new GEOM(baseMesh);
            Vector3 unit = new Vector3(1f, 1f, 1f);
            string missingBones = "";
            float weight = boneDelta.weight;

            morphMesh.SetupDeltas();

            // for (int i = boneDelta.adjustments.Length - 1; i >= 0; i--)
            foreach (BOND.BoneAdjust delta in boneDelta.adjustments)
            {
                //    BOND.BoneAdjust delta = boneDelta.adjustments[i];

                RIG.Bone bone = rig.GetBone(delta.slotHash);
                if (bone == null)
                {
                    missingBones += "Bone not found: " + delta.slotHash.ToString("X8") + ", ";
                    continue;
                }
                Vector3 localScale = new Vector3(delta.scaleX, delta.scaleY, delta.scaleZ);
                Vector3 localOffset = new Vector3(delta.offsetX, delta.offsetY, delta.offsetZ);
                Quaternion localRotation = new Quaternion(delta.quatX, delta.quatY, delta.quatZ, delta.quatW);
                if (localRotation.isEmpty) localRotation = Quaternion.Identity;
                if (!localRotation.isNormalized) localRotation.Balance();

                Vector3 worldScale = (bone.MorphRotation.toMatrix3D() * Matrix3D.FromScale(localScale + unit)).Scale - unit;
                Vector3 worldOffset = (bone.MorphRotation * localOffset * bone.MorphRotation.Conjugate()).toVector3();
                Quaternion worldRotation = bone.MorphRotation * localRotation * bone.MorphRotation.Conjugate();

                //BoneMorpher2(true, morphMesh, rig, bone, weight, localOffset, localScale, localRotation);
                morphMesh.BoneMorpher(bone, weight, worldOffset, worldScale, worldRotation);
                //rig.BoneMorpher(bone, weight, localScale, localOffset, localRotation);
                // BoneMorpher(morphMesh, rig, bone, weight, worldOffset, worldScale, worldRotation);
            }

            morphMesh.UpdatePositions();

            foreach (BOND.BoneAdjust delta in boneDelta.adjustments)
            {
                RIG.Bone bone = rig.GetBone(delta.slotHash);
                if (bone == null)
                {
                    continue;
                }
                Vector3 localScale = new Vector3(delta.scaleX, delta.scaleY, delta.scaleZ);
                Vector3 localOffset = new Vector3(delta.offsetX, delta.offsetY, delta.offsetZ);
                Quaternion localRotation = new Quaternion(delta.quatX, delta.quatY, delta.quatZ, delta.quatW);
                if (localRotation.isEmpty) localRotation = Quaternion.Identity;
                if (!localRotation.isNormalized) localRotation.Balance();

                rig.BoneMorpher(bone, weight, localScale, localOffset, localRotation);
            }

            if (missingBones.Length > 0)
            {
                errorMsg += Environment.NewLine + boneDelta.name + ": " + missingBones + Environment.NewLine;
            }
            return morphMesh;
        }

        //internal static void BoneMorpher(GEOM morphMesh, RIG rig, RIG.Bone bone, float weight, Vector3 offset, Vector3 scale, Quaternion rotation)
        //{
        //    morphMesh.BoneMorpher(bone, weight, offset, scale, rotation);
        //    RIG.Bone[] childBones = rig.GetChildren(bone.BoneHash);
        //    foreach (RIG.Bone child in childBones)
        //    {
        //        BoneMorpher(morphMesh, rig, child, pivotBone, weight, offset, scale, rotation);
        //    }
        //}

        //internal static void BoneMorpher2(bool isParent, GEOM morphMesh, RIG rig, RIG.Bone bone, float weight, Vector3 offset, Vector3 scale, Quaternion rotation)
        //{
        //    Vector3 tmp = bone.WorldPosition;
        //    morphMesh.BoneVectorize(bone);
        //    if (isParent)
        //    {
        //        bone.UpdateLocalData(weight, offset, rotation, scale);
        //        bone.CalculateTransforms();
        //    }
        //    else
        //    {
        //        bone.ScaleBone(scale, weight);
        //        bone.CalculateTransforms();
        //    }
        //   // MessageBox.Show(tmp.ToString() + Environment.NewLine + bone.WorldPosition.ToString());
        //    morphMesh.BoneMorpher2(bone, weight, scale, rotation);
        //    morphMesh.BoneDeVectorize(bone, weight);
        //    RIG.Bone[] childBones = rig.GetChildren(bone.BoneHash);
        //    foreach (RIG.Bone child in childBones)
        //    {
        //        BoneMorpher2(false, morphMesh, rig, child, weight, offset, scale, rotation);
        //    }
        //}

        public static GEOM LoadBGEOMorph(GEOM baseMesh, BGEO morph, int lod, Species species, AgeGender age, AgeGender gender)
        {
            if (baseMesh == null || !baseMesh.hasVertexIDs) return baseMesh;
            if (morph == null) return new GEOM(baseMesh);
            GEOM morphMesh = new GEOM(baseMesh);
            float weight = morph.weight;
            int entry = morph.GetSection1EntryIndex(species, age, gender);
            if (entry < 0) return new GEOM(baseMesh);
            //int startID = morph.getLODstartVertexID(entry, lod);
            //int startIndex = morph.getSection2StartIndex(entry, lod);
            List<BGEO.VertexData> vertDeltas = morph.GetDeltas(entry, lod);

            for (int i = 0; i < morphMesh.numberVertices; i++)
            {
                int vertID = morphMesh.getVertexID(i);
                if (vertDeltas.Exists(x => x.vertexID == vertID))
                {
                    BGEO.VertexData vert = vertDeltas.Find(x => x.vertexID == vertID);

                    Vector3 pos = new Vector3(morphMesh.getPosition(i));
                    Vector3 delta = vert.position;
                    morphMesh.setPosition(i, (pos + (delta * weight)).Coordinates);

                    Vector3 norm = new Vector3(morphMesh.getNormal(i));
                    Vector3 delta2 = vert.normals;
                    morphMesh.setNormal(i, (norm + (delta * weight)).Coordinates);
                }
            }
            return morphMesh;
        }

        /// <summary>
        /// Apply morph meshes
        /// </summary>
        /// <param name="baseMesh">base</param>
        /// <param name="morphs">morph meshes for one morph: fat, fit, thin or special</param>
        /// <param name="weight">morph weight</param>
        /// <returns></returns>
        public static GEOM LoadGEOMMorph(GEOM baseMesh, GEOM[] morphs, float weight)
        {
            if (baseMesh == null) return baseMesh;
            if (morphs.Length == 0 || morphs == null) return new GEOM(baseMesh);
            GEOM morphMesh = new GEOM(baseMesh);

            Dictionary<int, Vector3> deltaPos = new Dictionary<int, Vector3>();
            Dictionary<int, Vector3> deltaNorm = new Dictionary<int, Vector3>();
            for (int i = 0; i < morphs.Length; i++)
            {
                for (int j = 0; j < morphs[i].numberVertices; j++)
                {
                    int id = morphs[i].getVertexID(j);
                    if (!deltaPos.ContainsKey(id)) deltaPos.Add(id, new Vector3(morphs[i].getPosition(j)));
                    if (!deltaNorm.ContainsKey(id)) deltaNorm.Add(id, new Vector3(morphs[i].getNormal(j)));
                }
            }

            for (int i = 0; i < morphMesh.numberVertices; i++)
            {
                int vertID = morphMesh.getVertexID(i);
                Vector3 pos = new Vector3(morphMesh.getPosition(i));
                Vector3 norm = new Vector3(morphMesh.getNormal(i));

                Vector3 delta = new Vector3();
                deltaPos.TryGetValue(vertID, out delta);
                morphMesh.setPosition(i, (pos + (delta * weight)).Coordinates);
                Vector3 delta2 = new Vector3();
                deltaNorm.TryGetValue(vertID, out delta2);
                morphMesh.setNormal(i, (norm + (delta * weight)).Coordinates);
            }
            return morphMesh;
        }
        static byte Mix(float c1, float c2, float c3, float c4,
                    float w1, float w2, float w3, float w4, float lum)
        {
            float v = (c1 * w1 + c2 * w2 + c3 * w3 + c4 * w4) * lum;
            return (byte)Math.Max(0, Math.Min(255, v));
        }

        static byte[] ReadBytes(Bitmap bmp, out int stride)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            stride = data.Stride;
            byte[] bytes = new byte[Math.Abs(stride) * bmp.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            bmp.UnlockBits(data);
            return bytes;
        }

        static void WriteBytes(Bitmap bmp, byte[] bytes)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bmp.UnlockBits(data);
        }
        private Bitmap HairColoration(Bitmap grayBmp, Bitmap maskSrc, List<object> colors)
        {

            float[] colR = To255(colors[0]);  // where mask is red
            float[] colG = To255(colors[1]); // where mask is green
            float[] colB = To255(colors[2]); // where mask is blue
            float[] colBase = To255(colors[3]); // leftover (mask dark)

            int w = grayBmp.Width, h = grayBmp.Height;

            byte[] gray = ReadBytes(grayBmp, out int stride);
            byte[] mask = ReadBytes(maskSrc, out _);
            byte[] res = new byte[gray.Length];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * stride + x * 4; // BGRA order

                    float r = mask[i + 2] / 255f;
                    float g = mask[i + 1] / 255f;
                    float b = mask[i] / 255f;
                    float bs = Math.Max(0f, 1f - (r + g + b));

                    float sum = r + g + b + bs;
                    r /= sum; g /= sum; b /= sum; bs /= sum;

                    // luma of the B&W image
                    float lum = (0.299f * gray[i + 2] +
                                 0.587f * gray[i + 1] +
                                 0.114f * gray[i]) / 255f;

                    res[i + 2] = Mix(colR[0], colG[0], colB[0], colBase[0], r, g, b, bs, lum); // R
                    res[i + 1] = Mix(colR[1], colG[1], colB[1], colBase[1], r, g, b, bs, lum); // G
                    res[i] = Mix(colR[2], colG[2], colB[2], colBase[2], r, g, b, bs, lum); // B
                    res[i + 3] = gray[i + 3];
                }
            }

            var output = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            WriteBytes(output, res);
            return output;
        }
        private static Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private static float[] To255(object c) =>
            c != null ? ((float[])c).Select(x => x * 255f).ToArray() : null;
        private static byte ClampByte(float value)
        {
            return (byte)(Math.Max(0, Math.Min(Math.Round(value), 255)));
        }
        private Bitmap MakeupColoration(Bitmap grayBmp, Bitmap maskSrc, List<object> colors, float opacity = 1f)
        {
            float[][] cols = colors.Select(To255).ToArray();

            int w = grayBmp.Width;
            int h = grayBmp.Height;

            byte[] gray = ReadBytes(grayBmp, out int stride);
            byte[] mask = ReadBytes(maskSrc, out _);
            byte[] res = new byte[gray.Length];

            int channelCount = colors.Count(x => x != null);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int i = y * stride + x * 4;

                    // Mask channels, normalized to 0..1.
                    float r = mask[i + 2] / 255f;
                    float g = mask[i + 1] / 255f;
                    float b = mask[i] / 255f;

                    // Only channels actually supplied in colors participate.
                    float wr = channelCount >= 1 ? r : 0f;
                    float wg = channelCount >= 2 ? g : 0f;
                    float wb = channelCount >= 3 ? b : 0f;

                    // Fourth 'channel' is the leftover/dark portion.
                    float wbBase = channelCount >= 4
                        ? Math.Max(0f, 1f - (wr + wg + wb))
                        : 0f;

                    float sum = wr + wg + wb + wbBase;
                    float[] wX = new float[4];

                    if (sum > 0f)
                    {
                        wr /= sum;
                        wg /= sum;
                        wb /= sum;
                        wbBase /= sum;

                        wX = new float[] { wr, wg, wb, wbBase };
                    }

                    // Luma of grayscale multiplier.
                    float lum =
                        (0.299f * gray[i + 2] +
                         0.587f * gray[i + 1] +
                         0.114f * gray[i]) / 255f;

                    float outR, outG, outB;

                    if (sum > 0f)
                    {
                        // Start with black then add only the channels that exist.
                        outR = 0f;
                        outG = 0f;
                        outB = 0f;

                        for (int j = 0; j < channelCount; j++)
                        {
                            outR += cols[j][0] * wX[j];
                            outG += cols[j][1] * wX[j];
                            outB += cols[j][2] * wX[j];
                        }
                        /*if (channelCount >= 1)
                        {
                            outR += cols[0][0] * wr;
                            outG += cols[0][1] * wr;
                            outB += cols[0][2] * wr;
                        }

                        if (channelCount >= 2)
                        {
                            outR += cols[1][0] * wg;
                            outG += cols[1][1] * wg;
                            outB += cols[1][2] * wg;
                        }

                        if (channelCount >= 3)
                        {
                            outR += cols[2][0] * wb;
                            outG += cols[2][1] * wb;
                            outB += cols[2][2] * wb;
                        }

                        if (channelCount >= 4)
                        {
                            outR += cols[3][0] * wbBase;
                            outG += cols[3][1] * wbBase;
                            outB += cols[3][2] * wbBase;
                        }*/

                        outR *= lum;
                        outG *= lum;
                        outB *= lum;
                    }
                    else
                    {
                        // No mask component applies here.
                        // Preserve the original grayscale.
                        outR = gray[i + 2];
                        outG = gray[i + 1];
                        outB = gray[i];
                    }

                    res[i + 2] = ClampByte(outR);
                    res[i + 1] = ClampByte(outG);
                    res[i] = ClampByte(outB);

                    res[i + 3] = (byte)Math.Round(gray[i + 3] * opacity);
                }
            }

            var output = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            WriteBytes(output, res);
            return output;
        }

        private void GetCurrentModel()
        {
            int high_Hair = 0;
            int high_Scalp = 0;
            int high_Body = 0;
            int high_Face = 0;

            string fullInfo = "";
            presetList = "";
            AgeGender adjustedAge = currentAge >= AgeGender.Teen && currentAge <= AgeGender.Elder ? AgeGender.Adult : currentAge;
            string prefix = GetBodyCompletePrefix(currentSpecies, adjustedAge, currentGender);
            currentRig = GetTS3Rig(currentSpecies, adjustedAge, ref fullInfo);
            GEOM body = null;
            GEOM glass = null;
            CASP[] outfitParts = currentOutfits[outfitIndex].casps;
            string[] packNames = currentOutfits[outfitIndex].packages;
            BGEO[] morphBGEO = currentOutfits[outfitIndex].bgeos;
            BOND[] morphBOND = currentOutfits[outfitIndex].bonds;
            float[] physique = currentOutfits[outfitIndex].physique;
            imageStack = new List<ImageStack>();
            glassStack = new List<ImageStack>();
            outfitOverlays = new List<Image>();
            currentBodyModel = null;
            currentGlassModel = null;
            currentHairModel = null;
            currentFaceModel = null;
            currentScalpModel = null;
            currentAccessories = new List<GEOM>();
            currentAccessoryTexture = new List<Image>();
            currentAccessorySpecular = new List<Image>();
            currentAccessoryBump = new List<Image>();

            fullInfo += "Fat: " + physique[0].ToString() + ", Fit: " + physique[1].ToString() + ", Thin:" + physique[2].ToString() +
                Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < outfitParts.Length; i++)
            {
                PartTypeFlags? partType = outfitParts[i].outfitPreset.GetPartType();
                if (partType == null) partType = outfitParts[i].PartType;
                ClothingType partCategory = outfitParts[i].TypeOfClothing;
                fullInfo += outfitParts[i].PartName + " (" + packNames[i] + ")" + Environment.NewLine + Environment.NewLine;
                List<TGI> tgis = new List<TGI>();
                int meshLOD = 0;
                foreach (TGI t in outfitParts[i].VPXYLinks)
                {
                    VPXY vpxy = FetchGameVPXY(t, out packNames[i], ref errorList);
                    TGI[] tmp = vpxy.MeshLinks(0);
                    if (tmp.Length == 0)
                    {
                        tmp = vpxy.MeshLinks(1);
                        meshLOD = 1;
                    }
                    tgis.AddRange(tmp);
                }
                TGI bumpTGI = null;
                bool gotGlass = false;
                GEOM accessoryMesh = null;
                bool gotMesh = false;

                foreach (TGI tgi in tgis)
                {
                    GEOM geom = FetchGameGEOM(tgi, packNames[i], ref errorList);
                    if (geom == null) continue;
                    gotMesh = true;
                    TGI[] physiqueBlends = outfitParts[i].BlendLinks;
                    for (int p = 0; p < physique.Length; p++)
                    {
                        string packname;
                        BBLN bbln = FetchGameBlend(physiqueBlends[p], out packname, ref errorList);
                        if (bbln == null) continue;
                        TGI bgeoTGI = bbln.BGEOlink;
                        BGEO bgeo = FetchGameBGEO(bgeoTGI, ref errorList);
                        foreach (BBLN.Entry entry in bbln.Entries)
                        {
                            foreach (BBLN.MorphEntry g in entry.geomMorphs)
                            {
                                if (bgeo != null)
                                {
                                    bgeo.weight = physique[p] * g.amount;
                                    geom = LoadBGEOMorph(geom, bgeo, meshLOD, currentSpecies, currentAge, currentGender);
                                }
                                else if (bbln.TGIList != null && bbln.TGIList.Length > g.tgiIndex && geom.hasVertexIDs)
                                {
                                    TGI vpxyTGI = bbln.TGIList[g.tgiIndex];
                                    string packagename;
                                    VPXY vpxy = FetchGameVPXY(vpxyTGI, out packagename, ref errorList);
                                    if (vpxy != null)
                                    {
                                        TGI[] deltaLinks = vpxy.MeshLinks(meshLOD);
                                        List<GEOM> deltas = new List<GEOM>();
                                        foreach (TGI link in deltaLinks)
                                        {
                                            GEOM delta = FetchGameGEOM(link, packNames[i], ref errorList);
                                            if (delta != null) deltas.Add(delta);
                                        }
                                        geom = LoadGEOMMorph(geom, deltas.ToArray(), physique[p]);
                                    }
                                }
                            }
                            foreach (BBLN.MorphEntry bone in entry.boneMorphs)
                            {
                                TGI vpxyTGI = bbln.TGIList[bone.tgiIndex];
                                string packagename;
                                VPXY vpxy = FetchGameVPXY(vpxyTGI, out packagename, ref errorList);
                                if (vpxy == null) continue;
                                foreach (TGI t in vpxy.AllLinks)
                                {
                                    BOND bond = FetchGameBOND(t, ref errorList);
                                    if (bond != null)
                                    {
                                        bond.weight = physique[p] * bone.amount;
                                        geom = LoadBONDMorph(geom, bond, currentRig, ref errorList);
                                    }
                                }
                            }
                        }
                    }
                    foreach (BGEO b in morphBGEO)
                    {
                        geom = LoadBGEOMorph(geom, b, meshLOD, currentSpecies, currentAge, currentGender);
                    }
                    if (bumpTGI == null && geom.Shader.normalIndex >= 0) bumpTGI = geom.TGIList[geom.Shader.normalIndex];

                    if (partType == PartTypeFlags.Hair)
                    {
                        if (currentHairModel == null) currentHairModel = geom;
                        else currentHairModel.AppendMesh(geom);
                    }
                    else if (partType == PartTypeFlags.Face)
                    {
                        if (currentFaceModel == null) currentFaceModel = geom;
                        else
                        {
                            if (geom.ShaderHash == (uint)SimShader.SimEyelashes)
                                currentFaceModel.AppendMesh(geom);
                            else
                            {
                                geom.AppendMesh(currentFaceModel);
                                currentFaceModel = geom;
                            }
                        }
                    }
                    else if (partType == PartTypeFlags.Scalp)
                    {
                        if (currentScalpModel == null) currentScalpModel = geom;
                        else currentScalpModel.AppendMesh(geom);
                    }
                    else if (partType == PartTypeFlags.Body)
                    {
                        if (geom.ShaderHash == (uint)SimShader.SimHair || geom.ShaderHash == (uint)SimShader.SimGlass || geom.ShaderHash == (uint)SimShader.SimEyelashes)
                        {
                            gotGlass = true;
                            if (glass == null) glass = geom;
                            else glass.AppendMesh(geom);
                        }
                        else
                        {
                            if (body == null) body = geom;
                            else body.AppendMesh(geom);
                        }
                    }
                    else
                    {
                        if (accessoryMesh == null) accessoryMesh = geom;
                        else accessoryMesh.AppendMesh(geom);
                    }
                }

                if (accessoryMesh != null) currentAccessories.Add(accessoryMesh);

                presetList += outfitParts[i].outfitPreset.XML + Environment.NewLine;

                Image texture = null;
                Image specular = null;
                Image overlay = null;
                float opacity = 1f;
                Complate? complate = outfitParts[i].outfitPreset.GetComplate();
                if (complate == null)
                {
                    if (partType == PartTypeFlags.Hair) complate = Complate.HairUniversal;
                    else if (partType == PartTypeFlags.Face) complate = Complate.CasSkinOverlayTintable;
                    else if (partType == PartTypeFlags.Scalp) complate = null;
                    else complate = Complate.CasRgbMask;
                }
                if (complate == Complate.HairUniversal)
                {
                    texture = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Diffuse Map"), ref errorList);
                    specular = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Clothing Specular"), ref errorList);
                    var hair_mask = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Control Map"), ref errorList); //our mask!
                    float[] haircolor = outfitParts[i].outfitPreset.GetColor("Diffuse Color");

                    List<object> hairColors = getHairColors(outfitParts[i]);
                    hairColors.Add(haircolor);
                    if (texture == null || haircolor == null)
                    {
                        texture = new Bitmap(1024, 1024);
                        using (Graphics g = Graphics.FromImage(texture)) { g.Clear(Color.Transparent); }
                    }
                    else
                    {

                        if (texture.Width != hair_mask.Width || texture.Height != hair_mask.Height)
                            hair_mask = (Bitmap)ResizeImage(hair_mask, texture.Width, texture.Height);

                        if (texture.Width != specular.Width || texture.Height != specular.Height)
                            specular = (Bitmap)ResizeImage(specular, texture.Width, texture.Height);

                        texture = HairColoration((Bitmap)texture, hair_mask, hairColors);

                        if (texture.Width > high_Hair) high_Hair = texture.Width;

                        if (outfitParts[i].outfitPreset.IfKeyTrue("IsHat"))
                        {
                            Image texture2 = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Multiplier"), ref errorList);
                            if (texture2 != null)
                            {
                                uint[] mask = FetchImageArrayFromDDS(outfitParts[i].outfitPreset.GetLink("Mask"), ref errorList, texture2.Width, texture2.Height);
                                presetList += outfitParts[i].PartName + Environment.NewLine;
                                List<object> colors = GetPatternColors(outfitParts[i], ref presetList);
                                if (mask != null && colors != null)
                                    texture2 = DisplayableTexture((Bitmap)texture2, mask, colors, true);

                                if (texture.Width != texture2.Width || texture.Height != texture2.Height)
                                    texture2 = (Bitmap)ResizeImage(texture2, texture.Width, texture.Height);

                                using (Graphics g = Graphics.FromImage(texture))
                                {
                                    g.CompositingMode = CompositingMode.SourceOver;
                                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    g.SmoothingMode = SmoothingMode.HighQuality;

                                    g.DrawImage(texture2, 0, 0, texture2.Width, texture2.Height);
                                }
                            }
                        }

                        if (outfitParts[i].outfitPreset.IfKeyTrue("DrawsOnScalp"))
                        {
                            Image texture3 = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Scalp Diffuse Map"), ref errorList);
                            Image specular3 = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Scalp Specular Map"), ref errorList);
                            var hair_mask3 = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Scalp Control Map"), ref errorList); //our mask!
                            float[] haircolor3 = outfitParts[i].outfitPreset.GetColor("Diffuse Color");

                            List<object> hairColors3 = getHairColors(outfitParts[i]);
                            hairColors3.Add(haircolor3);
                            if (texture3 == null || haircolor3 == null)
                            {
                                texture3 = new Bitmap(1024, 1024);
                                using (Graphics g = Graphics.FromImage(texture3)) { g.Clear(Color.Transparent); }
                            }
                            else
                            {

                                if (texture3.Width != hair_mask3.Width || texture3.Height != hair_mask3.Height)
                                    hair_mask3 = (Bitmap)ResizeImage(hair_mask3, texture3.Width, texture3.Height);

                                if (texture3.Width != specular3.Width || texture3.Height != specular3.Height)
                                    specular3 = (Bitmap)ResizeImage(specular3, texture3.Width, texture3.Height);

                                texture3 = HairColoration((Bitmap)texture3, hair_mask3, hairColors3);
                            }
                            imageStack.Add(new ImageStack(PartTypeFlags.Scalp, ClothingType.Scalp, texture3, specular3, null, gotMesh, outfitParts[i].PartName, opacity));
                        }
                    }
                    //ALSO ADD FACE DRAWING TOO

                }
                //CasSkinOverlayTintable is face and brows, CasSkinOverlayColorizable is eyes (default). CasOverlayMultiTintable is for makeup
                else if (complate == Complate.CasOverlayMultiTintable)
                {
                    texture = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Face Overlay"), ref errorList);
                    specular = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Face Specular"), ref errorList);
                    if (texture != null)
                    {
                        var face_mask = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Mask"), ref errorList); //our mask!
                        opacity = outfitParts[i].outfitPreset.GetOpacity();


                        if (texture.Width != face_mask.Width || texture.Height != face_mask.Height)
                            face_mask = (Bitmap)ResizeImage(face_mask, texture.Width, texture.Height);

                        if (texture.Width != specular.Width || texture.Height != specular.Height)
                            specular = (Bitmap)ResizeImage(specular, texture.Width, texture.Height);

                        List<object> faceColors = getFaceColors(outfitParts[i]);

                        texture = MakeupColoration((Bitmap)texture, face_mask, faceColors, opacity);

                        if (texture.Width > high_Face) high_Face = texture.Width;
                    }
                }
                else if (complate == Complate.CasSkinOverlayColorizable || complate == Complate.CasSkinOverlayTintable)
                {
                    texture = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Face Overlay"), ref errorList);
                    specular = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Face Specular"), ref errorList);
                    if (texture != null)
                    {
                        var face_mask = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Mask"), ref errorList); //our mask!
                        if (face_mask == null)
                        {
                            face_mask = new Bitmap(texture.Width, texture.Height);
                            using (Graphics gfx = Graphics.FromImage(face_mask))
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0)))
                                gfx.FillRectangle(brush, 0, 0, texture.Width, texture.Height);
                        }
                        opacity = outfitParts[i].outfitPreset.GetOpacity();


                        if (texture.Width != face_mask.Width || texture.Height != face_mask.Height)
                            face_mask = (Bitmap)ResizeImage(face_mask, texture.Width, texture.Height);

                        if (texture.Width != specular.Width || texture.Height != specular.Height)
                            specular = (Bitmap)ResizeImage(specular, texture.Width, texture.Height);

                        float[] color;
                        if (complate == Complate.CasSkinOverlayColorizable) 
                            color = outfitParts[i].outfitPreset.GetColor("Color");
                        else
                            color = outfitParts[i].outfitPreset.GetColor("Tint Color");
                        //eye should use "Color", else tint

                        texture = MakeupColoration((Bitmap)texture, face_mask, new List<object>(){color}, opacity);

                        if (texture.Width > high_Face) high_Face = texture.Width;
                    }
                }
                else if (partType == PartTypeFlags.Scalp)
                {
                    texture = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Overlay"), ref errorList);
                    specular = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Specular"), ref errorList);
                    if (texture != null)
                        if (texture.Width > high_Scalp)
                            high_Scalp = texture.Width;
                }
                else //if (complate == Complate.CasRgbMask || complate == Complate.CasRbgaMask)
                {
                    texture = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Multiplier"), ref errorList);
                    specular = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Clothing Specular"), ref errorList);
                    overlay = FetchGameImageFromDDS(outfitParts[i].outfitPreset.GetLink("Overlay"), ref errorList);
                    uint[] mask = texture != null ? FetchImageArrayFromDDS(outfitParts[i].outfitPreset.GetLink("Mask"), ref errorList, texture.Width, texture.Height) : null;
                    presetList += outfitParts[i].PartName + Environment.NewLine;
                    List<object> colors = GetPatternColors(outfitParts[i], ref presetList);
                    if (texture != null && mask != null && colors != null)
                    {
                        texture = DisplayableTexture((Bitmap)texture, mask, colors, true);

                        texture = DrawOverlay(texture, overlay);

                        if (texture.Width > high_Body)
                            high_Body = texture.Width;
                    }
                    if (texture == null)
                        texture = overlay;
                }
                presetList += Environment.NewLine;

                Image bumpmap = null;
                imageStack.Add(new ImageStack(partType, partCategory, texture, specular, bumpmap, gotMesh, outfitParts[i].PartName, opacity));
            }

            fullInfo += currentOutfits[outfitIndex].blendNames;

            RIG glassRig = new RIG(currentRig);
            RIG hairRig = new RIG(currentRig);
            RIG scalpRig = new RIG(currentRig);
            RIG faceRig = new RIG(currentRig);
            RIG[] accRig = new RIG[currentAccessories.Count];
            for (int i = 0; i < currentAccessories.Count; i++) accRig[i] = new RIG(currentRig);
            foreach (BOND b in morphBOND)
            {
                body = LoadBONDMorph(body, b, currentRig, ref errorList);
                glass = LoadBONDMorph(glass, b, glassRig, ref errorList);
                currentHairModel = LoadBONDMorph(currentHairModel, b, hairRig, ref errorList);
                currentScalpModel = LoadBONDMorph(currentScalpModel, b, scalpRig, ref errorList);
                currentFaceModel = LoadBONDMorph(currentFaceModel, b, faceRig, ref errorList);
                for (int i = 0; i < currentAccessories.Count; i++)
                {
                    currentAccessories[i] = LoadBONDMorph(currentAccessories[i], b, accRig[i], ref errorList);
                }
            }

            BaseModel = body;
            BaseGlass = glass;

            //    if (pregnancyProgress > 0f)
            //    {
            //        if (currentSpecies == Species.Human)
            //        {
            //            if (pregnantModifier[0] != null) pregnantModifier[0].weight = pregnancyProgress / 1.25f;
            //            if (pregnantModifier[1] == null) pregnantModifier[1].weight = pregnancyProgress / 1.25f;
            //            CurrentModel = LoadDMapMorph(BaseModel, pregnantModifier[0], pregnantModifier[1]);
            //            if (BaseGlass != null) GlassModel = LoadDMapMorph(BaseGlass, pregnantModifier[0], pregnantModifier[1]);
            //        }
            //    }
            //    else
            //    {
            currentBodyModel = BaseModel;
            currentGlassModel = BaseGlass;
            //    }
            //}

            Image currentScalpSkin = null, currentFaceSkin = null, currentBodySkin = null;
            if (currentSpecies == Species.Human && currentOutfits[outfitIndex].skintone != null)
            {
                currentTONE = currentOutfits[outfitIndex].skintone;
                Bitmap skinRamp = FetchGameImageFromDDS(currentTONE.ToneRampLink, ref errorList, false);
                float skinColor = currentOutfits[outfitIndex].skinColorSlider;
                float skinCutness = currentOutfits[outfitIndex].cutnessSlider;
                float skinCleavage = currentOutfits[outfitIndex].cleavageSlider;
                currentScalpSkin = DisplayableSkintone(currentTONE, currentAge, currentGender, PartTypeFlags.Scalp, skinRamp, skinColor, skinCutness, skinCleavage);
                currentFaceSkin = DisplayableSkintone(currentTONE, currentAge, currentGender, PartTypeFlags.Face, skinRamp, skinColor, skinCutness, skinCleavage);
                currentBodySkin = DisplayableSkintone(currentTONE, currentAge, currentGender, PartTypeFlags.Body, skinRamp, skinColor, skinCutness, skinCleavage);
            }
            else
            {
                currentScalpSkin = new Bitmap(1024, 1024);
                using (Graphics g = Graphics.FromImage(currentScalpSkin)) { g.Clear(Color.LightGray); }
                currentFaceSkin = new Bitmap(1024, 1024);
                using (Graphics g = Graphics.FromImage(currentFaceSkin)) { g.Clear(Color.LightGray); }
                currentBodySkin = new Bitmap(1024, 1024);
                using (Graphics g = Graphics.FromImage(currentBodySkin)) { g.Clear(Color.LightGray); }
            }

            currentBodyTexture = currentBodySkin != null ? currentBodySkin : new Bitmap(high_Body, high_Body);
            currentFaceTexture = currentFaceSkin != null ? currentFaceSkin : new Bitmap(high_Face, high_Face);
            currentScalpTexture = currentScalpSkin != null ? currentScalpSkin : new Bitmap(high_Scalp, high_Scalp);
            currentHairTexture = new Bitmap(high_Hair, high_Hair);
            currentAccessoryTexture = new List<Image>();
            currentAccessorySpecular = new List<Image>();
            currentAccessoryName = new List<string>();

            //if (AllTextures_checkBox.Checked)
            //{
            //    using (Graphics gs = Graphics.FromImage(bump))
            //    {
            //        gs.Clear(Color.Gray);
            //    }
            //    Bitmap alpha = new Bitmap(bump);
            //    bump.SetAlphaFromImage(alpha);
            //    alpha.Dispose();
            //}

            bool gotBump = false;
            float[][] alphaMatrix = {
                   new float[] {1, 0, 0, 0, 0},       // m00 = red scaling factor
                   new float[] {0, 1, 0, 0, 0},       // m11 = green scaling factor
                   new float[] {0, 0, 1, 0, 0},       // m22 = blue scaling factor
                   new float[] {0, 0, 0, 1, 0},       // m33 = alpha scaling factor
                   new float[] {0, 0, 0, 0, 1}        // increments for R, G, B, A
                };
            imageStack = SortStack(imageStack);
            if (currentBodyTexture != null)
            {
                using (Graphics g = Graphics.FromImage(currentBodyTexture))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    Point origin = new Point(0, 0);
                    for (int i = 0; i < imageStack.Count; i++)
                    {
                        if (imageStack[i].image != null && imageStack[i].partType == PartTypeFlags.Body)
                        {
                            alphaMatrix[3][3] = imageStack[i].opacity;
                            ColorMatrix convert = new ColorMatrix(alphaMatrix);
                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            g.DrawImage(imageStack[i].image, new Rectangle(0, 0, currentBodyTexture.Width, currentBodyTexture.Height), 0, 0, imageStack[i].image.Width, imageStack[i].image.Height, GraphicsUnit.Pixel, attributes);
                            //g.DrawImage(imageStack[i].image, origin);
                        }
                        //  if (imageStack[i].specular != null) gs.DrawImage(DisplayableSpecular(imageStack[i].specular), origin);
                        //if (imageStack[i].specular != null & imageStack[i].image != null)
                        //    spec = LayerWithMask(spec, DisplayableSpecular(imageStack[i].specular), imageStack[i].image, imageStack[i].isMesh);
                        //else if (imageStack[i].specular != null)
                        //{
                        //    using (Graphics gs = Graphics.FromImage(spec))
                        //    {
                        //        gs.DrawImage(DisplayableSpecular(imageStack[i].specular), origin);
                        //    }
                        //}
                        //if (AllTextures_checkBox.Checked)
                        //{
                        //    if (imageStack[i].bumpmap != null & imageStack[i].image != null)
                        //    {
                        //        bump = LayerWithMask(bump, ExpandPartialImage(imageStack[i].bumpmap, imageStack[i].partType, currentSpecies),
                        //            imageStack[i].image, imageStack[i].isMesh);
                        //        gotBump = true;
                        //    }
                        //    else if (imageStack[i].bumpmap != null)
                        //    {
                        //        using (Graphics gs = Graphics.FromImage(bump))
                        //        {
                        //            gs.DrawImage(imageStack[i].bumpmap, GetImageStartPoint(imageStack[i].bumpmap, imageStack[i].partType, currentSpecies));
                        //        }
                        //        gotBump = true;
                        //    }
                        //}
                    }
                }
            }
            if (currentFaceTexture != null)
            {
                using (Graphics g = Graphics.FromImage(currentFaceTexture))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    Point origin = new Point(0, 0);
                    for (int i = 0; i < imageStack.Count; i++)
                    {
                        if (imageStack[i].image != null && imageStack[i].partType == PartTypeFlags.Face)
                        {
                            alphaMatrix[3][3] = imageStack[i].opacity;
                            ColorMatrix convert = new ColorMatrix(alphaMatrix);
                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            g.DrawImage(imageStack[i].image, new Rectangle(0, 0, currentFaceTexture.Width, currentFaceTexture.Height), 0, 0, imageStack[i].image.Width, imageStack[i].image.Height, GraphicsUnit.Pixel, attributes);
                        }
                    }
                }
            }
            if (currentScalpTexture != null)
            {
                using (Graphics g = Graphics.FromImage(currentScalpTexture))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    Point origin = new Point(0, 0);
                    for (int i = 0; i < imageStack.Count; i++)
                    {
                        if (imageStack[i].image != null && imageStack[i].partType == PartTypeFlags.Scalp)
                        {
                            alphaMatrix[3][3] = imageStack[i].opacity;
                            ColorMatrix convert = new ColorMatrix(alphaMatrix);
                            ImageAttributes attributes = new ImageAttributes();
                            attributes.SetColorMatrix(convert, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            g.DrawImage(imageStack[i].image, new Rectangle(0, 0, currentScalpTexture.Width, currentScalpTexture.Height), 0, 0, imageStack[i].image.Width, imageStack[i].image.Height, GraphicsUnit.Pixel, attributes);

                        }
                    }
                }
            }
            if (currentHairTexture != null)
            {
                using (Graphics g = Graphics.FromImage(currentHairTexture))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    Point origin = new Point(0, 0);
                    for (int i = 0; i < imageStack.Count; i++)
                    {
                        if (imageStack[i].image != null && imageStack[i].partType == PartTypeFlags.Hair)
                        {
                            g.DrawImage(imageStack[i].image, origin);
                        }
                    }
                }
            }

            for (int i = 0; i < imageStack.Count; i++)
            {
                if (imageStack[i].image != null && imageStack[i].partType == PartTypeFlags.Accessory && imageStack[i].gotMesh)
                {
                    currentAccessoryTexture.Add(imageStack[i].image);
                    currentAccessorySpecular.Add(imageStack[i].specular);
                    currentAccessoryName.Add("Accessory_" + currentAccessoryName.Count);
                    //currentAccessoryName.Add(imageStack[i].partName.Replace(" ", ""));
                }
            }

            SimInfo_button.Tag = simDesc + fullInfo;
            SimError_button.Tag = startupErrors + Environment.NewLine + errorList;
            SimError_button.Enabled = startupErrors.Length + errorList.Length > 0;
        }

        internal List<object> GetPatternColors(CASP outfitPart, ref string presetList)
        {
            List<object> colors = new List<object>();
            for (int p = 0; p < 4; p++)
            {
                if (outfitPart.outfitPreset.PatternEnabled(Patterns[p]))
                {
                    PatternInfo pat = outfitPart.outfitPreset.GetPatternInfo(Patterns[p]);
                    if (pat.Type == PatternType.Solid)
                    {
                        colors.Add(pat.solidColor);
                    }
                    else if (pat.Type == PatternType.Colored)
                    {
                        Bitmap rgb = DisplayableRGBPattern(pat);
                        colors.Add(rgb);
                    }
                    else if (pat.Type == PatternType.HSV)
                    {
                        Bitmap hsv = DisplayableHSVPattern(pat);
                        colors.Add(hsv);
                    }
                    else
                    {
                        colors.Add(null);
                    }
                    presetList += Patterns[p] + ": " + pat.Type.ToString() + Environment.NewLine;
                }
                else
                {
                    colors.Add(null);
                    presetList += Patterns[p] + ": " + "Null" + Environment.NewLine;
                }
            }
            return colors;
        }

        internal List<object> getFaceColors(CASP outfitPart)
        {
            List<object> colors = new List<object>();
            for (int p = 1; p <= 4; p++)
            {
                if (outfitPart.outfitPreset.PatternEnabled(MultiTintColors[p]))
                {
                    colors.Add(outfitPart.outfitPreset.GetColor(MultiTintColors[p]));
                }
                else
                {
                    colors.Add(null);
                }
            }
            return colors;
        }

        internal List<object> getHairColors(CASP outfitPart)
        {
            List<object> colors = new List<object>();
            for (int p = 0; p < 3; p++)
            {
                colors.Add(outfitPart.outfitPreset.GetColor(HairColors[p]));
            }
            return colors;
        }

        public class PatternInfo
        {
            internal string patternName;
            internal TGI rgbMask;
            internal float[] solidColor;
            private float[][] rgbColors;
            internal TGI background;
            internal TGI[] channels;
            internal bool[] channelEnabled;
            internal float[] hsvBaseBg;
            internal float[][] hsvBase;
            internal float[] hsvBg;
            internal float[][] hsv;
            internal float[] hsvShiftBg;
            internal float[][] hsvShift;

            //public override string ToString()
            //{
            //    string s = patternName + Environment.NewLine;
            //    s += "rbgMask: " + rgbMask.ToString() + Environment.NewLine;
            //    s += "solidColor: " + PrintFloatArray(solidColor) + Environment.NewLine;
            //    s += "rgbColors: ";
            //    for (int i = 0; i < rgbColors.GetLength(0); i++)
            //    {
            //        s += PrintFloatArray(rgbColors[i]);
            //        if (i < rgbColors.GetLength(0) - 1) s += " : ";
            //    }
            //}
            internal string PrintFloatArray(float[] values)
            {
                string s = "";
                for (int i = 0; i < solidColor.Length; i++)
                {
                    s += solidColor[i].ToString();
                    if (i < solidColor.Length - 1) s += ", ";
                }
                return s;
            }

            internal PatternType Type
            {
                get
                {
                    if (patternName.Contains("solidColor") || patternName.Contains("Flat Color")) return PatternType.Solid;
                    else if (patternName.Contains("None")) return PatternType.None;
                    else if (hsvBaseBg != null || hsvBase != null || hsvBg != null || hsv != null || hsvShiftBg != null || hsvShift != null) return PatternType.HSV;
                    else return PatternType.Colored;
                }
            }

            internal float[][] RGBColors
            {
                get
                {
                    List<float[]> colors = new List<float[]>();
                    for (int i = 0; i < rgbColors.GetLength(0); i++)
                    {
                        if (rgbColors[i] != null) colors.Add(rgbColors[i]);
                    }
                    return colors.ToArray();
                }
            }

            public PatternInfo(string patternName, TGI maskLink, float[] solidColor, float[][] channelColors, TGI background, TGI[] channels,
                bool[] channelEnabled, float[] hsvBaseBg, float[][] hsvBase, float[] hsvBg, float[][] hsv, float[] hsvShiftBg, float[][] hsvShift)
            {
                this.patternName = patternName;
                this.rgbMask = maskLink;
                this.solidColor = solidColor;
                this.rgbColors = channelColors;
                this.background = background;
                this.channels = channels;
                this.channelEnabled = channelEnabled;
                this.hsvBaseBg = hsvBaseBg;
                this.hsvBase = hsvBase;
                this.hsvShiftBg = hsvShiftBg;
                this.hsvShift = hsvShift;
                this.hsvBg = hsvBg;
                this.hsv = hsv;
            }
        }

        internal enum PatternType
        {
            Solid,
            Colored,
            HSV,
            None
        }

        internal class MeshInfo
        {
            internal ClothingType partType;
            internal AgeGender ageGender;
            internal GEOM geom;
            internal MeshInfo(ClothingType partType, AgeGender ageGender, GEOM mesh)
            {
                this.partType = partType;
                this.ageGender = ageGender;
                this.geom = mesh;
            }
        }

        internal class ImageStack
        {
            internal PartTypeFlags? partType;
            internal ClothingType partCat;
            internal float opacity;
            internal Image image;
            internal Image specular;
            internal Image bumpmap;
            internal bool gotMesh;
            internal string partName;
            internal ImageStack(PartTypeFlags? partType, ClothingType partCategory, Image texture, Image specular, Image bumpmap, bool gotMesh, string partName, float opacity)
            {
                this.partType = partType;
                this.partCat = partCategory;
                this.image = texture;
                this.specular = specular;
                this.bumpmap = bumpmap;
                this.gotMesh = gotMesh;
                this.partName = partName;
                this.opacity = opacity;
            }
        }

        private void SaveModelMorph(MeshFormat format)
        {
            string savename = "";
            Working_label.Visible = true;
            Working_label.Refresh();
            string path = Properties.Settings.Default.LastSavePath;
            List<GEOM> geomArray = new List<GEOM>();
            List<string> nameArray = new List<string>();
            List<bool[]> gotTextures = new List<bool[]>();
            byte[] tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(currentName);
            string normalName = System.Text.Encoding.UTF8.GetString(tempBytes).Replace(" ", "");
            if (currentFaceModel != null) { geomArray.Add(currentFaceModel); nameArray.Add("Face"); gotTextures.Add(new bool[] { currentFaceTexture != null, currentFaceSpecular != null }); }
            if (currentBodyModel != null) { geomArray.Add(currentBodyModel); nameArray.Add("Body"); gotTextures.Add(new bool[] { currentBodyTexture != null, currentBodySpecular != null }); }
            if (currentScalpModel != null) { geomArray.Add(currentScalpModel); nameArray.Add("Scalp"); gotTextures.Add(new bool[] { currentScalpTexture != null, currentScalpSpecular != null }); }
            for (int i = 0; i < currentAccessories.Count; i++)
            {
                geomArray.Add(currentAccessories[i]); nameArray.Add(currentAccessoryName[i].Replace(" ", "")); gotTextures.Add(new bool[] { currentAccessoryTexture[i] != null, currentAccessorySpecular[i] != null });
            }
            if (currentHairModel != null) { geomArray.Add(currentHairModel); nameArray.Add("Hair"); gotTextures.Add(new bool[] { currentHairTexture != null, currentHairSpecular != null }); }
            if (currentGlassModel != null) { geomArray.Add(currentGlassModel); nameArray.Add("Glass"); gotTextures.Add(new bool[] { currentGlassTexture != null, currentGlassSpecular != null }); }

            if (format == MeshFormat.OBJ)
            {
                OBJ obj = new OBJ(geomArray.ToArray(), 0, false, nameArray.ToArray());
                Working_label.Visible = false;
                savename = WriteOBJFile("Save OBJ of morphed model", obj, path + "\\" + normalName);
            }
            else if (format == MeshFormat.MS3D)
            {
                MS3D ms3d = new MS3D(geomArray.ToArray(), currentRig, 0, nameArray.ToArray());
                Working_label.Visible = false;
                savename = WriteMS3D("Save MS3D of morphed model", ms3d, path + "\\" + normalName);
            }
            else if (format == MeshFormat.DAE)
            {
                DAE dae = new DAE(geomArray.ToArray(), nameArray.ToArray(), gotTextures.ToArray(), currentRig, false, CleanDAE_checkBox.Checked);
                Working_label.Text = "Saving....";
                savename = WriteDAEFile("Save Collada DAE of morphed model", dae, false, path + "\\" + normalName);
                Working_label.Text = "Working....";
                Working_label.Visible = false;
            }
            else if (format == MeshFormat.GEOM)
            {
                Working_label.Visible = false;
                savename = WriteGEOM("Save GEOM of morphed model", currentBodyModel, path + "\\" + normalName);
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = Imagefilter;
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.Title = "Save Textures";
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.DefaultExt = "png";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.FileName = path + "\\" + normalName;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    savename = saveFileDialog1.FileName;
                }
            }
            if (savename.Length == 0) return;
            savename = Path.GetDirectoryName(savename) + '\\' + Path.GetFileNameWithoutExtension(savename).Replace(" ", "_");
            if (currentScalpTexture != null) currentScalpTexture.Save(savename + "_Scalp_diffuse.png");
            if (currentScalpSpecular != null) currentScalpSpecular.Save(savename + "_Scalp_specular.png");
            if (currentFaceTexture != null) currentFaceTexture.Save(savename + "_Face_diffuse.png");
            if (currentFaceSpecular != null) currentFaceSpecular.Save(savename + "_Face_specular.png");
            if (currentBodyTexture != null) currentBodyTexture.Save(savename + "_Body_diffuse.png");
            if (currentBodySpecular != null) currentBodySpecular.Save(savename + "_Body_specular.png");
            if (currentHairTexture != null) currentHairTexture.Save(savename + "_Hair_diffuse.png");
            if (currentHairSpecular != null) currentHairSpecular.Save(savename + "_Hair_specular.png");
            for (int i = 0; i < currentAccessoryTexture.Count; i++)
            {
                if (currentAccessoryTexture[i] != null) currentAccessoryTexture[i].Save(savename + "_Accessory_" + i + "_diffuse.png");
                if (currentAccessorySpecular[i] != null) currentAccessorySpecular[i].Save(savename + "_Accessory_" + i + "_specular.png");
            }

            Working_label.Visible = false;
            Properties.Settings.Default.LastSavePath = Path.GetDirectoryName(savename);
            Properties.Settings.Default.Save();
        }

        private void SimTrouble_button_Click(object sender, EventArgs e)
        {
            string path = Properties.Settings.Default.LastSavePath;
            byte[] tempBytes = null;
            if (currentName != null)
            {
                tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(currentName);
                string basename = System.Text.Encoding.UTF8.GetString(tempBytes).Replace(" ", "");
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.Title = "Save zip file with sim CC and save data";
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.DefaultExt = "zip";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.FileName = path + "\\" + basename + "_TroubleShoot.zip";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(saveFileDialog1.FileName)) File.Delete(saveFileDialog1.FileName);
                    using (ZipArchive zip = ZipFile.Open(saveFileDialog1.FileName, ZipArchiveMode.Create))
                    {
                        ZipArchiveEntry info = zip.CreateEntry("Sim Information.txt");
                        using (Stream zipEntryStream = info.Open())
                        {
                            MemoryStream ms = new MemoryStream(ASCIIEncoding.Default.GetBytes((string)SimInfo_button.Tag));
                            ms.CopyTo(zipEntryStream);
                        }
                        if (errorList.Length > 0)
                        {
                            ZipArchiveEntry err = zip.CreateEntry("Sim Errors.txt");
                            using (Stream zipEntryStream = err.Open())
                            {
                                MemoryStream ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(errorList));
                                ms.CopyTo(zipEntryStream);
                            }
                        }
                        if (TroubleshootPackage != null)
                        {
                            ZipArchiveEntry entry = zip.CreateEntry("Resources Package");
                            using (Stream zipEntryStream = entry.Open())
                            {
                                Stream s = new MemoryStream();
                                TroubleshootPackage.SaveAs(s);
                                s.Position = 0;
                                s.CopyTo(zipEntryStream);
                            }
                        }
                        for (int i = 0; i < currentSavePacks.Count; i++)
                        {
                            ZipArchiveEntry entry = zip.CreateEntry(currentSavePackNames[i]);
                            using (Stream zipEntryStream = entry.Open())
                            {
                                Stream s = new MemoryStream();
                                currentSavePacks[i].SaveAs(s);
                                s.Position = 0;
                                s.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
            }
        }
        public static string GetBodyCompletePrefix(Species species, AgeGender age, AgeGender gender)
        {
            string specifier = "";
            if (age == AgeGender.Toddler) specifier = (species == Species.Human ? "p" : "c");
            else if (age == AgeGender.Child) specifier = "c";
            else specifier = (species == Species.Human ? "y" : "a");
            if (species != Species.Human) specifier +=
                (age == AgeGender.Child && species == Species.LittleDog) ? "d" :
                species.ToString().Substring(0, 1).ToLower();
            else if (age <= AgeGender.Child) specifier += "u";
            else specifier += (gender == AgeGender.Male || gender == AgeGender.Female) ? gender.ToString().Substring(0, 1).ToLower() : "f";
            return specifier;
        }

        public static string GetPhysiquePrefix(Species species, AgeGender age, AgeGender gender)
        {
            string specifier = "";
            if (age == AgeGender.Toddler) specifier = (species == Species.Human ? "p" : "c");
            else if (species == Species.Human && age >= AgeGender.Teen && age <= AgeGender.Adult) specifier = "y";
            else if (species != Species.Human && age >= AgeGender.Teen && age <= AgeGender.Elder) specifier = "a";
            else specifier = age.ToString().Substring(0, 1).ToLower();
            if (species != Species.Human) specifier +=
                (age == AgeGender.Child && species == Species.LittleDog) ? "d" :
                species.ToString().Substring(0, 1).ToLower();
            else if (age <= AgeGender.Child) specifier += "u";
            else specifier += (gender == AgeGender.Male || gender == AgeGender.Female) ? gender.ToString().Substring(0, 1).ToLower() : "f";
            return specifier;
        }

        public static string GetRigPrefix(Species species, AgeGender age, AgeGender gender)
        {
            string specifier = "";
            if (age == AgeGender.Toddler) specifier = (species == Species.Human ? "p" : "c");
            else if (age == AgeGender.Child) specifier = "c";
            else specifier = "a";
            if (species != Species.Human) specifier +=
                (age == AgeGender.Child && species == Species.LittleDog) ? "d" :
                species.ToString().Substring(0, 1).ToLower();
            else specifier += "u";
            return specifier;
        }

        private Image DisplayableShadow(Image shadowImage)
        {
            if (shadowImage == null) return null;
            Bitmap shadow = new Bitmap(shadowImage);
            Rectangle rect = new Rectangle(0, 0, shadow.Width, shadow.Height);
            System.Drawing.Imaging.BitmapData bmpData = shadow.LockBits(rect, ImageLockMode.ReadWrite,
                shadow.PixelFormat);

            IntPtr ptr;
            if (bmpData.Stride > 0)
            {
                ptr = bmpData.Scan0;
            }
            else
            {
                ptr = bmpData.Scan0 + bmpData.Stride * (shadow.Height - 1);
            }

            int bytes = Math.Abs(bmpData.Stride) * shadow.Height;
            byte[] argbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            for (int i = 0; i < argbValues.Length; i += 4)
            {
                if (argbValues[i + 3] > 0)
                {
                    int tmp = ((((int)argbValues[i + 2] + (int)argbValues[i]) / 2) - 150) * 3;
                    tmp = Math.Min(tmp, 255);
                    tmp = Math.Max(tmp, 0);
                    int a = 255 - tmp;
                    argbValues[i + 3] = (byte)a;        //alpha
                    argbValues[i + 2] = 0;              //red
                    argbValues[i + 1] = 0;              //green
                    argbValues[i] = 0;                  //blue
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
            shadow.UnlockBits(bmpData);

            return shadow;
        }

        private Image DisplayableSpecular(Image specularImage)
        {
            if (specularImage == null) return null;
            //Bitmap specular = new Bitmap(specularImage);

            //Rectangle rect = new Rectangle(0, 0, specular.Width, specular.Height);
            //System.Drawing.Imaging.BitmapData bmpData = specular.LockBits(rect, ImageLockMode.ReadWrite, specular.PixelFormat);
            //IntPtr ptr;
            //if (bmpData.Stride > 0) ptr = bmpData.Scan0;
            //else ptr = bmpData.Scan0 + bmpData.Stride * (specular.Height - 1);
            //int bytes = Math.Abs(bmpData.Stride) * specular.Height;
            //byte[] argbValues = new byte[bytes];
            //System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, bytes);

            //for (int i = 0; i < argbValues.Length; i += 4)
            //{
            //    argbValues[i + 2] = 225;          //red
            //    argbValues[i + 1] = 225;          //green
            //    argbValues[i] = 225;              //blue
            //}
            //System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, bytes);
            //specular.UnlockBits(bmpData);

            Bitmap specular = new Bitmap(specularImage.Width, specularImage.Height);
            float[][] specMatrix = {
                   new float[] {0, 0, 0, 0, 0},       // m00 = red scaling factor
                   new float[] {0, 0, 0, 0, 0},       // m11 = green scaling factor
                   new float[] {0, 0, 0, 0, 0},       // m22 = blue scaling factor
                   new float[] {0, 0, 0, 1, 0},       // m33 = alpha scaling factor
                   new float[] {255, 255, 255, 0, 1}        // increments for R, G, B, A
                };
            using (Graphics gr = Graphics.FromImage(specular))
            {
                ColorMatrix spec = new ColorMatrix(specMatrix);
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(spec, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gr.DrawImage(specularImage, new Rectangle(0, 1, specular.Width, specular.Height), 0, 0, specularImage.Width, specularImage.Height, GraphicsUnit.Pixel, attributes);
            }

            return specular;
        }

        private Bitmap LayerWithMask(Image baseImage, Image layerImage, Image maskImage, bool isMesh)
        {
            if (baseImage == null || layerImage == null || maskImage == null) return null;
            Bitmap baseLayer = (Bitmap)baseImage;
            Bitmap topLayer = (Bitmap)layerImage;
            Bitmap mask = new Bitmap(maskImage, baseLayer.Width, baseLayer.Height);

            Rectangle rect = new Rectangle(0, 0, baseLayer.Width, baseLayer.Height);
            System.Drawing.Imaging.BitmapData bmpData = baseLayer.LockBits(rect, ImageLockMode.ReadWrite, baseLayer.PixelFormat);
            IntPtr ptr;
            if (bmpData.Stride > 0) ptr = bmpData.Scan0;
            else ptr = bmpData.Scan0 + bmpData.Stride * (baseLayer.Height - 1);
            int bytes = Math.Abs(bmpData.Stride) * baseLayer.Height;
            byte[] baseValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, baseValues, 0, bytes);

            Rectangle rect2 = new Rectangle(0, 0, topLayer.Width, topLayer.Height);
            System.Drawing.Imaging.BitmapData bmpData2 = topLayer.LockBits(rect2, ImageLockMode.ReadOnly, topLayer.PixelFormat);
            IntPtr ptr2;
            if (bmpData2.Stride > 0) ptr2 = bmpData2.Scan0;
            else ptr2 = bmpData2.Scan0 + bmpData2.Stride * (topLayer.Height - 1);
            int bytes2 = Math.Abs(bmpData2.Stride) * topLayer.Height;
            byte[] layerValues = new byte[bytes2];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, layerValues, 0, bytes2);

            Rectangle rect3 = new Rectangle(0, 0, mask.Width, mask.Height);
            System.Drawing.Imaging.BitmapData bmpData3 = mask.LockBits(rect3, ImageLockMode.ReadOnly, mask.PixelFormat);
            IntPtr ptr3;
            if (bmpData3.Stride > 0) ptr3 = bmpData3.Scan0;
            else ptr3 = bmpData3.Scan0 + bmpData3.Stride * (mask.Height - 1);
            int bytes3 = Math.Abs(bmpData3.Stride) * mask.Height;
            byte[] maskValues = new byte[bytes3];
            System.Runtime.InteropServices.Marshal.Copy(ptr3, maskValues, 0, bytes3);

            for (int i = 0; i < baseValues.Length; i += 4)
            {
                float maskAlpha;
                if (isMesh) maskAlpha = Math.Min((maskValues[i + 3] / 255) * 3f, 1f);
                else maskAlpha = maskValues[i + 3] > 5 ? 1 : 0;
                baseValues[i + 3] = (byte)Math.Min(((layerValues[i + 3] * maskAlpha) + (baseValues[i + 3] * (1f - maskAlpha))), 255);  //alpha
                baseValues[i + 2] = (byte)Math.Min(((layerValues[i + 2] * maskAlpha) + (baseValues[i + 2] * (1f - maskAlpha))), 255);  //red
                baseValues[i + 1] = (byte)Math.Min(((layerValues[i + 1] * maskAlpha) + (baseValues[i + 1] * (1f - maskAlpha))), 255);  //green
                baseValues[i] = (byte)Math.Min(((layerValues[i] * maskAlpha) + (baseValues[i] * (1f - maskAlpha))), 255);  //blue
            }

            System.Runtime.InteropServices.Marshal.Copy(baseValues, 0, ptr, bytes);
            baseLayer.UnlockBits(bmpData);
            topLayer.UnlockBits(bmpData2);
            mask.UnlockBits(bmpData3);

            return baseLayer;
        }

        static readonly string[] StackOrder =
        {
    "Scalp", "Hair", "Face", "EyeColor", "Eyebrow", "Eyeshadow", "Eyeliner", "Blush", "Lipstick",
    "Makeup", "Top", "Bottom", "Body", "Shoes"
};

        static readonly Dictionary<string, int> StackRank =
            StackOrder.Select((name, idx) => (name, idx))
                      .ToDictionary(x => x.name, x => x.idx, StringComparer.OrdinalIgnoreCase);

        static int GetStackRank(ImageStack s)
        {
            // try the category first then the part type
            if (StackRank.TryGetValue(s.partCat.ToString(), out int r)) return r;
            if (StackRank.TryGetValue(s.partType.ToString(), out r)) return r;
            return int.MaxValue;
        }

        static List<ImageStack> SortStack(List<ImageStack> stack)
        {
            return stack.OrderBy(GetStackRank).ToList();
        }

        static Image DrawOverlay(Image baseImg, Image top)
        {
            var result = new Bitmap(baseImg.Width, baseImg.Height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(result))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceOver;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(baseImg, 0, 0, baseImg.Width, baseImg.Height);
                g.DrawImage(top, 0, 0, baseImg.Width, baseImg.Height);
            }
            return result;
        }
    }
}