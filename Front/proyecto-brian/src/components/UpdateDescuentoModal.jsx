import React, { useEffect, useState } from "react";
import {
  updateDescuento,
  getAllDescuentos
} from "../services/orderService";

/**
 * TipoDescuento (enum backend)
 * 0 = Porcentaje
 * 1 = MontoFijo
 * Ajustá los valores si tu enum es distinto
 */
const TIPOS_DESCUENTO = [
  { value: 1, label: "Porcentaje (%)" },
  { value: 0, label: "Monto fijo ($)" }
];

export default function UpdateDescuentoModal({ onClose, onUpdated }) {

  /* ================= ESTADOS ================= */

  const [descuentos, setDescuentos] = useState([]);
  const [selectedId, setSelectedId] = useState(null);

  const descuento = descuentos.find(d => d.id === selectedId);

  const [nombre, setNombre] = useState("");
  const [tipoDescuento, setTipoDescuento] = useState(0);
  const [totalDescuento, setTotalDescuento] = useState(0);
  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const [habilitado, setHabilitado] = useState(false);

  /* ================= GET DESCUENTOS ================= */

  useEffect(() => {
    const fetch = async () => {
      const data = await getAllDescuentos();
      setDescuentos(data);
      if (data.length > 0) {
        setSelectedId(data[0].id);
      }
    };
    fetch();
  }, []);

  /* ================= CUANDO CAMBIA EL DESCUENTO ================= */

  useEffect(() => {
    if (!descuento) return;

    setNombre(descuento.nombre);
    setTipoDescuento(descuento.tipoDescuento);
    setTotalDescuento(descuento.totalDescuento);
    setFechaDesde(descuento.fecha_Desde?.split("T")[0] || "");
    setFechaHasta(descuento.fecha_Hasta?.split("T")[0] || "");
    setHabilitado(descuento.habilitado);
  }, [descuento]);

  /* ================= SUBMIT ================= */

  const handleSubmit = async () => {

    if (!nombre.trim()) {
      alert("Debe ingresar un nombre");
      return;
    }

    if (totalDescuento <= 0) {
      alert("El descuento debe ser mayor a 0");
      return;
    }

    if (!fechaDesde || !fechaHasta) {
      alert("Debe completar las fechas");
      return;
    }

    if (new Date(fechaDesde) > new Date(fechaHasta)) {
      alert("La fecha desde no puede ser mayor que la fecha hasta");
      return;
    }

    await updateDescuento(selectedId, {
      nombre,
      tipoDescuento,
      totalDescuento,
      fecha_Desde: `${fechaDesde}T00:00:00`,
      fecha_Hasta: `${fechaHasta}T23:59:59`,
      habilitado
    });

    onUpdated?.();
    onClose();
  };

  /* ================= RENDER ================= */

  return (
    <div className="modal-overlay">
      <div className="modern-modal">

        <h2>Editar descuento</h2>

        {/* Selector */}
        <select
          value={selectedId || ""}
          onChange={(e) => setSelectedId(Number(e.target.value))}
        >
          {descuentos.map(d => (
            <option key={d.id} value={d.id}>
              {d.nombre}
            </option>
          ))}
        </select>

        <hr />

        <input
          type="text"
          placeholder="Nombre del descuento"
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
        />

        <select
          value={tipoDescuento}
          onChange={(e) => setTipoDescuento(Number(e.target.value))}
        >
          {TIPOS_DESCUENTO.map(t => (
            <option key={t.value} value={t.value}>
              {t.label}
            </option>
          ))}
        </select>

        <input
          type="number"
          min="0"
          step="0.01"
          placeholder="Total descuento"
          value={totalDescuento}
          onChange={(e) => setTotalDescuento(Number(e.target.value))}
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

        <label className="modern-modal-regla-habilitado">
          <input
            type="checkbox"
            checked={habilitado}
            onChange={(e) => setHabilitado(e.target.checked)}
          />
          Habilitado
        </label>

        <div className="modal-buttons">
          <button className="save-btn" onClick={handleSubmit}>
            Guardar cambios
          </button>
          <button className="cancel-btn" onClick={onClose}>
            Cancelar
          </button>
        </div>

      </div>
    </div>
  );
}