import React, { useState, useEffect } from "react";
import CreateProductModal from "./CreateProductModal";
import EditProductModal from "./EditProductModal";

import CreateSizeModal from "./CreateSizeModal";
import EditSizeModal from "./EditSizeModal";

import CreateCategoryModal from "./CreateCategoryModal";
import EditCategoryModal from "./EditCategoryModal";

import CreateDescuentoModal from "./CreateDescuentoModal";
import UpdateDescuentoModal from "./UpdateDescuentoModal";
import CreateReglaDescuentoModal from "./CreateReglaDescuentoModal";
import UpdateReglaDescuentoModal from "./UpdateReglaDescuentoModal";



import "../styles/AdminPanel.css";

export default function AdminPanel() {
  const [panelOpen, setPanelOpen] = useState(false);
  const [activeSection, setActiveSection] = useState("producto");

  const [editingProduct, setEditingProduct] = useState(false);
  const [creatingProduct, setCreatingProduct] = useState(false);

  const [editingSize, setEditingSize] = useState(false);
  const [creatingSize, setCreatingSize] = useState(false);

  const [editingCategory, setEditingCategory] = useState(false);
  const [creatingCategory, setCreatingCategory] = useState(false);

  const [creatingDescuento, setCreatingDescuento] = useState(false);
  const [editingDescuento, setEditingDescuento] = useState(false);

  const [creatingRegla, setCreatingRegla] = useState(false);
  const [editingRegla, setEditingRegla] = useState(false);


  // Callbacks temporales para refrescar listas
  const fetchDescuentos = () => {};  
  const fetchReglas = () => {};

  return (
    <div className="admin-panel">
      <h1 className="panel-title">Panel de Administración</h1>

      <button className="open-panel-btn" onClick={() => setPanelOpen(true)}>
        Abrir Panel
      </button>

      {panelOpen && (
        <div className="panel-overlay" onClick={() => setPanelOpen(false)} />
      )}

      <div className={`side-panel ${panelOpen ? "open" : ""}`}>
        <button className="close-btn" onClick={() => setPanelOpen(false)}>
          ✕
        </button>

        {/* --- SIDEBAR --- */}
        <div className="panel-sidebar">
          <button
            className={activeSection === "producto" ? "active" : ""}
            onClick={() => setActiveSection("producto")}
          >
            Productos
          </button>

          <button
            className={activeSection === "talle" ? "active" : ""}
            onClick={() => setActiveSection("talle")}
          >
            Talles
          </button>

          <button
            className={activeSection === "categoria" ? "active" : ""}
            onClick={() => setActiveSection("categoria")}
          >
            Categorías
          </button>

          <button
            className={activeSection === "descuento" ? "active" : ""}
            onClick={() => setActiveSection("descuento")}
          >
            Descuentos
          </button>

          <button
            className={activeSection === "regla" ? "active" : ""}
            onClick={() => setActiveSection("regla")}
          >
            Reglas
          </button>
        </div>

        {/* --- CONTENIDO PRINCIPAL --- */}
        <div className="panel-content">
          {/* === PRODUCTOS === */}
          {activeSection === "producto" && (
            <>
              <h2 className="section-title">Productos</h2>

              <div className="admin-card">
                <h3>Crear Producto</h3>
                <button
                  onClick={() => setCreatingProduct(true)}
                  className="primary-btn"
                >
                  Crear Producto
                </button>
              </div>

              <div className="admin-card secondary">
                <h3>Editar Producto</h3>
                <button
                  onClick={() => setEditingProduct(true)}
                  className="secondary-btn"
                >
                  Editar Producto
                </button>
              </div>
            </>
          )}

          {/* === TALLES === */}
          {activeSection === "talle" && (
            <>
              <h2 className="section-title">Talles</h2>

              <div className="admin-card">
                <h3>Crear Talle</h3>
                <button
                  onClick={() => setCreatingSize(true)}
                  className="primary-btn"
                >
                  Crear Talle
                </button>
              </div>

              <div className="admin-card secondary">
                <h3>Editar Talle</h3>
                <button
                  onClick={() => setEditingSize(true)}
                  className="secondary-btn"
                >
                  Editar Talle
                </button>
              </div>
            </>
          )}

          {/* === CATEGORÍAS === */}
          {activeSection === "categoria" && (
            <>
              <h2 className="section-title">Categorías</h2>

              <div className="admin-card">
                <h3>Crear Categoría</h3>
                <button
                  onClick={() => setCreatingCategory(true)}
                  className="primary-btn"
                >
                  Crear Categoría
                </button>
              </div>

              <div className="admin-card secondary">
                <h3>Editar Categoría</h3>
                <button
                  onClick={() => setEditingCategory(true)}
                  className="secondary-btn"
                >
                  Editar Categoría
                </button>
              </div>
            </>
          )}

          {/* === DESCUENTOS === */}
          {activeSection === "descuento" && (
  <>
    <h2 className="section-title">Descuentos</h2>

    <div className="admin-card">
      <h3>Crear Descuento</h3>
      <button
        onClick={() => setCreatingDescuento(true)}
        className="primary-btn"
      >
        Crear Descuento
      </button>
    </div>

    <div className="admin-card secondary">
      <h3>Editar Descuento</h3>
      <button
        onClick={() => setEditingDescuento(true)}
        className="secondary-btn"
      >
        Editar Descuento
      </button>
    </div>
  </>
)}

          {/* === REGLAS === */}
          {activeSection === "regla" && (
            <>
              <h2 className="section-title">Reglas de Descuento</h2>

              <div className="admin-card">
                <h3>Crear Regla</h3>
                <button
                  onClick={() => setCreatingRegla(true)}
                  className="primary-btn"
                >
                  Crear Regla
                </button>
              </div>

              <div className="admin-card secondary">
                <h3>Editar Regla</h3>
                <button
                    className="secondary-btn"
                    onClick={() => {
                      ;
                      setEditingRegla(true);
                    }}
                  >
                    Editar Regla
                  </button>
              </div>
            </>
          )}
        </div>
      </div>

      {/* --- MODALES --- */}
      {creatingProduct && (
        <CreateProductModal onClose={() => setCreatingProduct(false)} />
      )}
      {editingProduct && (
        <EditProductModal onClose={() => setEditingProduct(false)} />
      )}

      {creatingSize && (
        <CreateSizeModal onClose={() => setCreatingSize(false)} />
      )}
      {editingSize && (
        <EditSizeModal onClose={() => setEditingSize(false)} />
      )}

      {creatingCategory && (
        <CreateCategoryModal onClose={() => setCreatingCategory(false)} />
      )}
      {editingCategory && (
        <EditCategoryModal onClose={() => setEditingCategory(false)} />
      )}

      {creatingDescuento && (
        <CreateDescuentoModal
          onClose={() => setCreatingDescuento(false)}
          onCreated={fetchDescuentos}
        />
      )}
      {editingDescuento && (
  <UpdateDescuentoModal
    onClose={() => setEditingDescuento(false)}
  />
)}


      {creatingRegla && (
        <CreateReglaDescuentoModal
          onClose={() => setCreatingRegla(false)}
          onCreated={fetchReglas}
        />
      )}

      {editingRegla && (
        <UpdateReglaDescuentoModal
          onClose={() => setEditingRegla(false)}
        />
      )}

    </div>
  );
}