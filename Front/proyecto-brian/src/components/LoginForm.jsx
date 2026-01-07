import React, { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { GoogleLogin } from "@react-oauth/google";
import { UserContext } from "../context/UserContext";
import "../styles/Auth.css";

const LoginForm = () => {
  const { login, loginWithGoogle } = useContext(UserContext);
  const navigate = useNavigate();

  const [usuario, setUsuario] = useState("");
  const [contraseña, setContraseña] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    try {
      await login(usuario, contraseña);
      navigate("/home");
    } catch (err) {
      setError(err.message || "Usuario o contraseña incorrectos");
    }
  };

  const handleGoogleSuccess = async (credentialResponse) => {
    try {
      await loginWithGoogle(credentialResponse.credential);
      navigate("/home");
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="auth-wrapper">
      <form onSubmit={handleSubmit} className="auth-card">
        <h2>Bienvenido 👋</h2>
        <p className="auth-subtitle">Iniciá sesión para continuar</p>

        {error && <p className="auth-error">{error}</p>}

        <input
          type="text"
          placeholder="Usuario"
          value={usuario}
          onChange={(e) => setUsuario(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Contraseña"
          value={contraseña}
          onChange={(e) => setContraseña(e.target.value)}
          required
        />

        <button type="submit" className="btn primary full">
          Ingresar
        </button>

        <div style={{ margin: "15px 0", textAlign: "center" }}>o</div>

        <GoogleLogin
          onSuccess={handleGoogleSuccess}
          onError={() => setError("Error al iniciar sesión con Google")}
        />

        <p className="auth-footer">
          ¿No tenés cuenta? <a href="/register">Registrate</a>
        </p>
      </form>
    </div>
  );
};

export default LoginForm;