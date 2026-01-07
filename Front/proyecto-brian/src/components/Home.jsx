import React, { useEffect, useState } from "react";
import ProductCard from "./ProductCard";
import ProductDetailModal from "./ProductDetailModal";
import OrdersSlide from "./OrdersSlide";
import { useUser } from "../context/UserContext";
import axiosClient from "../services/axiosClient";
import "../styles/Home.css";
import banner from "../../public/banner/banner.avif";

export default function Home() {
  const { user } = useUser();

  const [products, setProducts] = useState([]);
  const [featured, setFeatured] = useState([]);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [ordersOpen, setOrdersOpen] = useState(false);
  const [loading, setLoading] = useState(true);

  // ===============================
  // 📦 CARGA PRODUCTOS (PREVIEW)
  // ===============================
  useEffect(() => {
    const fetchHomeProducts = async () => {
      try {
        setLoading(true);
        const res = await axiosClient.get(
          "/Product/Paged?page=1&pageSize=8&onlyEnabled=true"
        );

        const items = res.data.items || [];
        setProducts(items);

        // destacados: con descuento o más vendidos
        const destacados = items
          .filter(p => p.descuentoPorcentaje > 0)
          .slice(0, 4);

        setFeatured(destacados.length ? destacados : items.slice(0, 4));
      } catch (err) {
        console.error("Error Home productos:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchHomeProducts();
  }, []);

  return (
    <main className="home-main">

      {/* =====================================================
          🎥 HERO
      ===================================================== */}
      <section className="hero-ultra">
        <img src={banner} alt="Banner" className="hero-bg" />

        <div className="hero-overlay">
          <h1>La Cabaña Deportiva</h1>
          <p>Indumentaria y equipamiento para rendir al máximo</p>

          <div className="hero-actions">
            <a href="/products" className="btn primary">
              Ver catálogo
            </a>
            {!user && (
              <a href="/login" className="btn secondary">
                Iniciar sesión
              </a>
            )}
          </div>
        </div>
      </section>

      {/* =====================================================
          💎 BENEFICIOS
      ===================================================== */}
      <section className="benefits">
        <div>🚚 Envíos a todo el país</div>
        <div>💳 Cuotas sin interés</div>
        <div>🔥 Ofertas semanales</div>
        <div>🔒 Compra segura</div>
      </section>

      {/* =====================================================
          ⭐ DESTACADOS
      ===================================================== */}
      <section className="home-section">
        <h2>🔥 Destacados</h2>

        {loading ? (
          <p className="empty">Cargando productos...</p>
        ) : (
          <div className="product-grid">
            {featured.map(p => (
              <ProductCard
                key={p.id}
                product={p}
                onClick={() => setSelectedProduct(p)}
              />
            ))}
          </div>
        )}
      </section>

      {/* =====================================================
          🆕 NUEVOS INGRESOS
      ===================================================== */}
      <section className="home-section alt">
        <h2>🆕 Nuevos ingresos</h2>

        {loading ? (
          <p className="empty">Cargando productos...</p>
        ) : (
          <div className="product-grid">
            {products.map(p => (
              <ProductCard
                key={p.id}
                product={p}
                onClick={() => setSelectedProduct(p)}
              />
            ))}
          </div>
        )}

        <div className="center-cta">
          <a href="/products" className="btn outline">
            Ver catálogo completo →
          </a>
        </div>
      </section>

      {/* =====================================================
          📦 ÓRDENES
      ===================================================== */}
      {user?.userId && (
        <>
          <button
            className="orders-toggle-btn"
            onClick={() => setOrdersOpen(prev => !prev)}
          >
            {ordersOpen ? "Cerrar Órdenes" : "Mis Órdenes"}
          </button>

          <OrdersSlide
            isOpen={ordersOpen}
            onClose={() => setOrdersOpen(false)}
          />
        </>
      )}

      {/* =====================================================
          🔍 MODAL PRODUCTO
      ===================================================== */}
      {selectedProduct && (
        <ProductDetailModal
          product={selectedProduct}
          onClose={() => setSelectedProduct(null)}
          allProducts={products}
          onChangeProduct={setSelectedProduct}
        />
      )}
    </main>
  );
}