using Microsoft.AspNetCore.SignalR;
using CoupleCalendar.Application.Services;
using CoupleCalendar.API.Hubs;

namespace CoupleCalendar.API.Workers;

public class EventNotifierWorker : BackgroundService
{
    private readonly ILogger<EventNotifierWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<CalendarHub> _hubContext; // Para gritarle al frontend

    public EventNotifierWorker(
        ILogger<EventNotifierWorker> logger,
        IServiceScopeFactory scopeFactory,
        IHubContext<CalendarHub> hubContext)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("El vigía de turnos ha arrancado en segundo plano.");

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

                var allEvents = await eventService.GetAllEventsAsync(null);

                var now = DateTime.Now; // Ojo a si usas UTC en tu base de datos
                var upcomingEvents = allEvents.Where(e =>
                    e.StartDate > now &&
                    e.StartDate <= now.AddMinutes(15));

                foreach (var evt in upcomingEvents)
                {
                    var mensaje = $"¡Alerta! El turno '{evt.Title}' de {evt.Owner} empieza en breve.";
                    _logger.LogInformation(mensaje);

                    await _hubContext.Clients.All.SendAsync("ReceiveAlert", mensaje, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revisando los próximos turnos.");
            }
        }
    }
}