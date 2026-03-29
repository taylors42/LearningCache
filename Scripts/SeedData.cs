using LearningCache.Data;
using LearningCache.Entities;

namespace LearningCache.Scripts;

public static class SeedData
{
    private static readonly string[] FirstNames =
    [
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
        "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa",
        "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra"
    ];

    private static readonly string[] LastNames =
    [
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White"
    ];

    private static readonly string[] Domains =
    [
        "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "example.com",
        "company.com", "mail.com", "test.com"
    ];

    private static readonly Random Random = new();

    public static void SeedRandomClients(DataContext context, int count = 50)
    {
        var existingCount = context.Clients.Count();
        
        if (existingCount > 0)
        {
            Console.WriteLine($"Database already contains {existingCount} clients. Skipping seed.");
            return;
        }

        var clients = GenerateRandomClients(count);
        context.Clients.AddRange(clients);
        context.SaveChanges();
        
        Console.WriteLine($"Successfully seeded {count} random clients into the database.");
    }

    private static List<Client> GenerateRandomClients(int count)
    {
        var clients = new List<Client>();
        var usedEmails = new HashSet<string>();

        for (int i = 0; i < count; i++)
        {
            string email;
            do
            {
                email = GenerateRandomEmail();
            } while (!usedEmails.Add(email));

            var client = new Client(
                Id: Guid.NewGuid(),
                Name: GenerateRandomName(),
                Email: email
            );
            
            clients.Add(client);
        }

        return clients;
    }

    private static string GenerateRandomName()
    {
        var firstName = FirstNames[Random.Next(FirstNames.Length)];
        var lastName = LastNames[Random.Next(LastNames.Length)];
        return $"{firstName} {lastName}";
    }

    private static string GenerateRandomEmail()
    {
        var firstName = FirstNames[Random.Next(FirstNames.Length)].ToLower();
        var lastName = LastNames[Random.Next(LastNames.Length)].ToLower();
        var domain = Domains[Random.Next(Domains.Length)];
        var number = Random.Next(1, 9999);
        
        var patterns = new[]
        {
            $"{firstName}.{lastName}{number}@{domain}",
            $"{firstName}{lastName}{number}@{domain}",
            $"{firstName}_{lastName}{number}@{domain}",
            $"{firstName}{number}@{domain}",
            $"{lastName}{firstName}{number}@{domain}"
        };
        
        return patterns[Random.Next(patterns.Length)];
    }
}
