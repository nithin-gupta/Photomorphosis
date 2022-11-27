using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BitmapInterface
{
    public static class Helpers
    {
        public static List<byte> ToByteAdd(uint x, List<byte> y)
        {
            y.Add((byte)(x));
            y.Add((byte)(x >> 8));
            y.Add((byte)(x >> 16));
            y.Add((byte)(x >> 24));

            return y;
        }

        public static List<byte> ToByteAdd(int x, List<byte> y)
        {
            y.Add((byte)(x));
            y.Add((byte)(x >> 8));
            y.Add((byte)(x >> 16));
            y.Add((byte)(x >> 24));

            return y;
        }

        public static List<byte> ToByteAdd(ushort x, List<byte> y)
        {
            y.Add((byte)(x));
            y.Add((byte)(x >> 8));

            return y;
        }

        public static ushort ToBigEndian(ushort x)
        {
            ushort c = (ushort)(x >> 8);
            ushort d = x;

            ushort e = 0;
            e = (ushort)(e | c);

            d <<= 8;
            e = (ushort)(e | d);

            return e;
        }

        public static uint ToBigEndian(uint x)
        {
            uint a = (uint)(x >> 24);
            uint b = (uint)(x << 8);
            b = (uint)(b >> 24);
            b = (uint)(b << 8);
            uint c = (uint)(x << 16);
            c = (uint)(c >> 24);
            c = (uint)(c << 16);
            uint d = x;
            d = (uint)(d << 24);

            uint e = 0;
            e |= (uint)a;
            e |= (uint)b;
            e |= (uint)c;
            e |= (uint)d;
            return e;
        }

        public static int ToBigEndian(int x)
        {
            byte[] a = BitConverter.GetBytes(x);

            int e = BitConverter.ToInt32(a);
            return e;
        }
    }

    class BitmapException : Exception
    {
        public BitmapException()
        {

        }

        public BitmapException(string exceptionText) : base(String.Format("Bitmap exception: {0}", exceptionText))
        {

        }
    }
    /// <summary>
    /// A bitmap pixel. Contains the Red, Green, Blue, and Alpha channel values of the pixel.
    /// </summary>
    class RGBAPixel
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", Red.ToString("x"), Green.ToString("x"), Blue.ToString("x"), Alpha.ToString("x"));
        }

        public RGBAPixel()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
            Alpha = 1;
        }

    }
    /// <summary>
    /// An interface for a Bitmap image. Only works with uncompressed images.
    /// </summary>
    class Bitmap
    {
        static readonly byte[] BMHeaderBegin = { 0x42, 0x4D };
        public uint BMBitmapSize { get; set; }
        public ushort BMRes1 { get; set; }
        public ushort BMRes2 { get; set; }
        public static readonly uint BMOffset = 54;

        public static readonly uint DIBHeaderSize = 40;
        public int DIBWidth { get; set; }
        public int DIBHeight { get; set; }
        public ushort DIBColourPlanes { get; set; }

        public ushort DIBColourDepth { get; set; }

        public uint DIBCompressionMethod { get; set; }

        public uint DIBBitmapSize { get; set; }

        public uint DIBVerticalResolution { get; set; }
        public uint DIBHorizontalResolution { get; set; }

        public uint DIBNumberColours { get; set; }

        public uint DIBImportantColours { get; set; }

        public string filename;

        public byte[] BitmapArray;

        public uint rowWidth;

        public Bitmap(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Couldn't find the specified file.");
            }

            FileStream fileStream = File.OpenRead(filename);

            using BinaryReader binaryReader = new BinaryReader(fileStream);

            byte[] header = binaryReader.ReadBytes(2);

            if (header[0] != BMHeaderBegin[0] || header[1] != BMHeaderBegin[1])
            {
                throw new BitmapException("Unsupported file format." + header[0].ToString("X") + header[1].ToString("X"));
            }

            uint v = binaryReader.ReadUInt32();
            Console.WriteLine(v.ToString("X"));
            BMBitmapSize = v;

            BMRes1 = binaryReader.ReadUInt16();

            BMRes2 = (binaryReader.ReadUInt16());

            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            DIBWidth = (binaryReader.ReadInt32());
            DIBHeight = (binaryReader.ReadInt32());

            DIBColourPlanes = (binaryReader.ReadUInt16());
            DIBColourDepth = (binaryReader.ReadUInt16());
            DIBCompressionMethod = (binaryReader.ReadUInt32());

            DIBBitmapSize = (binaryReader.ReadUInt32());

            DIBVerticalResolution = (binaryReader.ReadUInt32());
            DIBHorizontalResolution = (binaryReader.ReadUInt32());

            DIBNumberColours = (binaryReader.ReadUInt32());
            DIBImportantColours = (binaryReader.ReadUInt32());

            try
            {
                BitmapArray = binaryReader.ReadBytes((int)(BMBitmapSize - BMOffset));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;

            }
            Console.WriteLine("BMPArraySize for " + filename + "is " + DIBBitmapSize);

            this.filename = filename;

            rowWidth = (uint)((BMBitmapSize - BMOffset) / DIBHeight);

            fileStream.Close();
        }


        public Bitmap()
        {

        }

        public Bitmap(int width, int height, int bitDepth)
        {
            DIBWidth = width;
            DIBHeight = height;
            DIBColourDepth = (ushort)((ushort)bitDepth / (ushort)8);
            DIBBitmapSize = (uint)width / DIBColourDepth;
            DIBBitmapSize += DIBBitmapSize % 4;
            rowWidth = DIBBitmapSize;
            DIBBitmapSize *= (uint)height;
            BMBitmapSize = BMOffset + DIBBitmapSize;
            DIBColourPlanes = 1;
            filename = "default";


        }

        public Bitmap(int width, int height, int bitDepth, byte[] bitmap)
        {
            DIBWidth = width;
            DIBHeight = height;
            DIBColourDepth = (ushort)bitDepth;
            DIBBitmapSize = (uint)bitmap.Length;
            rowWidth = (uint)(DIBBitmapSize / DIBHeight);
            BMBitmapSize = BMOffset + DIBBitmapSize;
            DIBColourPlanes = 1;
            BitmapArray = bitmap;
            filename = "default0";

            //DIBCompressionMethod = 0;
            //BMRes1 = BMRes2 = 0;
            //DIBVerticalResolution = DIBHorizontalResolution = 0xb13;
            //DIBNumberColours = 64;
            //DIBImportantColours = 0;
        }
        /// <summary>
        /// Get the data of the pixels from the image.
        /// </summary>
        /// <returns>A 2D Array of pixels with the data of each pixel in the image.</returns>
        public RGBAPixel[,] GetPixelData()
        {
            List<string> lines = new List<string>();

            RGBAPixel[,] pixels = new RGBAPixel[DIBHeight, DIBWidth];

            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0, k = 0; rowWidth - k >= DIBColourDepth / 8 && j < DIBWidth; j++, k += DIBColourDepth / 8)
                {
                    pixels[i, j] = new RGBAPixel
                    {
                        Blue = BitmapArray[i * rowWidth + k],
                        Green = BitmapArray[i * rowWidth + k + 1],
                        Red = BitmapArray[i * rowWidth + k + 2]
                    };
                    if (DIBColourDepth == 32)
                    {
                        pixels[i, j].Alpha = BitmapArray[i * rowWidth + k + 3];
                    }
                    lines.Add(pixels[i, j].ToString());
                }
            }

            File.WriteAllLines(String.Format("{0}Log.txt", filename), lines.ToArray());


            return pixels;
        }

        /// <summary>
        /// Sets each pixel in the image as supplied in an array of RGBAPixels. Ensure that the size of the array is correct.
        /// </summary>
        /// <param name="pixels"></param>
        public void SetPixelData(RGBAPixel[,] pixels)
        {
            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0, k = 0; rowWidth - k >= DIBColourDepth / 8 && j < DIBWidth; ++j, k += DIBColourDepth / 8)
                {
                    BitmapArray[i * rowWidth + k] = pixels[i, j].Blue;
                    BitmapArray[i * rowWidth + k + 1] = pixels[i, j].Green;
                    BitmapArray[i * rowWidth + k + 2] = pixels[i, j].Red;
                    if (DIBColourDepth == 32)
                    {
                        BitmapArray[i * rowWidth + k + 3] = pixels[i, j].Alpha;
                    }
                }
            }
        }
        ///<summary>
        ///Box blurs an image. 3 is the default blur radius if not specified.
        ///</summary>
        public void AverageBlur(int blurRadius)
        {
            RGBAPixel[,] pixels = GetPixelData();
            RGBAPixel[,] mod = new RGBAPixel[DIBHeight, DIBWidth];

            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0; j < DIBWidth; ++j)
                {
                    mod[i, j] = new RGBAPixel
                    {
                        Red = pixels[i, j].Red,
                        Green = pixels[i, j].Green,
                        Blue = pixels[i, j].Blue,
                        Alpha = pixels[i, j].Alpha
                    };
                }
            }
            int stat;
            for (int i = 0; i < DIBHeight; i++)
            {
                for (int j = 0; j < DIBWidth; j++)
                {
                    int averageRed = 0, averageBlue = 0, averageGreen = 0;
                    stat = 0;
                    // Box blurRadius
                    for (int rows = i - ((blurRadius - 1) / 2); rows <= i + ((blurRadius - 1) / 2); rows++)
                    {
                        for (int cols = j - ((blurRadius - 1) / 2); cols <= j + ((blurRadius - 1) / 2); cols++)
                        {
                            if ((rows >= 0 && rows < DIBHeight) && (cols >= 0 && cols < DIBWidth))
                            {
                                averageBlue += pixels[rows, cols].Blue;
                                averageGreen += pixels[rows, cols].Green;
                                averageRed += pixels[rows, cols].Red;
                                stat++;
                            }
                        }
                    }
                    if (stat != 0)
                    {
                        averageBlue = (byte)Math.Round(averageBlue / (double)stat);
                        averageGreen = (byte)Math.Round(averageGreen / (double)stat);
                        averageRed = (byte)Math.Round(averageRed / (double)stat);
                        mod[i, j].Blue = (byte)averageBlue;
                        mod[i, j].Green = (byte)averageGreen;
                        mod[i, j].Red = (byte)averageRed;
                    }
                    else
                    {
                        i = DIBHeight;
                        break;
                    }
                }
            }

            SetPixelData(mod);
        }
        public void AverageBlur()
        {
            RGBAPixel[,] pixels = GetPixelData();
            RGBAPixel[,] mod = new RGBAPixel[DIBHeight, DIBWidth];
            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0; j < DIBWidth; ++j)
                {
                    mod[i, j] = new RGBAPixel
                    {
                        Red = pixels[i, j].Red,
                        Green = pixels[i, j].Green,
                        Blue = pixels[i, j].Blue,
                        Alpha = pixels[i, j].Alpha,
                    };
                }
            }

            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0; j < DIBWidth; ++j)
                {
                    int averageRed = 0, averageBlue = 0, averageGreen = 0;
                    int averagePixelCount = 0;

                    if (i == 0)
                    {
                        //Corner
                        if (j == 0)
                        {
                            for (int k = 0; k < 2; ++k)
                            {
                                for (int l = 0; l < 2; ++l)
                                {
                                    averageRed += pixels[i + k, j + l].Red;
                                    averageBlue += pixels[i + k, j + l].Blue;
                                    averageGreen += pixels[i + k, j + l].Green;
                                    averagePixelCount = 4;
                                }
                            }
                        }
                        //Corner
                        else if (j == DIBWidth - 1)
                        {
                            for (int k = 0; k < 2; ++k)
                            {
                                for (int l = 0; l < 2; ++l)
                                {
                                    averageRed += pixels[i + k, j + l - 1].Red;
                                    averageBlue += pixels[i + k, j + l - 1].Blue;
                                    averageGreen += pixels[i + k, j + l - 1].Green;
                                    averagePixelCount = 4;
                                }
                            }
                        }
                        //Edge
                        else
                        {
                            for (int k = 0; k < 2; ++k)
                            {
                                for (int l = 0; l < 3; ++l)
                                {
                                    averageRed += pixels[i + k, j + l - 1].Red;
                                    averageBlue += pixels[i + k, j + l - 1].Blue;
                                    averageGreen += pixels[i + k, j + l - 1].Green;
                                    averagePixelCount = 6;
                                }
                            }
                        }

                    }
                    else if (j == 0)
                    {
                        //Corner
                        if (i == DIBHeight - 1)
                        {
                            for (int k = 0; k < 2; ++k)
                            {
                                for (int l = 0; l < 2; ++l)
                                {
                                    averageRed += pixels[i + k - 1, j + l].Red;
                                    averageBlue += pixels[i + k - 1, j + l].Blue;
                                    averageGreen += pixels[i + k - 1, j + l].Green;
                                    averagePixelCount = 4;
                                }
                            }
                        }
                        //Edge
                        else
                        {
                            for (int k = 0; k < 3; ++k)
                            {
                                for (int l = 0; l < 2; ++l)
                                {
                                    averageRed += pixels[i + k - 1, j + l].Red;
                                    averageBlue += pixels[i + k - 1, j + l].Blue;
                                    averageGreen += pixels[i + k - 1, j + l].Green;
                                    averagePixelCount = 6;
                                }
                            }
                        }
                    }

                    else if (i == DIBHeight - 1)
                    {
                        //Corner
                        if (j == DIBWidth - 1)
                        {
                            for (int k = 0; k < 2; ++k)
                            {
                                for (int l = 0; l < 2; ++l)
                                {
                                    averageRed += pixels[i + k - 1, j + l - 1].Red;
                                    averageBlue += pixels[i + k - 1, j + l - 1].Blue;
                                    averageGreen += pixels[i + k - 1, j + l - 1].Green;
                                    averagePixelCount = 4;
                                }
                            }
                        }

                        //Edge 
                        else if (j != 0 && j != DIBWidth - 1)
                        {
                            for (int k = 0; k < 2; ++k)
                            {
                                for (int l = 0; l < 2; ++l)
                                {
                                    averageRed += pixels[i + k - 1, j + l - 1].Red;
                                    averageBlue += pixels[i + k - 1, j + l - 1].Blue;
                                    averageGreen += pixels[i + k - 1, j + l - 1].Green;
                                    averagePixelCount = 6;
                                }
                            }
                        }
                    }

                    else if (j == DIBWidth - 1)
                    {
                        for (int k = 0; k < 2; ++k)
                        {
                            for (int l = 0; l < 2; ++l)
                            {
                                averageRed += pixels[i + k - 1, j + l - 1].Red;
                                averageBlue += pixels[i + k - 1, j + l - 1].Blue;
                                averageGreen += pixels[i + k - 1, j + l - 1].Green;
                                averagePixelCount = 6;
                            }
                        }
                    }

                    else
                    {
                        for (int k = 0; k < 3; ++k)
                        {
                            for (int l = 0; l < 3; ++l)
                            {
                                averageRed += pixels[i + k - 1, j + l - 1].Red;
                                averageBlue += pixels[i + k - 1, j + l - 1].Blue;
                                averageGreen += pixels[i + k - 1, j + l - 1].Green;
                                averagePixelCount = 9;
                            }
                        }
                    }

                    averageRed = (byte)MathF.Ceiling(averageRed / averagePixelCount);
                    averageBlue = (byte)MathF.Ceiling(averageBlue / averagePixelCount);
                    averageGreen = (byte)MathF.Ceiling(averageGreen / averagePixelCount);

                    mod[i, j].Red = (byte)(averageRed);
                    mod[i, j].Green = (byte)(averageGreen);
                    mod[i, j].Blue = (byte)(averageBlue);

                }
            }
            SetPixelData(mod);
        }

        /// <summary>
        /// Brightens the image by a specified amount between 0 and 255 (doesn't brighten a pixel if it's already at maximum brightness)
        /// </summary>
        /// <param name="brightnessOffset"></param>
        public void Brighten(byte brightnessOffset)
        {
            RGBAPixel[,] pixels = GetPixelData();
            foreach (RGBAPixel pixel in pixels)
            {
                if (pixel.Red + brightnessOffset < 255 && pixel.Blue + brightnessOffset < 255 && pixel.Green + brightnessOffset < 255)
                {
                    pixel.Red += brightnessOffset;
                    pixel.Green += brightnessOffset;
                    pixel.Blue += brightnessOffset;
                }
            }
            SetPixelData(pixels);
        }
        /// <summary>
        /// Increases the contrast of the image. Still being developed, a correct contrast is not guaranteed.
        /// </summary>
        public void Encontrast()
        {
            RGBAPixel[,] pixels = GetPixelData();
            foreach (RGBAPixel pixel in pixels)
            {
                if (pixel.Red > 0x80)
                {
                    pixel.Red += (byte)(pixel.Red - 0x80);
                    pixel.Blue += (byte)(pixel.Blue - 0x80);
                    pixel.Green += (byte)(pixel.Green - 0x80);
                }
                //else if (pixel.Red == 0x80)
                //{
                //    double x =RNG.Next(1, 10);
                //    pixel.Red += (byte)(pixel.Red + (x - 5) * x);
                //    pixel.Blue += (byte)(pixel.Blue + (x - 5) * x);
                //    pixel.Green += (byte)(pixel.Green + (x - 5) * x);
                //}
                else
                {
                    pixel.Red -= (byte)(pixel.Red - 0x80);
                    pixel.Blue -= (byte)(pixel.Blue - 0x80);
                    pixel.Green -= (byte)(pixel.Green - 0x80);
                }
            }
            SetPixelData(pixels);
        }
        public void Encontrast(byte contrastOffset)
        {
            RGBAPixel[,] pixels = GetPixelData();
            foreach (RGBAPixel pixel in pixels)
            {
                if (pixel.Red > contrastOffset)
                {
                    pixel.Red += (byte)(pixel.Red - contrastOffset);
                }
                else if (pixel.Red == contrastOffset)
                {
                    //    double c;
                    //    double x = RNG.Next(10, 10);
                    //    pixel.Red += (byte)((c = (pixel.Red + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                    //    pixel.Blue += (byte)((c = (pixel.Blue + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                    //    pixel.Green += (byte)((c = (pixel.Green + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                }
                else
                {
                    pixel.Red -= (byte)(pixel.Red - contrastOffset);
                }
                if (pixel.Blue > contrastOffset)
                {
                    pixel.Blue += (byte)(pixel.Blue - contrastOffset);
                }
                else if (pixel.Blue == contrastOffset)
                {
                    //    double c;
                    //    double x = RNG.Next(10, 10);
                    //    pixel.Blue += (byte)((c = (pixel.Blue + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                    //    pixel.Blue += (byte)((c = (pixel.Blue + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                    //    pixel.Green += (byte)((c = (pixel.Green + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                }
                else
                {
                    pixel.Blue -= (byte)(pixel.Blue - contrastOffset);
                }
                if (pixel.Green > contrastOffset)
                {
                    pixel.Green += (byte)(pixel.Green - contrastOffset);
                }
                else if (pixel.Green == contrastOffset)
                {
                    //    double c;
                    //    double x = RNG.Next(10, 10);
                    //    pixel.Green += (byte)((c = (pixel.Green + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                    //    pixel.Blue += (byte)((c = (pixel.Blue + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                    //    pixel.Green += (byte)((c = (pixel.Green + (x - 5) * x)) > 255 || c < 0 ? 255 : c);
                }
                else
                {
                    pixel.Green -= (byte)(pixel.Green - contrastOffset);
                }
            }
            SetPixelData(pixels);
        }

        /// <summary>
        /// Inverts every pixel in the image.
        /// </summary>
        public void Invert()
        {
            RGBAPixel[,] pixels = GetPixelData();
            foreach (RGBAPixel pixel in pixels)
            {
                pixel.Red = (byte)~pixel.Red;
                pixel.Green = (byte)~pixel.Green;
                pixel.Blue = (byte)~pixel.Blue;
            }
            SetPixelData(pixels);
        }
        public void GenerateBitmap(string fname)
        {
            filename = fname;
            List<byte> bitmap = new List<byte>();

            bitmap.Add(BMHeaderBegin[0]);
            bitmap.Add(BMHeaderBegin[1]);
            bitmap = Helpers.ToByteAdd(BMBitmapSize, bitmap);
            bitmap = Helpers.ToByteAdd(BMRes1, bitmap);
            bitmap = Helpers.ToByteAdd(BMRes2, bitmap);
            bitmap = Helpers.ToByteAdd(BMOffset, bitmap);
            bitmap = Helpers.ToByteAdd(DIBHeaderSize, bitmap);
            bitmap = Helpers.ToByteAdd(DIBWidth, bitmap);
            bitmap = Helpers.ToByteAdd(DIBHeight, bitmap);
            bitmap = Helpers.ToByteAdd(DIBColourPlanes, bitmap);
            bitmap = Helpers.ToByteAdd(DIBColourDepth, bitmap);
            bitmap = Helpers.ToByteAdd(DIBCompressionMethod, bitmap);
            bitmap = Helpers.ToByteAdd(DIBBitmapSize, bitmap);
            bitmap = Helpers.ToByteAdd(DIBVerticalResolution, bitmap);
            bitmap = Helpers.ToByteAdd(DIBHorizontalResolution, bitmap);
            bitmap = Helpers.ToByteAdd(DIBNumberColours, bitmap);
            bitmap = Helpers.ToByteAdd(DIBImportantColours, bitmap);

            foreach (byte x in BitmapArray)
            {
                bitmap.Add(x);
            }

            using BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filename + ".bmp", FileMode.Create));
            foreach (byte x in bitmap)
            {
                binaryWriter.Write(x);
            }
        }
    }
}