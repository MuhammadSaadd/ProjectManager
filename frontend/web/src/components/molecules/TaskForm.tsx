import { useState } from "react";
import type {
  CreateTaskRequest,
  TaskItemDto,
  TaskStatus,
  UpdateTaskRequest,
} from "../../lib/api/types";
import { TaskStatusSelect } from "./TaskStatusSelect";

type FormData = CreateTaskRequest | UpdateTaskRequest;

interface Props {
  initial?: TaskItemDto;
  projectId?: string;
  onSubmit: (data: FormData) => void;
  onCancel: () => void;
  loading: boolean;
}

export function TaskForm({ initial, projectId, onSubmit, onCancel, loading }: Props) {
  const [title, setTitle] = useState(initial?.title ?? "");
  const [description, setDescription] = useState(initial?.description ?? "");
  const [status, setStatus] = useState<TaskStatus>(initial?.status ?? 0);
  const [dueDate, setDueDate] = useState(initial?.dueDate?.split("T")[0] ?? "");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const base = {
      title,
      description: description || null,
      status,
      dueDate: dueDate ? new Date(dueDate).toISOString() : null,
    };
    if (initial) {
      onSubmit(base as UpdateTaskRequest);
    } else {
      onSubmit({ ...base, projectId: projectId! } as CreateTaskRequest);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <label className="block text-sm font-medium text-gray-700">Title</label>
        <input
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
          maxLength={200}
          className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <div>
        <label className="block text-sm font-medium text-gray-700">
          Description
        </label>
        <textarea
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={2}
          maxLength={2000}
          className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <div className="flex gap-4">
        <div className="flex-1">
          <label className="block text-sm font-medium text-gray-700">Status</label>
          <TaskStatusSelect value={status} onChange={setStatus} />
        </div>
        <div className="flex-1">
          <label className="block text-sm font-medium text-gray-700">
            Due Date
          </label>
          <input
            type="date"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
            className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          />
        </div>
      </div>
      <div className="flex justify-end gap-2">
        <button
          type="button"
          onClick={onCancel}
          disabled={loading}
          className="rounded-lg border border-gray-300 px-4 py-2 text-sm text-gray-700 hover:bg-gray-50"
        >
          Cancel
        </button>
        <button
          type="submit"
          disabled={loading}
          className="rounded-lg bg-blue-600 px-4 py-2 text-sm text-white hover:bg-blue-700 disabled:opacity-50"
        >
          {loading ? "Saving..." : initial ? "Update" : "Create"}
        </button>
      </div>
    </form>
  );
}
