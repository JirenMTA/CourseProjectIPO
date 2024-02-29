using ConsoleModelsLVS.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static void PrintData(List<Double > Data)
    {
        for (int i = 0; i < Data.Count; i++)
        {
            switch(i)
            {
                case 0:
                    Console.WriteLine("Failure: " + Data[i]);
                    break;
                case 1:
                    Console.WriteLine("Denial: " + Data[i]);
                    break;
                case 2:
                    Console.WriteLine("Busy: " + Data[i]);
                    break;
                case 3:
                    Console.WriteLine("Generator: " + Data[i]);
                    break;
            }
        }
    }

    private static List<List<Double>> test(int clientsAmount, int messages, int groupsAmount, 
                                           double genProb, double denProb, double failPro, double busyProb)
    {
        // arguments (args):
        // 0 clientsAmount, 1 messages, 2 groups,
        // probabilities (probs):
        // 0 genProb, 1 denProb, 2 failProb, 3 busyProb
        LVS lvs = LVS.testLVS(clientsAmount, genProb, denProb, failPro, busyProb);

        List<List<Double>> statistics = new ();

        int restMessages = messages;

        for (int i = 0; i < groupsAmount; i++)
        {
            statistics.Add(new List<Double>());
            var sessions = (int)(restMessages * 1.0 / (clientsAmount * (groupsAmount - i)));     
            restMessages -= sessions * clientsAmount;

            double M = .0;
            double SKO = .0;

            double[] intervals = new double[sessions];

            List<Double> data = new();
            for (int j = 0; j < 5; j++)
                data.Add(.0f);
        
            for (int j = 0; j<sessions; j++)
                try {
                    lvs.start(data);
                    M += data[4]/(sessions*clientsAmount);
                    intervals[j] = data[4] / clientsAmount;

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }

            for (int j = 0; j < sessions; j++)
                SKO += Math.Pow(intervals[j] - M, 2);

            SKO = Math.Sqrt(SKO / (sessions * clientsAmount));

            statistics[i].Add((double)sessions * clientsAmount);
            statistics[i].Add(data[0]);
            statistics[i].Add(data[1]);
            statistics[i].Add(data[2]);
            statistics[i].Add(data[3]);
            statistics[i].Add(M * (sessions * clientsAmount));
            statistics[i].Add(M);
            statistics[i].Add(SKO);
        }
        return statistics;
    }

    public static void PrintStatistic(List<List<Double>> statistics)
    {
        foreach (List<Double> list in statistics)
        {
            foreach (Double value in list)
            {
                Console.Write(value + " ,");
            }
            Console.WriteLine("");
        }
    }

    public static void testOne()
    {

    }

    static void Main(string[] args)
    {
        LVS lvs = LVS.testLVS(5, 0.5, 0.1, 0.1, 0.1);

        List<Double> data = new();
        for (int j = 0; j < 5; j++)
            data.Add(.0f);

        lvs.start(data);
        foreach (Double value in data)
            Console.WriteLine(value);
    }
}