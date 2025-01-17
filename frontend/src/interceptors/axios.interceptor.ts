import axios, {
  AxiosError,
  AxiosInstance,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";

const instance: AxiosInstance = axios.create({
  baseURL: "https://api.transfer-streaming.ngrok.dev/",
  withCredentials: true,
  timeout: 10_000,
  headers: {
    "Content-Type": "application/json",
  },
});

instance.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    return config;
  },
  (error: any) => {
    // Manejo de errores en la configuración de la petición
    return Promise.reject(error);
  }
);

instance.interceptors.response.use(
  (response: AxiosResponse) => {
    // Do something with response data
    return response;
  },
  async (error: AxiosError) => {
    if (error.response?.status === 401) {
      await instance.get("/api/auth/refresh");
      return;
    }

    return Promise.reject(error);
  }
);

export default instance;
