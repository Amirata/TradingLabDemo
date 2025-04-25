import { Outlet } from "react-router-dom";

import PageLayout from "../layouts/page-layout.tsx";
import ProtectedRoute from "../components/auth/protected-route.tsx";

export default function PlansPage() {
  return (
    <ProtectedRoute>
      <PageLayout>
        <Outlet />
      </PageLayout>
    </ProtectedRoute>
  );
}
