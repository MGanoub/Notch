namespace Notch.Shared.Dto;

public record StartTimerRequest (Guid TaskItemId);

public record TimeEntryDto
(
    Guid Id, Guid TaskItemId, DateTime StartedAtUtc,
     DateTime? EndedAtUtc
);