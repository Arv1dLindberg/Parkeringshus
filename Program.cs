namespace Parkeringshus
{
    public class Program
    {
        // Lista av fordon
        public static List<Vehicle>[] parkingSlots = new List<Vehicle>[25];
        public static List<double> slotSpaces = new List<double>();
        static int maxSlots = 25;
        static bool isRunning = true;

        //För att följa parkeringsdata
        static int totalCarSeconds = 0;
        static int totalMotorcycleSeconds = 0;
        static int totalBusSeconds = 0;

        static int carCount = 0;
        static int motorcycleCount = 0;
        static int busCount = 0;

        public static int vehiclesFined = 0; // Bötfällda fordon
        public static double fineAmount = 0;
        static Random random = new Random();

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

            Console.WriteLine("Press v to park");
            Console.WriteLine("Press p to pay for your parking");
            Console.WriteLine("Press i for prices and additional information");
            Console.WriteLine("Press b to return to the main menu.");

            char input = Console.ReadKey().KeyChar;
            if (input == 'v')
            {
                Console.Clear();
                Console.WriteLine("Press c for Car");
                Console.WriteLine("Press m for Motorcycle");
                Console.WriteLine("Press b for Bus");

                char vehicleInput = Console.ReadKey().KeyChar;
                if (vehicleInput == 'c' || vehicleInput == 'm' || vehicleInput == 'b')
                {
                    string vehicleType = vehicleInput == 'c' ? "Car" : vehicleInput == 'm' ? "Motorcycle" : "Bus";

                    // Uppdatera räkning av fordon
                    if (vehicleType == "Car") carCount++;
                    else if (vehicleType == "Motorcycle") motorcycleCount++;
                    else if (vehicleType == "Bus") busCount++;

                    RegistrationNumber(vehicleType);
                }
                else
                {
                    VehicleType(); // Kör om vehicleType menyn om du inte klickar rätt
                }
            }
            else if (input == 'p')
            {
                ParkingPayment();
            }
            else if (input == 'i')
            {
                DisplayParkingInformation(); // Anta en metod för att visa priser och info
            }
            else if (input == 'b')
            {
                MainMenu();
            }
            else
            {
                VehicleType(); // Kör om main vehicleType menyn om du inte klickar rätt
            }
        }

        static void DisplayParkingInformation()
        {
            Console.Clear();
            Console.WriteLine("The price for parking is 1,5 sek/s, with an additional fee of 500 sek if the parking duration has expired");
            Console.WriteLine("The maximum height for vehicles is 2,5 m");
            Console.WriteLine("");
            Console.WriteLine("Press b to return to the main menu.");

            char input = Console.ReadKey().KeyChar;
            if (input == 'b')
            {
                MainMenu();
            }
            else
            {
                DisplayParkingInformation(); // Kör om menyn om man inte skriver rätt
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
            VehicleProperties(vehicleType, registrationNumber);
        }

        static void VehicleProperties(string vehicleType, string registrationNumber) // Här kan man skriva in sin färg på sitt fordon
        {
            Console.Clear();
            Console.WriteLine("The color of your vehicle:");
            string color = Console.ReadLine();

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();

            if (vehicleType == "Car")
            {
                bool isValidInput = false;
                bool isElectricCar = false;

                while (!isValidInput)
                {
                    Console.Clear();
                    Console.WriteLine("Is your car electric? (y/n):");
                    char electricInput = Console.ReadKey().KeyChar;
                    Console.WriteLine(); // Move to the next line after the key press

                    if (electricInput == 'y' || electricInput == 'Y')
                    {
                        isElectricCar = true;
                        isValidInput = true;
                    }
                    else if (electricInput == 'n' || electricInput == 'N')
                    {
                        isElectricCar = false;
                        isValidInput = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                        Console.WriteLine("\nPress Enter to try again.");
                        Console.ReadLine();
                        Console.Clear();
                    }
                }

                ParkingDuration(vehicleType, registrationNumber, color, isElectricCar: isElectricCar);
            }
            else if (vehicleType == "Motorcycle")
            {
                Console.Clear();
                Console.WriteLine("What is the brand of your motorcycle?");
                string brand = Console.ReadLine();
                ParkingDuration(vehicleType, registrationNumber, color, motorcycleBrand: brand);
            }
            else if (vehicleType == "Bus")
            {
                Console.Clear();
                Console.WriteLine("How many passengers does your bus have?");
                if (int.TryParse(Console.ReadLine(), out int passengers))
                {
                    ParkingDuration(vehicleType, registrationNumber, color, busPassengers: passengers);
                }
                else
                {
                    Console.WriteLine("Invalid input for passengers. Please enter a number.");
                    VehicleProperties(vehicleType, registrationNumber);
                }
            }
        }

        static void ParkingDuration(string vehicleType, string registrationNumber, string color, bool isElectricCar = false, string motorcycleBrand = "", int busPassengers = 0)
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

                    // Lägger till fordon i platser och justera återstående utrymme
                    parkingSlots[slotIndex].Add(vehicle);
                    slotSpaces[slotIndex] -= vehicleSize;

                    if (vehicleSize > 1.0) // För bussar
                    {
                        slotSpaces[slotIndex + 1] -= (vehicleSize - 1.0);
                        parkingSlots[slotIndex + 1].Add(vehicle); // Lägger till bussen på nästa plats
                        Console.WriteLine($"{vehicleType} parked in slots {slotIndex + 1} and {slotIndex + 2}.");
                    }
                    else // För mc och bilar
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

        public static bool CanParkVehicle(double size, out int slotIndex)
        {
            for (int i = 0; i < slotSpaces.Count; i++)
            {
                if (size > 1.0) // För bussar
                {
                    if (i + 1 < slotSpaces.Count && slotSpaces[i] >= 1.0 && slotSpaces[i + 1] >= size - 1.0)
                    {
                        slotIndex = i;
                        return true;
                    }
                }
                else if (size == 0.5) // För motorcyklar
                {
                    if (slotSpaces[i] >= 0.5)
                    {
                        slotIndex = i;
                        return true;
                    }
                }
                else if (size <= slotSpaces[i]) // För bilar
                {
                    slotIndex = i;
                    return true;
                }
            }
            slotIndex = -1; // ingen plats hittad
            return false;
        }


        public static void ParkingSlots()
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

        public static void InitializeParkingSlots()
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

                HashSet<Vehicle> checkedVehicles = new HashSet<Vehicle>();

                foreach (var slot in parkingSlots)
                {
                    if (slot != null) // ser till att platserna inte är null
                    {
                        foreach (var vehicle in slot)
                        {

                            if (checkedVehicles.Contains(vehicle))
                                continue;

                            if (vehicle.Duration > 0)
                            {
                                vehicle.Duration--;

                                if (vehicle.VehicleType1 == "Car") totalCarSeconds++;
                                else if (vehicle.VehicleType1 == "Motorcycle") totalMotorcycleSeconds++;
                                else if (vehicle.VehicleType1 == "Bus") totalBusSeconds++;
                            }

                            checkedVehicles.Add(vehicle);
                        }
                    }
                }
            }
        }

        // Utcheckningssystem
        public static void ParkingPayment()
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

                    // Gå tillbaka till main menyn
                    if (payingRegNumber.ToLower() == "b")
                    {
                        MainMenu();
                        return;
                    }

                    // Söker efter fordon i parkeringsplatser
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
                        Console.WriteLine("\nPress Enter to try again or type B to go back to the Main Menu.");
                        string retryInput = Console.ReadLine();

                        if (retryInput.ToLower() == "b")
                        {
                            MainMenu();
                            return;
                        }
                    }
                }

                bool validInput = false;
                while (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine($"Vehicle Type: {payingVehicle.VehicleType1}");
                    Console.WriteLine($"Registration Number: {payingVehicle.RegistrationNumber1}");
                    Console.WriteLine($"Color: {payingVehicle.Color}");

                    // Beräkna den totala avgiften
                    DateTime currentTime = DateTime.Now;
                    TimeSpan actualParkedTime = currentTime - payingVehicle.ParkingTimestamp;
                    int actualParkedSeconds = (int)actualParkedTime.TotalSeconds;

                    double durationPayment = actualParkedSeconds * 1.5; // Betalning baserad på de parkerade sekunderna
                    double fineAmount = payingVehicle.Duration <= 0 ? 500 : 0; // Inkludera böter om fordonet har överskridit parkeringstiden

                    double amountToPay = fineAmount + durationPayment; // Totalt belopp

                    Console.WriteLine($"The amount to pay is: {amountToPay:F2} SEK");
                    Console.WriteLine("Please enter the amount to confirm payment:");

                    string paymentInput = Console.ReadLine();
                    if (double.TryParse(paymentInput, out double payment) && payment == amountToPay)
                    {
                        validInput = true;

                        // Frigör utrymmet i parkeringsplatsen
                        double vehicleSize = payingVehicle.Size;
                        int slotIndex = Array.FindIndex(parkingSlots, slot => slot == vehicleSlot);
                        slotSpaces[slotIndex] += vehicleSize;
                        vehicleSlot.Remove(payingVehicle);

                        if (vehicleSize > 1.0 && slotIndex + 1 < slotSpaces.Count)
                        {
                            slotSpaces[slotIndex + 1] += (vehicleSize - 1.0);
                            parkingSlots[slotIndex + 1].Remove(payingVehicle);
                        }

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


        public static void IssueParkingFines()
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

                        Console.Clear(); // Displayar böter meddelandet
                        Console.WriteLine($"[FINE ISSUED] Vehicle: {vehicle.RegistrationNumber1}, Fine: 500 SEK");

                        // Pausa i 2 sek så man kan see meddlendadet
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        // Vehicle class för att lagra fordonets detaljer
        public class Vehicle
        {
            public string VehicleType1 { get; }
            public string RegistrationNumber1 { get; }
            public string Color { get; }
            public int Duration { get; set; }
            public double Size { get; } // Storlek: 0,5 för motorcyklar, 1 för bilar och 2 för bussar
            public int OriginalDuration { get; }
            public bool HasBeenFined { get; set; }
            public bool IsElectricCar { get; }
            public string MotorcycleBrand { get; }
            public int BusPassengers { get; }
            public DateTime ParkingTimestamp { get; }


            public Vehicle(string vehicleType, string registrationNumber, string color, int duration, double size, bool isElectricCar = false, string motorcycleBrand = "", int busPassengers = 0)
            {
                VehicleType1 = vehicleType;
                RegistrationNumber1 = registrationNumber;
                Color = color;
                Duration = duration;
                Size = size;
                OriginalDuration = duration;
                HasBeenFined = false;
                IsElectricCar = isElectricCar;
                MotorcycleBrand = motorcycleBrand;
                BusPassengers = busPassengers;
                ParkingTimestamp = DateTime.Now;

            }
        }
    }
}
