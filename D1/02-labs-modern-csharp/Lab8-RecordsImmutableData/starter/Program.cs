// ============================================================
// Lab 8: Applying Records for Immutable Data Models
// Topics: Value Equality, With-Expressions, Record Inheritance,
//         Deconstruction, Pattern Matching with Records
// ============================================================

Console.WriteLine("=== Lab 8: Records for Immutable Data Models ===\n");

// ----- Exercise 1: Value Equality in Records -----
Console.WriteLine("--- Exercise 1: Value Equality in Records ---");

// TODO: Define a record: record Address(string Street, string City, string Country);

// TODO: Create two Address instances with identical values:
//       var addr1 = new Address("Dam 1", "Amsterdam", "NL");
//       var addr2 = new Address("Dam 1", "Amsterdam", "NL");

// TODO: Verify value equality:
//       Console.WriteLine($"  addr1 == addr2: {addr1 == addr2}");              // True
//       Console.WriteLine($"  ReferenceEquals: {ReferenceEquals(addr1, addr2)}"); // False

// TODO: Create a third Address with a different city:
//       var addr3 = new Address("Dam 1", "Rotterdam", "NL");
//       Console.WriteLine($"  addr1 == addr3: {addr1 == addr3}");              // False

// TODO: Show matching hash codes for equal records:
//       Console.WriteLine($"  addr1 hash: {addr1.GetHashCode()}");
//       Console.WriteLine($"  addr2 hash: {addr2.GetHashCode()}");
//       Console.WriteLine($"  Hashes equal: {addr1.GetHashCode() == addr2.GetHashCode()}");

Console.WriteLine();

// ----- Exercise 2: With-Expressions for Immutability -----
Console.WriteLine("--- Exercise 2: With-Expressions ---");

// TODO: Define a record: record Employee(string Name, string Department, decimal Salary);

// TODO: Create an employee:
//       var emp = new Employee("Alice", "Engineering", 85000m);
//       Console.WriteLine($"  Original: {emp}");

// TODO: Create copies with modifications:
//       var promoted = emp with { Department = "Management" };
//       Console.WriteLine($"  Promoted: {promoted}");
//
//       var raised = emp with { Salary = 95000m };
//       Console.WriteLine($"  With raise: {raised}");
//
//       var both = emp with { Department = "Management", Salary = 100000m };
//       Console.WriteLine($"  Both changes: {both}");

// TODO: Verify original is unchanged:
//       Console.WriteLine($"  Original unchanged: {emp}");

Console.WriteLine();

// ----- Exercise 3: Record Inheritance -----
Console.WriteLine("--- Exercise 3: Record Inheritance ---");

// TODO: Define a record hierarchy:
//       record Vehicle(string Make, int Year);
//       record Car(string Make, int Year, int Doors) : Vehicle(Make, Year);
//       record Truck(string Make, int Year, double PayloadTons) : Vehicle(Make, Year);

// TODO: Create instances:
//       var car = new Car("Toyota", 2024, 4);
//       var truck = new Truck("Toyota", 2024, 5.0);
//       Console.WriteLine($"  Car: {car}");
//       Console.WriteLine($"  Truck: {truck}");

// TODO: Show that different types are not equal even with same base properties:
//       Console.WriteLine($"  car == truck: {car == (Vehicle)truck}");  // False

// TODO: Use with on a derived record:
//       var newCar = car with { Year = 2025 };
//       Console.WriteLine($"  New car: {newCar}");

Console.WriteLine();

// ----- Exercise 4: Deconstruction and Positional Patterns -----
Console.WriteLine("--- Exercise 4: Deconstruction & Positional Patterns ---");

// TODO: Define: record Temperature(string Location, double Celsius, DateTime MeasuredAt);

// TODO: Deconstruct:
//       var reading = new Temperature("Amsterdam", 18.5, DateTime.Now);
//       var (location, celsius, measuredAt) = reading;
//       Console.WriteLine($"  Location: {location}, Temp: {celsius}°C, At: {measuredAt:HH:mm}");

// TODO: Write a static method ClassifyReading(Temperature reading) using positional patterns:
//       reading switch
//       {
//           (_, < 0, _)             => "Freezing",
//           (_, >= 0 and < 15, _)   => "Cold",
//           (_, >= 15 and < 30, _)  => "Comfortable",
//           (_, >= 30, _)           => "Hot",
//       };

// TODO: Test with various readings:
//       Temperature[] readings = [
//           new("Oslo", -5, DateTime.Now),
//           new("London", 10, DateTime.Now),
//           new("Amsterdam", 20, DateTime.Now),
//           new("Dubai", 42, DateTime.Now)
//       ];
//       foreach (var r in readings)
//           Console.WriteLine($"  {r.Location} ({r.Celsius}°C): {ClassifyReading(r)}");

Console.WriteLine();

// ----- Exercise 5: Records + Pattern Matching (Payment Processor) -----
Console.WriteLine("--- Exercise 5: Payment Processor ---");

// TODO: Define a record hierarchy:
//       abstract record Payment(decimal Amount);
//       record CreditCard(decimal Amount, string CardNumber, string Expiry) : Payment(Amount);
//       record BankTransfer(decimal Amount, string IBAN) : Payment(Amount);
//       record DigitalWallet(decimal Amount, string Provider) : Payment(Amount);

// TODO: Write ProcessPayment(Payment payment) using pattern matching:
//       CreditCard { Amount: > 10000 }                              => "Credit card over limit — declined"
//       CreditCard c                                                 => $"Processing credit card ending in {c.CardNumber[^4..]}"
//       BankTransfer { Amount: > 50000 }                             => "Large transfer — requires verification"
//       BankTransfer b                                               => $"Processing bank transfer to {b.IBAN}"
//       DigitalWallet { Provider: "PayPal" or "Apple Pay" } w        => $"Processing {w.Provider} payment of {w.Amount:C}"
//       DigitalWallet w                                              => $"Unknown wallet provider: {w.Provider}"
//       _                                                            => "Unknown payment type"

// TODO: Test:
//       Payment[] payments = [
//           new CreditCard(500m, "4111111111111111", "12/26"),
//           new CreditCard(15000m, "5500000000000004", "06/27"),
//           new BankTransfer(2500m, "NL91ABNA0417164300"),
//           new BankTransfer(75000m, "DE89370400440532013000"),
//           new DigitalWallet(49.99m, "PayPal"),
//           new DigitalWallet(29.99m, "CryptoWallet")
//       ];
//       foreach (var payment in payments)
//           Console.WriteLine($"  {ProcessPayment(payment)}");

Console.WriteLine("\nLab 8 completed!");

// ----- Type definitions below -----
