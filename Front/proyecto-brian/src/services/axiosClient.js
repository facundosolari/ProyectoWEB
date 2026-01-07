import axios from "axios";

const axiosClient = axios.create({
  baseURL: "https://localhost:7076/api",
  withCredentials: true, // cookies HttpOnly se envían automáticamente
});

export default axiosClient;

