import { createContext, useState, useEffect } from "react";

export const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem("token") || null);

  useEffect(() => {
    if (token) {
      localStorage.setItem("token", token);
    } else {
      localStorage.removeItem("token");
    }
  }, [token]);

  const login = (userData, token) => {
    setUser(userData);
    setToken(token);
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    window.location.href = "/login";
  };

  useEffect(() => {
    const handleUnauthorized = () => {
      logout();
      alert("Session expired, please log in again.");
    };

    window.addEventListener("api-unauthorized", handleUnauthorized);

    return () => {
      window.removeEventListener("api-unauthorized", handleUnauthorized);
    };
  }, []);

  return (
    <AuthContext.Provider value={{ user, token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
