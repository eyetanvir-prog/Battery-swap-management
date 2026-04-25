using BatterySwap.API.Data;
using BatterySwap.API.Models;
using BatterySwap.API.Options;
using BCrypt.Net;
using BatterySwap.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BatterySwap.API.Services;

public static class AppInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AppInitializer");
        var adminSeedOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedOptions>>().Value;

        await dbContext.Database.MigrateAsync(cancellationToken);

        var admin = await dbContext.Admins.FirstOrDefaultAsync(x => x.Email == adminSeedOptions.Email, cancellationToken);
        if (admin is null)
        {
            admin = new Admin
            {
                Name = adminSeedOptions.Name,
                Phone = adminSeedOptions.Phone,
                Email = adminSeedOptions.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminSeedOptions.Password),
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Admins.Add(admin);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded default admin account: {Email}", admin.Email);
        }

        if (!await dbContext.Stations.AnyAsync(cancellationToken))
        {
            dbContext.Stations.AddRange(
                new Station { StationName = "Rampura Station", StationCode = "STN001", Address = "Rampura, Dhaka", Latitude = 23.763800m, Longitude = 90.424200m, Status = "Open" },
                new Station { StationName = "Mirpur Station", StationCode = "STN002", Address = "Mirpur 10, Dhaka", Latitude = 23.806700m, Longitude = 90.368700m, Status = "Open" },
                new Station { StationName = "Badda Station", StationCode = "STN003", Address = "Badda Link Road, Dhaka", Latitude = 23.780600m, Longitude = 90.425500m, Status = "Open" },
                new Station { StationName = "Uttara Station", StationCode = "STN004", Address = "Uttara Sector 7, Dhaka", Latitude = 23.875900m, Longitude = 90.379500m, Status = "Open" });
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded demo stations.");
        }
        else if (!await dbContext.Stations.AnyAsync(x => x.StationCode == "STN004", cancellationToken))
        {
            dbContext.Stations.Add(new Station
            {
                StationName = "Uttara Station",
                StationCode = "STN004",
                Address = "Uttara Sector 7, Dhaka",
                Latitude = 23.875900m,
                Longitude = 90.379500m,
                Status = "Open"
            });
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added demo station STN004 for future employee assignment.");
        }

        if (!await dbContext.Employees.AnyAsync(cancellationToken))
        {
            var stations = await dbContext.Stations.OrderBy(x => x.Id).ToListAsync(cancellationToken);
            var employees = new[]
            {
                new Employee { Name = "Arif Hossain", Phone = "01711000001", Nid = "1993123456789", Address = "Rampura, Dhaka", Username = "arif.h", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Status = "Active", JoiningDate = new DateOnly(2026, 4, 1), StationId = stations[0].Id },
                new Employee { Name = "Jerin Sultana", Phone = "01711000002", Nid = "1992456789123", Address = "Mirpur, Dhaka", Username = "jerin.s", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Status = "Active", JoiningDate = new DateOnly(2026, 4, 3), StationId = stations[1].Id },
                new Employee { Name = "Farhan Ahmed", Phone = "01711000003", Nid = "1991876543210", Address = "Badda, Dhaka", Username = "farhan.a", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!"), Status = "Active", JoiningDate = new DateOnly(2026, 4, 5), StationId = stations[2].Id }
            };

            dbContext.Employees.AddRange(employees);
            await dbContext.SaveChangesAsync(cancellationToken);

            for (var i = 0; i < stations.Count && i < employees.Length; i++)
            {
                stations[i].EmployeeId = employees[i].Id;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded demo employees.");
        }

        if (!await dbContext.Batteries.AnyAsync(cancellationToken))
        {
            var stations = await dbContext.Stations.OrderBy(x => x.Id).ToListAsync(cancellationToken);
            var batteries = new List<Battery>();
            var counter = 1;

            foreach (var station in stations)
            {
                for (var i = 0; i < 6; i++)
                {
                    batteries.Add(new Battery
                    {
                        BatteryCode = $"BAT-{counter:000}",
                        StationId = station.Id,
                        Status = "Available",
                        LastUpdated = DateTime.UtcNow
                    });
                    counter++;
                }
            }

            dbContext.Batteries.AddRange(batteries);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded demo batteries.");
        }
        else
        {
            var stationsWithoutBatteries = await dbContext.Stations
                .Where(station => !dbContext.Batteries.Any(battery => battery.StationId == station.Id))
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);

            if (stationsWithoutBatteries.Count > 0)
            {
                var nextBatteryNumber = await dbContext.Batteries.CountAsync(cancellationToken) + 1;
                var newBatteries = new List<Battery>();

                foreach (var station in stationsWithoutBatteries)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        newBatteries.Add(new Battery
                        {
                            BatteryCode = $"BAT-{nextBatteryNumber:000}",
                            StationId = station.Id,
                            Status = "Available",
                            LastUpdated = DateTime.UtcNow
                        });
                        nextBatteryNumber++;
                    }
                }

                dbContext.Batteries.AddRange(newBatteries);
                await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Added batteries for new demo stations.");
            }
        }

        if (!await dbContext.Clients.AnyAsync(cancellationToken))
        {
            var clients = new[]
            {
                new Client { Name = "Nasima Akter", Phone = "01822000001", Nid = "1989987654321", Address = "Rampura, Dhaka", VehicleType = "Rickshaw", VehicleNumber = "DHK-R-1023", Balance = 1220m, Status = "Active", RegistrationDate = DateTime.UtcNow.AddHours(-6), CreatedByAdminId = admin.Id },
                new Client { Name = "Mahmudul Hasan", Phone = "01822000002", Nid = "1992234567891", Address = "Mirpur, Dhaka", VehicleType = "Bike", VehicleNumber = "DHK-B-5521", Balance = 860m, Status = "Active", RegistrationDate = DateTime.UtcNow.AddHours(-5), CreatedByAdminId = admin.Id },
                new Client { Name = "Abdur Rahim", Phone = "01822000003", Nid = "1987456123456", Address = "Badda, Dhaka", VehicleType = "Rickshaw", VehicleNumber = "DHK-R-9987", Balance = 120m, Status = "Active", RegistrationDate = DateTime.UtcNow.AddHours(-4), CreatedByAdminId = admin.Id },
                new Client { Name = "Sumaiya Noor", Phone = "01822000004", Nid = "1995345678912", Address = "Uttara, Dhaka", VehicleType = "Bike", VehicleNumber = "DHK-B-1208", Balance = 1000m, Status = "Active", RegistrationDate = DateTime.UtcNow.AddHours(-3), CreatedByAdminId = admin.Id }
            };

            dbContext.Clients.AddRange(clients);
            await dbContext.SaveChangesAsync(cancellationToken);

            var assignedClients = await dbContext.Clients.OrderBy(x => x.Id).Take(3).ToListAsync(cancellationToken);
            var availableBatteries = new List<Battery>();
            var seedStationIds = await dbContext.Stations
                .OrderBy(x => x.Id)
                .Take(3)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            foreach (var stationId in seedStationIds)
            {
                var battery = await dbContext.Batteries
                    .Where(x => x.StationId == stationId)
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (battery is not null)
                {
                    availableBatteries.Add(battery);
                }
            }

            for (var i = 0; i < assignedClients.Count && i < availableBatteries.Count; i++)
            {
                assignedClients[i].CurrentBatteryId = availableBatteries[i].Id;
                availableBatteries[i].CurrentClientId = assignedClients[i].Id;
                availableBatteries[i].Status = "WithClient";
                availableBatteries[i].LastUpdated = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded demo clients.");
        }

        if (!await dbContext.BalanceRecharges.AnyAsync(cancellationToken))
        {
            var firstClient = await dbContext.Clients.OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);
            var firstStation = await dbContext.Stations.OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);

            if (firstClient is not null && firstStation is not null)
            {
                dbContext.BalanceRecharges.Add(new BalanceRecharge
                {
                    ClientId = firstClient.Id,
                    StationId = firstStation.Id,
                    Amount = 500m,
                    AddedByAdminId = admin.Id,
                    RechargeTime = DateTime.UtcNow.AddHours(-2)
                });

                await dbContext.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Seeded demo recharge activity.");
            }
        }

        if (!await dbContext.Swaps.AnyAsync(cancellationToken))
        {
            var clientsWithBattery = await dbContext.Clients
                .Where(x => x.CurrentBatteryId != null)
                .OrderBy(x => x.Id)
                .Take(2)
                .ToListAsync(cancellationToken);

            foreach (var client in clientsWithBattery)
            {
                var currentBattery = await dbContext.Batteries.FirstOrDefaultAsync(x => x.Id == client.CurrentBatteryId, cancellationToken);
                if (currentBattery is null)
                {
                    continue;
                }

                var station = await dbContext.Stations.FirstOrDefaultAsync(x => x.Id == currentBattery.StationId, cancellationToken);
                if (station is null)
                {
                    continue;
                }

                var employee = await dbContext.Employees.FirstOrDefaultAsync(
                    x => x.StationId == station.Id && x.Status == "Active",
                    cancellationToken);

                if (employee is null)
                {
                    continue;
                }

                var assignedBattery = await dbContext.Batteries
                    .Where(x => x.StationId == station.Id && x.Status == "Available" && x.Id != currentBattery.Id)
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (assignedBattery is null)
                {
                    continue;
                }

                var swapTime = DateTime.UtcNow.AddMinutes(-(clientsWithBattery.Count - client.Id + 1) * 25);

                currentBattery.Status = "Available";
                currentBattery.CurrentClientId = null;
                currentBattery.LastUpdated = swapTime;

                assignedBattery.Status = "WithClient";
                assignedBattery.CurrentClientId = client.Id;
                assignedBattery.LastUpdated = swapTime;

                client.CurrentBatteryId = assignedBattery.Id;
                client.Balance = Math.Max(0m, client.Balance - BatterySwapDefaults.SwapCost);

                dbContext.Swaps.Add(new Swap
                {
                    ClientId = client.Id,
                    StationId = station.Id,
                    ReturnedBatteryId = currentBattery.Id,
                    AssignedBatteryId = assignedBattery.Id,
                    SwapCost = BatterySwapDefaults.SwapCost,
                    SwapTime = swapTime,
                    ProcessedByEmployeeId = employee.Id
                });
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded demo swap history.");
        }
    }
}
