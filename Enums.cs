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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TS3SimRipper
{
    public enum Game : uint
    {
        Sims3 = 0,
        SimsMedieval = 1
    }

    public enum Species : uint
    {
        None = 0x000,
        Human = 0x100,
        Horse = 0x200,
        Cat = 0x300,
        Dog = 0x400,
        LittleDog = 0x500,
        Deer = 0x600,
        Raccoon = 0x700,
        LargeBird = 0x800,
        MedievalHuman = 0x10000
    }

    [Flags]
    public enum AgeGender : uint
    {
        None = 0,
        Baby = 0x00000001,
        Toddler = 0x00000002,
        Child = 0x00000004,
        Teen = 0x00000008,
        YoungAdult = 0x00000010,
        Adult = 0x00000020,
        Elder = 0x00000040,
        Male = 0x00001000,
        Female = 0x00002000,
        Unisex = 0x00003000
    }

    public enum ResourceTypes : uint
    {
        FaceBlend = 0x0358B08A,
        BodyBlend = 0x062C8204,
        BlendGeometry = 0x067CAA11U,
        CASP = 0x034AEECB,
        DDS = 0x00B2D882,
        DDSuncompressed = 0xB6C8B6A0,
        BoneDelta = 0x0355E0A6U,
        NameMap = 0x0166038CU,
        SimExport = 0x04F88964,
        SimOutfit = 0x025ED6F4,
        Thumbnail = 0x5B282D45U,
        TONE = 0x0354796A,
        PeltLayer = 0x26AF8338,
        Rig = 0x8EAF13DE
    }

    [Flags]
    public enum PartTypeFlags : uint
    {
        Hair = 0x00000001,
        Scalp = 0x00000002,
        Face = 0x00000004,
        Body = 0x00000008,
        Accessory = 0x00000010,
    }

    public enum ClothingCategory : uint
    {
        None = 0x00000000,
        Naked = 0x00000001,
        Everyday = 0x00000002,
        Formalwear = 0x00000004,
        Sleepwear = 0x00000008,
        Swimwear = 0x00000010,
        Athletic = 0x00000020,
        Singed = 0x00000040,
        MartialArts = 0x00000080,
        Career = 0x00000100,
        FireFighting = 0x00000200,
        Makeover = 0x00000400,
        SkinnyDippingTowel = 0x00000800,
        Racing = 0x00001000,
        Jumping = 0x00002000,
        Bridle = 0x00004000,
        MarkingsNonHorse = 0x00007000,
        ChildImagination = 0x00010000,
        Special = 0x00020000,
        Outerwear = 0x00040000,
        Supernatural = 0x00080000,
        CategoryMask = 0x0007FFFF,
        ValidForMaternity = 0x00100000,
        ValidForRandom = 0x00200000,
        IsHat = 0x00400000,
        IsRevealing = 0x00800000,
        IsHiddenInCAS = 0x01000000,
        RegionHead = 0x02000000,
        RegionChest = 0x04000000,
        RegionBelly = 0x06000000,
        RegionBack = 0x08000000,
        RegionTail = 0x0A000000,
        RegionFrontLeftLeg = 0x0C000000,
        RegionFrontRightLeg = 0x0E000000,
        RegionRearLeftLeg = 0x10000000,
        RegionRearRightLeg = 0x12000000,
        RegionAll = 0x14000000,
        RegionMask = 0x1E000000,
        All = 0xFFFFFFFF
    }

    [Flags]
    public enum ClothingCategoryFlags : uint
    {
        Naked = 0x00000001,
        Everyday = 0x00000002,
        Formalwear = 0x00000004,
        Sleepwear = 0x00000008,

        Swimwear = 0x00000010,
        Athletic = 0x00000020,
        Singed = 0x00000040,
        MartialArts = 0x00000080,

        Career = 0x00000100,
        FireFighting = 0x00000200,
        Makeover = 0x00000400,
        SkinnyDippingTowel = 0x00000800,

        Racing = 0x00001000,
        Jumping = 0x00002000,
        Bridle = 0x00004000,
        //Markings = 0x00007000,
        Unknown0F = 0x00008000,

        Unknown10 = 0x00010000,
        Unknown11 = 0x00020000,
        Outerwear = 0x00040000,
        //CategoryMask = 0x0007FFFF,
        Plumbotwear = 0x00080000,

        //"Extended" flags
        ValidForMaternity = 0x00100000,
        ValidForRandom = 0x00200000,
        IsHat = 0x00400000,
        IsRevealing = 0x00800000,

        IsHiddenInCAS = 0x01000000,

        //  C BA9
        //  0 001x - head
        //  0 010x - chest
        //  0 011x - belly
        //  0 100x - back
        //  0 101x - tail
        //  0 110x - front left leg
        //  0 111x - front right leg
        //  1 000x - rear left leg
        //  1 001x - rear right leg
        //  1 010x - all
        //Unknown19 = 0x02000000,
        //Unknown1A = 0x04000000,
        //Unknown1B = 0x08000000,
        //Unknown1C = 0x10000000,
        RegionHead = 0x02000000,
        RegionChest = 0x04000000,
        RegionBelly = 0x06000000,
        RegionBack = 0x08000000,
        RegionTail = 0x0A000000,
        RegionFrontLeftLeg = 0x0C000000,
        RegionFrontRightLeg = 0x0E000000,
        RegionRearLeftLeg = 0x10000000,
        RegionRearRightLeg = 0x12000000,
        RegionAll = 0x14000000,
        //RegionMask = 0x1E000000,

        Unknown1D = 0x20000000,
        Unknown1E = 0x40000000,
        Unknown1F = 0x80000000,
    }

    public enum CASregions
    {
        Undefined = 0,
        Body = 1024,
        Brow = 256,
        Ears = 16,
        Eyelashes = 2048,
        Eyes = 1,
        Face = 64,
        Head = 128,
        Jaw = 512,
        Mouth = 4,
        Nose = 2,
        TranslateEyes = 32,
        TranslateMouth = 8
    }

    public enum SimOccult
    {
        CurrentForm = 0,
        Human = 1,
        Alien = 2,
        Vampire = 4,
        Mermaid = 8,
        Spellcaster = 16
    }

    public enum MeshFormat
    {
        None = 0,
        OBJ = 1,
        DAE = 2,
        MS3D = 3,
        GEOM = 4,
    }

    public enum MeshFormatFlag
    {
        Position = 1,
        Normals = 2,
        UV = 3,
        BoneIndices = 4,
        BoneWeights = 5,
        Tangents = 6,
        Color = 7,
        Binormal = 8,
        WeightMap = 9,
        VertexID = 10
    }

    public enum MeshFormatDatatype
    {
        Float = 1,
        Byte4 = 2,  //4 bytes
        Color = 3,
        Uint = 4
    }

    public enum SimShader : uint
    {
        SimEyelashes = 0x9D9DA161,
        SimEyes = 0xCF8A70B4,
        SimGlass = 0x5EDA9CDE,
        SimHair = 0x84FD7152,
        SimSkin = 0x548394B9,
        SolidPhong = 0x47C6638C
    }

    public enum Complate
    {
        CasRgbMask,
        CasRgbaMask,
        CasOverlayMultiTintable,
        CasSkinOverlayTintable,
        CasSkinOverlayColorizable,
        Glasses,
        HairUniversal
    }

    public enum PatternType
    {
        None,
        Solid,
        Flat,
        RGB,
        HSV
    }

    public enum ClothingType : uint
    {
        None = 0x00000000,
        Hair = 0x00000001,
        Scalp = 0x00000002,
        Face = 0x00000003,
        Body = 0x00000004,
        Top = 0x00000005,
        Bottom = 0x00000006,
        Shoes = 0x00000007,

        FirstAccessory = 0x00000008,
        Necklace = 0x00000009,
        NoseRing = 0x0000000A,
        Earrings = 0x0000000B,
        Glasses_F = 0x0000000C,
        Bracelets = 0x0000000D,
        Ring_Lt = 0x0000000E,
        Ring_Rt = 0x0000000F,

        Beard = 0x00000010,
        Lipstick = 0x00000011,
        Eyeshadow = 0x00000012,
        Eyeliner = 0x00000013,
        Blush = 0x00000014,
        Makeup = 0x00000015,
        Eyebrow = 0x00000016,
        EyeColor = 0x00000017,

        Glove = 0x00000018,
        Socks = 0x00000019,
        Mascara = 0x0000001A,
        Moles = 0x0000001B,
        Freckles = 0x0000001C,
        Weathering = 0x0000001D,
        EarringL = 0x0000001E,
        EarringR = 0x0000001F,

        ArmBand = 0x00000020,
        Tattoo = 0x00000021,
        TattooTemplate = 0x00000022,
        Dental = 0x00000023,
        LeftGarter = 0x00000024,
        RightGarter = 0x00000025,
        BirthMark = 0x00000026,
        BodyHairChestUpper = 0x00000027,

        BodyHairChestStomach = 0x00000028,
        BodyHairBackLower = 0x00000029,
        BodyHairBackUpper = 0x0000002A,
        BodyHairBackFull = 0x0000002B,
        BodyHairForearms = 0x0000002C,
        BodyHairLegsCalves = 0x0000002D,
        BodyHairLegsFeet = 0x0000002E,
        PetBody = 0x0000002F,

        PetTail = 0x00000030,
        PetEars = 0x00000031,
        PetMane = 0x00000032,
        PetCollarBridle = 0x00000033,
        PetSaddle = 0x00000034,
        PeltLayer = 0x00000035,
        BasePeltLayer = 0x00000036,
        PetHooves = 0x00000037,

        PetHorn = 0x00000038,
        PetBlanket = 0x00000039,
        PetBreastCollar = 0x0000003A,
        PetBeard = 0x0000003B,
        Unknown3C = 0x0000003C,
        Unknown3D = 0x0000003D,
        Unknown3E = 0x0000003E,
        Unknown3F = 0x0000003F,
    }


    //temporary
    [Flags]
    public enum CASParamFlag : byte
    {
        RestrictOppositeGender = 1 << 7,
        AllowForLiveRandom = 1 << 6,
        ShowInCASDemo = 1 << 5,
        ShowInSimInfoPanel = 1 << 4,
        ShowInUI = 1 << 3,
        AllowForCASRandom = 1 << 2,
        DefaultThumbnailPart = 1 << 1,
        DefaultForBodyType = 1
    }
    [Flags]
    public enum CASParamFlag2 : byte
    {
        DefaultForBodyTypeFemale = 1 << 2,
        DefaultForBodyTypeMale = 1 << 1,
        RestrictOppositeFrame = 1
    }

    public enum ShaderParam : uint
    {
        AlignAcrossDirection = 0x01885886,
        AlignToDirection = 0x17B78AF6,
        AlphaMap = 0xc3faac4f,
        AlphaMaskThreshold = 0xe77a2b60,
        AlwaysOn = 0xF019641D,
        Ambient = 0x04a5daa3,
        AmbientOcclusionMap = 0xb01cba60,
        AmbientUVSelector = 0x797f8e81,
        AnimDir = 0x3f89c2ef,
        AnimSpeed = 0xd600cb63,
        AutoRainbow = 0x5f7800ea,
        AverageColor = 0x449A3A67,
        BackFaceDiffuseContribution = 0xd641a1b1,
        Births = 0x568E0367,
        bHasDetailmap = 0xe9008abe,
        bHasNormalMap = 0x5e99ee74,
        bIsTerrainRoad = 0xa4a17516,
        BlendDestMode = 0x9BDECB37,
        BlendOperation = 0x2D13B939,
        BlendSourceMode = 0x2D13B939,
        BloomFactor = 0x4168508b,
        BloomScale = 0xE29BA4AC,
        BounceAmountMeters = 0xd8542d8b,
        BouncePower = 0xFBA6B898,
        ColorRamp = 0x581835D6,
        ContourSmoothing = 0x1e27dccd,
        CounterMatrixRow1 = 0x1ef8655d,
        CounterMatrixRow2 = 0x1ef8655e,
        CutoutValidHeights = 0x6d43d7b7,
        DaytimeOnly = 0x6BB389BC,
        DebouncePower = 0x656025DF,
        DeflectionThreshold = 0x7D621D61,
        DeformerOffset = 0x8BDF4746,
        DetailMap = 0x9205daa8,
        DetailUVScale = 0xcd985a0b,
        Diffuse = 0x637daa05,
        DiffuseMap = 0x6cc0fd85,
        DiffuseMapUVChannel = 0xC45A5F41,
        DiffuseUVScale = 0x2d4e507e,
        DiffuseUVSelector = 0x91eebaff,
        DimmingCenterHeight = 0x01adace0,
        DimmingRadius = 0x32dfa298,
        DirtOverlay = 0x48372e62,
        DivetScale = 0xCE8C8311,
        DropShadowAtlas = 0x22ad8507,
        DropShadowStrength = 0x1b1ab4d5,
        EdgeDarkening = 0x8c27d8c9,
        Emission = 0x3bd441a0,
        EmissionMap = 0xf303d152,
        EmissionMapUVSelector = 0xBC823DDC,
        EmissiveBloomMultiplier = 0x490e6eb4,
        EmissiveLightMultiplier = 0x8ef71c85,
        FadeDistance = 0x957210ea,
        ForceAmount = 0xD4D51D02,
        ForceDirection = 0x29881F55,
        ForceVector = 0xEBA4727B,
        FrameData = 0x1E5B2324,
        FramesPerSecond = 0x406ADE00,
        FramesRandomStartFactor = 0x7211F24F,
        FresnelOffset = 0xfb66a8cb,
        HaloBlur = 0xc3ad4f50,
        HaloHighColor = 0xd4043258,
        HaloLowColor = 0x2EB8E8D4,
        HaloRamp = 0x84f6e0fb,
        HighlightColor = 0x90F8DCF0,
        ImpostorDetailTexture = 0x56e1c6b2,
        ImposterTexture = 0xbdcf71c5,
        ImposterTextureAOandSI = 0x15c9d298,
        ImposterTextureWater = 0xbf3fb9fa,
        ImpostorWater = 0x277cf8eb,
        index_of_refraction = 0xDAA9532D,
        IsFloor = 0xD9C05335,
        IsGenericBox = 0x347C9E07,
        IsObject = 0x4C12ECE8,
        IsPartition = 0x5250023D,
        IsSolidObject = 0x3BBF99CF,
        JetTexture = 0x52CE211B,
        Layer2Shift = 0x92692cb2,
        LayerOffset = 0x80d9bfe1,
        LifetimeSeconds = 0x84212733,
        LightingDirectScale = 0xEF270EE4,
        LightingEnabled = 0xA15E4594,
        LightMapScale = 0x4f7dcb9b,
        MaskHeight = 0x849cdadc,
        MaskWidth = 0x707f712f,
        MultiplyMap = 0xCD869A45,
        NoAutomaticDaylightDimming = 0x3CB5FA70,
        NoiseMap = 0xe19fd579,
        NoiseMapScale = 0x5e86dea1,
        NormalBumpScale = 0x88C64AE2,
        NormalMap = 0x6e56548a,
        NormalMapScale = 0x3c45e334,
        NormalMapUVSelector = 0x415368b4,
        NormalUVScale = 0xba2d1ab9,
        OverlayTexture = 0x4DC0C8BC,
        OverrideDirection = 0x0C12DED8,
        OverrideFactor = 0x8E35CCC0,
        OverrideSpeed = 0xA3D6342E,
        OverrideVelocity = 0x14677578,
        ParticleCount = 0xCC31B828,
        PositionTweak = 0xEF36D180,
        PosOffset = 0x790ebf2c,
        PosScale = 0x487648e5,
        Reflective = 0x73c9923e,
        reflectivity = 0x29BCDD1F,
        RefractionDistortionScale = 0xc3c472a1,
        RevealMap = 0xf3f22ac4,
        RippleDistanceScale = 0xccb35b98,
        RippleHeights = 0x6a07d7e1,
        RippleSpeed = 0x52dec070,
        RoadDetailMap = 0x28392dc6,
        RoadNormalMap = 0xbca022bc,
        RoadTexture = 0x53521204,
        RoomLightMap = 0xe7ca9166,
        RootColor = 0xE90599F6,
        RotateSpeedRadsSec = 0x16BF7A44,
        RotationSpeed = 0x32003ad4,
        RugSort = 0x906997a9,
        SelfIlluminationMap = 0x6E067554,
        ShadowAlphaTest = 0xfeb1f9cb,
        SharpSpecControl = 0x11483F01,
        SharpSpecThreshold = 0x903BE4D3,
        Shininess = 0xf755f7ff,
        SizeScaleEnd = 0x891A3133,
        SizeScaleStart = 0x9A6C2EC8,
        SparkleCube = 0x1d90c086,
        SparkleSpeed = 0xba13921e,
        SpecCompositeTexture = 0xD652FADE,
        SpecStyle = 0x9554d40f,
        Specular = 0x2ce11842,
        SpecularMap = 0xad528a60,
        SpecularMapUVChannel = 0xCB053686,
        SpecularUVScale = 0xf12e27c3,
        SpecularUVSelector = 0xb63546ac,
        SpeedStretchFactor = 0x66479028,
        StretchRect = 0x8D38D12E,
        TerrainLightMap = 0x5fd5b006,
        TextureSpeedScale = 0x583df357,
        TimelineLength = 0x0081AE98,
        Transparency = 0x05d22fd3,
        Transparent = 0x988403f9,
        UseDiffuseForAlphaTest = 0xB597FA7F,
        UseLampColor = 0x56B220CD,
        UVOffset = 0x57582869,
        UVScale = 0x159ba53e,
        UVScales = 0x420520e9,
        UVScrollSpeed = 0xf2eea6ec,
        UVTiling = 0x773cab85,
        VertexColorScale = 0xa2fd73ca,
        VisibleOnlyAtNight = 0xAC5D0A82,
        WaterScrollSpeedLayer1 = 0xafa11436,
        WaterScrollSpeedLayer2 = 0xafa11435,
        WindSpeed = 0x66e9b6bc,
        WindStrength = 0xbc4a2544
    }
}
