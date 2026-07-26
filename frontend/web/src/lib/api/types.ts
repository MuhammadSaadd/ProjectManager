export enum TaskStatus {
  ToDo = 0,
  InProgress = 1,
  Done = 2,
}

export interface ProjectDto {
  id: string;
  name: string;
  description: string | null;
  createdAt: string;
}

export interface TaskItemDto {
  id: string;
  projectId: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string | null;
}

export interface CreateProjectRequest {
  name: string;
  description: string | null;
}

export interface UpdateProjectRequest {
  name: string;
  description: string | null;
}

export interface CreateTaskRequest {
  projectId: string;
  title: string;
  description: string | null;
  dueDate: string | null;
  status: TaskStatus;
}

export interface UpdateTaskRequest {
  title: string;
  description: string | null;
  dueDate: string | null;
  status: TaskStatus;
}

export interface ChangeTaskStatusRequest {
  status: TaskStatus;
}
