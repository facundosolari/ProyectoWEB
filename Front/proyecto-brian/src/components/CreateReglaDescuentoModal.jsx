import React, { useState, useEffect } from "react";
import { createRegla, getAllDescuentos } from "../services/orderService";

export default function CreateReglaDescuentoModal({ onClose, onCreated }) {
  const [descuentoId, setDescuentoId] = useState(0);
  const [productIds, setProductIds] = useState([]);
  const [categoryIds, setCategoryIds] = useState([]);
  const [fechaDesde, setFechaDesde] = useState("");
  const [fechaHasta, setFechaHasta] = useState("");
  const [habilitado, setHabilitado] = useState(true);

  const [descuentos, setDescuentos] = useState([]);

  useEffect(() => {
    const fetchDescuentos = async () => {
      try {
        const data = await getAllDescuentos();
        setDescuentos(data);
      } catch (error) {
        console.error(error);
      }
    };
    fetchDescuentos();
  }, []);

  const handleSubmit = async () => {
    try {
      await createRegla({
        DescuentoId: descuentoId,
        ProductIds: productIds,
        CategoryIds: categoryIds,
        Fecha_Desde: fechaDesde,
        Fecha_Hasta: fechaHasta,
        Habilitado: habilitado,
      });
      onCreated();
      onClose();
    } catch (error) {
      console.error(error);
      alert("Error al crear la regla de descuento");
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modern-modal">
        <h2>Crear Regla de Descuento</h2>

        <select value={descuentoId} onChange={(e) => setDescuentoId(Number(e.target.value))}>
          <option value={0}>Seleccionar descuento</option>
          {descuentos.map((d) => (
            <option key={d.id} value={d.id}>
              {d.nombre}
            </option>
          ))}
        </select>

        <input
          type="text"
          placeholder="IDs de productos separados por comas"
          onChange={(e) => setProductIds(e.target.value.split(",").map((x) => Number(x)))}
        />
        <input
          type="text"
          placeholder="IDs de categorías separados por comas"
          onChange={(e) => setCategoryIds(e.target.value.split(",").map((x) => Number(x)))}
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
          />{" "}
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