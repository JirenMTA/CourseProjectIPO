namespace ConsoleModelsLVS.Models
{
    public enum TimeType
    {
        WORD, COMMAND, ANSWER, BLOCK, UNBLOCK,
        PAUSE_IF_BUSY, PAUSE_BEFORE_ANSWER, PAUSE_BETWEEN_MESSAGES
    }
    public class TimeCounter
    {
        private int total_time = 0;
        private Dictionary<TimeType, int> timeMap = new Dictionary<TimeType, int>()
        {
            { TimeType.PAUSE_IF_BUSY, 5000 },
            { TimeType.COMMAND, 20 },
            { TimeType.PAUSE_BEFORE_ANSWER, 12 },
            { TimeType.WORD, 12 * 20 },
            { TimeType.BLOCK, 20 },
            { TimeType.UNBLOCK, 20 },
            { TimeType.PAUSE_BETWEEN_MESSAGES, 1000 },
            { TimeType.ANSWER, 20 },
        };
        public void addTime(TimeType type)
        {
            total_time += timeMap[type];
        }

        public int getTime()
        {
            return total_time;
        }
    }
};
