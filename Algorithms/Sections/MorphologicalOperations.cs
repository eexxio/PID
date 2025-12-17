using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Algorithms.Sections
{
    public class MorphologicalOperations
    {
        public static bool[,] CreateCircularStructuringElement(int size)
        {
            if (size % 2 == 0)
            {
                size = size + 1;
            }

            bool[,] se = new bool[size, size];
            int center = size / 2;
            double radius = center;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    double distance = Math.Sqrt((i - center) * (i - center) + (j - center) * (j - center));
                    se[i, j] = distance <= radius;
                }
            }

            return se;
        }

        public static Image<Gray, byte> Dilate(Image<Gray, byte> inputImage, bool[,] structuringElement)
        {
            int seSize = structuringElement.GetLength(0);
            int padding = seSize / 2;

            Image<Gray, byte> paddedImage = Filters.PadImage(inputImage, padding);
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte maxValue = 0;

                    for (int i = 0; i < seSize; i++)
                    {
                        for (int j = 0; j < seSize; j++)
                        {
                            if (structuringElement[i, j])
                            {
                                byte pixelValue = paddedImage.Data[y + i, x + j, 0];
                                if (pixelValue > maxValue)
                                {
                                    maxValue = pixelValue;
                                }
                            }
                        }
                    }

                    result.Data[y, x, 0] = maxValue;
                }
            }

            return result;
        }

        public static Image<Gray, byte> CreateSeedImage(Image<Gray, byte> templateImage, List<Point> seedPoints)
        {
            Image<Gray, byte> seedImage = new Image<Gray, byte>(templateImage.Width, templateImage.Height);

            for (int y = 0; y < templateImage.Height; y++)
            {
                for (int x = 0; x < templateImage.Width; x++)
                {
                    seedImage.Data[y, x, 0] = 0;
                }
            }

            foreach (Point point in seedPoints)
            {
                int x = point.X;
                int y = point.Y;

                if (x >= 0 && x < templateImage.Width && y >= 0 && y < templateImage.Height)
                {
                    seedImage.Data[y, x, 0] = 255;
                }
            }

            return seedImage;
        }

        public static Image<Gray, byte> BitwiseAnd(Image<Gray, byte> img1, Image<Gray, byte> img2)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(img1.Width, img1.Height);

            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    byte value1 = img1.Data[y, x, 0];
                    byte value2 = img2.Data[y, x, 0];
                    result.Data[y, x, 0] = (byte)(value1 & value2);
                }
            }

            return result;
        }

        public static Image<Gray, byte> BitwiseOr(Image<Gray, byte> img1, Image<Gray, byte> img2)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(img1.Width, img1.Height);

            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    byte value1 = img1.Data[y, x, 0];
                    byte value2 = img2.Data[y, x, 0];
                    result.Data[y, x, 0] = (byte)(value1 | value2);
                }
            }

            return result;
        }

        public static Image<Gray, byte> BitwiseNot(Image<Gray, byte> img)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(img.Width, img.Height);

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    result.Data[y, x, 0] = (byte)(255 - img.Data[y, x, 0]);
                }
            }

            return result;
        }

        public static bool ImagesEqual(Image<Gray, byte> img1, Image<Gray, byte> img2)
        {
            if (img1.Width != img2.Width || img1.Height != img2.Height)
            {
                return false;
            }

            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    if (img1.Data[y, x, 0] != img2.Data[y, x, 0])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static Image<Gray, byte> RegionFilling(Image<Gray, byte> binaryImage, List<Point> seedPoints, int structuringElementSize)
        {
            bool[,] structuringElement = CreateCircularStructuringElement(structuringElementSize);

            Image<Gray, byte> xCurrent = CreateSeedImage(binaryImage, seedPoints);

            Image<Gray, byte> aComplement = BitwiseNot(binaryImage);

            int iteration = 0;
            int maxIterations = 10000;

            while (iteration < maxIterations)
            {
                Image<Gray, byte> xDilated = Dilate(xCurrent, structuringElement);

                Image<Gray, byte> xNext = BitwiseAnd(xDilated, aComplement);

                if (ImagesEqual(xNext, xCurrent))
                {
                    break;
                }

                xCurrent = xNext;
                iteration++;
            }

            Image<Gray, byte> result = BitwiseOr(xCurrent, binaryImage);

            return result;
        }
    }
}