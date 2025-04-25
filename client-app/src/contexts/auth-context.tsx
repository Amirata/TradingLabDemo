import React, { createContext, useState, useEffect, ReactNode } from "react";
import { jwtDecode } from "jwt-decode";

import {
  getToken,
  setToken,
  setRefreshToken,
  clearTokens, getRefreshToken,
} from "../libs/token-service";

import apisWrapper from "../libs/apis-wrapper.ts";
import {useAppStore} from "../libs/stores/app-store.ts";

interface JWTPayload {
  userName: string;
  jti: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role":
    | string[]
    | string;
  exp: number;
  iss: string;
  aud: string;
}

type user = {
  id: string;
  username: string;
  role: string[] | string;
};

interface AuthContextType {
  isAuthenticated: boolean;
  user: user | null;
  login: (accessToken: string, refreshToken: string) => void;
  logout: () => void;
  hasRole: (role: string) => boolean;
}

export const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  user: null,
  login: () => {},
  logout: () => {},
  hasRole: () => false,
});

type AuthProviderProps = {
  children: ReactNode;
};

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<user | null>(null);

  const parseJwt = (token: string): JWTPayload | null => {
    try {
      return jwtDecode<JWTPayload>(token);
    } catch (error) {
      console.error("Error decoding JWT", error);

      return null;
    }
  };

  useEffect(() => {
    // On initial load, attempt to read existing tokens from memory or localStorage
    const token = getToken();

    if (token) {
      const decoded = parseJwt(token);

      if (decoded && decoded.exp * 1000 > Date.now()) {
        setUser({
          id: decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
          ],
          username: decoded.userName,
          role: decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ],
        });
      } else {
        // Token is expired
        clearTokens();
        setUser(null);
      }
    }
  }, []);

  const login = (accessToken: string, refresh: string) => {
    setToken(accessToken);
    setRefreshToken(refresh);
    const decoded = parseJwt(accessToken);

    setUser({
      id: decoded![
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ],
      username: decoded!.userName,
      role: decoded![
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ],
    });
  };

  const logout = async () => {
    const { reset } = useAppStore.getState();
    clearTokens();
    setUser(null);
    reset();
    await apisWrapper.requests.post("/accounts/logout", {
      refreshToken: getRefreshToken(),
    });
  };

  const hasRole = (role: string): boolean => {
    if (!user) return false;
    if (Array.isArray(user.role)) {
      //console.log(user.role);
      return user.role.includes(role);
    } else {
      //console.log(user.role);
      return user.role === role;
    }
  };

  const value: AuthContextType = {
    isAuthenticated: !!user,
    user,
    login,
    logout,
    hasRole,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
