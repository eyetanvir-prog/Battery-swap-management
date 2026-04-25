using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BatterySwap.MVC.Models;

namespace BatterySwap.MVC.Services;

public class BatterySwapApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
{
    public async Task<DashboardViewModel> GetDashboardAsync(string role, CancellationToken cancellationToken = default)
    {
        var endpoint = string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase)
            ? "/api/dashboard/employee"
            : "/api/dashboard/admin";

        var client = CreateAuthorizedClient();
        var response = await client.GetAsync(endpoint, cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<ApiDashboardResponse>(cancellationToken: cancellationToken)
            ?? new ApiDashboardResponse();

        return new DashboardViewModel
        {
            StatCards = payload.StatCards.Select(x => new StatCardViewModel
            {
                Title = x.Title,
                Value = x.Value,
                Subtitle = x.Subtitle,
                AccentClass = x.AccentClass,
                IconClass = x.IconClass
            }).ToList(),
            Highlights = payload.Highlights.Select(x => new StatCardViewModel
            {
                Title = x.Title,
                Value = x.Value,
                Subtitle = x.Subtitle,
                AccentClass = x.AccentClass,
                IconClass = x.IconClass
            }).ToList(),
            RecentActivities = payload.RecentActivities.Select(x => new ActivityItemViewModel
            {
                Title = x.Title,
                Detail = x.Detail,
                RelativeTime = ToRelativeTime(x.TimestampUtc),
                BadgeClass = x.BadgeClass,
                BadgeText = x.BadgeText
            }).ToList(),
            StationStocks = payload.StationStocks.Select(x => new StationStockViewModel
            {
                StationName = x.StationName,
                StationCode = x.StationCode,
                AvailableBatteries = x.AvailableBatteries,
                TotalBatteries = x.TotalBatteries,
                EmployeeName = x.EmployeeName,
                Status = x.Status
            }).ToList()
        };
    }

