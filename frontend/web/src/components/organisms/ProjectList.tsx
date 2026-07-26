import { Link } from "react-router-dom";
import type { ProjectDto } from "../../lib/api/types";
import { EmptyState } from "../atoms/EmptyState";
import { ErrorAlert } from "../atoms/ErrorAlert";
import { Spinner } from "../atoms/Spinner";

interface Props {
  projects: ProjectDto[] | undefined;
  loading: boolean;
  error: Error | null;
  onRetry: () => void;
  onDelete: (project: ProjectDto) => void;
}

export function ProjectList({
  projects,
  loading,
  error,
  onRetry,
  onDelete,
}: Props) {
  if (loading) return <Spinner />;
  if (error) return <ErrorAlert message={error.message} onRetry={onRetry} />;
  if (!projects?.length)
    return (
      <EmptyState
        title="No projects yet"
        description="Create your first project to get started."
      />
    );

  return (
    <div className="overflow-hidden rounded-lg border border-gray-200">
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Name
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Description
            </th>
            <th className="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
              Created
            </th>
            <th className="px-4 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
              Actions
            </th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200 bg-white">
          {projects.map((project) => (
            <tr key={project.id} className="hover:bg-gray-50">
              <td className="px-4 py-3">
                <Link
                  to={`/projects/${project.id}`}
                  className="font-medium text-blue-600 hover:text-blue-800"
                >
                  {project.name}
                </Link>
              </td>
              <td className="max-w-xs truncate px-4 py-3 text-sm text-gray-600">
                {project.description ?? "—"}
              </td>
              <td className="whitespace-nowrap px-4 py-3 text-sm text-gray-500">
                {new Date(project.createdAt).toLocaleDateString()}
              </td>
              <td className="whitespace-nowrap px-4 py-3 text-right text-sm">
                <button
                  onClick={() => onDelete(project)}
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
