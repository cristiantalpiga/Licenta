import React, { useState, useEffect } from "react";
import Login from "./Login";
import AngajatiPage from "./AngajatiPage";

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    setIsAuthenticated(!!token);
  }, []);

  return (
    <div className="min-h-screen bg-gray-100">
      {isAuthenticated ? (
        <AngajatiPage />
      ) : (
        <Login onLoginSuccess={() => setIsAuthenticated(true)} />
      )}
    </div>
  );
}

export default App;
