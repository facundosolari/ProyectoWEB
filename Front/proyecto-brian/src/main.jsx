import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { GoogleOAuthProvider } from '@react-oauth/google';
import './index.css';
import './styles.css';
import "./styles/global.css";
import App from './App.jsx';

const CLIENT_ID = "526092607598-0chpt9rgs8v11tkhco22e9at3u99224d.apps.googleusercontent.com"; // reemplazá con tu Client ID de Google

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <GoogleOAuthProvider clientId={CLIENT_ID}>
      <App />
    </GoogleOAuthProvider>
  </StrictMode>
);