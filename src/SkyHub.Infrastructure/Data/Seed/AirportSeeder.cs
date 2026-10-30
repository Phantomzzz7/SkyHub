using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using SkyHub.Domain.Entities;

namespace SkyHub.Infrastructure.Data.Seed;

public class AirportSeeder
{
    private const string CsvUrl = "https://davidmegginson.github.io/ourairports-data/airports.csv";

    private readonly SkyHubDbContext _context;

    public AirportSeeder(SkyHubDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // If there are already airports in the database, do nothing
        if (await _context.Airports.AnyAsync())
        {
            return;
        }

        using var httpClient = new HttpClient();
        using var stream = await httpClient.GetStreamAsync(CsvUrl);
        using var reader = new StreamReader(stream);

        using var parser = new TextFieldParser(reader);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        // First line = column names. We remember the position of each column.
        var headerFields = parser.ReadFields();
        if (headerFields == null)
        {
            return;
        }

        var columns = new Dictionary<string, int>();
        for (int i = 0; i < headerFields.Length; i++)
        {
            columns[headerFields[i]] = i;
        }

        var airports = new List<Airport>();
        var usedIcaoCodes = new HashSet<string>();

        while (!parser.EndOfData)
        {
            var fields = parser.ReadFields();
            if (fields == null)
            {
                continue;
            }

            var type = GetValue(fields, columns, "type");
            var scheduledService = GetValue(fields, columns, "scheduled_service");
            var icaoCode = GetValue(fields, columns, "icao_code");

            if (!IsGoodAirport(type, scheduledService, icaoCode))
            {
                continue;
            }

            // Skip duplicates, because IcaoCode must be unique
            if (!usedIcaoCodes.Add(icaoCode))
            {
                continue;
            }

            var latitudeText = GetValue(fields, columns, "latitude_deg");
            var longitudeText = GetValue(fields, columns, "longitude_deg");

            if (!double.TryParse(latitudeText, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude))
            {
                continue;
            }

            if (!double.TryParse(longitudeText, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude))
            {
                continue;
            }

            var iataCode = GetValue(fields, columns, "iata_code");
            var countryCode = GetValue(fields, columns, "iso_country");

            var airport = new Airport
            {
                IcaoCode = icaoCode,
                IataCode = string.IsNullOrWhiteSpace(iataCode) ? null : iataCode,
                Name = GetValue(fields, columns, "name"),
                City = GetValue(fields, columns, "municipality"),
                Country = GetCountryName(countryCode),
                Latitude = latitude,
                Longitude = longitude
            };

            airports.Add(airport);
        }

        _context.Airports.AddRange(airports);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Seeded {airports.Count} airports.");
    }

    // Returns the value of a column by its name, or empty text if it does not exist
    private static string GetValue(string[] fields, Dictionary<string, int> columns, string columnName)
    {
        if (!columns.ContainsKey(columnName))
        {
            return "";
        }

        int index = columns[columnName];

        if (index >= fields.Length)
        {
            return "";
        }

        return fields[index];
    }

    private static bool IsGoodAirport(string type, string scheduledService, string icaoCode)
    {
        // Only airports with scheduled flights
        if (scheduledService != "yes")
        {
            return false;
        }

        // We need an ICAO code
        if (string.IsNullOrWhiteSpace(icaoCode))
        {
            return false;
        }

        return type == "large_airport"
            || type == "medium_airport"
            || type == "small_airport";
    }

    private static string GetCountryName(string countryCode)
    {
        try
        {
            return new RegionInfo(countryCode).EnglishName;
        }
        catch
        {
            // If the code is not recognized, keep the code itself
            return countryCode;
        }
    }
}