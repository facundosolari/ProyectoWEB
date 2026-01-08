import axios from "axios";

const axiosClient = axios.create({
  baseURL: "http://proyectoweb.railway.app",
  withCredentials: true, // cookies HttpOnly se envían automáticamente
});

export default axiosClient;

