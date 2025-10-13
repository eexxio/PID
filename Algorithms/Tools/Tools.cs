using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Tools
{
    public class Tools
    {
        #region Copy
        public static Image<Gray, byte> Copy(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Clone();
            return result;
        }

        public static Image<Bgr, byte> Copy(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = inputImage.Clone();
            return result;
        }
        #endregion

        #region Invert
        public static Image<Gray, byte> Invert(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                }
            }
            return result;
        }

        public static Image<Bgr, byte> Invert(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, x, 0] = (byte)(255 - inputImage.Data[y, x, 0]);
                    result.Data[y, x, 1] = (byte)(255 - inputImage.Data[y, x, 1]);
                    result.Data[y, x, 2] = (byte)(255 - inputImage.Data[y, x, 2]);
                }
            }
            return result;
        }
        #endregion

        #region Convert color image to grayscale image
        public static Image<Gray, byte> Convert(Image<Bgr, byte> inputImage)
        {
            Image<Gray, byte> result = inputImage.Convert<Gray, byte>();
            return result;
        }
        #endregion

        #region Binary
        public static Image<Gray, byte> Binary(Image<Gray, byte> inputImage, int threshold)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = (byte)(pixelValue >= threshold ? 255 : 0);
                }
            }
            return result;
        }
        #endregion

        #region Mirror Image
        public static Image<Gray, byte> MirrorImage(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, inputImage.Width - 1 - x, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> MirrorImage(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Size);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[y, inputImage.Width - 1 - x, 0] = inputImage.Data[y, x, 0];
                    result.Data[y, inputImage.Width - 1 - x, 1] = inputImage.Data[y, x, 1];
                    result.Data[y, inputImage.Width - 1 - x, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }
        #endregion

        #region Rotate Image
        public static Image<Gray, byte> RotateClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[x, inputImage.Height - 1 - y, 0] = inputImage.Data[y, x, 0];
                    result.Data[x, inputImage.Height - 1 - y, 1] = inputImage.Data[y, x, 1];
                    result.Data[x, inputImage.Height - 1 - y, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }

        public static Image<Gray, byte> RotateAntiClockwise(Image<Gray, byte> inputImage)
        {
            Image<Gray, byte> result = new Image<Gray, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                }
            }
            return result;
        }

        public static Image<Bgr, byte> RotateAntiClockwise(Image<Bgr, byte> inputImage)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(inputImage.Height, inputImage.Width);

            for (int y = 0; y < inputImage.Height; ++y)
            {
                for (int x = 0; x < inputImage.Width; ++x)
                {
                    result.Data[inputImage.Width - 1 - x, y, 0] = inputImage.Data[y, x, 0];
                    result.Data[inputImage.Width - 1 - x, y, 1] = inputImage.Data[y, x, 1];
                    result.Data[inputImage.Width - 1 - x, y, 2] = inputImage.Data[y, x, 2];
                }
            }
            return result;
        }
        #endregion

        #region Crop Image
        public static Image<Gray, byte> CropImage(Image<Gray, byte> inputImage, int x1, int y1, int x2, int y2, out double mean, out double stdDev)
        {
            int width = x2 - x1;
            int height = y2 - y1;
            Image<Gray, byte> result = new Image<Gray, byte>(width, height);

            double sum = 0;
            int count = width * height;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    byte pixelValue = inputImage.Data[y1 + y, x1 + x, 0];
                    result.Data[y, x, 0] = pixelValue;
                    sum += pixelValue;
                }
            }

            mean = sum / count;

            double sumSquaredDiff = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double diff = result.Data[y, x, 0] - mean;
                    sumSquaredDiff += diff * diff;
                }
            }

            stdDev = System.Math.Sqrt(sumSquaredDiff / count);

            return result;
        }

        public static Image<Bgr, byte> CropImage(Image<Bgr, byte> inputImage, int x1, int y1, int x2, int y2, out double mean, out double stdDev)
        {
            int width = x2 - x1;
            int height = y2 - y1;
            Image<Bgr, byte> result = new Image<Bgr, byte>(width, height);

            double sumB = 0, sumG = 0, sumR = 0;
            int count = width * height;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    result.Data[y, x, 0] = inputImage.Data[y1 + y, x1 + x, 0];
                    result.Data[y, x, 1] = inputImage.Data[y1 + y, x1 + x, 1];
                    result.Data[y, x, 2] = inputImage.Data[y1 + y, x1 + x, 2];
                    sumB += result.Data[y, x, 0];
                    sumG += result.Data[y, x, 1];
                    sumR += result.Data[y, x, 2];
                }
            }

            mean = (sumB + sumG + sumR) / (count * 3);

            double sumSquaredDiff = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    double diffB = result.Data[y, x, 0] - (sumB / count);
                    double diffG = result.Data[y, x, 1] - (sumG / count);
                    double diffR = result.Data[y, x, 2] - (sumR / count);
                    sumSquaredDiff += diffB * diffB + diffG * diffG + diffR * diffR;
                }
            }

            stdDev = System.Math.Sqrt(sumSquaredDiff / (count * 3));

            return result;
        }
        #endregion
    }
}