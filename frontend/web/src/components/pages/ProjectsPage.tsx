import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { api } from "../../lib/api/client";
import type { CreateProjectRequest, ProjectDto } from "../../lib/api/types";
import { Modal } from "../atoms/Modal";
import { ProjectForm } from "../molecules/ProjectForm";
import { ConfirmDelete } from "../molecules/ConfirmDelete";
import { ProjectList } from "../organisms/ProjectList";

export function ProjectsPage() {
  const queryClient = useQueryClient();
  const [showCreate, setShowCreate] = useState(false);
  const [deleting, setDeleting] = useState<ProjectDto | null>(null);

  const projectsQuery = useQuery({
    queryKey: ["projects"],
    queryFn: api.projects.getAll,
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateProjectRequest) => api.projects.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      setShowCreate(false);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.projects.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      setDeleting(null);
    },
  });

  return (
    <div className="mx-auto max-w-5xl px-4 py-8">
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-2xl font-bold">Projects</h1>
        <button
          onClick={() => setShowCreate(true)}
          className="rounded-lg bg-blue-600 px-4 py-2 text-sm text-white hover:bg-blue-700"
        >
          + New Project
        </button>
      </div>

      <ProjectList
        projects={projectsQuery.data}
        loading={projectsQuery.isLoading}
        error={projectsQuery.error as Error | null}
        onRetry={() => projectsQuery.refetch()}
        onDelete={(p) => setDeleting(p)}
      />

      <Modal
        open={showCreate}
        title="Create Project"
        onClose={() => setShowCreate(false)}
      >
        <ProjectForm
          onSubmit={(data) => createMutation.mutate(data as CreateProjectRequest)}
          onCancel={() => setShowCreate(false)}
          loading={createMutation.isPending}
        />
      </Modal>

      <ConfirmDelete
        open={deleting !== null}
        title="Delete Project"
        message={`Are you sure you want to delete "${deleting?.name}"? This will also delete all tasks in this project.`}
        loading={deleteMutation.isPending}
        onConfirm={() => deleting && deleteMutation.mutate(deleting.id)}
        onCancel={() => setDeleting(null)}
      />
    </div>
  );
}
