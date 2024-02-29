using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleModelsLVS.Models
{
    public class Message
    {

        String state;
        private Dictionary<String, int> commands;
        private Dictionary<String, int> modes;
        String type;
        int address_from;
        int address_to;

        public Message()
        {
            commands = new();
            modes = new();

            commands.Add("give_response", 1);
            commands.Add("block", 2);
            commands.Add("unblock", 3);

            modes.Add("command", 31);
            modes.Add("subaddr", 30);
        }

        private String IntStr(int num)
        {
            if (num < 32)
            {
                int tmp = num;
                char[] buf = new char[5];
                int i = 4;
                for (; tmp != 0; i--)
                {
                    buf[i] = ((tmp % 2 == 1) ? '1' : '0');
                    tmp = tmp / 2;
                }
                for (; i >= 0; i--)
                {
                    buf[i] = '0';
                }
                return new String(buf);
            }
            return "Error";
        }

        public String encodeMessage(int address, int type, String mode_name, String command, int[] respattr)
        {
            String message = "";
            String synchr = "111";
            String addrTo = IntStr(address);

            int lastbit = 0; // Бит четности
            switch (type)
            {
                case 1:
                    String K = "0"; // Разряд "Прием/Передача"
                    String mode; // Подадрес или режим управления
                    String wordCount; // Количество СД или КУ
                                      //Кодирование режима управления
                    int nmode = modes[mode_name];
                    mode = IntStr(nmode);

                    //Кодирование КУ или числа СД
                    if (nmode == 30) wordCount = IntStr(12);
                    else
                    {
                        int nCom = commands[command];
                        wordCount = IntStr(nCom);
                    }

                    message = synchr + addrTo + K + mode + wordCount;
                    break;
                case 2:
                    // Слово данных
                    message = synchr + "0110101001010010";
                    break;
                case 3:
                    // Ответное слово
                    String meserr = ((respattr[0] == 1) ? "1" : "0");
                    String resp = "1";
                    String req = ((respattr[1] == 1) ? "1" : "0");
                    String reserve = "000";
                    String group = ((respattr[2] == 1) ? "1" : "0");
                    String busy = ((respattr[3] == 1) ? "1" : "0");
                    String abfail = ((respattr[4] == 1) ? "1" : "0");
                    String control = ((respattr[5] == 1) ? "1" : "0");
                    String oufail = ((respattr[6] == 1) ? "1" : "0");
                    message = synchr + addrTo + meserr + resp + req + reserve + group + busy + abfail + control + oufail;
                    break;
            }
            for (int i = 0; i < 19; i++)
            {
                int bit = (message[i] == '1') ? 1 : 0;
                lastbit += bit;
            }
            message += (lastbit % 2).ToString();

            return message;
        }
    }
}
