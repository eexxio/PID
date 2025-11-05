using Emgu.CV;
using Emgu.CV.Structure;

namespace Algorithms.Sections
{
    public class Filters
    {
        public static byte Median(int[] H, int l)
        {
            int k = 0;
            int s = 0;
            int n = (l * l) / 2;

            while (k <= 255 && s + H[k] <= n)
            {
                s = s + H[k];
                k = k + 1;
            }

            return (byte)k;
        }

        public static Image<Gray, byte> MedianFiltering(Image<Gray, byte> I, int l)
        {
            if (l % 2 == 0)
            {
                l = l + 1;
            }

            Image<Gray, byte> result = new Image<Gray, byte>(I.Width, I.Height);
            int[] H = new int[256];
            int r = l / 2;

            for (int y = r; y < I.Height - r; y++)
            {
                for (int x = r; x < I.Width - r; x++)
                {
                    if (x == r)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            H[i] = 0;
                        }

                        for (int i = -r; i <= r; i++)
                        {
                            for (int j = -r; j <= r; j++)
                            {
                                H[I.Data[y + i, x + j, 0]]++;
                            }
                        }
                    }
                    else
                    {
                        for (int k = -r; k <= r; k++)
                        {
                            H[I.Data[y + k, x - r - 1, 0]]--;
                            H[I.Data[y + k, x + r, 0]]++;
                        }
                    }

                    result.Data[y, x, 0] = Median(H, l);
                }
            }

            return result;
        }
    }
}