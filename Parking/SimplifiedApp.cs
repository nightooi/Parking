using Parking.Instantiations;

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace Parking
{
    public class SimplifiedApp
    {
        Random random = new Random();
       IParkingLot ParkingLot { get; set; }
       IList<IParked> Parked { get; set; }
       public SimplifiedApp()
       {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            this.Parked = new List<IParked>();
            ParkingLot = (new ParkingLotFactory() 
                as ISimpleFactory<IFactory<IParkingLot>, IParkingLot>).Create();
       }
       public void Start()
       {
            Console.ReadKey();
            Helpers.StandardWrite("Welcome To parkinglot", ConsoleColor.White);
            Console.ReadKey();
            ConsoleKeyInfo info = new();
            while(info.Key !=  ConsoleKey.A)
            {
                if(info.Key == ConsoleKey.E)
                {
                    Enter();
                }
                else if(info.Key == ConsoleKey.Enter)
                {
                    
                    string[] a = new string[]
                    {
                        $"{this.RandomData(0)}",
                        $"{this.RandomData(1)}",
                        $"{this.RandomData(2)}"
                    };
                    Enter(a);
                }
                else if(info.Key == ConsoleKey.X)
                {
                    Remove(); 
                }
                Helpers.PrintRows(Config.RowAmount,Config.RowAmount);
                ShowParked();
                Helpers.StandardWrite(
                    "Press [a] to exit or [e] to" +
                    " Enter Data [x] To remove",
                    ConsoleColor.White);
                info = Console.ReadKey();
            }
       }
       private void Remove()
       {
            var res = GetRegRemove();
            if (res == null) return;
            GetPaid(res);
            ClearOutput();
            FreeParking(res);
       }
       public void ClearOutput()
       {
            int y = 5;
            int x = 75;
            foreach(var item in Parked)
            {
                if (y < 20)
                {
                    y += 1;
                }
                else
                {
                    y = 5;
                    x += 30;
                }
                Helpers.ClearLastOutPut(x, y, Helpers.SetOutPuts(item));
                Thread.Sleep(200);
            }
       }
       private void FreeParking(IParked parked)
       {
            this.Parked.Remove(parked);
            this.ParkingLot.FreeSpot(parked.Vehicle,parked.ParkingUId);
       }
       private void GetPaid(IParked parked)
        {
            var time = parked.EndParking();
            string construct = $"you parked for{time.Minutes}: {time.Seconds}," +
                $"the price will be{(time.Minutes * 60 + time.Seconds) * 2.5}kr";
            Helpers.StandardWrite(construct, ConsoleColor.DarkGreen);
            Helpers.StandardIn(ConsoleColor.Red);
        }
       private IParked? GetRegRemove()
       {
            IParked res2;
            string res = "";
            string unim = "";
            bool k = false;
            while (res != "end")
            {
                res2 = this.ParseTrans<IParked?>(out res, out k,
                    (i) =>
                    {
                        IParked res;
                        if ((res = this.Parked.First(x => x.Registration == i.ToUpper())) is not null)
                        {
                            return res;
                        }
                        return null;
                    }, "Enter The registration", "this", "end",
                    Stage.Checkout,
                    Register.Registration)!;
                if (res2 is not null)
                    return res2;
                else Helpers.ExceptionMessage("Registration did not exist", ConsoleColor.DarkMagenta);
            }
            return null;
       }
       private string RandomData(int stage)
        {
            return stage switch
            {
                0 => this.RandomColor(),
                1 => this.RandomReg(),
                2 => this.RadomSize()

            };
        }

        private string RandomColor()
        {
            return random.Next(0, 3) switch
            {
                0 => "blue",
                1 => "red",
                2 => "green"
            };
        }
        private string RandomReg()
        {
            return $"{random.Next(0, 9)}" +
                   $"{random.Next(0, 9)}" +
                   $"{random.Next(0, 9)}" +
                   $"{Convert.ToChar(random.Next(65, 91))}" +
                   $"{Convert.ToChar(random.Next(65, 91))}" +
                   $"{Convert.ToChar(random.Next(65, 91))}";
        }
        private string RadomSize()
        {
            return random.Next(0, 3) switch
            {
                0=> "buss",
                1=> "car",
                2=> "bike"
            };
        }
       private void Enter()
        {
            AddItems(null);
        }
        private void Enter(string[] data)
        {
            AddItems(data);
        }
        private void AddItems(string[]? data)
        {
             while (true)
            {
                ValueTuple<ConsoleColor, string,VehicleSize>  res;
                if (data is not null)
                {
                    res = EnterData(data);
                }
                else res = EnterData();
                if(ParkingLot.IsUnavailable(res.Item3) is not null)
                {
                    Helpers.StandardWrite("We're sadly full", ConsoleColor.Red);
                    Thread.Sleep(1000);
                    return;
                }    
                if(res.Item2 != string.Empty)
                {
                    Vehicle vehicle = VehicleFactory.Create(
                        res.Item3,
                        res.Item2,
                        res.Item1);
                    var space =ParkingLot.AssignSpot(vehicle);
                    var parked = ParkedFactorySingleTon
                        .Create(res.Item2, space, vehicle);
                    this.Parked.Add(parked);
                    return;
                }
                return;
            }

        }
       private void ShowParked()
       {
            int y = 5;
            int x = 75;
            foreach(var item in Parked)
            {
                if (y < 20)
                {
                    y += 1;
                }
                else
                {
                    y = 5;
                    x += 30;
                }
                Helpers.SetOut(Helpers.SetOutPuts(item), x, y, item.Vehicle.Color);
                if(item.Vehicle.Size != VehicleSize.small)
                {
                    Helpers.SetRegInPlace(item);
                }
                else
                {
                    Helpers.SetRegInPlace(item);
                }
            }
       }
private (ConsoleColor, string, VehicleSize) EnterData(string[] enterData)
       {
            bool parsed = false;
            string currentParse = "color";
            bool c = false;
            ConsoleColor color = ConsoleColor.Black;
            string reg = string.Empty;
            VehicleSize size = VehicleSize.UN;
            while (!parsed || Console.ReadKey().KeyChar == 'x')
            {
                switch (currentParse)
                {
                    case "color":
                        color = this.ParseTrans(out currentParse,out c,
                        (o) =>
                            {
                                return o.ToLower() switch
                                {
                                    "green" => ConsoleColor.Green,
                                    "red" => ConsoleColor.Red,
                                    "blue" => ConsoleColor.Blue
                                };
                            },
                            "Enter Color Red, gree or blue, hit X to leave",
                            "reg",
                            "color",
                            Stage.Register,
                            Register.Color,
                            enterData[0]);
                        if (!c) continue;
                       break;

                    case "reg":
                        reg = this.ParseTrans(out currentParse, out c,
                            (i) => { 

                                return i;
                            },
                            "Enter Registration 3 numbers, 3 letters",
                            "size",
                            "reg",
                            Stage.Register,
                            Register.Registration,
                            enterData[1]);
                        if (!c) continue;
                       break;

                    case "size":
                        size = this.ParseTrans(out currentParse, out c,
                            (i) =>
                            {
                                return i.ToLower() switch
                                {
                                    "bike" => VehicleSize.small,
                                    "car" => VehicleSize.normal,
                                    "buss" => VehicleSize.large
                                };
                            },
                            "Enter Size of you vehicle, Bike, Car, Buss",
                            "end",
                            "size",
                            Stage.Register,
                            Register.Size,
                            enterData[2]);
                        if (!c) continue;
                        return (color, reg, size);
                    case "end":
                    return (ConsoleColor.White, "", VehicleSize.UN);
                }
            }
            return (color, reg, size);
       }
       private (ConsoleColor, string, VehicleSize) EnterData()
       {
            bool parsed = false;
            string currentParse = "color";
            bool c = false;
            ConsoleColor color = ConsoleColor.Black;
            string reg = string.Empty;
            VehicleSize size = VehicleSize.UN;
            while (!parsed || Console.ReadKey().KeyChar == 'x')
            {
                switch (currentParse)
                {
                    case "color":
                        color = this.ParseTrans(out currentParse,out c,
                        (o) =>
                            {
                                return o.ToLower() switch
                                {
                                    "green" => ConsoleColor.Green,
                                    "red" => ConsoleColor.Red,
                                    "blue" => ConsoleColor.Blue
                                };
                            },
                            "Enter Color Red, gree or blue, hit X to leave",
                            "reg",
                            "color",
                            Stage.Register,
                            Register.Color);
                        if (!c) continue;
                       break;

                    case "reg":
                        reg = this.ParseTrans(out currentParse, out c,
                            (i) => { 

                                return i;
                            },
                            "Enter Registration 3 numbers, 3 letters",
                            "size",
                            "reg",
                            Stage.Register,
                            Register.Registration);
                        if (!c) continue;
                       break;

                    case "size":
                        size = this.ParseTrans(out currentParse, out c,
                            (i) =>
                            {
                                return i.ToLower() switch
                                {
                                    "bike" => VehicleSize.small,
                                    "car" => VehicleSize.normal,
                                    "buss" => VehicleSize.large
                                };
                            },
                            "Enter Size of you vehicle, Bike, Car, Buss",
                            "end",
                            "size",
                            Stage.Register,
                            Register.Size);
                        if (!c) continue;
                        return (color, reg, size);
                    case "end":
                        return (ConsoleColor.White, "", VehicleSize.UN);
                }
            }
            return (color, reg, size);
       }
       private Func<Func<string, Stage, Register, bool>,Func<string, T?>, T?>
            SPTransFormer<T>(
           string input,
           Stage stage,
           Register reg,
           Checkout? checkout)
       {
            if (checkout is not null)
                return new Func<Func<string, Stage, Register, bool>, Func<string, T>, T?>
                    ((checker, parser) =>
                    {
                        if(checker(input, stage, reg))
                        {
                            return parser(input);
                        }
                        return default(T);
                    })!;
            return (Checker, parser) =>
            {
                if (Checker(input, stage, reg))
                {
                    return parser(input);
                }
                return default(T);
            };

       }
       private T ParseTrans<T>(
           out string currentParse,
           out bool c,
           Func<string, T?> parser,
           string message,
           string next,
           string current,
           Stage stage,
           Register reg,
           string data)
       {
            Helpers.StandardWrite(message, ConsoleColor.White);
                var res = SPTransFormer<T>(data, stage, reg, null)(
                    Parse,
                parser);
            if(res is null || res.Equals(default(T)))
            {
                NotAccepted(data);
                c = false;
                currentParse = current;
                return res;
            }
            c = true;
            currentParse = next;
            return res;
       }
private T ParseTrans<T>(
           out string currentParse,
           out bool c,
           Func<string, T?> parser,
           string message,
           string next,
           string current,
           Stage stage,
           Register reg)
       {
            Helpers.StandardWrite(message, ConsoleColor.White);
            string data = Helpers.StandardIn(ConsoleColor.DarkGray);
                var res = SPTransFormer<T>(data, stage, reg, null)(
                    Parse,
                parser);
            if(data == "X")
            {
                currentParse = "end";
            }
            else if(res is null || res.Equals(default(T)))
            {
                NotAccepted(data);
                c = false;
                currentParse = current;
                return res;
            }
            else
            {
                currentParse = next;
            }
            c = true;
            return res;
       }
       private void NotAccepted(string item)
       {
            Helpers.ExceptionMessage($"{item} not accepted", ConsoleColor.DarkMagenta);
       }
       private bool Parse(string input, Stage stage, Register reg)
       {
            return stage switch
            {
                Stage.Register => RegstrationParse(input, reg),
                Stage.Checkout => RegParse(input, true)
            };
       }
        private bool RegstrationParse(string input, Register reg)
        {
            return reg switch
            {
                Register.Registration => RegParse(input, false),
                Register.Size => SizeParse(input),
                Register.Color => ColorParse(input)
            };
        }
       private bool ColorParse(string input)
       {
            string colors = "red green blue";
            return colors.Split(" ").Contains(input.ToLower());
       }
       private bool RegParse(string input, bool allowRegDouble)
       {
            int res = 0;
            if(input.Length < 6 || input.Length > 6)
            {
                return false;
            }
            bool numRes  = int.TryParse(input.Substring(0, 3),out res);
            bool signRes = input.Substring(2, 4).Any(x => (char.IsLetter(x) && !char.IsDigit(x)));
            bool exists  = this.Parked.Any(x => x.Registration == input);
            if (allowRegDouble)
            {
                return (numRes & signRes) & exists;
            }
            return (numRes & signRes) & (!exists);

       }
       private bool SizeParse(string input)
       {
            string alts = $"bike car buss";
            return alts.Split(" ").Contains(input.ToLower());
       }
       enum Stage { Register, Checkout}
       enum Register { Color, Registration, Size}
       enum Checkout { Registration }
    }
    public class ParkingLotFactory : ISimpleFactory<IFactory<IParkingLot>, IParkingLot>
    {
        IFactory<IParkingLot> _factory;
        public IFactory<IParkingLot> Factory => _factory;
        public ParkingLotFactory()
        {
            _factory = new ParkingFactory();
        }
    }
    public class ParkingFactory : IFactory<IParkingLot>
    {
        object[] _params;
        public object[] parameters => _params;
        Func<object[], IParkingLot> _imp;
        public Func<object[], IParkingLot> Imp => _imp;
        public ParkingFactory()
        {
            _imp = (o) =>
            {
                return new Parkinglot(new SimpleParkingLotFactory(new ParkingRowFact()));
            };
        }
    }

    public class SimpleParkingLotFactory : ISimpleFactory<IFactory<IEnumerable<IParkingRow>>, IEnumerable<IParkingRow>>
    {
        IFactory<IEnumerable<IParkingRow>> _factory;
        public IFactory<IEnumerable<IParkingRow>> Factory => _factory;

        public SimpleParkingLotFactory(IFactory<IEnumerable<IParkingRow>> parkingFact)
        {
            _factory = parkingFact;
        }
    }
    public class ParkingRowFact : IFactory<IEnumerable<IParkingRow>>
    {
        object[] _params;
        public object[] parameters => _params;
        Func<object[], IEnumerable<IParkingRow>> _rowFact;
        public Func<object[], IEnumerable<IParkingRow>> Imp => _rowFact;
        public ParkingRowFact()
        {
            _params = CreateParams();

            _rowFact = (o) =>
            {
                int i = 0;
                ParkingRow[] parkingRows = new ParkingRow[Config.RowAmount];
                SpaceFact[] s = new SpaceFact[Config.RowAmount];
                for(int k =0; k < parkingRows.Length; k++)
                {
                    parkingRows[k] =
                    new ParkingRow(true, k, ((int)o[1]), ((string)o[0]).ToUpper()[k].ToString(),
                    ((k%2==0) ? (((k + 1 <= parkingRows.Length-1) ? k+1 : -1)) : -1) ,Config.RowAmount,
                       (s[k] = new SpaceFact(new SpaceFactImp(Config.RowAmount, ((string)o[0]).ToUpper()[k].ToString()))));
                }
                return parkingRows;
            };
        }
        private object[] CreateParams()
        {
            return ["abcdefghijk", Config.RowAmount, true];
        }
    }

    public class SpaceFact : ISimpleFactory<IFactory<IEnumerable<IParkingSpace>>,
                             IEnumerable<IParkingSpace>>
    {
        IFactory<IEnumerable<IParkingSpace>> _factory;
        public IFactory<IEnumerable<IParkingSpace>> Factory => _factory;

        public SpaceFact(IFactory<IEnumerable<IParkingSpace>> fact)
        {
            _factory = fact;
        }
    }

    public class SpaceFactImp : IFactory<IEnumerable<IParkingSpace>>
    {
        object[] _params;
        public object[] parameters => _params;
        Func<object[], IEnumerable<IParkingSpace>> _imp;
        public Func<object[], IEnumerable<IParkingSpace>> Imp => _imp;

        public SpaceFactImp(int numb, string a)
        {
            _params = [numb, a];

            _imp = (o) =>
            {
                ParkingSpace[] spaces = new ParkingSpace[Config.RowAmount];
                for(int i = 0; i < (int)(o[0]); i++)
                {
                    spaces[i] = new((string)(o[1]), i);
                }
                return spaces;
            };
        }
    }
}
