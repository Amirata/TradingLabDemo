import React, { JSX } from "react";
import { Navigate } from "react-router-dom";

import { useAuth } from "../../hooks/auth/use-auth.ts";

interface RoleProtectedRouteProps {
  children: JSX.Element;
  requiredRole: string;
}

const RoleProtectedRoute: React.FC<RoleProtectedRouteProps> = ({
  children,
  requiredRole,
}) => {
  const { isAuthenticated, hasRole } = useAuth();

  if (!isAuthenticated || !hasRole(requiredRole)) {
    return <Navigate replace to="/no-access" />;
  }

  return children;
};

export default RoleProtectedRoute;
