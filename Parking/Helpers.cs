using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Parking
{
    internal static class Config
    {
        public static int RowAmount { get; set; } = 8;
    }

    internal static class Helpers
    {
        private static Queue<(int, int, int)> _clearQue = new Queue<(int, int, int)>();
        private static List<IParked> V = new List<IParked>();
        private static Queue<(int, int, int)>ClearQue { 
            get
            {
                if(_clearQue.Count > 5 )
                {
                   foreach(var i in _clearQue.Reverse())
                    {
                        ClearQueMess(i);
                    }
                    _clearQue.Clear();
                }
                return _clearQue;
            } 
            set 
            { 
                _clearQue = value;
            }
        }
        public static void ClearQueMess((int, int, int) tups)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Black;
            string a = "";
            for(int i = tups.Item3; i > 0; i--)
            {
                 a += " ";
            }
            Console.SetCursorPosition(tups.Item1, tups.Item2);
            Console.Write(a);
        }
        public static void ExceptionMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            int y = 35;
            y += ClearQue.Count;
            ClearQue.Enqueue((110, y, message.Length));
            Console.SetCursorPosition(110, y);
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void StandardWrite(string message, ConsoleColor color)
        {
            Console.SetCursorPosition(110, 30);
            Console.Write("                                                               ");
            Console.SetCursorPosition(110, 30);
            Console.Write(message);
        }
        public static void ClearLastSMessage(string message)
        {
            Console.SetCursorPosition(110, 35);
            Console.Write(message);
        }
        public static string StandardIn(ConsoleColor color)
        {
            Console.SetCursorPosition(120, 32);
            Console.ForegroundColor = color;
            string message = Console.ReadLine();
            Helpers.ClearLastOutPut(120, 32, message);
            return message.ToUpper();
        }
        public static void ClearLastOutPut(int x, int y, string output)
        {
            string clearLen = string.Empty;
            for (int i = output.Length; i > 0; i--) 
                clearLen += " ";
            Console.SetCursorPosition(x, y);
            Console.Write(clearLen);
        }
        public static void Set() 
        {

        }
        public static void PrintRows(int len, int h)
        {
            int revMod = (Config.RowAmount % 2 == 1) ? 0 : 1;
            Console.CursorTop = 5;
            Console.CursorLeft = 5;
            for(int i =h; i > 0; i--)
            {
                for(int k = len; k> 0; k--)
                {
                    WriteoutSpace();
                    WriteoutSpace();
                    Console.CursorTop++;
                }
                if(i % 2 == revMod)
                {
                    Console.CursorLeft += 3;
                }
                Console.CursorLeft += 7;
                Console.CursorTop = 5;
            }
        }
        private static void WriteoutSpace()
        {
                Console.Write("****** ");
                Console.CursorTop++;
                Console.CursorLeft -= 7;
        }
        public static string  SetOutPuts(IParked vehicle)
        {
            if (vehicle.Vehicle.Size == VehicleSize.large)
            {
                return SetBus(vehicle);
            }
            else
            return SetNormal(vehicle);
        }
        private static string SetNormal(IParked vehicle)
        {
           return vehicle.Registration + " :: "
                + vehicle.Vehicle.GetType() + " :: " +
                FormatSpan(vehicle.ElapsedTime())
                + " :: " + vehicle.ParkingUId;
    
        }
        private static string FormatSpan(TimeSpan span)
        {
            return span.TotalSeconds.ToString("N");
        }
        private static string SetBus(IParked vehicle)
        {
            var res = UIDToPos.UIdToPos(vehicle.ParkingUId);
            res.Item1 += 65 +1;
            var b = Convert.ToChar(res.Item1) + res.Item2.ToString();
            string output =  (vehicle.Registration + " ::"
                + vehicle.Vehicle.GetType() + " ::" + FormatSpan(vehicle.ElapsedTime())
                +" ::"+ vehicle.ParkingUId+b);
            return output;
        }
        public static void SetRegInPlace(IParked parked)
        {
            var res = UIDToPos.UIdToPos(parked.ParkingUId);
            int Y = 0;
            int X = 0;
            for(int i = res.Item2; i >= 0; i--)
            {
               if(i< 1)
               {
                    Y += 2;
               }
               if(parked.Vehicle.Size == VehicleSize.small 
                   && V.Contains(parked))
               {
                    Y += 3;
               }
               else if(parked.Vehicle.Size == VehicleSize.small 
                    && !V.Contains(parked))
               {
                    Y += 3;
                    if(i == 0 && !V.Any(x => x.ParkingUId == parked.ParkingUId))
                    {
                        V.Add(parked);
                    }
                    if(i == 0)
                    {
                        Y++;
                    }
               }
               else
               {
                    Y += 3;
               }
            }
            for(int k =res.Item1; k >=0; k--)
            {
                if( k < 1)
                {
                    X += 5;
                }
                else
                {
                    X += 7;
                    if(k%2 == 0)
                    {
                        X += 3;
                    }
                }
            }
            if(parked.Vehicle.Size == VehicleSize.normal)
            {
                Helpers.SetOut(parked.Registration, X, Y, parked.Vehicle.Color);
                Helpers.SetOut(parked.Registration, X, ++Y, parked.Vehicle.Color);
            }
            else if(parked.Vehicle.Size == VehicleSize.large)
            {
                Helpers.SetOut(parked.Registration+parked.Registration, X, Y, parked.Vehicle.Color);
                Helpers.SetOut(parked.Registration+parked.Registration, X, ++Y, parked.Vehicle.Color);
            }
            else
            {
                Helpers.SetOut(parked.Registration, X, Y, parked.Vehicle.Color);
            }
        }
        public static void SetOut(string message, int x, int y, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