    public async Task<IReadOnlyList<EmployeeSummaryViewModel>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/employees", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiEmployeeListItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(employee => new EmployeeSummaryViewModel
        {
            Id = employee.Id,
            Name = employee.Name,
            Username = employee.Username,
            Phone = employee.Phone,
            NID = employee.Nid,
            StationName = employee.StationName,
            Status = employee.Status,
            JoiningDate = employee.JoiningDate?.ToString("yyyy-MM-dd") ?? "N/A"
        }).ToList();
    }

    public async Task<EditEmployeeViewModel> GetEmployeeForEditAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/employees/{employeeId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<ApiEmployeeDetail>(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Employee response was empty.");

        var stations = await GetStationsAsync(cancellationToken);
        return new EditEmployeeViewModel
        {
            Id = payload.Id,
            Name = payload.Name,
            Phone = payload.Phone,
            NID = payload.Nid,
            Address = payload.Address,
            StationId = payload.StationId,
            Username = payload.Username,
            Status = payload.Status,
            JoiningDate = payload.JoiningDate?.ToDateTime(TimeOnly.MinValue),
            Stations = stations.ToList()
        };
    }

    public async Task<IReadOnlyList<ClientSummaryViewModel>> GetClientsAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/clients", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiClientListItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(clientItem => new ClientSummaryViewModel
        {
            Id = clientItem.Id,
            Name = clientItem.Name,
            Phone = clientItem.Phone,
            VehicleType = clientItem.VehicleType,
            VehicleNumber = clientItem.VehicleNumber,
            CurrentBatteryCode = clientItem.CurrentBatteryCode,
            Balance = $"{clientItem.Balance:0.##} Tk",
            Status = clientItem.Status,
            DisplayStatus = clientItem.Balance < 140 && string.Equals(clientItem.Status, "Active", StringComparison.OrdinalIgnoreCase)
                ? "Low Balance"
                : clientItem.Status,
            IsInactive = !string.Equals(clientItem.Status, "Active", StringComparison.OrdinalIgnoreCase)
        }).ToList();
    }

    public async Task<EditClientViewModel> GetClientForEditAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var payload = await GetClientApiAsync(clientId, cancellationToken);

        return new EditClientViewModel
        {
            Id = payload.Id,
            Name = payload.Name,
            Phone = payload.Phone,
            NID = payload.Nid,
            Address = payload.Address,
            VehicleType = payload.VehicleType,
            VehicleNumber = payload.VehicleNumber,
            Status = payload.Status,
            CurrentBatteryCode = payload.CurrentBatteryCode,
            BalanceDisplay = $"{payload.Balance:0.##} Tk"
        };
    }

    public async Task<IReadOnlyList<StationOptionViewModel>> GetStationsAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/stations/lookup", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiStationLookupItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(x => new StationOptionViewModel
        {
            Id = x.Id,
            Label = $"{x.StationName} ({x.StationCode})"
        }).ToList();
    }

    public async Task<IReadOnlyList<StationSummaryViewModel>> GetStationsListAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/stations", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiStationListItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(x => new StationSummaryViewModel
        {
            Id = x.Id,
            StationName = x.StationName,
            StationCode = x.StationCode,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
            AvailableBatteries = x.AvailableBatteries,
            TotalBatteries = x.TotalBatteries,
            EmployeeName = x.EmployeeName,
            Status = x.Status
        }).ToList();
    }

    public async Task<EditStationViewModel> GetStationForEditAsync(int stationId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/stations/{stationId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<ApiStationListItem>(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Station response was empty.");

        return new EditStationViewModel
        {
            Id = payload.Id,
            StationName = payload.StationName,
            StationCode = payload.StationCode,
            Address = payload.Address,
            Latitude = payload.Latitude,
            Longitude = payload.Longitude,
            Status = payload.Status
        };
    }

    public async Task<IReadOnlyList<BatterySummaryViewModel>> GetBatteryListAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/batteries", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiBatteryListItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(x => new BatterySummaryViewModel
        {
            Id = x.Id,
            BatteryCode = x.BatteryCode,
            StationId = x.StationId,
            StationName = x.StationName,
            Status = x.Status,
            CurrentClientId = x.CurrentClientId,
            CurrentClientName = x.CurrentClientName,
            LastUpdated = x.LastUpdated.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
        }).ToList();
    }

    public async Task<EditBatteryViewModel> GetBatteryForEditAsync(int batteryId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/batteries/{batteryId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<ApiBatteryDetail>(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Battery response was empty.");

        var stations = await GetStationsAsync(cancellationToken);
        return new EditBatteryViewModel
        {
            Id = payload.Id,
            BatteryCode = payload.BatteryCode,
            StationId = payload.StationId,
            Status = payload.Status,
            CurrentClientName = payload.CurrentClientName,
            IsAssignedToClient = payload.CurrentClientId.HasValue,
            LastUpdated = payload.LastUpdated.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
            Stations = stations.ToList()
        };
    }

    public async Task<IReadOnlyList<SwapHistoryItemViewModel>> GetSwapHistoryAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/swaps", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiSwapHistoryItem>>(cancellationToken: cancellationToken)
            ?? [];

        return MapSwapHistory(payload);
    }

    public async Task<IReadOnlyList<SwapHistoryItemViewModel>> GetClientSwapHistoryAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/swaps/client/{clientId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiSwapHistoryItem>>(cancellationToken: cancellationToken)
            ?? [];

        return MapSwapHistory(payload);
    }

    public async Task<IReadOnlyList<RechargeHistoryItemViewModel>> GetRechargeHistoryAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/recharges", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiRechargeHistoryItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(x => new RechargeHistoryItemViewModel
        {
            Id = x.Id,
            ClientId = x.ClientId,
            ClientName = x.ClientName,
            ClientPhone = x.ClientPhone,
            StationName = x.StationName,
            Amount = $"{x.Amount:0.##} Tk",
            RechargeTime = x.RechargeTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
            AddedBy = x.AddedBy
        }).ToList();
    }

    public async Task CreateEmployeeAsync(CreateEmployeeViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/api/employees", new
        {
            model.Name,
            model.Phone,
            Nid = model.NID,
            model.Address,
            model.StationId,
            model.Username,
            model.Password,
            JoiningDate = model.JoiningDate.HasValue ? DateOnly.FromDateTime(model.JoiningDate.Value) : (DateOnly?)null
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task UpdateEmployeeAsync(EditEmployeeViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"/api/employees/{model.Id}", new
        {
            model.Name,
            model.Phone,
            Nid = model.NID,
            model.Address,
            model.StationId,
            model.Username,
            model.Password,
            model.Status,
            JoiningDate = model.JoiningDate.HasValue ? DateOnly.FromDateTime(model.JoiningDate.Value) : (DateOnly?)null
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task DeactivateEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"/api/employees/{employeeId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task CreateStationAsync(CreateStationViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/api/stations", new
        {
            model.StationName,
            model.StationCode,
            model.Address,
            model.Latitude,
            model.Longitude
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task UpdateStationAsync(EditStationViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"/api/stations/{model.Id}", new
        {
            model.StationName,
            model.StationCode,
            model.Address,
            model.Latitude,
            model.Longitude,
            model.Status
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task CreateBatteryAsync(CreateBatteryViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/api/batteries", new
        {
            model.BatteryCode,
            model.StationId
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task UpdateBatteryAsync(EditBatteryViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"/api/batteries/{model.Id}", new
        {
            model.BatteryCode,
            model.StationId
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task CreateClientAsync(CreateClientViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/api/clients", new
        {
            model.Name,
            model.Phone,
            Nid = model.NID,
            model.Address,
            model.VehicleType,
            model.VehicleNumber
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task UpdateClientAsync(EditClientViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"/api/clients/{model.Id}", new
        {
            model.Name,
            model.Phone,
            Nid = model.NID,
            model.Address,
            model.VehicleType,
            model.VehicleNumber,
            model.Status
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task DeactivateClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"/api/clients/{clientId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task<ClientDetailViewModel> GetClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        var payload = await GetClientApiAsync(clientId, cancellationToken);
        return MapClient(payload);
    }

    public async Task<ClientDetailViewModel> SearchClientByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/clients/search?phone={Uri.EscapeDataString(phone)}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<ApiClientDetail>(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Client response was empty.");

        return MapClient(payload);
    }

    public async Task RechargeClientAsync(RechargeClientViewModel model, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync($"/api/clients/{model.ClientId}/recharge", new
        {
            model.Amount,
            model.StationId
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);
    }

    public async Task<IReadOnlyList<BatteryOptionViewModel>> GetMyAvailableBatteriesAsync(CancellationToken cancellationToken = default)
    {
        var stationId = GetCurrentStationId();
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/batteries/available/{stationId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        var payload = await response.Content.ReadFromJsonAsync<List<ApiBatteryLookupItem>>(cancellationToken: cancellationToken)
            ?? [];

        return payload.Select(x => new BatteryOptionViewModel
        {
            Id = x.Id,
            BatteryCode = x.BatteryCode
        }).ToList();
    }

    public async Task<ApiSwapResponse> ProcessSwapAsync(int clientId, int assignedBatteryId, CancellationToken cancellationToken = default)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("/api/swaps", new
        {
            ClientId = clientId,
            AssignedBatteryId = assignedBatteryId
        }, cancellationToken);

        await EnsureAuthorizedSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<ApiSwapResponse>(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Swap response was empty.");
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("No session token is available.");
        }

        var client = httpClientFactory.CreateClient("BatterySwapApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private int GetCurrentStationId()
    {
        var stationId = httpContextAccessor.HttpContext?.Session.GetInt32("StationId");
        if (!stationId.HasValue)
        {
            throw new UnauthorizedAccessException("No station is associated with the current session.");
        }

        return stationId.Value;
    }

    private static async Task EnsureAuthorizedSuccessAsync(HttpResponseMessage response)
    {
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new UnauthorizedAccessException("The current session is not authorized.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed with {(int)response.StatusCode}: {body}");
        }
    }

    private static string ToRelativeTime(DateTime timestampUtc)
    {
        var delta = DateTime.UtcNow - timestampUtc;
        if (delta.TotalMinutes < 1)
        {
            return "Just now";
        }

        if (delta.TotalHours < 1)
        {
            return $"{Math.Max(1, (int)delta.TotalMinutes)} minutes ago";
        }

        if (delta.TotalDays < 1)
        {
            return $"{Math.Max(1, (int)delta.TotalHours)} hours ago";
        }

        return $"{Math.Max(1, (int)delta.TotalDays)} days ago";
    }

    private static ClientDetailViewModel MapClient(ApiClientDetail payload)
    {
        return new ClientDetailViewModel
        {
            Id = payload.Id,
            Name = payload.Name,
            Phone = payload.Phone,
            NID = payload.Nid,
            Address = payload.Address,
            VehicleType = payload.VehicleType,
            VehicleNumber = payload.VehicleNumber,
            BalanceValue = payload.Balance,
            BalanceDisplay = $"{payload.Balance:0.##} Tk",
            Status = payload.Balance < 140 ? "Low Balance" : payload.Status,
            CurrentBatteryId = payload.CurrentBatteryId,
            CurrentBatteryCode = payload.CurrentBatteryCode
        };
    }

    private static IReadOnlyList<SwapHistoryItemViewModel> MapSwapHistory(IEnumerable<ApiSwapHistoryItem> payload)
    {
        return payload.Select(x => new SwapHistoryItemViewModel
        {
            Id = x.Id,
            ClientId = x.ClientId,
            ClientName = x.ClientName,
            ClientPhone = x.ClientPhone,
            StationName = x.StationName,
            ReturnedBatteryCode = x.ReturnedBatteryCode,
            AssignedBatteryCode = x.AssignedBatteryCode,
            SwapCost = $"{x.SwapCost:0.##} Tk",
            SwapTime = x.SwapTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
            ProcessedByEmployee = x.ProcessedByEmployee
        }).ToList();
    }

    private async Task<ApiClientDetail> GetClientApiAsync(int clientId, CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"/api/clients/{clientId}", cancellationToken);
        await EnsureAuthorizedSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<ApiClientDetail>(cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Client response was empty.");
    }
}
