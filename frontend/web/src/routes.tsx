import type { RouteObject } from "react-router-dom";
import { ProjectDetailPage } from "./components/pages/ProjectDetailPage";
import { ProjectsPage } from "./components/pages/ProjectsPage";

export const routes: RouteObject[] = [
  {
    path: "/",
    element: <ProjectsPage />,
  },
  {
    path: "/projects/:id",
    element: <ProjectDetailPage />,
  },
];
