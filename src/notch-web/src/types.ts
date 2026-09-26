export interface TaskItem {
    id: string;
    title: string;
    parentTaskId: string | null;
    status: TaskStatus;
    createdAtUtc: string;
}

export enum TaskStatus 
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
}

export enum TaskStatus {
    Todo = 0,
    InProgress = 1,
    Done = 2,
}

export interface TokenResponse{
    accessToken: string;
    refreshToken: string;
    accessTokenExpiresAtUtc: string;
}