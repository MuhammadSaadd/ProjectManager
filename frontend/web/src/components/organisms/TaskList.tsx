import type { TaskItemDto } from "../../lib/api/types";
import { EmptyState } from "../atoms/EmptyState";
import { ErrorAlert } from "../atoms/ErrorAlert";
import { Spinner } from "../atoms/Spinner";
interface Props {
  tasks: TaskItemDto[] | undefined;
  loading: boolean;
  error: Error | null;
  onRetry: () => void;
  onEdit: (task: TaskItemDto) => void;
  onDelete: (task: TaskItemDto) => void;
  onChangeStatus: (task: TaskItemDto, status: number) => void;
}

export function TaskList({
  tasks,
  loading,
  error,
  onRetry,
  onEdit,
  onDelete,
  onChangeStatus,
}: Props) {
  if (loading) return <Spinner />;
  if (error) return <ErrorAlert message={error.message} onRetry={onRetry} />;
  if (!tasks?.length)
    return (
      <EmptyState
        title="No tasks yet"
        description="Add a task to this project."
      />
    );

  return (
    <div className="overflow-hidden rounded-lg border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Title
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Status
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Due Date
            </th>
            <th className="px-4 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
              Actions
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200 bg-white">
          {tasks.map((task) => (
            <tr key={task.id} className="hover:bg-gray-50">
              <td className="px-4 py-3">
                <div>
                  <p className="font-medium text-gray-900">{task.title}</p>
                  {task.description && (
                    <p className="mt-0.5 text-xs text-gray-500">
                      {task.description}
                    </p>
                  )}
                </div>
              </td>
              <td className="px-4 py-3">
                <select
                  value={task.status}
                  onChange={(e) =>
                    onChangeStatus(task, Number(e.target.value))
                  }
                  className="rounded border border-gray-300 px-2 py-1 text-xs"
                >
                  <option value={0}>To Do</option>
                  <option value={1}>In Progress</option>
                  <option value={2}>Done</option>
                </select>
              </td>
              <td className="whitespace-nowrap px-4 py-3 text-sm text-gray-500">
                {task.dueDate
                  ? new Date(task.dueDate).toLocaleDateString()
                  : "—"}
              </td>
              <td className="whitespace-nowrap px-4 py-3 text-right text-sm">
                <button
                  onClick={() => onEdit(task)}
                  className="mr-2 text-blue-600 hover:text-blue-800"
                >
                  Edit
                </button>
                <button
                  onClick={() => onDelete(task)}
                  className="text-red-600 hover:text-red-800"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
