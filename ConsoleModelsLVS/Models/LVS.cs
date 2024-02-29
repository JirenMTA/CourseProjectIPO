namespace ConsoleModelsLVS.Models
{
    public class LVS
    {
        public LineState State { get; set; } = LineState.A_WORKING;
        public List<TerminalDevice> Devices { get; set; } = new ();
        public int SleepAmount { get; set; }
        private bool real;
        public LineController LineController { get; set; }

        public LVS(bool real, int sleepAmount, int devicesAmount, double gen, double den, double fail, double busy)
        {

            this.SleepAmount = sleepAmount;

            Dictionary<DeviceState, Double> chances = new() {
                { DeviceState.GENERATOR, gen },
                { DeviceState.DENIAL, den },
                { DeviceState.FAILURE, fail },
                { DeviceState.BUSY, busy },  
            };

            this.real = real;
            LineController = new LineController(real, this);

            for (int i = 0; i<devicesAmount; i++)
                Devices.Add(new TerminalDevice(chances, this, i));
        }

        public static LVS realLVS(int sleepAmount, int devicesAmount)
        {

            return new LVS(true, sleepAmount, devicesAmount, 0, 0, 0, 0);
        }

        public static LVS testLVS(int devicesAmount, double gen, double den, double fail, double busy)
        {

            return new LVS(false, 0, devicesAmount, gen, den, fail, busy);
        }

        public List<TerminalDevice> getDevices()
        {
            return Devices;
        }

        public void SetLineState(LineState state)
        {
            if (state == LineState.A_WORKING)
            {
                foreach (TerminalDevice device in Devices)
                    if (device.State == DeviceState.GENERATOR)
                    {
                        State = LineState.A_GENERATION;
                        return;
                    }
            }
            State = state;
        }

        public void start(List<Double> data)
        {
            double initTime = LineController.getTime();
            if (!real)
            {
                foreach (TerminalDevice client in Devices)
                {
                    //================= Симуляция работы ===================
                    DeviceState finalState = client.process();
                    Console.WriteLine("Initial: " + finalState.ToString());
                    //================= Подсчёт ошибок =====================
                    switch (finalState)
                    {
                        case DeviceState.FAILURE:
                            data[0] += 1;
                            break;
                        case DeviceState.DENIAL:
                            data[1] += 1;
                            break;
                        case DeviceState.BUSY:
                            data[2] += 1;
                            break;
                        case DeviceState.GENERATOR:
                            data[3] += 1;
                            break;
                        default:
                            break;
                    }
                }
            }
            else Thread.Sleep(SleepAmount);


            while (State == LineState.A_GENERATION)
            {
                LineController.findGenerator();            
            }
            //====== Запуск действия контроллера =======
            foreach (TerminalDevice client in Devices)
                LineController.reactOn(client);

            //====== Сохранение времени работы ========
            if (!real) data[4] = LineController.getTime() - initTime;
        }
    }
}
