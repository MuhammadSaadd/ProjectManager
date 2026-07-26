import type { TaskStatus } from "../../lib/api/types";

const labels: Record<TaskStatus, string> = {
  [0]: "To Do",
  [1]: "In Progress",
  [2]: "Done",
};

const colors: Record<TaskStatus, string> = {
  [0]: "bg-gray-100 text-gray-700",
  [1]: "bg-blue-100 text-blue-700",
  [2]: "bg-green-100 text-green-700",
};

interface Props {
  status: TaskStatus;
}

export function StatusBadge({ status }: Props) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${colors[status]}`}
    >
      {labels[status]}
    </span>
  );
}
