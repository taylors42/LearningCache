using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using LearningCache.Data;
using LearningCache.Entities;
using LearningCache.Scripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LearningCache.Benchmark;


[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ClientBenchmark
{
    [Params(100, 1_000, 10_000)]
    public int DatabaseSize { get; set; }

    private DataContext _context = null!;
    private IMemoryCache _cache = null!;
    private string _searchName = null!;

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"BenchmarkDb - {DatabaseSize} - {Guid.NewGuid()}")
            .Options;

        _context = new DataContext(options);

        SeedDatabase();

        _searchName = _context.Clients.First().Name;

        _cache = new MemoryCache(new MemoryCacheOptions());
        var client = _context
            .Clients
            .First(c => c.Name == _searchName);

        _cache.Set(_searchName, client, TimeSpan.FromMinutes(5));
    }

    private void SeedDatabase()
    {
        var random = new Random(42);
        var names = new[] { "Alice", "Bob", "Charlie", "David", "Emma" };

        for (int i = 0; i < DatabaseSize; i++)
        {
            _context.Clients.Add(new Client(
                Id: Guid.NewGuid(),
                Name: $"{names[random.Next(names.Length)]} {i}",
                Email: $"user{i}@teste.com"
            ));
        }

        _context.SaveChanges();
    }
    
    [GlobalCleanup]
    public void CleanUp()
    {
        _context.Dispose();
        _cache.Dispose();
    }

    [Benchmark(Baseline = true, Description = "Database Query Only")]
    public async Task<Client?> WithoutCache()
    {
        return await _context
            .Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == _searchName);
    }

    [Benchmark(Description = "Memory Cache Hit")]
    public Task<Client?> WithCacheHit()
    {
        if (_cache.TryGetValue(_searchName, out Client? client))
            return Task.FromResult(client);
        
        return Task.FromResult<Client?>(null);
    }

    [Benchmark(Description = "Cache Miss + Db Query")]
    public async Task<Client?> WithCacheMiss()
    {
        var misskey = "NonExistentKey";
        if (_cache.TryGetValue(misskey, out Client? client))
            return client;

        var result = await _context
            .Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == _searchName);

        if (result is not null)
            _cache.Set(misskey, result, TimeSpan.FromMinutes(5));

        return result;
        // 1s 0.100 0.010 0.001
        // 0.000000001
    }
}