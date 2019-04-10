using System;
namespace FNO.Utils
{
    public class MyRandom
    {
        private static int _seed = 0;
        public static int GetRandom(int max)
        {
            _seed = (DateTime.Now.Millisecond + _seed + (int)DateTime.Now.Ticks) % Int32.MaxValue;
            return new Random(_seed).Next(max);
        }
    }
}
