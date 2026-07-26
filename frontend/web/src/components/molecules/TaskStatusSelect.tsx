import type { TaskStatus } from "../../lib/api/types";

const options: { value: TaskStatus; label: string }[] = [
  { value: 0, label: "To Do" },
  { value: 1, label: "In Progress" },
  { value: 2, label: "Done" },
];

interface Props {
  value: TaskStatus;
  onChange: (value: TaskStatus) => void;
}

export function TaskStatusSelect({ value, onChange }: Props) {
  return (
    <select
      value={value}
      onChange={(e) => onChange(Number(e.target.value) as TaskStatus)}
      className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
    >
      {options.map((opt) => (
        <option key={opt.value} value={opt.value}>
          {opt.label}
        </option>
      ))}
    </select>
  );
}
