import type {
  CreateProjectRequest,
  CreateTaskRequest,
  ProjectDto,
  TaskItemDto,
  UpdateProjectRequest,
  UpdateTaskRequest,
} from "./types";

const BASE = "/api";

async function request<T>(
  url: string,
  options?: RequestInit,
): Promise<T> {
  const res = await fetch(`${BASE}${url}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });
  if (!res.ok) {
    const body = await res.json().catch(() => null);
    throw new Error(body?.detail ?? body?.title ?? `Request failed (${res.status})`);
  }
  if (res.status === 204) return undefined as T;
  return res.json();
}

export const api = {
  projects: {
    getAll: () => request<ProjectDto[]>("/projects"),
    getById: (id: string) => request<ProjectDto>(`/projects/${id}`),
    create: (data: CreateProjectRequest) =>
      request<ProjectDto>("/projects", {
        method: "POST",
        body: JSON.stringify(data),
      }),
    update: (id: string, data: UpdateProjectRequest) =>
      request<ProjectDto>(`/projects/${id}`, {
        method: "PUT",
        body: JSON.stringify(data),
      }),
    delete: (id: string) =>
      request<void>(`/projects/${id}`, { method: "DELETE" }),
  },
  tasks: {
    getByProject: (projectId: string) =>
      request<TaskItemDto[]>(`/projects/${projectId}/tasks`),
    getById: (id: string) => request<TaskItemDto>(`/tasks/${id}`),
    getByStatus: (status: number) =>
      request<TaskItemDto[]>(`/tasks?status=${status}`),
    create: (data: CreateTaskRequest) =>
      request<TaskItemDto>("/tasks", {
        method: "POST",
        body: JSON.stringify(data),
      }),
    update: (id: string, data: UpdateTaskRequest) =>
      request<TaskItemDto>(`/tasks/${id}`, {
        method: "PUT",
        body: JSON.stringify(data),
      }),
    delete: (id: string) =>
      request<void>(`/tasks/${id}`, { method: "DELETE" }),
    changeStatus: (id: string, status: number) =>
      request<TaskItemDto>(`/tasks/${id}/status`, {
        method: "PATCH",
        body: JSON.stringify({ status }),
      }),
  },
};
