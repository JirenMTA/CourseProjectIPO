namespace ConsoleModelsLVS.Models
{
    public class TerminalDevice
    {
        public DeviceState State { get; set; } = DeviceState.INITIAL;
        private bool active = false;
        private DeviceState previousState = DeviceState.WORKING;
        public string LastMessage { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public LVS LVS { get; set; }  

        public void startMessaging(String message)
        {
            switch (State)
            {
                case DeviceState.WORKING: 
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case DeviceState.GENERATOR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case DeviceState.DENIAL:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }
            Console.WriteLine("ОУ " + Id.ToString() + " - Start messaging: " + message);

            Console.ResetColor();

            LastMessage = message;
            active = true;
        }

        public void endMessaging(String message)
        {
            Console.WriteLine("ОУ " + Id.ToString() + " - End messaging: " + message);
            LastMessage = message;
            active = false;
        }
        private Dictionary<DeviceState, Double> chances;


        public TerminalDevice(Dictionary<DeviceState, Double> chances, LVS lvs, int Id)
        {
            this.chances = chances;
            this.Id = Id;
            this.LVS = lvs;
        }

        public void changeState(DeviceState st)
        {
            if (st == DeviceState.UNBLOCKING && State == DeviceState.BLOCKED)
            {
                State = previousState;
                changeLineState(st, LVS);
            }

            else if (State != DeviceState.BLOCKED && State != DeviceState.DENIAL)
            {
                previousState = State;
                State = st;
                changeLineState(st, LVS);
            }
        }

        public DeviceState process()
        {
            if (State == DeviceState.INITIAL)
            {
                changeState(DeviceState.WORKING);
            }
            previousState = State;
            
            DeviceState randomState = MyRandom.GetRandomState(
                chances[DeviceState.GENERATOR],
                chances[DeviceState.DENIAL],
                chances[DeviceState.FAILURE],
                chances[DeviceState.BUSY]
            );

            changeState(randomState);
            return randomState;
        }

        public void changeLineState(DeviceState newState, LVS lvs)
        {
            if (newState == DeviceState.GENERATOR && lvs.State == LineState.A_WORKING)
            {
                lvs.SetLineState(LineState.A_GENERATION);
            }
            if (State == DeviceState.GENERATOR)
            {
                foreach (TerminalDevice device in lvs.Devices)
                {
                    if (device.State == DeviceState.GENERATOR)
                        return;
                }
                if (lvs.State == LineState.A_GENERATION)
                    lvs.SetLineState(LineState.A_WORKING);
            }
        }
    }
}