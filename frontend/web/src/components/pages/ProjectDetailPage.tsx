import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { api } from "../../lib/api/client";
import type {
  CreateTaskRequest,
  TaskItemDto,
  UpdateTaskRequest,
} from "../../lib/api/types";
import { ErrorAlert } from "../atoms/ErrorAlert";
import { Modal } from "../atoms/Modal";
import { Spinner } from "../atoms/Spinner";
import { ConfirmDelete } from "../molecules/ConfirmDelete";
import { ProjectForm } from "../molecules/ProjectForm";
import { TaskForm } from "../molecules/TaskForm";
import { TaskList } from "../organisms/TaskList";

export function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [showEditProject, setShowEditProject] = useState(false);
  const [showCreateTask, setShowCreateTask] = useState(false);
  const [editingTask, setEditingTask] = useState<TaskItemDto | null>(null);
  const [deletingTask, setDeletingTask] = useState<TaskItemDto | null>(null);

  const projectQuery = useQuery({
    queryKey: ["project", id],
    queryFn: () => api.projects.getById(id!),
    enabled: !!id,
  });

  const tasksQuery = useQuery({
    queryKey: ["tasks", id],
    queryFn: () => api.tasks.getByProject(id!),
    enabled: !!id,
  });

  const updateProjectMutation = useMutation({
    mutationFn: (data: { name: string; description: string | null }) =>
      api.projects.update(id!, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["project", id] });
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      setShowEditProject(false);
    },
  });

  const createTaskMutation = useMutation({
    mutationFn: (data: CreateTaskRequest) => api.tasks.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks", id] });
      setShowCreateTask(false);
    },
  });

  const updateTaskMutation = useMutation({
    mutationFn: ({ taskId, data }: { taskId: string; data: UpdateTaskRequest }) =>
      api.tasks.update(taskId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks", id] });
      setEditingTask(null);
    },
  });

  const deleteTaskMutation = useMutation({
    mutationFn: (taskId: string) => api.tasks.delete(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks", id] });
      setDeletingTask(null);
    },
  });

  const changeStatusMutation = useMutation({
    mutationFn: ({ taskId, status }: { taskId: string; status: number }) =>
      api.tasks.changeStatus(taskId, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks", id] });
    },
  });

  if (projectQuery.isLoading) return <Spinner />;
  if (projectQuery.error)
    return (
      <div className="mx-auto max-w-3xl px-4 py-8">
        <ErrorAlert
          message={(projectQuery.error as Error).message}
          onRetry={() => projectQuery.refetch()}
        />
      </div>
    );

  const project = projectQuery.data!;

  return (
    <div className="mx-auto max-w-5xl px-4 py-8">
      <button
        onClick={() => navigate("/")}
        className="mb-4 text-sm text-gray-500 hover:text-gray-700"
      >
        ← Back to Projects
      </button>

      <div className="mb-6 flex items-start justify-between">
        <div>
          <h1 className="text-2xl font-bold">{project.name}</h1>
          {project.description && (
            <p className="mt-1 text-gray-600">{project.description}</p>
          )}
        </div>
        <button
          onClick={() => setShowEditProject(true)}
          className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm text-gray-700 hover:bg-gray-50"
        >
          Edit
        </button>
      </div>

      <div className="mb-4 flex items-center justify-between">
        <h2 className="text-lg font-semibold">Tasks</h2>
        <button
          onClick={() => setShowCreateTask(true)}
          className="rounded-lg bg-blue-600 px-3 py-1.5 text-sm text-white hover:bg-blue-700"
        >
          + Add Task
        </button>
      </div>

      <TaskList
        tasks={tasksQuery.data}
        loading={tasksQuery.isLoading}
        error={tasksQuery.error as Error | null}
        onRetry={() => tasksQuery.refetch()}
        onEdit={(task) => setEditingTask(task)}
        onDelete={(task) => setDeletingTask(task)}
        onChangeStatus={(task, status) =>
          changeStatusMutation.mutate({ taskId: task.id, status })
        }
      />

      <Modal
        open={showEditProject}
        title="Edit Project"
        onClose={() => setShowEditProject(false)}
      >
        <ProjectForm
          initial={project}
          onSubmit={(data) => updateProjectMutation.mutate(data)}
          onCancel={() => setShowEditProject(false)}
          loading={updateProjectMutation.isPending}
        />
      </Modal>

      <Modal
        open={showCreateTask}
        title="Add Task"
        onClose={() => setShowCreateTask(false)}
      >
        <TaskForm
          projectId={id}
          onSubmit={(data) =>
            createTaskMutation.mutate(data as CreateTaskRequest)
          }
          onCancel={() => setShowCreateTask(false)}
          loading={createTaskMutation.isPending}
        />
      </Modal>

      <Modal
        open={editingTask !== null}
        title="Edit Task"
        onClose={() => setEditingTask(null)}
      >
        {editingTask && (
          <TaskForm
            initial={editingTask}
            onSubmit={(data) =>
              updateTaskMutation.mutate({
                taskId: editingTask.id,
                data: data as UpdateTaskRequest,
              })
            }
            onCancel={() => setEditingTask(null)}
            loading={updateTaskMutation.isPending}
          />
        )}
      </Modal>

      <ConfirmDelete
        open={deletingTask !== null}
        title="Delete Task"
        message={`Are you sure you want to delete "${deletingTask?.title}"?`}
        loading={deleteTaskMutation.isPending}
        onConfirm={() => deletingTask && deleteTaskMutation.mutate(deletingTask.id)}
        onCancel={() => setDeletingTask(null)}
      />
    </div>
  );
}
