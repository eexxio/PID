using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class PointwiseOperations
    {
        public static Image<Gray, byte> ContrastAndBrightness(Image<Gray, byte> inputImage, double alpha, double beta)
        {
            Image<Gray, byte> result = inputImage.CopyBlank();

            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double value = alpha * i + beta;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)value;
            }

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    byte pixelValue = inputImage.Data[y, x, 0];
                    result.Data[y, x, 0] = lut[pixelValue];
                }
            }

            return result;
        }

        public static Image<Bgr, byte> ContrastAndBrightness(Image<Bgr, byte> inputImage, double alpha, double beta)
        {
            Image<Bgr, byte> result = inputImage.CopyBlank();

            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double value = alpha * i + beta;
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                lut[i] = (byte)value;
            }

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    result.Data[y, x, 0] = lut[inputImage.Data[y, x, 0]];
                    result.Data[y, x, 1] = lut[inputImage.Data[y, x, 1]];
                    result.Data[y, x, 2] = lut[inputImage.Data[y, x, 2]];
                }
            }

            return result;
        }
    }
}