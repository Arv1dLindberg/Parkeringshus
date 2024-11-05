namespace Parkeringshus1
{
    internal class Program
    {
        // Lista av fordon
        static List<Vehicle> parkingSlots = new List<Vehicle>();
        static int maxSlots = 25;

        static void Main(string[] args)
        {
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

            string visitorTxt = "Visitor: Press v";
            int visitorTxtPadding = (windowWidth / 2) + (visitorTxt.Length / 2);
            Console.WriteLine(String.Format("{0," + visitorTxtPadding + "}", visitorTxt)); // För att få "Visitor: Press v" i mitten

            char input = Console.ReadKey().KeyChar;
            if (input == 'v' || input == 'V')
            {
                VehicleType();
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

            char input = Console.ReadKey().KeyChar;
            if (input == 'c' || input == 'm' || input == 'b')
            {
                string vehicleType = input == 'c' ? "Car" : input == 'm' ? "Motorcycle" : "Bus";
                RegistrationNumber(vehicleType);
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

        static void ParkingDuration(string vehicleType, string registrationNumber, string color) // Här skriver man in tiden man ska parkera
        {
            Console.Clear();
            Console.WriteLine("How long do you wish to park here (in seconds):");
            string durationInput = Console.ReadLine();

            if (int.TryParse(durationInput, out int duration))
            {
                // Skapar ett nytt fordon och lägger till den i listan
                if (parkingSlots.Count < maxSlots)
                {
                    parkingSlots.Add(new Vehicle(vehicleType, registrationNumber, color, duration));
                }
                else
                {
                    Console.WriteLine("Parking is full. Unable to add more vehicles."); // Om parkeringshuset skulle vara fullt
                    Console.WriteLine("\nPress Enter to return to the main menu...");  
                    Console.ReadLine();
                    MainMenu();
                    return;
                }

                ParkingSlots();
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                ParkingDuration(vehicleType, registrationNumber, color);
            }
        }

        static void ParkingSlots()
        {
            Console.Clear();
            Console.WriteLine("Parking Slots:");

            // Loopar igenom platserna för att se om det är ett fordon på det eller fritt
            for (int i = 0; i < maxSlots; i++)
            {
                if (i < parkingSlots.Count)
                {
                    Vehicle vehicle = parkingSlots[i];
                    Console.WriteLine($"Slot {i + 1}: {vehicle.VehicleType1} - {vehicle.RegistrationNumber1}, Color: {vehicle.Color}, Duration: {vehicle.Duration} seconds");
                }
                else
                {
                    Console.WriteLine($"Slot {i + 1}: Free");
                }
            }

            Console.WriteLine("\nPress Enter to return to the Main Menu...");
            Console.ReadLine();
            MainMenu(); // Går tillbaka till main menu
        }

        // Vehicle class för att lagra fordonets detaljer
        class Vehicle
        {
            public string VehicleType1 { get; }
            public string RegistrationNumber1 { get; }
            public string Color { get; }
            public int Duration { get; }

            public Vehicle(string vehicleType, string registrationNumber, string color, int duration)
            {
                VehicleType1 = vehicleType;
                RegistrationNumber1 = registrationNumber;
                Color = color;
                Duration = duration;
            }
        }

    }
}
