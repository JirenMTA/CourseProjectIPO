namespace ConsoleModelsLVS.Models
{
    public class LineController
    {
        private TimeCounter timer = new TimeCounter();
        private bool real;
        private LVS lvs;

        public LineController(bool real, LVS lvs)
        {
            this.real = real;
            this.lvs = lvs;
        }

        public int getTime()
        {
            return timer.getTime();
        }

        public void reactOn(TerminalDevice td)
        {

            td.startMessaging("Опрос компьютера");
            if (real)
                Thread.Sleep(lvs.SleepAmount);

            switch (td.State)
            {
                // Абонент занят
                case DeviceState.BUSY:
                    td.endMessaging("! Занят : ожидание...");
                    busy();
                    if (real) Thread.Sleep(lvs.SleepAmount);
                    td.changeState(DeviceState.WORKING);
                    break;
                // Сбой
                case DeviceState.FAILURE:
                    td.endMessaging("! Содержит ошибку : исправление...");
                    failure();
                    if (real) Thread.Sleep(lvs.SleepAmount);
                    td.changeState(DeviceState.WORKING);
                    break;
                // Отказ или блокировка ОУ
                case DeviceState.DENIAL:
                    denial();
                    td.endMessaging("!!!Вышел из строя");
                    break;
                case DeviceState.BLOCKED:
                    denial();
                    td.endMessaging("!!!Заблокирован администратором");
                    break;
                default:
                    td.endMessaging("Компьютер в порядке");
                    break;
            }
            normalWork();

            if (real) Thread.Sleep(lvs.SleepAmount);
        }

        private void failure()
        {
            timer.addTime(TimeType.COMMAND);
            timer.addTime(TimeType.WORD);
            timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
        }

        private void denial()
        {
            for (int i = 0; i < 2; i++)
            {
                timer.addTime(TimeType.COMMAND);
                timer.addTime(TimeType.WORD);
                timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
            }
        }

        private void busy()
        {
            timer.addTime(TimeType.COMMAND);
            timer.addTime(TimeType.WORD);
            timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
            timer.addTime(TimeType.ANSWER);
            timer.addTime(TimeType.PAUSE_IF_BUSY);
        }
        private void normalWork()
        {
            timer.addTime(TimeType.COMMAND);
            timer.addTime(TimeType.WORD);
            timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
            timer.addTime(TimeType.ANSWER);
        }
        public void findGenerator()
        {
            int lastDevice = 0;

            // ================ Тест МКО ====================
            for (int i = 0; i < lvs.Devices.Count(); i++)
            {
                timer.addTime(TimeType.COMMAND);
                timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
            }

            //=============== Блокировка всех ОУ =============
            lvs.SetLineState(LineState.B_WORKING);
            foreach (TerminalDevice client in lvs.Devices)
            {
                timer.addTime(TimeType.BLOCK);
                timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
                timer.addTime(TimeType.ANSWER);

                client.startMessaging("Блокировка компьютера");
                if (real) Thread.Sleep(lvs.SleepAmount / 2);
                client.changeState(DeviceState.BLOCKED);
                if (real) Thread.Sleep(lvs.SleepAmount / 2);
                client.endMessaging("");
            }

            for (int i = 0; i < lvs.Devices.Count(); i++)
            {

                lvs.SetLineState(LineState.B_WORKING);
                if (real) Thread.Sleep(lvs.SleepAmount);
                //======= Разблокировка одного ОУ ==========
                timer.addTime(TimeType.UNBLOCK);
                timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
                timer.addTime(TimeType.ANSWER);

                lvs.Devices[i].startMessaging("Разблокировка");
                if (real) Thread.Sleep(lvs.SleepAmount);
                lvs.Devices[i].changeState(DeviceState.UNBLOCKING);
                if (real) Thread.Sleep(lvs.SleepAmount / 2);

                if (lvs.Devices[i].State == DeviceState.DENIAL)
                {
                    for (int i1 = 0; i1 < 2; i1++)
                    {
                        timer.addTime(TimeType.COMMAND);
                        timer.addTime(TimeType.WORD);
                        timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
                    }
                    lvs.Devices[i].endMessaging("Устройство не отвечает");
                    if (real) Thread.Sleep(lvs.SleepAmount / 2);
                    continue;
                }

                lvs.Devices[i].endMessaging("");
                if (real) Thread.Sleep(lvs.SleepAmount / 2);
                //===========================================
                //============= Опрос текущего ОУ =================
                lvs.Devices[i].startMessaging("Опрос текущего ОУ по линии А");
                if (real) Thread.Sleep(lvs.SleepAmount / 2);
                lvs.SetLineState(LineState.A_WORKING);
                timer.addTime(TimeType.COMMAND);
                timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);

                //=========== Если элемент сети не генерирует сигнала =========
                if (!(lvs.Devices[i].State == DeviceState.GENERATOR))
                {
                    timer.addTime(TimeType.ANSWER);
                    if (real) Thread.Sleep(lvs.SleepAmount);
                    lvs.Devices[i].endMessaging("ОУ не является генератором");
                    if (real) Thread.Sleep(lvs.SleepAmount / 2);
                }
                //================ Если элемент - генератор ===================
                else
                {
                    Console.WriteLine("Here: " + i.ToString());
                    //======== Опрос предыдущего ОУ ==========
                    timer.addTime(TimeType.COMMAND);
                    timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
                    //========================================
                    if (real) Thread.Sleep(lvs.SleepAmount);
                    lvs.SetLineState(LineState.B_WORKING);
                    if (real) Thread.Sleep(lvs.SleepAmount / 2);
                    //====== Блокируем генерящий элемент ======
                    timer.addTime(TimeType.BLOCK);
                    timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
                    timer.addTime(TimeType.ANSWER);
                    lvs.Devices[i].startMessaging("Устройство является генератором!");
                    if (real) Thread.Sleep(lvs.SleepAmount / 2);
                    lvs.Devices[i].endMessaging("Является генератором. Блокировка...");
                    if (real) Thread.Sleep(lvs.SleepAmount / 2);
                    lvs.Devices[i].changeState(DeviceState.DENIAL);
                    //=========================================

                    //======= Остановка после обнаружения генерящего элемента ===========
                    lastDevice = i;
                    break;
                }
            }

            //===== Разблокировка ОУ после генерящего =====
            for (int i = lastDevice + 1; i < lvs.Devices.Count; i++)
            {
                timer.addTime(TimeType.UNBLOCK);
                timer.addTime(TimeType.PAUSE_BEFORE_ANSWER);
                timer.addTime(TimeType.ANSWER);
                lvs.Devices[i].startMessaging("Разблокировка компьютера");
                if (real) Thread.Sleep(lvs.SleepAmount / 2);
                lvs.Devices[i].changeState(DeviceState.UNBLOCKING);
                if (real) Thread.Sleep(lvs.SleepAmount / 2);
                lvs.Devices[i].endMessaging("");
                //==============================================
            }
            lvs.SetLineState(LineState.A_WORKING);
            if (real) Thread.Sleep(lvs.SleepAmount);
        }
    }
}
