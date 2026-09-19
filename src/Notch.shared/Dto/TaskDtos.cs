using Notch.Shared.Enums;

namespace Notch.Shared.Dto;

public record TaskItemDto ( Guid Id, string Title, Guid? ParentTaskId, NotchStatus Status, DateTime CreatedAtUtc );

public record CreateTaskRequest(string Title, Guid? ParentTaskId );

public record UpdateTaskStatusRequest(NotchStatus Status );
