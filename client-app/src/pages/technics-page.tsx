import { Outlet } from "react-router-dom";


import ProtectedRoute from "../components/auth/protected-route.tsx";
import PageLayout from "../layouts/page-layout.tsx";


export default function TechnicsPage() {
  return (
    <ProtectedRoute>
      <PageLayout>
        <Outlet />
      </PageLayout>
    </ProtectedRoute>
  );
}
