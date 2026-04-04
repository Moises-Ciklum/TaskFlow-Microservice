using CoupleCalendar.Application.DTOs;
using CoupleCalendar.Application.Services;
using CoupleCalendar.Core.Entities;
using CoupleCalendar.Core.Interfaces;
using Moq;
using NUnit.Framework;

namespace CoupleCalendar.UnitTests;

[TestFixture]
public class EventServiceTests
{
    private Mock<IEventRepository> _repositoryMock;
    private EventService _eventService;

    // This runs BEFORE every single test to give us a fresh state
    [SetUp]
    public void SetUp()
    {
        // We create a "fake" repository
        _repositoryMock = new Mock<IEventRepository>();

        // We inject the fake repository into the real service
        _eventService = new EventService(_repositoryMock.Object);
    }

    [Test]
    public void CreateEventAsync_WithOverlappingDates_ThrowsInvalidOperationException()
    {
        // ==========================================
        // 1. ARRANGE (Preparar el escenario)
        // ==========================================
        var ownerName = "Mi Pareja";

        // Simular que ya hay un turno en la base de datos de 10:00 a 14:00
        var existingEventsFromDb = new List<CalendarEvent>
        {
            new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = "Turno Mañana",
                StartDate = new DateTime(2026, 4, 4, 10, 0, 0),
                EndDate = new DateTime(2026, 4, 4, 14, 0, 0),
                Owner = ownerName
            }
        };

        // Entrenamos al "Mock" para que cuando el servicio le pida los datos, devuelva nuestra lista falsa
        _repositoryMock.Setup(repo => repo.GetAllAsync(ownerName))
                       .ReturnsAsync(existingEventsFromDb);

        // El payload que intentaremos enviar (de 12:00 a 16:00, choca claramente)
        var overlappingRequest = new CreateEventDto
        {
            Title = "Partido Solapado",
            StartDate = new DateTime(2026, 4, 4, 12, 0, 0),
            EndDate = new DateTime(2026, 4, 4, 16, 0, 0),
            Owner = ownerName
        };

        // ==========================================
        // 2. ACT & 3. ASSERT (Actuar y Comprobar)
        // ==========================================
        // Comprobamos que al intentar crear el evento, se lanza la excepción correcta
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _eventService.CreateEventAsync(overlappingRequest));

        // Comprobamos que el mensaje de error es exactamente el que diseñamos
        Assert.That(exception.Message, Is.EqualTo("El evento se solapa con otro turno existente."));

        // Verificamos que el repositorio NUNCA fue llamado para guardar (.AddAsync)
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<CalendarEvent>()), Times.Never);
    }

    [Test]
    public async Task CreateEventAsync_WithValidDates_SavesAndReturnsEvent()
    {
        // ==========================================
        // 1. ARRANGE
        // ==========================================
        var ownerName = "Mi Pareja";

        // Simular que el usuario NO tiene turnos ese día (lista vacía)
        var existingEventsFromDb = new List<CalendarEvent>();

        _repositoryMock.Setup(repo => repo.GetAllAsync(ownerName))
                       .ReturnsAsync(existingEventsFromDb);

        var validRequest = new CreateEventDto
        {
            Title = "Turno Tarde",
            StartDate = new DateTime(2026, 4, 4, 15, 0, 0),
            EndDate = new DateTime(2026, 4, 4, 23, 0, 0),
            Owner = ownerName,
            Type = EventType.Work
        };

        // Entrenamos al mock para que, cuando se llame a AddAsync, devuelva el mismo evento que le pasamos
        _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<CalendarEvent>()))
                       .ReturnsAsync((CalendarEvent e) => e);

        // ==========================================
        // 2. ACT
        // ==========================================
        var result = await _eventService.CreateEventAsync(validRequest);

        // ==========================================
        // 3. ASSERT
        // ==========================================
        // Comprobamos que el resultado no es nulo y los datos coinciden
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo(validRequest.Title));
        Assert.That(result.Owner, Is.EqualTo(ownerName));

        // Lo más importante: Verificamos que el repositorio guardó el evento EXACTAMENTE UNA VEZ
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<CalendarEvent>()), Times.Once);
    }

    [Test]
    public void UpdateEventAsync_WithOverlappingDates_ThrowsInvalidOperationException()
    {
        // ==========================================
        // 1. ARRANGE (Preparar el escenario)
        // ==========================================
        var ownerName = "Mi Pareja";
        var eventToUpdateId = Guid.NewGuid(); // El evento que queremos modificar
        var otherEventId = Guid.NewGuid();    // El evento con el que vamos a chocar

        // Simulamos DOS eventos en la base de datos
        var existingEventsFromDb = new List<CalendarEvent>
        {
            new CalendarEvent
            {
                Id = eventToUpdateId,
                Title = "Turno Original (Temprano)",
                StartDate = new DateTime(2026, 4, 4, 8, 0, 0),
                EndDate = new DateTime(2026, 4, 4, 10, 0, 0),
                Owner = ownerName
            },
            new CalendarEvent
            {
                Id = otherEventId,
                Title = "Turno Mañana (El Obstáculo)",
                StartDate = new DateTime(2026, 4, 4, 10, 0, 0),
                EndDate = new DateTime(2026, 4, 4, 14, 0, 0),
                Owner = ownerName
            }
        };

        // Entrenamos al "Mock" para que cuando el servicio le pida los datos, devuelva nuestra lista falsa
        _repositoryMock.Setup(repo => repo.GetAllAsync(ownerName))
                       .ReturnsAsync(existingEventsFromDb);

        // Payload: Intentamos mover nuestro primer evento a las 12:00, chocando con el segundo
        var overlappingRequest = new UpdateEventDto
        {
            Title = "Turno Movido (Solapado)",
            StartDate = new DateTime(2026, 4, 4, 12, 0, 0),
            EndDate = new DateTime(2026, 4, 4, 16, 0, 0),
            Owner = ownerName,
            Type = EventType.Leisure
        };

        // ==========================================
        // 2. ACT & 3. ASSERT (Actuar y Comprobar)
        // ==========================================
        // Comprobamos que al intentar actualizar el evento, se lanza la excepción correcta
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _eventService.UpdateEventAsync(eventToUpdateId, overlappingRequest));

        // Comprobamos que el mensaje de error es exactamente el que diseñamos
        Assert.That(exception.Message, Is.EqualTo("El evento se solapa con otro turno existente."));

        // Verificamos que el repositorio NUNCA fue llamado para guardar (.AddAsync)
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<CalendarEvent>()), Times.Never);
    }
}