namespace ConsoleModelsLVS.Models
{
    using System;

    class MyRandom
    {
        private static int Sum(int[] ints)
        {

            int res = 0;
            foreach (int i in ints) res += i;
            return res;
        }

        private static double Sum(double[] doubles)
        {

            double res = .0;
            foreach (double d in doubles) res += d;
            return res;
        }

        public static int Gcd(int a, int b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }

        public static int Lcm(int a, int b)
        {
            return a * b / Gcd(a, b);
        }

        private static int Gcd(int[] ints)
        {

            if (ints.Length == 0) return 1;
            int result = ints[0];
            foreach (int i in ints) result = Gcd(result, i);
            return result;
        }

        private static int Lcm(int[] ints)
        {

            if (ints.Length == 0) return 1;
            int result = ints[0];
            foreach (int i in ints) result = Lcm(result, i);
            return result;
        }

        private static int[] GetProportions(
                int[] fractions)
        {

            int[] ints = new int[fractions.Length];
            int[] outArr = new int[fractions.Length];

            Array.Copy(fractions, ints, ints.Length);

            int gcd = Gcd(ints);

            for (int i = 0; i < ints.Length; i++)
                ints[i] = (ints[i] / gcd);

            int lcm = Lcm(ints);

            for (int i = 0; i < ints.Length; i++)
                outArr[i] = (int)(lcm / ints[i]);

            return outArr;
        }

        private static int GetNBRandom(double[] probs)
        {

            int[] denoms = new int[probs.Length];
            for (int i = 0; i < probs.Length; i++)
            {
                denoms[i] = (int)Math.Round(1 / probs[i]);
            }

            Random r = new Random();

            if (r.NextDouble() < 1.0 - Sum(probs))
                return 0;

            int[] proportions = GetProportions(denoms);
            int sum = 0;
            int random = r.Next(Sum(proportions));

            for (int i = 0; i < proportions.Length; i++)
                if (random < (sum += proportions[i]))
                    return i + 1;

            return proportions.Length;
        }

        public static DeviceState GetRandomState(
                double genProb, double denProb, double failProb, double busyProb)
        {

            int randomState = GetNBRandom(new double[] { genProb, denProb, failProb, busyProb });

            switch (randomState)
            {
                case 1:
                    return DeviceState.GENERATOR;
                case 2:
                    return DeviceState.DENIAL;
                case 3:
                    return DeviceState.FAILURE;
                case 4:
                    return DeviceState.BUSY;
            }

            return DeviceState.WORKING;

        }
    }

}
