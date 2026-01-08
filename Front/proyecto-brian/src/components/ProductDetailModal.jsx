import React, { useState, useEffect } from "react";
import { useUser } from "../context/UserContext";
import "../styles/ProductDetailModal.css";

const ProductDetailModal = ({ product, onClose, allProducts, onChangeProduct }) => {
  const { addToCart, user } = useUser();
  const baseUrl = "http://proyectoweb.railway.app";

  // preselecciona el primer talle habilitado
  const firstSizeId = product.sizes?.find((s) => s.habilitado)?.id.toString() || "";
  const [selectedSizeId, setSelectedSizeId] = useState(firstSizeId);
  const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
  const [relatedProducts, setRelatedProducts] = useState([]);

  if (!product) return null;

  // Reset automático cuando cambia el producto
  useEffect(() => {
    setCurrentPhotoIndex(0);
    setSelectedSizeId(firstSizeId);
  }, [product]);

  // Fotos del producto
  const photos =
    product.fotos?.length > 0
      ? product.fotos.map((f) => `${baseUrl}${f}`)
      : ["https://via.placeholder.com/400"];

  const handlePrevPhoto = () => {
    setCurrentPhotoIndex((prev) => (prev === 0 ? photos.length - 1 : prev - 1));
  };

  const handleNextPhoto = () => {
    setCurrentPhotoIndex((prev) => (prev === photos.length - 1 ? 0 : prev + 1));
  };

  // Productos relacionados por categorías
  useEffect(() => {
    if (!allProducts || !product?.categories?.length) return;

    const categoryIds = product.categories.map((c) => c.id);

    const filtered = allProducts.filter((p) => {
      if (p.id === product.id) return false;
      const pCategoryIds = p.categories?.map((c) => c.id) || [];
      return pCategoryIds.some((id) => categoryIds.includes(id));
    });

    setRelatedProducts(filtered);
  }, [product, allProducts]);

  // Agregar al carrito
  const handleAddToCart = () => {
    if (!selectedSizeId) {
      alert("Selecciona un talle antes de agregar al carrito");
      return;
    }

    const selectedSize = product.sizes.find((s) => s.id === parseInt(selectedSizeId));

    addToCart({
  id: selectedSize.id,
  productId: product.id,
  nombre: product.nombre,
  precioUnitario: product.precioConDescuento ?? product.precio,
  precioOriginal: product.precio,
  descuentoPorcentaje: product.descuentoPorcentaje ?? 0,
  talle: selectedSize.talle,
  fotos: photos,
});

    alert("Producto agregado al carrito!");
  };

  // stock del talle seleccionado
  const selectedSizeStock = selectedSizeId
    ? product.sizes.find((s) => s.id === parseInt(selectedSizeId))?.stock || 0
    : null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        {/* Botón de cierre */}
        <button className="close-btn-top" onClick={onClose}>×</button>

        {/* Cuerpo principal */}
        <div className="modal-body">
          
          {/* Fotos */}
          <div className="modal-left">
            <button className="photo-nav photo-prev" onClick={handlePrevPhoto}>&lt;</button>
            <img src={photos[currentPhotoIndex]} alt={product.nombre} />
            <button className="photo-nav photo-next" onClick={handleNextPhoto}>&gt;</button>
          </div>

          {/* Información */}
          <div className="modal-right">
            <h2>{product.nombre}</h2>
            
            {product.descuentoPorcentaje ? ( 
  <p className="modal-price">
    <span className="original-price">${product.precio}</span>{" "}
    <span className="discounted-price">${product.precioConDescuento}</span>{" "}
    <span className="discount-badge">-{product.descuentoPorcentaje}%🔥</span>
  </p>
) : (
  <p className="modal-price">${product.precio}</p>
)}
            <p>{product.descripcion}</p>
            <p>{console.log("MODAL PRODUCT 👉", product)}</p>

            {/* STOCK DEL TALLE SELECCIONADO */}
            {selectedSizeStock !== null && (
              <p>Stock: {selectedSizeStock}</p>
            )}

            {user && (
              <button className="modal-add-btn" onClick={handleAddToCart}>
                Agregar al carrito
              </button>
            )}
            <select
              value={selectedSizeId}
              onChange={(e) => setSelectedSizeId(e.target.value)}
              className="modal-select"
            >
              {product.sizes?.filter((s) => s.habilitado).map((s) => (
                <option key={s.id} value={s.id}>
                  Talle: {s.talle}
                </option>
              ))}
            </select>

            
          </div>
        </div>

        <div className="related-slider-container">
          <h3 className="related-title">Productos relacionados</h3>
          <div className="related-slider">
            {relatedProducts.length === 0 && (
              <span style={{ marginLeft: "10px" }}>No hay productos relacionados.</span>
            )}
            {relatedProducts.map((rp) => (
              <div
                key={rp.id}
                className="related-card"
                onClick={() => onChangeProduct(rp)}
              >
                <img
                  src={rp.fotos?.length ? baseUrl + rp.fotos[0] : "https://via.placeholder.com/150"}
                  alt={rp.nombre}
                />
                <p>{rp.nombre}</p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailModal;