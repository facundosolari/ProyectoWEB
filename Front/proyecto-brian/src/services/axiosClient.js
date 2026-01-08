import axios from "axios";

const axiosClient = axios.create({
  baseURL: "http://proyectoweb.railway.app/weatherforecast",
  withCredentials: true, // cookies HttpOnly se envían automáticamente
});

export default axiosClient;

