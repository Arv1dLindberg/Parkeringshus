using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Parkeringshus1
{
    internal class Program
    {
        // Lista av fordon
        static List<Vehicle>[] parkingSlots = new List<Vehicle>[25];
        static List<double> slotSpaces = new List<double>();
        static int maxSlots = 25;
        static bool isRunning = true;

        //För att följa parkeringsdata
        static int totalCarSeconds = 0;
        static int totalMotorcycleSeconds = 0;
        static int totalBusSeconds = 0;

        static int carCount = 0;
        static int motorcycleCount = 0;
        static int busCount = 0;

        static int vehiclesFined = 0; // Bötfällda fordon
        static double fineAmount = 0;
        static Random random = new Random(); // Random number generator för fines

        static void Main(string[] args)
        {
            Task.Run(() => CountdownTimer());
            InitializeParkingSlots();
            MainMenu();
        }

        static void MainMenu()
        {
            Console.Clear();
            int windowWidth = Console.WindowWidth;

            string title = "P-Garage";
            int titlePadding = (windowWidth / 2) + (title.Length / 2);
            Console.WriteLine(String.Format("{0," + titlePadding + "}", title)); // För att få "P-Garage" i mitten

            Console.WriteLine("\n");

            string visitorTxt = "Visitor: Press V";
            int visitorTxtPadding = (windowWidth / 2) + (visitorTxt.Length / 2);
            Console.WriteLine(String.Format("{0," + visitorTxtPadding + "}", visitorTxt)); // För att få "Visitor: Press v" i mitten

            Console.WriteLine("\n");

            string managerPrompt = "Manager: Press M";
            int managerPadding = (windowWidth / 2) + (managerPrompt.Length / 2);
            Console.WriteLine(String.Format("{0," + managerPadding + "}", managerPrompt)); // För att få "Manager: Press M" i mitten

            Console.WriteLine("\n");

            string attendantPrompt = "Parking attendant: Press P";
            int attendantPadding = (windowWidth / 2) + (attendantPrompt.Length / 2);
            Console.WriteLine(String.Format("{0," + attendantPadding + "}", attendantPrompt)); // För att få "Parking attendant: Press P" i mitten

            char input = Console.ReadKey().KeyChar;
            if (input == 'v' || input == 'V')
            {
                VehicleType();
            }
            else if (input == 'm' || input == 'M')
            {
                ManagerLogin();
            }
            else if (input == 'p' || input == 'P')
            {
                AttendantLogin();
            }
            else
            {
                MainMenu(); // Kör om main menu om du inte klickar rätt
            }
        }

        static void VehicleType()
        {
            Console.Clear();
            Console.WriteLine("Press c for Car");
            Console.WriteLine("Press m for Motorcycle");
            Console.WriteLine("Press b for Bus");
            Console.WriteLine("Press p to pay for your parking");

            char input = Console.ReadKey().KeyChar;
            if (input == 'c' || input == 'm' || input == 'b')
            {
                string vehicleType = input == 'c' ? "Car" : input == 'm' ? "Motorcycle" : "Bus";

                //Uppdatera räkning av fordon
                if (vehicleType == "Car") carCount++;
                else if (vehicleType == "Motorcycle") motorcycleCount++;
                else if (vehicleType == "Bus") busCount++;

                RegistrationNumber(vehicleType);
            }
            else if (input == 'p')
            {
                ParkingPayment();
            }
            else
            {
                VehicleType(); // Kör om vehicleType menyn om du inte klickar rätt
            }
        }

        static void RegistrationNumber(string vehicleType) // Här skapar den ett random registreringsnummer
        {
            Console.Clear();
            Random random = new Random();

            string letters = "";
            for (int i = 0; i < 3; i++)
                letters += (char)random.Next('A', 'Z' + 1);

            string numbers = random.Next(100, 1000).ToString();
            string registrationNumber = letters + numbers;

            Console.WriteLine($"Vehicle Type: {vehicleType}");
            Console.WriteLine($"Registration Number: {registrationNumber}");

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
            VehicleColor(vehicleType, registrationNumber);
        }

        static void VehicleColor(string vehicleType, string registrationNumber) // Här kan man skriva in sin färg på sitt fordon
        {
            Console.Clear();
            Console.WriteLine("The color of your vehicle:");
            string color = Console.ReadLine();

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
            ParkingDuration(vehicleType, registrationNumber, color);
        }

        static void ParkingDuration(string vehicleType, string registrationNumber, string color)
        {
            Console.Clear();
            Console.WriteLine("How long do you wish to park here:");
            string durationInput = Console.ReadLine();

            if (int.TryParse(durationInput, out int duration))
            {
                double vehicleSize = vehicleType == "Motorcycle" ? 0.5 : vehicleType == "Car" ? 1.0 : 2.0;

                if (CanParkVehicle(vehicleSize, out int slotIndex))
                {
                    Vehicle vehicle = new Vehicle(vehicleType, registrationNumber, color, duration, vehicleSize);

                    parkingSlots[slotIndex].Add(vehicle);
                    slotSpaces[slotIndex] -= vehicleSize;

                    if (vehicleSize > 1.0) // för bussar
                    {
                        slotSpaces[slotIndex + 1] -= (vehicleSize - 1.0);
                        parkingSlots[slotIndex + 1].Add(vehicle); // lägger till bussen i andra platsen
                        Console.WriteLine($"{vehicleType} parked in slots {slotIndex + 1} and {slotIndex + 2}.");
                    }
                    else
                    {
                        Console.WriteLine($"{vehicleType} parked in slot {slotIndex + 1}.");
                    }

                    Thread.Sleep(2000);
                    ParkingSlots();
                }
                else
                {
                    Console.WriteLine("No available space for this vehicle.");
                    Thread.Sleep(2000);
                    MainMenu();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                ParkingDuration(vehicleType, registrationNumber, color);
            }
        }

        static bool CanParkVehicle(double size, out int slotIndex)
        {
            for (int i = 0; i < slotSpaces.Count; i++)
            {
                // För bussar
                if (size > 1.0)
                {
                    if (i + 1 < slotSpaces.Count && slotSpaces[i] >= 1.0 && slotSpaces[i + 1] >= size - 1.0)
                    {
                        slotIndex = i; 
                        return true;
                    }
                }
                else if (size <= slotSpaces[i]) // För bilar och mc
                {
                    slotIndex = i;
                    return true;
                }
            }
            slotIndex = -1; // ingen plats hittat
            return false;
        }


        static void ParkingSlots()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Parking Slots:");

                // Loopar igenom platserna för att se om det är ett fordon på det eller fritt
                for (int i = 0; i < parkingSlots.Length; i++)
                {
                    Console.Write($"Slot {i + 1}: ");

                    if (parkingSlots[i].Count > 0)
                    {
                        foreach (var vehicle in parkingSlots[i])
                        {
                            string status = vehicle.Duration > 0 ? $"{vehicle.Duration}s left" : "Time's up!";
                            Console.Write($"[{vehicle.VehicleType1} - {vehicle.RegistrationNumber1}, Color: {vehicle.Color}, {status}] ");
                        }
                    }
                    else
                    {
                        Console.Write("Free");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("\nPress Enter to return to the Main Menu...");

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        MainMenu();
                        return;
                    }
                }

                // Sov i en sekund
                Thread.Sleep(1000);
            }
        }

        static void InitializeParkingSlots()
        {
            for (int i = 0; i < 25; i++)
            {
                parkingSlots[i] = new List<Vehicle>(); //varje plats har ett tomt fordon
                slotSpaces.Add(1.0); // varje plats har 1 i utrymme
            }
        }

        static async Task CountdownTimer()
        {
            while (true)
            {
                await Task.Delay(1000);

                foreach (var slot in parkingSlots)
                {
                    if (slot != null) // ser till att platserna inte är null
                    {
                        foreach (var vehicle in slot)
                        {
                            if (vehicle.Duration > 0)
                            {
                                vehicle.Duration--;

                                if (vehicle.VehicleType1 == "Car") totalCarSeconds++;
                                else if (vehicle.VehicleType1 == "Motorcycle") totalMotorcycleSeconds++;
                                else if (vehicle.VehicleType1 == "Bus") totalBusSeconds++;
                            }
                        }
                    }
                }
            }
        }

        // Utcheckningssystem
        static void ParkingPayment()
        {
            while (true)
            {
                Vehicle payingVehicle = null;
                List<Vehicle> vehicleSlot = null;

                while (payingVehicle == null)
                {
                    Console.Clear();
                    Console.WriteLine("Enter your registration number or press B to go back to the Main Menu:");

                    string payingRegNumber = Console.ReadLine();

                    // Kicka b för att gå tillbaka till main menyn
                    if (payingRegNumber.ToLower() == "b")
                    {
                        MainMenu();
                        return;
                    }

                    foreach (var slot in parkingSlots)
                    {
                        payingVehicle = slot.FirstOrDefault(vehicle => vehicle.RegistrationNumber1 == payingRegNumber);
                        if (payingVehicle != null)
                        {
                            vehicleSlot = slot;
                            break;
                        }
                    }

                    if (payingVehicle == null)
                    {
                        Console.WriteLine("No vehicle found with the given registration number.");
                        Console.WriteLine("\nPress Enter to try again.");
                        Console.ReadLine();
                    }
                }

                bool validInput = false;
                while (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine($"Vehicle Type: {payingVehicle.VehicleType1}");
                    Console.WriteLine($"Registration Number: {payingVehicle.RegistrationNumber1}");
                    Console.WriteLine($"Color: {payingVehicle.Color}");

                    // kalkylera totala avgiften
                    double fineAmount = payingVehicle.HasBeenFined ? 500 : 0; // Inkludera bötern om fordondet har blivit bötfälld eller bara 0 om den inte är bötfälld
                    double durationPayment = payingVehicle.Duration * 1.5; // Betalning baserad på parkeringsavgiften
                    double amountToPay = fineAmount + durationPayment; // Totala avgiften

                    Console.WriteLine($"The amount to pay is: {amountToPay} SEK");
                    Console.WriteLine("Please enter the amount to confirm payment:");

                    string paymentInput = Console.ReadLine();
                    if (double.TryParse(paymentInput, out double payment) && payment == amountToPay)
                    {
                        validInput = true;
                        vehicleSlot.Remove(payingVehicle); // Remove the vehicle from the parking slot
                        Console.Clear();
                        Console.WriteLine("Payment successful! You are now checked out. Have a nice day!");
                        Console.WriteLine("\nPress Enter to return to the Main Menu.");
                        Console.ReadLine();
                        MainMenu();
                    }
                    else
                    {
                        Console.WriteLine("Incorrect payment amount.");
                        Console.WriteLine("\nPress Enter to try again.");
                        Console.ReadLine();
                    }
                }
            }
        }


        static void ManagerLogin()
        {
            Console.Clear();
            Console.WriteLine("\n\nEnter password or press B to go back to main menu");

            string password = Console.ReadLine();

            if (password.ToLower() == "b")
            {
                MainMenu(); // Går tillbaka till main menyn om man trycker på b och sedan enter
            }
            else if (password == "qwe123")
            {
                ShowManagerStats();
            }
            else
            {
                Console.WriteLine("Wrong password, try again.");
                Thread.Sleep(2000); // Kommer pausa 2 sek sedan kommer det du har skrivit som inte är rätt lösenord och texten "Wrong password, try again." försvinna
                ManagerLogin();
            }
        }

        static void ShowManagerStats()
        {
            Console.Clear();

            double carEarnings = totalCarSeconds * 1.5;
            double motorcycleEarnings = totalMotorcycleSeconds * 1.5;
            double busEarnings = totalBusSeconds * 1.5;

            Console.WriteLine("Manager Stats:");
            Console.WriteLine($"Total Cars Parked: {carCount}, Total Seconds: {totalCarSeconds}, Earnings: {carEarnings} kr");
            Console.WriteLine($"Total Motorcycles Parked: {motorcycleCount}, Total Seconds: {totalMotorcycleSeconds}, Earnings: {motorcycleEarnings} kr");
            Console.WriteLine($"Total Buses Parked: {busCount}, Total Seconds: {totalBusSeconds}, Earnings: {busEarnings} kr");
            Console.WriteLine($"Total Vehicles Fined: {vehiclesFined}");
            Console.WriteLine($"Total fine amount: {fineAmount} SEK");

            Console.WriteLine("\nPress Enter to return to the main menu...");
            Console.ReadLine();
            MainMenu();
        }

        static void AttendantLogin()
        {
            Console.Clear();
            Console.WriteLine("\n\nEnter password or press B to go back to main menu:");

            string password = Console.ReadLine();

            if (password.ToLower() == "b")
            {
                MainMenu();
            }
            else if (password == "123qwe")
            {
                AttendantMenu();
            }
            else
            {
                Console.WriteLine("Wrong password, try again.");
                Thread.Sleep(2000);
                AttendantLogin();
            }
        }

        static void AttendantMenu()
        {
            Console.Clear();

            
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10000); // väntar 10 sek
                    IssueParkingFines(); 
                }
            });

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Parking Attendant Menu:");
                Console.WriteLine($"Vehicles fined: {vehiclesFined}");
                Console.WriteLine($"Total fine amount: {fineAmount} SEK");

                Console.WriteLine("\nVehicles available to be fined:");

                bool TimedUpVehicles = false; // checkar om det är några fordon som är "timed up"
                foreach (var slot in parkingSlots)
                {
                    foreach (var vehicle in slot)
                    {
                        if (vehicle.Duration <= 0)
                        {
                            TimedUpVehicles = true;
                            string fineStatus = vehicle.HasBeenFined ? "Already fined" : "Not fined";
                            Console.WriteLine($"[Vehicle: {vehicle.VehicleType1}, Reg: {vehicle.RegistrationNumber1}, Color: {vehicle.Color}, {fineStatus}]");
                        }
                    }
                }

                if (!TimedUpVehicles)
                {
                    Console.WriteLine("No timed up vehicles currently.");
                }

                Console.WriteLine("\nPress B to go back to the Main Menu.");

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).KeyChar;
                    if (char.ToLower(key) == 'b')
                    {
                        MainMenu();
                        return;
                    }
                }

                Thread.Sleep(1000); // vilar i en sek
            }
        }


        static void IssueParkingFines()
        {
            foreach (var slot in parkingSlots)
            {
                foreach (var vehicle in slot)
                {
                    if (vehicle.Duration <= 0 && !vehicle.HasBeenFined && random.Next(0, 2) == 0) // 50% chans
                    {
                        vehiclesFined++;
                        fineAmount += 500;
                        vehicle.HasBeenFined = true; // markerar den bötfällda fordonet
                                                     
                        Console.Clear(); // Displayar fine meddelandet
                        Console.WriteLine($"[FINE ISSUED] Vehicle: {vehicle.RegistrationNumber1}, Fine: 500 SEK");

                        // Pausa i 2 sek så man kan see meddlendadet
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        // Vehicle class för att lagra fordonets detaljer
        class Vehicle
        {
            public string VehicleType1 { get; }
            public string RegistrationNumber1 { get; }
            public string Color { get; }
            public int Duration { get; set; }
            public double Size { get; } // Storlek: 0,5 för motorcyklar, 1 för bilar och 2 för bussar
            public bool HasBeenFined { get; set; }


            public Vehicle(string vehicleType, string registrationNumber, string color, int duration, double size)
            {
                VehicleType1 = vehicleType;
                RegistrationNumber1 = registrationNumber;
                Color = color;
                Duration = duration;
                Size = size;
                HasBeenFined = false;
            }
        }
    }
}
    

