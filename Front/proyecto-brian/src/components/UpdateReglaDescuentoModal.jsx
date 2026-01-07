import React, { useState, useEffect } from "react";
import {
  updateRegla,
  getAllDescuentos,
  getAllReglas
} from "../services/orderService";

export default function UpdateReglaDescuentoModal({ onClose, onUpdated }) {

  /* ================= ESTADOS ================= */

  const [reglas, setReglas] = useState([]);
  const [selectedReglaId, setSelectedReglaId] = useState(null);

  const regla = reglas.find(r => r.id === selectedReglaId);

  const [descuentoId, setDescuentoId] = useState(0);

  // 🔑 estados lógicos (arrays)
  const [productIds, setProductIds] = useState([]);
  const [categoryIds, setCategoryIds] = useState([]);

  // 🔑 estados de UI (strings para permitir comas)
  const [productIdsInput, setProductIdsInput] = useState("");
  const [categoryIdsInput, setCategoryIdsInput] = useState("");

  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const [habilitado, setHabilitado] = useState(false);

  const [descuentos, setDescuentos] = useState([]);

  /* ================= GET REGLAS ================= */

  useEffect(() => {
    const fetchReglas = async () => {
      const data = await getAllReglas();
      setReglas(data);
      if (data.length > 0) {
        setSelectedReglaId(data[0].id);
      }
    };
    fetchReglas();
  }, []);

  /* ================= CUANDO CAMBIA LA REGLA ================= */

  useEffect(() => {
    if (!regla) return;

    setDescuentoId(regla.descuentoId);

    const prodIds = regla.products?.map(p => p.id) || [];
    const catIds = regla.categories?.map(c => c.id) || [];

    setProductIds(prodIds);
    setCategoryIds(catIds);

    // 🔑 sincroniza inputs
    setProductIdsInput(prodIds.join(","));
    setCategoryIdsInput(catIds.join(","));

    setFechaDesde(regla.fecha_Desde?.split("T")[0] || "");
    setFechaHasta(regla.fecha_Hasta?.split("T")[0] || "");
    setHabilitado(regla.habilitado);
  }, [regla]);

  /* ================= GET DESCUENTOS ================= */

  useEffect(() => {
    const fetchDescuentos = async () => {
      const data = await getAllDescuentos();
      setDescuentos(data);
    };
    fetchDescuentos();
  }, []);

  /* ================= SUBMIT ================= */

  const handleSubmit = async () => {
    if (!selectedReglaId) {
      alert("Debe seleccionar una regla");
      return;
    }

    if (!descuentoId) {
      alert("Debe seleccionar un descuento");
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

    if (productIds.length === 0 && categoryIds.length === 0) {
      alert("La regla debe aplicar al menos a un producto o una categoría");
      return;
    }

    await updateRegla(selectedReglaId, {
      descuentoId,
      productIds,
      categoryIds,
      fecha_Desde: `${fechaDesde}T00:00:00`,
      fecha_Hasta: `${fechaHasta}T23:59:59`,
      habilitado,
    });

    onUpdated?.();
    onClose();
  };

  /* ================= HELPERS ================= */

  const parseIds = (value) =>
    value
      .split(",")
      .map(x => Number(x.trim()))
      .filter(x => !isNaN(x) && x > 0);

  /* ================= RENDER ================= */

  return (
    <div className="modal-overlay">
      <div className="modern-modal">
        <h2>Editar regla de descuento</h2>

        {/* Selector de regla */}
        <select
          value={selectedReglaId || ""}
          onChange={(e) => setSelectedReglaId(Number(e.target.value))}
        >
          {reglas.map(r => (
            <option key={r.id} value={r.id}>
              Regla #{r.id}
            </option>
          ))}
        </select>

        <hr />

        <select
          value={descuentoId}
          onChange={(e) => setDescuentoId(Number(e.target.value))}
        >
          <option value={0}>Seleccionar descuento</option>
          {descuentos.map(d => (
            <option key={d.id} value={d.id}>
              {d.nombre}
            </option>
          ))}
        </select>

        <input
          type="text"
          placeholder="IDs de productos (ej: 1,2,3)"
          value={productIdsInput}
          onChange={(e) => {
            const value = e.target.value;
            setProductIdsInput(value);
            setProductIds(parseIds(value));
          }}
        />

        <input
          type="text"
          placeholder="IDs de categorías (ej: 4,5)"
          value={categoryIdsInput}
          onChange={(e) => {
            const value = e.target.value;
            setCategoryIdsInput(value);
            setCategoryIds(parseIds(value));
          }}
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