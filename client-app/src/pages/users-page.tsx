import { Outlet } from "react-router-dom";

import PageLayout from "../layouts/page-layout.tsx";
import RoleProtectedRoute from "../components/auth/role-protected-route.tsx";

export default function UsersPage() {
  return (
    <RoleProtectedRoute requiredRole={"Admin"}>
      <PageLayout>
        <Outlet />
      </PageLayout>
    </RoleProtectedRoute>
  );
}
