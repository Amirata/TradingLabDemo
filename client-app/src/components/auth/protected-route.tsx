import React, { JSX } from "react";
import { Navigate } from "react-router-dom";

import { useAuth } from "../../hooks/auth/use-auth.ts";

interface ProtectedRouteProps {
  children: JSX.Element;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) {
    return <Navigate replace to="/login" />;
  }

  return children;
};

export default ProtectedRoute;
