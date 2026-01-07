import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useUser } from "../context/UserContext";
import "../styles/Auth.css";
import { GoogleLogin } from "@react-oauth/google";

export default function RegisterForm() {
  const { register, loginWithGoogle } = useUser();
  const navigate = useNavigate();

  const [form, setForm] = useState({
    usuario: "",
    contraseña: "",
    nombre: "",
    apellido: "",
    celular: "",
    email: "",
  });

  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      await register(form);
      alert("Registro exitoso. Ya podés iniciar sesión");
      navigate("/login");
    } catch (err) {
      setError(err.response?.data?.message || err.message || "Error al registrarse");
    }
  };

  const handleGoogleSuccess = async (credentialResponse) => {
    try {
      await loginWithGoogle(credentialResponse.credential);
      navigate("/home");
    } catch (err) {
      setError(err.message || "Error registrándose con Google");
    }
  };

  const handleGoogleError = () => {
    setError("Error registrándose con Google");
  };

  return (
    <div className="auth-wrapper">
      <form onSubmit={handleSubmit} className="auth-card">
        <h2>Crear cuenta ✨</h2>
        <p className="auth-subtitle">Registrate en La Cabaña Deportiva</p>

        {error && <p className="auth-error">{error}</p>}

        <div className="auth-grid">
          <input
            name="usuario"
            placeholder="Usuario"
            value={form.usuario}
            onChange={handleChange}
            required
          />

          <input
            type="password"
            name="contraseña"
            placeholder="Contraseña"
            value={form.contraseña}
            onChange={handleChange}
            required
          />

          <input
            name="nombre"
            placeholder="Nombre"
            value={form.nombre}
            onChange={handleChange}
          />

          <input
            name="apellido"
            placeholder="Apellido"
            value={form.apellido}
            onChange={handleChange}
          />

          <input
            name="celular"
            placeholder="Celular"
            value={form.celular}
            onChange={handleChange}
          />

          <input
            type="email"
            name="email"
            placeholder="Email"
            value={form.email}
            onChange={handleChange}
            required
          />
        </div>

        <button type="submit" className="btn primary full">
          Registrarse
        </button>

        <div style={{ marginTop: "10px" }}>
          <GoogleLogin
            onSuccess={handleGoogleSuccess}
            onError={handleGoogleError}
          />
        </div>

        <p className="auth-footer">
          ¿Ya tenés cuenta? <a href="/login">Iniciar sesión</a>
        </p>
      </form>
    </div>
  );
}