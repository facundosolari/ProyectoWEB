import React, { useState } from "react";
import { createDescuento } from "../services/orderService";

export default function CreateDescuentoModal({ onClose, onCreated }) {
  const [nombre, setNombre] = useState("");
  const [tipo, setTipo] = useState(0); // 0=Monto, 1=Porcentaje
  const [total, setTotal] = useState(0);
  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const [habilitado, setHabilitado] = useState(true);

  const handleSubmit = async () => {
    try {
      await createDescuento({
        Nombre: nombre,
        TipoDescuento: tipo,
        TotalDescuento: total,
        Fecha_Desde: new Date(fechaDesde).toISOString(),
        Fecha_Hasta: new Date(fechaHasta).toISOString(),
        Habilitado: habilitado,
      });
      onCreated();
      onClose();
    } catch (error) {
      console.error(error);
      alert("Error al crear el descuento");
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modern-modal">
        <h2>Crear Descuento</h2>

        <input
          type="text"
          placeholder="Nombre"
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
        />

        <select
          value={tipo}
          onChange={(e) => setTipo(Number(e.target.value))}
        >
          <option value={0}>Monto</option>
          <option value={1}>Porcentaje</option>
        </select>

        <input
          type="number"
          placeholder="Total Descuento"
          value={total}
          onChange={(e) => setTotal(Number(e.target.value))}
        />

        <input
          type="date"
          value={fechaDesde}
          onChange={(e) => setFechaDesde(e.target.value)}
        />
        <input
          type="date"
          value={fechaHasta}
          onChange={(e) => setFechaHasta(e.target.value)}
        />

        <label className="mordern-modal-regla-habilitado">
          <input
            type="checkbox"
            checked={habilitado}
            onChange={(e) => setHabilitado(e.target.checked)}
          />
          Habilitado
        </label>

        <div className="modal-buttons">
          <button className="save-btn" onClick={handleSubmit}>
            Guardar
          </button>
          <button className="cancel-btn" onClick={onClose}>
            Cancelar
          </button>
        </div>
      </div>
    </div>
  );
}